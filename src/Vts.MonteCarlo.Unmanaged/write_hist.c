#include <stdlib.h>
#include <math.h>
#include <stdio.h>
#include "mc_main.h"
#include "pert.h"
#include "protos.h"

struct Photon *photptr;
struct Tissue *tissptr;
struct Output *outptr;
struct perturb *pertptr;
struct History *histptr;
struct SourceDefinition *source;
struct DetectorDefinition *detector;

/************************************************************/
void Write_Photon_To_Disk(int r_bin)
{
	int last_pt, write_me_to_disk, i, too_big, tot_col;
	double x,y;
	FILE *fp;

	fp=outptr->binary_file[r_bin];
	last_pt=histptr->num_pts_stored-1;
	write_me_to_disk=0;

	if((histptr->num_pts_stored>=MAX_HISTORY_PTS-4))
		return ;  

	write_me_to_disk=1;

	too_big=0;
	if(write_me_to_disk==1)
	{
		for(i=0;i<histptr->num_pts_stored;i++) 
		{
			if ((fabs(histptr->xh[i]) >= MAX_COORD) ||
				(fabs(histptr->yh[i]) >= MAX_COORD) ||
				(fabs(histptr->zh[i]) >= MAX_COORD))
			{
				too_big=1;
				/* printf("WARNING: photon coordinate > 30 cm\nThrowing this photon away.\n"); */
			}
		}
	}

	if((write_me_to_disk==1) && (too_big==0)) 
	{
		/* write header info the 1st time around */
		if(photptr->num_photons_written[r_bin] < 1)
		{
			fwrite(tissptr,sizeof(struct Tissue),1,fp);
			fwrite(tissptr->layerprops,MAX_NUM_LAYERS*sizeof(struct Layer),1,fp);
			fwrite(pertptr,sizeof(struct perturb),1,fp);
			printf("wrote header datafile=%i\n",r_bin);
		}
		/* history data write */
		/* write tot col - boundary collisions */
		tot_col=histptr->num_pts_stored-pertptr->col_hit_bdry;
		fwrite(&(tot_col),sizeof(int),1,fp);
		/* printf("hist:num_pts_stored=%i\n",histptr->num_pts_stored);  
		printf("hist:col_hit_bdry=%i\n",pertptr->col_hit_bdry);  
		printf("hist:tot_col=%i\n",tot_col);   */
		for (i=1;i<=tissptr->num_layers;++i)
		{
			fwrite(&(pertptr->col_in_layer[i]),sizeof(int),1,fp);
			fwrite(&(pertptr->pathlen_in_layer[i]), sizeof(double),1,fp);
			/*   printf("hist:col_in_layer[%i]=%i\n",i,pertptr->col_in_layer[i]);   */
		}
		fwrite(&(histptr->cum_path_length),sizeof(double),1,fp);
		//fwrite(&(pertptr->col_in_pert),sizeof(int),1,fp);
		//fwrite(&(pertptr->len_in_pert),sizeof(double),1,fp);
		//fwrite(&(pertptr->track_in_pert),sizeof(int),1,fp);
		fwrite(&(histptr->xh[histptr->num_pts_stored-1]),
			sizeof(double),1,fp);
		fwrite(&(histptr->yh[histptr->num_pts_stored-1]),
			sizeof(double),1,fp);
		fwrite(&(histptr->uzh[histptr->num_pts_stored-1]),
			sizeof(double),1,fp);

		/* Display_Photon_Data(); */
		photptr->num_photons_written[r_bin] += 1.0;
	}
}
