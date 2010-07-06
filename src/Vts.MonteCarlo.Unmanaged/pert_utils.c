#include <stdio.h>
#include <math.h>
#include <string.h>
#include <float.h>
#include <time.h>

#include "protos.h"
#include "pert.h"
#include "mc_main.h"
#include "nrutil.h"

/********GLOBAL variables *****************************/
extern struct Photon *photptr;
extern struct Tissue *tissptr;
extern struct Output *outptr;
extern struct perturb *pertptr;
extern struct History *histptr;
extern struct SourceDefinition *source;
extern struct DetectorDefinition *detector;
/******************************************************/

/**********************************************************/
int In_Detector(double x, double y, double z, double r1, double r2)
{
  /* output is which detector bin (x,y) is in (-1=no bin) */
  int i,j,bin=-1;
  int xy_incircle=0;
  double slab_thick=0.0; 

  for (j=1;j<=tissptr->num_layers;++j)
    slab_thick+=tissptr->layerprops[j].d;
  for (i=0;i<detector->nr;++i) {
    if ( (sqrt((x-detector->det_ctr[i])*
               (x-detector->det_ctr[i])+y*y)>=r1) &&
         (sqrt((x-detector->det_ctr[i])*
               (x-detector->det_ctr[i])+y*y)<=r2) )
        xy_incircle=1;
    else
      xy_incircle=0;
    if (detector->reflect_flag) {
      if (fabs(z)<1e-9)  { /* allow for small z's */
        ++pertptr->tot_out_top;
        if (xy_incircle) 
          bin=i;
      }
      else {
        if (fabs(z-slab_thick)<1e-9)
        ++pertptr->tot_out_bot;
      }
    }
    else  { /* transmission */
      if (fabs(z-slab_thick)<1e-9) {
        ++pertptr->tot_out_bot;
        if (xy_incircle) 
          bin=i;
      }
      else {
        if (fabs(z)<1e-9) 
	  ++pertptr->tot_out_top;
      }
    }
  }
 /* printf("In_Det: x,y,z=%f,%f,%f bin=%i r1=%f r2=%f\n",
        x,y,z,bin,r1,r2);   
 printf("In_Det: out_top=%d out_bot=%d\n",pertptr->tot_out_top,
		 pertptr->tot_out_bot); */
  return bin;
}
/**********************************************************/
int InEllipsoid(double x, double y, double z)
{
	double inside = 3;
  /*returns 1 if inside, 0 if outside, 3 if on the boundary, 2 if ray_ellip=0*/

  /* NOTE: setting any ellipsoid radius=0 is equivalent to performing */
  /* the perturbation with the whole space as the ellipsoid or */
  /* performing the background calculations with the perturbed data */

  /* check if any radius=0 */
  if ((tissptr->ellip_rad_x==0.0) || 
      (tissptr->ellip_rad_y==0.0) || 
      (tissptr->ellip_rad_z==0.0))
    {
      return 2;  /* 2 means treat whole space as ellipsoid */
    }
  else
  {
	  inside = (x-tissptr->ellip_x)*(x-tissptr->ellip_x)/
                (tissptr->ellip_rad_x*tissptr->ellip_rad_x)+
          (y-tissptr->ellip_y)*(y-tissptr->ellip_y)/
                (tissptr->ellip_rad_y*tissptr->ellip_rad_y)+
          (z-tissptr->ellip_z)*(z-tissptr->ellip_z)/
                (tissptr->ellip_rad_z*tissptr->ellip_rad_z);
      //printf("inside: %f \n ux=%f, uy=%f, uz=%f",inside, photptr->ux, photptr->uy, photptr->uz); =========================================================cancella

	  if (inside < 0.99999999999) { // CKH FIX 11/11/08 added 4 more 9's 
		  return 1;}
	  else if (inside > 1.00000000001){
		  return 0;}
	  else {//if (inside == 1.0)
		  return 3;}
    }
}

//int InEllipsoid(double x, double y, double z)
//{
//  /* NOTE: setting any ellipsoid radius=0 is equivalent to performing */
//  /* the perturbation with the whole space as the ellipsoid or */
//  /* performing the background calculations with the perturbed data */
//
//  /* check if any radius=0 */
//  if ((tissptr->ellip_rad_x==0.0) || 
//      (tissptr->ellip_rad_y==0.0) || 
//      (tissptr->ellip_rad_z==0.0))
//    {
//      return 2;  /* 2 means treat whole space as ellipsoid */
//    }
//  else
//    {
//      if ((x-tissptr->ellip_x)*(x-tissptr->ellip_x)/
//                (tissptr->ellip_rad_x*tissptr->ellip_rad_x)+
//          (y-tissptr->ellip_y)*(y-tissptr->ellip_y)/
//                (tissptr->ellip_rad_y*tissptr->ellip_rad_y)+
//          (z-tissptr->ellip_z)*(z-tissptr->ellip_z)/
//                (tissptr->ellip_rad_z*tissptr->ellip_rad_z) < 1.0)  
//        return 1;
//      else 
//        return 0;
//    }
//}

