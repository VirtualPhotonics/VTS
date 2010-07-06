#include <stdio.h>
#include <math.h>
#include <stdlib.h>
#include "nrutil.h"
#include "mc_main.h"
#include "mc_v.h"
#include "pert.h"

#define MU_LB 0.01
 
/* global variables */
extern struct Photon *photptr;
extern struct Tissue *tissptr;
extern struct Output *outptr;
extern struct perturb *pertptr;
extern struct History *histptr;
struct bvolume *bananaptr;
extern struct SourceDefinition *source;
extern struct DetectorDefinition *detector;

/***********************************************************/
void init_banana_allvox()
{
  int ix,iy,iz,iw,num_sides=6;
  bananaptr=(struct bvolume *)malloc(sizeof(struct bvolume));
  /* use cylindrical data for cartesian data */
  bananaptr->nx=2*detector->nr+1;  /* center source */
  bananaptr->ny=1;  
  bananaptr->nz=detector->nz; 
  printf("banana:nx,ny,nz=%d,%d,%d\n",
    bananaptr->nx,bananaptr->ny,bananaptr->nz);
  //outptr->out_side_allvox=dvector(0,num_sides-1);
  //outptr->in_side_allvox=dvector(0,num_sides-1);
  for (iw=0;iw<num_sides;++iw) {
    outptr->out_side_allvox[iw]=d3tensor(0,bananaptr->nx-1,0,bananaptr->ny-1,0,bananaptr->nz-1);
    outptr->in_side_allvox[iw]=d3tensor(0,bananaptr->nx-1,0,bananaptr->ny-1,0,bananaptr->nz-1);
  }
  for (ix=0;ix<bananaptr->nx;++ix)
    for (iy=0;iy<bananaptr->ny;++iy)
       for (iz=0;iz<bananaptr->nz;++iz)
         for (iw=0;iw<num_sides;++iw)
         {
	   outptr->out_side_allvox[iw][ix][iy][iz]=0.0; // CKH 09jan31 agree with c# and linux
	   outptr->in_side_allvox[iw][ix][iy][iz]=0.0;
	  }
   bananaptr->banana_photons=0;
   outptr->wt_pathlen_out_top=0.0;
   outptr->wt_pathlen_out_bot=0.0;
   outptr->wt_pathlen_out_sides=0.0;
   outptr->R_rt[0][0]=0.0;
}

