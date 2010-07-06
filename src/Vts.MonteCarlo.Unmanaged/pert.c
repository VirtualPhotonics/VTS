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
/* extern struct g_perturb *g_pertptr; */
extern struct perturb *pertptr;
extern struct History *histptr;
extern struct SourceDefinition *source;
extern struct DetectorDefinition *detector;
/******************************************************/

//void pert()
//{
//  int i,j,k;
//  int ti,r_bin;
//  double prev_x,prev_y,prev_z;
//  double this_x,this_y,this_z;
//  double next_x,next_y,next_z;
//  int num_scats,killed;
//  double col_score;
//
//  pertptr->tot_phot++;
//  Display_Status(10000);  
//  /* num_scats is index of collision out surface */
//  num_scats=histptr->num_pts_stored - 1;
//  /* determine if last collision out detector */
//  r_bin=In_Detector(histptr->xh[num_scats],
//     histptr->yh[num_scats],histptr->zh[num_scats],
//     0.0,detector->det_rad); 
//  if (r_bin!=-1)  /* photon exited detector */
//    {
//      /* printf("last z=%5.3f\n",histptr->zh[num_scats]); */
//      //pertptr->col_in_pert=0;
//      //pertptr->len_in_pert=0;
//      //pertptr->track_in_pert=0;
//      pertptr->col_hit_bdry=0;
//      for (i=1;i<=tissptr->num_layers;++i)
//	  {
//		pertptr->col_in_layer[i]=0;
//		pertptr->pathlen_in_layer[i]=0.0;
//	  }
//      k=0;
//      killed=0;
//      while ((k<num_scats) && (!killed))
//        { /* Get two consecutive positions */
//          this_x=histptr->xh[k]; 
//          this_y=histptr->yh[k];
//          this_z=histptr->zh[k];
//          next_x=histptr->xh[k+1];
//          next_y=histptr->yh[k+1];
//          next_z=histptr->zh[k+1];
//          /* base all calcs on this_* (rather than next_*) */
//          for (i=1;i<=tissptr->num_layers;++i)
//            {  /* check if truly inside layer and not bdry point */
//              if ( ((this_z>tissptr->layerprops[i].zbegin)&&
//                  (this_z<tissptr->layerprops[i].zend)) &&
//		 ((fabs(this_z-tissptr->layerprops[i].zbegin)>1e-9)&&
//		  (fabs(this_z-tissptr->layerprops[i].zend)>1e-9)) )
//	        ++pertptr->col_in_layer[i];
//            }
//          /* if ((int)pertptr->tot_phot!=-99)   
//           printf("Phot=%5i col=%5i x=%e y=%e z=%e hit=%i trk=%i\n", 
//           (int)pertptr->tot_phot,k,this_x,this_y,this_z,     
//           histptr->boundary_col[k],pertptr->track_in_pert); */
//        
//          /* determine col_InEllipsoid,len_InEllipsoid,track_InEllipsoid */
//          if (k!=0)
//            {
//              prev_x=histptr->xh[k-1];
//              prev_y=histptr->yh[k-1];
//              prev_z=histptr->zh[k-1];
//              /* only perturb non-boundary collisions */
//              if (histptr->boundary_col[k]==0)
//                Pert_Col(this_x,this_y,this_z);
//              else
//                ++pertptr->col_hit_bdry;
//              Pert_Path_Len(prev_x,prev_y,prev_z,this_x,this_y,this_z,k); 
//            }
//          ++k;
//        } /* while scattering and not killed  */
//      /* pert last track segment */
//      Pert_Path_Len(this_x,this_y,this_z,next_x,next_y,next_z,k); 
//      /* printf("Phot=%5i col=%5i x=%e y=%e z=%e hit=%i\n", 
//           (int)pertptr->tot_phot,k,next_x,next_y,next_z,     
//           histptr->boundary_col[k]);        
//      printf("pert:k=%i pertptr->col_in_pert=%i\n",k,pertptr->col_in_pert); 
//      printf("pert:k=%i pertptr->track_in_pert=%i\n",k,pertptr->track_in_pert); 
//      printf(" x=%e\n",histptr->xh[histptr->num_pts_stored-1]);
//      printf(" y=%e\n",histptr->yh[histptr->num_pts_stored-1]);
//      printf(" z=%e\n",histptr->zh[histptr->num_pts_stored-1]);
//      printf(" uz=%e\n", 
//        histptr->uzh[histptr->num_pts_stored-1]);     */
//
//      //Write_Photon_To_Disk(r_bin);   /* comment out to not gen db */
//    } /* if out detector */
//}

/*********************************************/
void init_pert()
{
  int i;
  /* nr is the number of radial bins */
  if (detector->nr==0)
    {
      printf("Warning: No detectors specified\n");
      detector->nr=1;
    }
  if (detector->det_rad==0.0)
    {
      printf("Warning: Zero detector radius specified\n");
      detector->nr=1;
    }
  /* init counters */
  pertptr->tot_out_top=0;
  pertptr->tot_out_bot=0;
}