/**********************************************************/
//void Pert_Col(double this_x,double this_y,double this_z)
//{
//  int i,col_InEllipsoid;
//
//  switch (tissptr->do_ellip_layer)
//  {
//    case 0:   /* no perturbation */
//      break;
//    case 1:   /* ellipsoidal perturbation */
//      col_InEllipsoid=InEllipsoid(this_x,this_y,this_z);
//      /* perturb collision weight */
//      if ((col_InEllipsoid==1) || (col_InEllipsoid==2)) 
//        /* 1 means this_ is in ellipsoid */
//        /* 2 means all points in ellipsoid (whole space=ellipsoid) */
//        {
//          ++pertptr->col_in_pert;
//        }
//      break;
//    case 2:   /* layer perturbation */
//      /* do layer pert THIS NEEDS TO MATCH ray_intersect_layer! */
//      if ((this_z>=tissptr->layer_z_min) && (this_z<=tissptr->layer_z_max))
//        ++pertptr->col_in_pert;
//      break;
//  }
//}
/**********************************************************/
//void Pert_Path_Len(double prev_x,double prev_y,double prev_z,
//                   double this_x,double this_y,double this_z,
//                   int col)
//{
//  int j;
//  double in_pert_dist=0.0;
//  double out_pert_dist=0.0;
//  double slab_thick;
//  switch (tissptr->do_ellip_layer)
//  {
//    case 0:  /* no perturbation */
//      break;
//    case 1:  /* ellipsoidal pert */
//      Ray_Intersect_Ellip(prev_x,prev_y,prev_z,
//           this_x,this_y,this_z,&in_pert_dist,&out_pert_dist);
//      break;
//    case 2:  /* layer pert */
//      Ray_Intersect_Layer(prev_x,prev_y,prev_z,
//                      this_x,this_y,this_z,&in_pert_dist,&out_pert_dist);
//      break;
//  }
//  /* perturb path length weight */
//  if (in_pert_dist>0.0)
//    {
//      pertptr->len_in_pert+=in_pert_dist;
//      /* don't increase track_in_pert for reflecting col within pert */
//      for (j=1;j<=tissptr->num_layers;++j)
//        slab_thick+=tissptr->layerprops[j].d;
//      if ((col!=histptr->num_pts_stored-1) &&
//          ((fabs(this_z)<1e-9) || (fabs(this_z-slab_thick)<1e-9)))
//        { } 
//       else 
//          ++pertptr->track_in_pert;
//    }
//}
/**********************************************************/
void Ray_Intersect_Layer(double x1, double y1, double z1,
                     double x2, double y2, double z2,
                     double *dist_in_layer, double *dist_out_layer)
{
  int in_layer=0; /* 0=neither, 1=1only, 2=2only, 3=both */
  double r_parm,r2_parm;
  double xint,yint,zint,xint2,yint2,zint2,dist_1to2;

  /* printf("Ray_Int_Layer: 1=%e,%e,%e 2=%e,%e,%e\n",
        x1,y1,z1,x2,y2,z2);  */
  /* determine distance from (x1,y1,z1) to (x2,y2,z2) */
  dist_1to2=sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2)+
                 (z1-z2)*(z1-z2));
  /* check if prev in layer */
  if ((z1>=tissptr->layer_z_min) && (z1<=tissptr->layer_z_max))
    in_layer+=1;
  if ((z2>=tissptr->layer_z_min) && (z2<=tissptr->layer_z_max))
    in_layer+=2;
  switch (in_layer) {
  case 3: /* both in layer */
    *dist_in_layer=dist_1to2;
    *dist_out_layer=0.0;
    /* printf("Ray_Int_Layer: both in\n");  */
    break;
  case 2:  /* 2 in only: cross into layer */
    if (z1<tissptr->layer_z_min) /* from above */
      r_parm=(tissptr->layer_z_min-z1)/(z2-z1);
    else  /* from below */
      r_parm=(tissptr->layer_z_max-z1)/(z2-z1);
    xint=x1+(x2-x1)*r_parm;
    yint=y1+(y2-y1)*r_parm;
    zint=z1+(z2-z1)*r_parm;
    /* portion of track in pert is latter part */
    *dist_in_layer=sqrt((x2-xint)*(x2-xint)+
                        (y2-yint)*(y2-yint)+
                        (z2-zint)*(z2-zint));
    *dist_out_layer=dist_1to2-(*dist_in_layer);
    /* printf("Ray_Int_Layer: prev out, this in int=%e,%e,%e\n",
        xint,yint,zint);  */
    break;
  case 1: /* 1 in only: cross out of layer */
    if (z2<tissptr->layer_z_min) /* from below */
       r_parm=(tissptr->layer_z_min-z1)/(z2-z1);
    else  /* from above */
       r_parm=(tissptr->layer_z_max-z1)/(z2-z1);
    xint=x1+(x2-x1)*r_parm;
    yint=y1+(y2-y1)*r_parm;
    zint=z1+(z2-z1)*r_parm;
    /* portion of track in pert is first part */
    *dist_in_layer=sqrt((x1-xint)*(x1-xint)+
                        (y1-yint)*(y1-yint)+
                        (z1-zint)*(z1-zint));
    *dist_out_layer=dist_1to2-(*dist_in_layer);
    /* printf("Ray_Int_Layer: prev in, this out int=%e,%e,%e\n",
      xint,yint,zint);  */
    break; 
  case 0: /* both out: check if through layer */
    if ( ((z1<tissptr->layer_z_min) && (z2>tissptr->layer_z_max)) ||
         ((z1>tissptr->layer_z_max) && (z2<tissptr->layer_z_min)) ) 
      { 
        /* check if through from above */
        if ((z1<tissptr->layer_z_min) && (z2>tissptr->layer_z_max)) 
          {
            r_parm=(tissptr->layer_z_min-z1)/(z2-z1);
            r2_parm=(tissptr->layer_z_max-z1)/(z2-z1);
          }
        /* check if through from below */
        if ((z1>tissptr->layer_z_max) && (z2<tissptr->layer_z_min)) 
          {
            r_parm=(tissptr->layer_z_max-z1)/(z2-z1);
            r2_parm=(tissptr->layer_z_min-z1)/(z2-z1);
          }
        xint=x1+(x2-x1)*r_parm;
        yint=y1+(y2-y1)*r_parm;
        zint=z1+(z2-z1)*r_parm;
        xint2=x1+(x2-x1)*r2_parm;
        yint2=y1+(y2-y1)*r2_parm;
        zint2=z1+(z2-z1)*r2_parm;
        *dist_in_layer=sqrt((xint2-xint)*(xint2-xint)+
                            (yint2-yint)*(yint2-yint)+
                            (zint2-zint)*(zint2-zint));
        *dist_out_layer=dist_1to2-(*dist_in_layer);
        /* printf("Ray_Int_Layer: thru int=%e,%e,%e int2=%e,%e,%e\n",
          xint,yint,zint,xint2,yint2,zint2);  */
      }
    else  /* no intersection with layer */
      {
        *dist_in_layer=0.0;
        *dist_out_layer=dist_1to2;
      }
    break;
  } /* switch */
}
/**********************************************************/
void Ray_Intersect_Ellip(double x1, double y1, double z1,
                     double x2, double y2, double z2,
                     double *dist_InEllipsoid, double *dist_out_ellip)
{
  double A,B,C,root1,root2,root,xto,yto,zto,xfro,yfro,zfro,dist_1to2;
  int one_in,two_in,numint;

  /* determine distance from (x1,y1,z1) to (x2,y2,z2) */
  dist_1to2=sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2)+
                 (z1-z2)*(z1-z2));

  /* determine intersection with ellipsoid */ 
  one_in=InEllipsoid(x1,y1,z1);
  two_in=InEllipsoid(x2,y2,z2);

  /* check if ellipsoid exists */
  if (one_in==2)  /* no ellipsoid exists -> treat as whole space ellipsoid */
    {
      *dist_out_ellip=0.0;
      *dist_InEllipsoid=dist_1to2;
      return;
    }

  *dist_InEllipsoid=0.0;
  if (one_in && two_in)  /* ray within ellipsoid */
    {
      *dist_InEllipsoid=dist_1to2;
    }
  else 
    { 
      /* sanity check dimensions of ellipsoid */
      if ((tissptr->ellip_rad_x == 0.0) ||
          (tissptr->ellip_rad_y == 0.0) ||
          (tissptr->ellip_rad_z == 0.0))
        {
          printf("PERT ERROR: one ellipsoid radial dimension = 0.0\n");
        }
      else
        {
          A=(x2-x1)*(x2-x1)/(tissptr->ellip_rad_x*tissptr->ellip_rad_x)+
            (y2-y1)*(y2-y1)/(tissptr->ellip_rad_y*tissptr->ellip_rad_y)+
            (z2-z1)*(z2-z1)/(tissptr->ellip_rad_z*tissptr->ellip_rad_z);
          B=2*(x2-x1)*(x1-tissptr->ellip_x)/(tissptr->ellip_rad_x*tissptr->ellip_rad_x)+
            2*(y2-y1)*(y1-tissptr->ellip_y)/(tissptr->ellip_rad_y*tissptr->ellip_rad_y)+
            2*(z2-z1)*(z1-tissptr->ellip_z)/(tissptr->ellip_rad_z*tissptr->ellip_rad_z);
          C=(x1-tissptr->ellip_x)*(x1-tissptr->ellip_x)/(tissptr->ellip_rad_x*tissptr->ellip_rad_x)+
            (y1-tissptr->ellip_y)*(y1-tissptr->ellip_y)/(tissptr->ellip_rad_y*tissptr->ellip_rad_y)+
            (z1-tissptr->ellip_z)*(z1-tissptr->ellip_z)/(tissptr->ellip_rad_z*tissptr->ellip_rad_z)-
            1.0;
          if (B*B-4*A*C > 0)  /* roots are real */
            {
              root1=(-B-sqrt(B*B-4*A*C))/(2*A);
              root2=(-B+sqrt(B*B-4*A*C))/(2*A);
              numint=0;
              if ((root1<1)&&(root1>0)) 
                {
                  numint+=1;
                  root=root1;
                }
              if ((root2<1)&&(root2>0)) 
                {
                  numint+=1;
                  root=root2;
                }
              /* printf("root1=%5.3f root2=%5.3f numint=%2i\n",
                root1,root2,numint);   */
              switch (numint) 
                {
                  case 0: /* roots real but no intersection */
                    break;
                  case 1:
                    if (one_in==1) /* exiting ellipsoid */
                      {
                        xfro=x1;
                        yfro=y1;
                        zfro=z1;
                        xto=x1+root*(x2-x1);
                        yto=y1+root*(y2-y1);
                        zto=z1+root*(z2-z1);
                      }
                    else  /* entering ellipsoid */
                      {  /* one_in==0 -> two_in==1 */
                        xfro=x1+root*(x2-x1);
                        yfro=y1+root*(y2-y1);
                        zfro=z1+root*(z2-z1);
                        xto=x2;
                        yto=y2;
                        zto=z2;
                      }
                    *dist_InEllipsoid=sqrt((xto-xfro)*(xto-xfro)+
                              (yto-yfro)*(yto-yfro)+
                              (zto-zfro)*(zto-zfro));
                    /* fprintf(ofp_col,"%5.3f %5.3f %5.3f\n",
                      x1,y1,z1); */
                    break;
                  case 2:  /* went through ellipoid */
                    xfro=x1+root1*(x2-x1);
                    yfro=y1+root1*(y2-y1);
                    zfro=z1+root1*(z2-z1);
                    xto=x1+root2*(x2-x1);
                    yto=y1+root2*(y2-y1);
                    zto=z1+root2*(z2-z1);
                    *dist_InEllipsoid=sqrt((xto-xfro)*(xto-xfro)+
                               (yto-yfro)*(yto-yfro)+
                               (zto-zfro)*(zto-zfro));
                    /* fprintf(ofp_col,"%5.3f %5.3f %5.3f\n",
                      x1,y1,z1); */
                    break;
                } /* switch */
    
            } /* BB-4AC>0 */
          else /* roots imaginary -> no intersection */
            {
            } 
        } /* check on radial axis of ellipsoid */
      } /* intersection with ellipsoid */
  *dist_out_ellip=dist_1to2-(*dist_InEllipsoid);
}
/*****************************************************************/
void Display_Status(int num_phot)
{
  time_t t;
  FILE *ofp_status;
  time(&t);
  if (fmod(pertptr->tot_phot,(double)num_phot) == 0.0)
    {
      ofp_status=fopen("status","w"); 
      fprintf(ofp_status,"Number of photons processed=%i %s\n",
        (int)pertptr->tot_phot,ctime(&t));
      fclose(ofp_status);
    }
}