/**************************************************************/
void Compute_Prob_allvox()
{
  int debug=0,i,j,k,m,iw,ktrk,jfix,num,N,in_layer,curr_layer,dead=0;
  int ix,iy,iz;
  int nx,ny,nz,side,next_same_vox,exit;
  double this_x,this_y,this_z,next_x,next_y,next_z;
  double w=1.0,dx,dy,dz,xmid,ymid,zmid,xmid2,ymid2,zmid2;
  double s[6],mins,dist_in_vox,tracklen;
  double phot_disc_wt;
  double mu;
  int bdry_col=0;
  struct vox_list *this_vox=NULL,*head;
  double MIN_X,MAX_X,MIN_Y,MAX_Y,MIN_Z,MAX_Z;
  MIN_X=-detector->nr*detector->dr-detector->dr/2;	  
  MAX_X= detector->nr*detector->dr+detector->dr/2;	  
  MIN_Y=-detector->nr*detector->dr-detector->dr/2;	  
  MAX_Y= detector->nr*detector->dr+detector->dr/2;	  
  MIN_Z=0.0;	  
  MAX_Z= detector->nz*detector->dz;	  
  dx=detector->dr;
  dy=detector->nr*detector->dr+detector->dr;
  dz=detector->dz;
  nx=bananaptr->nx;
  ny=bananaptr->ny;
  nz=bananaptr->nz;
  N=source->num_photons;
  num=histptr->num_pts_stored;
  ++bananaptr->banana_photons;
  phot_disc_wt=1.0;  
  in_layer=0;
  exit=0;  /* this needs to be 0 so that for/adj work simultaneously */
  for(ktrk=0;ktrk<num-1;++ktrk)  {  /* for all tracks */
    if (!dead) {
      this_x=histptr->xh[ktrk];
      this_y=histptr->yh[ktrk];
      this_z=histptr->zh[ktrk];
      next_x=histptr->xh[ktrk+1];
      next_y=histptr->yh[ktrk+1];
      next_z=histptr->zh[ktrk+1];
      tracklen=sqrt((next_x-this_x)*(next_x-this_x)+
                    (next_y-this_y)*(next_y-this_y)+
                    (next_z-this_z)*(next_z-this_z));
      xmid=this_x;
      ymid=this_y;
      zmid=this_z;
      next_same_vox=0;
      /* DEBUG */
      /* if ((bananaptr->banana_photons>=1041)&&(ktrk>=270)) debug=1; 
      else debug=0;     */
      if (debug) { 
        printf("N %3d TRACK %5d: this=(%15.10f,%15.10f,%15.10f)\n",
        bananaptr->banana_photons,ktrk,this_x,this_y,this_z);
        printf("                 : next=(%15.10f,%15.10f,%15.10f)\n",
        next_x,next_y,next_z);
	printf("phot_dist_wt=%f\n",phot_disc_wt);
      } 
      /* flag if out sides */
      if ((fabs(next_x)>MAX_X)||(fabs(next_y)>MAX_Y)) {
	in_layer=0;
	if (debug) printf("outside of layer \n");
      }  /* if out sides */
      else { /* not out sides */
        histptr->boundary_col[ktrk]=0;
	for (i=1;i<=tissptr->num_layers;i++) {
	/* following based on "this" s.t. check later for weights agrees */
	  if (fabs(this_z-tissptr->layerprops[i].zbegin)<1e-9) {
	    histptr->boundary_col[ktrk]=1;
	    if (debug) printf("boundary collision!\n");
	  }
        }
        if (!in_layer) { /* check if enter layer */
	  if ((MIN_Z==0.0)&&(ktrk==0)) { /* layer at origin */
	    /* NOTE: this may not handle if passes thru 1st voxel */
	    in_layer=1;
	    ix=floor((this_x-MIN_X)/dx);
	    iy=floor((this_y-MIN_Y)/dy);
	    iz=0;
	    side=0;
            /* adjoint */
	    outptr->in_side_allvox[ix][iy][iz][side]+=phot_disc_wt; 
	  } 
	  else { /* layer not at origin */  
            s[0]=0;
            if ((this_z<=MIN_Z)&&(next_z>MIN_Z))  
	       s[0]=(MIN_Z-this_z)/(next_z-this_z);
            else if ((this_z>MAX_Z)&&(next_z<MAX_Z))  
	       s[0]=(MAX_Z-this_z)/(next_z-this_z);
	    /* determine point of intersection */
	    if ((s[0]>0)&&(s[0]<=1)) {  /* crossed into layer */
	      xmid=this_x+s[0]*(next_x-this_x);
	      ymid=this_y+s[0]*(next_y-this_y);
	      zmid=this_z+s[0]*(next_z-this_z);
	      /* NOTE: ix,iy,iz always min of voxel */
	      ix=floor((xmid-MIN_X)/dx);
	      iy=floor((ymid-MIN_Y)/dy);
	      if (next_z>this_z) { /* enter from top */
	        iz=0;
		side=0;
                /* adjoint */
		if (in_layer)
	          outptr->in_side_allvox[ix][iy][iz][side]+=phot_disc_wt; 
              }
	      else  { /* enter from bottom */
	        iz=nz-1;
		side=5;
                /* adjoint */
		if (in_layer)
	          outptr->in_side_allvox[ix][iy][iz][side]+=phot_disc_wt; 
              }
	      in_layer=1;
	    }  /* crossed into layer */
	  } /* layer not at origin */
          if ((debug)&&(in_layer)) {
	    printf("enter layer at (x,y,z)=(%f,%f,%f)\n",xmid,ymid,zmid);
	    printf("enter layer at (ix,iy,iz)=(%d,%d,%d)\n",ix,iy,iz); }
          } /* check if enter layer */
        else { /* check if exit layer */
	  /* NOTE: this may not handle if passes thru edge voxel */
          s[0]=0;
          if ((this_z>MIN_Z)&&(next_z<=MIN_Z))  
	    s[0]=(MIN_Z-this_z)/(next_z-this_z);
          else if ((this_z<MAX_Z)&&(next_z>MAX_Z))  
	    s[0]=(MAX_Z-this_z)/(next_z-this_z);
	  /* determine point of intersection */
	  if ((s[0]>0)&&(s[0]<=1)) {  /* crossed out of layer */
	    xmid=this_x+s[0]*(next_x-this_x);
	    ymid=this_y+s[0]*(next_y-this_y);
	    zmid=this_z+s[0]*(next_z-this_z);
	    ix=floor((xmid-MIN_X)/dx);
	    iy=floor((ymid-MIN_Y)/dy);
	    if (next_z>this_z) { /* exit bottom */
	      iz=nz-1;
	      side=5;
              mu=(next_z-this_z)/tracklen;
	      if (in_layer) {
                if (fabs(mu)>MU_LB)
	          outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/mu; 
	        else
	          outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/(MU_LB/2); 
              }
	    }
            else { /* exit top */
	      iz=0;
	      side=0;
              mu=-(next_z-this_z)/tracklen;
	      if (in_layer) {
                if (fabs(mu)>MU_LB)
	          outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/mu;
                else
	          outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/(MU_LB/2);
              }
            }
	    in_layer=0;
	    if (debug) {
	    printf("out[side=%d,%d,%d,%d]=%e\n",side,ix,iy,iz,
			    outptr->out_side_allvox[ix][iy][iz][side]);
	    printf("exit layer at (x,y,z)=(%f,%f,%f)\n",xmid,ymid,zmid);
	    printf("exit: dist_in_vox(%d,%d,%d)=%f\n",ix,iy,iz,
	                sqrt((this_x-xmid)*(this_x-xmid)+
	                     (this_y-ymid)*(this_y-ymid)+
	                     (this_z-zmid)*(this_z-zmid))); 
	    }
	  } /* crossed out of layer */
        } /* check if exit layer */
        while ((!next_same_vox)&&(in_layer)&&(!dead)) {
          /* if boundary upward boundary collision first segment, save exiting wt */
          if ((histptr->boundary_col[ktrk]==1)&&
			  (this_z>next_z)&&(this_z==zmid)) {
	    if (fabs(mu)>MU_LB)
	      outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/mu;
	    else
	      outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/(MU_LB/2);
	    if (debug) 
	    printf("out[side=%d,%d,%d,%d]=%e\n",side,ix,iy,iz,
			    outptr->out_side_allvox[ix][iy][iz][side]);
            --iz;
          }
          /* check if ended in this voxel */
	  if (debug) printf("floor((next_x-MIN_X)/dx)=%4.1f ix=%d\n",
	                     floor((next_x-MIN_X)/dx),ix);
	  if (debug) printf("floor((next_y-MIN_Y)/dy)=%4.1f iy=%d\n",
	                     floor((next_y-MIN_Y)/dy),iy);
	  if (debug) printf("floor((next_z-MIN_Z)/dz)=%4.1f iz=%d\n",
	                     floor((next_z-MIN_Z)/dz),iz);
	  if ( (floor((next_x-MIN_X)/dx)==(double)ix)&&
	       (floor((next_y-MIN_Y)/dy)==(double)iy)&&
	       (floor((next_z-MIN_Z)/dz)==(double)iz) ) { /* floor fix */
            next_same_vox=1; 
	    if (debug) {
	      printf("while:track ended in vox\n"); 
	      printf("while:dist_in_vox(%d,%d,%d)=%f\n",ix,iy,iz,
	                sqrt((next_x-xmid)*(next_x-xmid)+
	                     (next_y-ymid)*(next_y-ymid)+
	                     (next_z-zmid)*(next_z-zmid))); 
	    }
          } /* track ended in vox */
          else { /* check which face track exited */
          /* i of s[i] based on 0=top,1=front,2=left,3=back,4=right,5=bot */
            if (debug) printf("fmod(xmid-MIN_X,dx)=%f y=%f z=%f\n",
              fmod(xmid-MIN_X,dx),fmod(ymid-MIN_Y,dy),fmod(zmid-MIN_Z,dz));
	    if ((fmod(xmid-MIN_X,dx)<1e-10)|| /* on y-z face */
	      (fabs(fmod(xmid-MIN_X,dx)-dx)<1e-10)) { 
	      if (debug) printf("on y-z face\n");
	      if (next_x>this_x) {
	        s[2]=99;
                s[4]=(MIN_X+(ix+1)*dx-xmid)/(next_x-xmid);
	      }
              else {
                s[2]=(MIN_X+(ix)*dx-xmid)/(next_x-xmid);
	        s[4]=99;
	      }
              s[3]=(MIN_Y+iy*dy-ymid)/(next_y-ymid);
              s[1]=(MIN_Y+(iy+1)*dy-ymid)/(next_y-ymid);
              s[0]=(MIN_Z+iz*dz-zmid)/(next_z-zmid);
              s[5]=(MIN_Z+(iz+1)*dz-zmid)/(next_z-zmid);
	    }
	    else if ((fmod(ymid-MIN_Y,dy)<1e-10)||  /* on x-z face */
	        (fabs(fmod(ymid-MIN_Y,dy)-dy)<1e-10)) { 
	      if (debug) printf("on x-z face\n");
              s[2]=(MIN_X+ix*dx-xmid)/(next_x-xmid);
              s[4]=(MIN_X+(ix+1)*dx-xmid)/(next_x-xmid);
	      if (next_y>this_y) {
	        s[3]=99;
                s[1]=(MIN_Y+(iy+1)*dy-ymid)/(next_y-ymid);
              }
	      else {
                s[3]=(MIN_Y+(iy)*dy-ymid)/(next_y-ymid);
	        s[1]=99;
              }
              s[0]=(MIN_Z+iz*dz-zmid)/(next_z-zmid);
              s[5]=(MIN_Z+(iz+1)*dz-zmid)/(next_z-zmid);
	    } 
	    else if ((fmod(zmid-MIN_Z,dz)<1e-10)||  /* on x-y face */
	        (fabs(fmod(zmid-MIN_Z,dz)-dz)<1e-10)) { 
	      if (debug) printf("on x-y face\n");
                s[2]=(MIN_X+ix*dx-xmid)/(next_x-xmid);
                s[4]=(MIN_X+(ix+1)*dx-xmid)/(next_x-xmid);
                s[3]=(MIN_Y+iy*dy-ymid)/(next_y-ymid);
                s[1]=(MIN_Y+(iy+1)*dy-ymid)/(next_y-ymid);
	        if (next_z>this_z) {
                  s[0]=99;
                  s[5]=(MIN_Z+(iz+1)*dz-zmid)/(next_z-zmid);
	        }
	        else {
                  s[0]=(MIN_Z+(iz)*dz-zmid)/(next_z-zmid);
	          s[5]=99;
	        }
            }
            else { /* this interior to voxel */
	      if (debug) printf("interior to voxel\n"); 
              s[2]=(MIN_X+ix*dx-xmid)/(next_x-xmid);
              s[4]=(MIN_X+(ix+1)*dx-xmid)/(next_x-xmid);
              s[3]=(MIN_Y+iy*dy-ymid)/(next_y-ymid);
              s[1]=(MIN_Y+(iy+1)*dy-ymid)/(next_y-ymid);
              s[0]=(MIN_Z+(iz)*dz-zmid)/(next_z-zmid);
              s[5]=(MIN_Z+(iz+1)*dz-zmid)/(next_z-zmid);
            }
            mins=99;
	    jfix=-1;
            for (j=0;j<6;++j) {
              if ((s[j]>0)&&(s[j]<=1+1e-10)) { /* hit plane */  
	         if (fabs(s[j])>1e-10) { 
                   if (s[j]<=mins) {
	             mins=s[j]; 
		     jfix=j;
		   }
	         }
	      }
            } 
            if ((mins==99)&&(histptr->boundary_col[ktrk]==0)) {
	      printf("WARNING: N=%d trk=%d mins==99 x,y,z=%f,%f,%f\n",
	         bananaptr->banana_photons,ktrk,this_x,this_y,this_z);
	      dead=1;
	    } 
	    side=jfix;
	    switch (jfix) {
	      case 0: mu=-(next_z-this_z)/tracklen; break;
	      case 1: mu=(next_y-this_y)/tracklen; break;
	      case 2: mu=-(next_x-this_x)/tracklen; break;
	      case 3: mu=-(next_y-this_y)/tracklen; break;
	      case 4: mu=(next_x-this_x)/tracklen; break;
	      case 5: mu=(next_z-this_z)/tracklen; break;
	    }
	    xmid2=xmid+mins*(next_x-xmid);
	    ymid2=ymid+mins*(next_y-ymid);
	    zmid2=zmid+mins*(next_z-zmid);
	    if (debug) {
	      printf("jfix=%d s=(%7.5f,%7.5f,%7.5f,%7.5f,%7.5f,%7.5f)\n",
	        jfix,s[0],s[1],s[2],s[3],s[4],s[5]); 
	      printf("intra layer at (x,y,z)=(%7.5f,%7.5f,%7.5f)\n",
	        xmid2,ymid2,zmid2); 
	      printf("intra:dist_in_vox(%d,%d,%d)=%f\n",ix,iy,iz,
	                sqrt((xmid2-xmid)*(xmid2-xmid)+
	                     (ymid2-ymid)*(ymid2-ymid)+
	                     (zmid2-zmid)*(zmid2-zmid))); 
            }
	    /* save weights since exiting current voxel */
	    if ( (ix>=nx)||(ix<0)||(iy>=ny)||(iy<0)||(iz>=nz)||(iz<0) )
	      in_layer=0;
	    if (in_layer) {
	      if (fabs(mu)>MU_LB)
	        outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/mu;
	      else
	        outptr->out_side_allvox[ix][iy][iz][side]+=phot_disc_wt/(MU_LB/2);
            }
	    if (debug) printf("out[side=%d,%d,%d,%d]=%e\n",side,ix,iy,iz,
			    outptr->out_side_allvox[ix][iy][iz][side]);
	    /* move to the next voxel */
	    switch (jfix) {
	      case 0: --iz; if (!exit) side=5; break;
	      case 1: ++iy; if (!exit) side=3; break;
	      case 2: --ix; if (!exit) side=4; break;
	      case 3: --iy; if (!exit) side=1; break;
	      case 4: ++ix; if (!exit) side=2; break;
	      case 5: ++iz; if (!exit) side=0; break;
	    }
	    if (debug) printf("next (ix,iy,iz)=(%d,%d,%d)\n",ix,iy,iz);
	    if ( (ix>=nx)||(ix<0)||(iy>=ny)||(iy<0)||(iz>=nz)||(iz<0) )
	      in_layer=0;
	    /* adjoint: save weights since entering voxel */
	    if (in_layer) {
                outptr->in_side_allvox[ix][iy][iz][side]+=phot_disc_wt; 
	    }
            xmid=xmid2;
            ymid=ymid2;
            zmid=zmid2;
	  } /* else crossed voxel face */
        } /* while not at end of track or dead */
      } /* not out sides */
      /* only deweight non-boundary collisions */
      if ((histptr->boundary_col[ktrk]==0)||(ktrk==0)) {
        curr_layer=0;
        for (i=1;i<=tissptr->num_layers;i++) {
          if ( (this_z>=tissptr->layerprops[i].zbegin)&& /* must >= */
               (this_z<=tissptr->layerprops[i].zend) )
             curr_layer=i;
        }
        phot_disc_wt*=tissptr->layerprops[curr_layer].mus/
            (tissptr->layerprops[curr_layer].mus+
             tissptr->layerprops[curr_layer].mua);
      } /* if not boundary collision */
      else
        ++bdry_col;
    } /* if not dead */
  } /* for all tracks */
}
/**************************************************************/
void Add_To_List(struct vox_list **this_vox, 
                int ix, int iy, int iz, int col, int sa_idx)
{
  struct vox_list *temp;
  if ((temp=malloc(sizeof(*temp)))==NULL) 
    nrerror("allocation failure in Add_To_List()");
  temp->ix=ix;
  temp->iy=iy;
  temp->iz=iz;
  temp->col=col;
  temp->sa_idx=sa_idx;
  temp->next=*this_vox;
  *this_vox=temp;
  /* printf("Add end: *this_vox=%d ix,iy,iz=%d,%d,%d\n",
    *this_vox,ix,iy,iz);  */
}
/************************************************************/
void Free_List(struct vox_list *head)
{
  struct vox_list *temp;
  /* printf("Free_List: head=%d\n",head);  */
  while (head!=NULL) {
    temp=head->next;
    free(head);
    head=temp;
  }
}
/************************************************************/
void Output_Wts_allvox(void)
 {
   FILE *ofp[6];
   int ix,iy,iz,iw,N,num_sides=6;
   char tmp[256];
   /* QFIX: assume symmetry -> use forward for adjoint */
   double src_NA=source->src_NA,det_NA=src_NA;
   double dx,dy,dz,delmu,delphi,Rhoog_norm;
   double Asrc,Adet,n=tissptr->layerprops[1].n;
   /* delmu=1.0/bananaptr->num_mu;
   delphi=2*PI/bananaptr->num_phi; */
   delmu=1.0; /* assume 1 angular bin now */
   delphi=2*PI;
   dx=detector->dr;
   dy=detector->dr;
   dz=detector->dz;
   N=source->num_photons;
   /* NOTE: norm/denom(dx*dy*dz*dmu*dphi) factor of adjoint files */
     for (iw=0;iw<num_sides;++iw) {
       sprintf(tmp,"%s%i","wts_out_side",iw);
       ofp[iw]=fopen(tmp,"w");
       for(iz=0;iz<bananaptr->nz;++iz) {
         for(ix=0;ix<bananaptr->nx;++ix) {
           for(iy=0;iy<bananaptr->ny;++iy) {
             fprintf(ofp[iw],"%.6e ",
               outptr->out_side_allvox[iw][ix][iy][iz]); // CKH 09jan31 make consist with c#
           } /* for iy */
         } /* for ix */
         fprintf(ofp[iw],"\n");
       } /* for iz */
       fclose(ofp[iw]);
     } /* for iw */
     if (source->beam_radius==0.0)
       Asrc=1;
     else
       Asrc=PI*source->beam_radius*source->beam_radius;
     /* Adet=PI*detector->det_rad*detector->det_rad; */
     Adet=Asrc;
     Rhoog_norm=Asrc*2*PI*(1-sqrt(1-(src_NA/n)*(src_NA/n)))*
	        Adet*2*PI*(1-sqrt(1-(det_NA/n)*(det_NA/n)));
     //printf("in_side[0][200][0][0]=%e\n",outptr->in_side_allvox[0][200][0][0]);
     for (iw=0;iw<num_sides;++iw) {
       sprintf(tmp,"%s%i","wts_in_side",iw);
       ofp[iw]=fopen(tmp,"w");
       for(iz=0;iz<bananaptr->nz;++iz) {
         for(ix=0;ix<bananaptr->nx;++ix) {
           for(iy=0;iy<bananaptr->ny;++iy) {
             if ((iw==0)||(iw==5)) {
               fprintf(ofp[iw],"%.6e ",
                 Rhoog_norm*outptr->in_side_allvox[iw][ix][iy][iz]/
		 (delmu*delphi*dx*dy*N*N));
                
             }
             else if ((iw==1)||(iw==3)) {
               fprintf(ofp[iw],"%.6e ",
                 Rhoog_norm*outptr->in_side_allvox[iw][ix][iy][iz]/
		 (delmu*delphi*dx*dz*N*N));
	     }
             else if ((iw==2)||(iw==4)) {
               fprintf(ofp[iw],"%.6e ",
                 Rhoog_norm*outptr->in_side_allvox[iw][ix][iy][iz]/
		 (delmu*delphi*dy*dz*N*N));
             }
           } /* for iy */
         } /* for ix */
         fprintf(ofp[iw],"\n");
       } /* for iz */
       fclose(ofp[iw]);
     } /* for iw */
 }
