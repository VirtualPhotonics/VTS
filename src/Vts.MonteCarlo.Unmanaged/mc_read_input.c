#include <stdio.h>
#include <stdlib.h>
#include "mc_main.h"
#include "pert.h"
#include "mc_read_input.h"

/* global variables */
extern struct Photon *photptr;
extern struct Tissue *tissptr;
extern struct Output *outptr;
extern struct perturb *pertptr;
extern struct SourceDefinition *source;
extern struct DetectorDefinition *detector;
/************************************************************/
void ReadInput(FILE * comm_file_ptr)

{
  char temp[256];
  char temp2[256];
  char in_fname[256];
  FILE * file_ptr;
  int i;

  if ( comm_file_ptr == NULL ) {
    printf("Enter input filename: ");
    scanf("%s",in_fname);
    file_ptr = fopen(in_fname, "r");
  }
  else
    file_ptr = comm_file_ptr;
  
  if (file_ptr == NULL ) {
    printf("\nERROR - Could not find or open input file %s\n",in_fname);
    exit(0);
  }
  
  /* Read in output file name */
  fscanf(file_ptr, "%s %*[^\n]s",  pertptr->output_filename);

  /* Read in number of layers */
  fscanf(file_ptr, "%hd %*[^\n]s", &tissptr->num_layers);
  if ( tissptr->num_layers > 12 ) {
    printf("\nERROR - number of layers must be less than 12\n");
    exit(0);
    }

  /* Read in index of outside medium */
  fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[0].n);

  /* loop through number of layers and read properties of each */
  for (i=1 ;i<tissptr->num_layers+1 ;i++ ) {
    fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[i].n);
    fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[i].mus);
    fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[i].mua);
    fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[i].g);
    fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[i].d);
  }

  /* read in index of outside, bottom medium */
  fscanf(file_ptr, "%lf %*[^\n]s", &tissptr->layerprops[tissptr->num_layers+1].n);
  /* Read in flat or Gaussian beam */
  fscanf(file_ptr, "%s %*[^\n]s", source->beamtype);
  //if ( (source->beamtype[0] != 'f') && (source->beamtype[0] != 'F')) {
  //  if ((source->beamtype[0] != 'g') && (source->beamtype[0] != 'G') )
  //    {
  //      printf("\nERROR - beam type must be either g (Gaussian) or f (flat)\n");
  //      exit(0);
  //    }
  //}

  if ( (source->beamtype[0] != 'f') && (source->beamtype[0] != 'F')&&
	  (source->beamtype[0] != 'r') && (source->beamtype[0] != 'R')&&
	  (source->beamtype[0] != 'g') && (source->beamtype[0] != 'G') )
  {
	  printf("\nERROR - beam type must be either g (Gaussian) or f (flat) or r (rectangular)\n");
	  exit(0);
  }

  /* Read in beam radius */  
  fscanf(file_ptr, "%lf %*[^\n]s", &source->beam_center_x); 
  fscanf(file_ptr, "%lf %*[^\n]s", &source->beam_radius);   
  fscanf(file_ptr, "%lf %*[^\n]s", &source->src_NA);

  /* Read in nr,dr,nz,dz,na */
  fscanf(file_ptr, "%hd %*[^\n]s", &detector->nr);
  fscanf(file_ptr, "%lf %*[^\n]s", &detector->dr);
  fscanf(file_ptr, "%hd %*[^\n]s", &detector->nz);
  fscanf(file_ptr, "%lf %*[^\n]s", &detector->dz);
  
  fscanf(file_ptr, "%hd %*[^\n]s", &detector->nx); //=================================
  fscanf(file_ptr, "%lf %*[^\n]s", &detector->dx);
  fscanf(file_ptr, "%hd %*[^\n]s", &detector->ny);
  fscanf(file_ptr, "%lf %*[^\n]s", &detector->dy);

  /*  fscanf(file_ptr, "%hd %*[^\n]s", &detector->na);*/
  detector->na=1;

  /* compute da=(pi/2)/na */
  detector->da = (PI/2.0)/detector->na;

  /* Read in number of photons */
  fscanf(file_ptr, "%ld %*[^\n]s",  &source->num_photons);

  /* read dt */
  fscanf(file_ptr, "%hd %*[^\n]s", &detector->nt);
  fscanf(file_ptr, "%lf %*[^\n]s", &detector->dt);

  /* Compute starting and ending coordinates for each layer */
  tissptr->layerprops[0].zend = 0.0;
  for ( i=1;i<tissptr->num_layers+1;i++ )
  {
    tissptr->layerprops[i].zbegin = tissptr->layerprops[i-1].zend;
    tissptr->layerprops[i].zend = tissptr->layerprops[i].d + 
                                  tissptr->layerprops[i].zbegin;
  }

  /* compute albedo for each layer */
  for (i=1;i<tissptr->num_layers+1;i++)
    {
      tissptr->layerprops[i].albedo=tissptr->layerprops[i].mus/(tissptr->layerprops[i].mus+tissptr->layerprops[i].mua);
    }

  Read_Perturbation_Input(file_ptr);

  fclose(file_ptr);
  }
/***************************************************************/
void Read_Perturbation_Input(FILE *file_ptr)
{
  int i;
  /* read in ellipsoid or layer pert flag */
  fscanf(file_ptr,"%d %*[^\n]s",&tissptr->do_ellip_layer); // CKH FIX 2/09 needs to be d not hd
  /* read in ellipsoid dimensions */
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->ellip_x);
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->ellip_y);
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->ellip_z);
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->ellip_rad_x);
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->ellip_rad_y);
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->ellip_rad_z);
  /* read in layer z min and max */
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->layer_z_min);
  fscanf(file_ptr,"%lf %*[^\n]s",&tissptr->layer_z_max);

  /* read in detector data */
  fscanf(file_ptr,"%d %*[^\n]s",&detector->nr);
  /* read in reflect/transmit flag */
  fscanf(file_ptr,"%d %*[^\n]s",&detector->reflect_flag);
  /* loop through number of detectors and read center */
  for (i=0 ;i<detector->nr ;++i) {
    fscanf(file_ptr, "%lf %*[^\n]s", &detector->det_ctr[i]);
  }
  fscanf(file_ptr,"%lf %*[^\n]s",&detector->det_rad);

}
