/* Monte Carlo simulation of light propagation in
*  mulit-layered tissues. This code was written by
*  Andy Dunn, University of Texas at Austin, 1996.
*
*  The program prompts the user for an input filename
*  (see example input file for details) and returns all
*  of the output values to the output file specified by
*  user in the input file.
*
*  Parts of this code were adapted from MCML, written
*  by S. Jacques and L. Wang, MD Anderson Cancer Center.
*  The major difference is that this program does not
*  require a separate convolution program for finite beam
*  diameters. */
#include <stdio.h>
#include <math.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#include "mc_main.h"
#include "mc_utils.h"
#include "pert.h"
#include "mc_read_input.h"
#include "protos.h"

#define Boolean char
#define COS90D 1.0E-6
#define COSZERO (1.0-1e-12)
#define Weight_Limit 1.0e-4
#define CHANCE 0.1
#define ONE (1.0-1e-12)

//#define PURE_ANALOG 0 field in flags structure now

/* global variables */
struct Photon *photptr;
struct Tissue *tissptr;
struct Output *outptr;
struct perturb *pertptr;
struct History *histptr;
struct Flags *flagptr;
struct SourceDefinition *source;
struct DetectorDefinition *detector;


/*************************************************************/
/* begin main() */
void main(int argc, char *argv[])
{     
	char name[256];
	strcpy(name, "..\\..\\files\\MonteCarloTest\\infile_sphere.txt");  /*path of the input file*/
	
	RunMCCHInternal(name);
}  /* end of main() */

void RunMCCHInternal(char* inFileName)
{
	initialize(inFileName);
	flagptr->AbsWtType=1; // set flags passed in by managed on external runs
	flagptr->Seed=0;

	RunMCLoop();

	SaveResults();

	FreeMemory(); // CKH: todo write to de-malloc all malloced
}

__declspec(dllexport) void RunTest(struct Photon *photptr_ex,
	struct Tissue *tissptr_ex, struct Perturb *pertptr_ex, struct Output *outptr_ex)
{
    photptr = photptr_ex;
	printf("mc_main:source->beam_radius=%f\n",source->beam_radius);
	printf("mc_main:source->beamtype=%c\n",source->beamtype[0]);
	printf("mc_main:source->src_NA=%f\n",source->src_NA);
	printf("mc_main:tissptr->layerprops[1].mua=%f\n",tissptr->layerprops[1].mua);
	tissptr = tissptr_ex;
	printf("mc_main:detector->dr=%f\n",detector->dr);
	printf("mc_main:source->num_photons=%d\n",source->num_photons);
	pertptr = pertptr_ex;
	printf("mc_main:tissptr->ellip_rad_y=%f\n",tissptr->ellip_rad_y);
	outptr = outptr_ex;
	printf("mc_main:output->Flu_rz[0][0]=%f\n",outptr->Flu_rz[0][0]);
	printf("mc_main:output->Flu_rz[0][1]=%f\n",outptr->Flu_rz[0][1]);
	printf("mc_main:output->Flu_rz[1][0]=%f\n",outptr->Flu_rz[1][0]);
	printf("mc_main:output->Flu_rz[1][1]=%f\n",outptr->Flu_rz[1][1]);
}

__declspec(dllexport) void RunMCLoopExternal(struct Photon *photptr_ex, struct Tissue *tissptr_ex, 
	struct SourceDefinition *source_ex, struct Output *outptr_ex,struct History *histptr_ex,struct Flags *flagptr_ex)
{
	photptr = photptr_ex;
	tissptr = tissptr_ex;
	outptr = outptr_ex;
	source = source_ex;
	histptr = histptr_ex;
	flagptr = flagptr_ex;

	printf("Seed=%d AbsWtType=%d\n",flagptr->Seed,flagptr->AbsWtType);

	RunMCLoop();

	printf("end of RunMCLooopExternal\n");
}

void RunMCLoop(void)
{
	int n=1;
	short hit;

	for (n=1; n<=source->num_photons; n++) {
		printf("n=%d\n",n);
		photptr->curr_n = n;
		if (n%(source->num_photons/10) == 0)
			DisplayStatus(n,source->num_photons);
		init_photon();   
		do { /* begin do while  */
			
			SetStepSize();
			hit=HitBoundary();

			if (hit == 1)  { /*begin if hit layer*/
				Move_Photon();
				CrossLayer(); 
			} /*end if hit layer*/

			else if (hit == 2|| hit==4) {  /*begin if hit ellipsoid*/      //------new
				Move_Photon();
				CrossEllip(); 
			}/*end if hit ellipsoid*/

			else if (hit == 0 || hit==3) { /*begin if no hit */
				Move_Photon();
				if(flagptr->AbsWtType==0) // ANALOG=0
					Scatter_Or_Absorb();
				else
				{
					Absorb(); 
					Scatter(); /* 3D scattering */
					/*Scatter1D();*/   /* 1D scattering */
				}
			}/*end if no hit*/

			/*Test_Distance(); */
			TestWeight();

		} while (photptr->dead != 1); /* end do while */     

		//pert();
		//DCFIX UNCOMMENT!!!! Compute_Prob_allvox();  /* FIX added call */
	} /* end of for n loop */
}

void SaveResults(void)
{
	int i=0;
	NormalizeResults();
	SaveTextResult();
	Output_Wts_allvox(); /* FIX added call  */
	for (i=0;i<detector->nr;++i)
		printf("det at %f -> %i photons written\n",detector->det_ctr[i],
		(int)photptr->num_photons_written[i]);
	printf("tot phot out top=%i(%4.2f) bot=%i(%4.2f)\n",
		pertptr->tot_out_top,(double)pertptr->tot_out_top/source->num_photons,
		pertptr->tot_out_bot,(double)pertptr->tot_out_bot/source->num_photons);
}

/********************************************************/
void DisplayIntro()
{
	printf("\n");
	printf("     Monte Carlo Simulation of Light Propagation\n");
	printf("           in a multi-layered tissue           \n");
	printf("     with pMC and photon banana generation      \n");
	printf("\n");
	printf("             written by Dunn/Hayakawa           \n");
	printf("            Beckman Laser Institute         \n");
	printf("\n");
}
/********************************************************/
double *AllocVector(short nl, short nh)  /* this mallocs AND inits */
{
	double *v;
	short i;

	v=(double *)malloc((unsigned) (nh-nl+1)*sizeof(double));
	if (!v) {
		printf("Memory allocation error\n");
		exit(1);
	}

	v -= nl;
	for(i=nl;i<=nh;i++) v[i] = 0.0;       /* init. */
	return v;
}

/***********************************************************
*      Allocate a matrix with row index from nrl to nrh 
*      inclusive, and column index from ncl to nch
*      inclusive.
****/
double **AllocMatrix(short nrl,short nrh,
					 short ncl,short nch)
{
	short i,j;
	double **m;

	m=(double **) malloc((unsigned) (nrh-nrl+1)
		*sizeof(double*));
	if (!m) {
		printf("allocation failure 1 in matrix()");
		exit(1);
	}
	m -= nrl;

	for(i=nrl;i<=nrh;i++) {
		m[i]=(double *) malloc((unsigned) (nch-ncl+1)
			*sizeof(double));
		if (!m[i]) {
			printf("allocation failure 2 in matrix()");
			exit(1);
		}
		m[i] -= ncl;
	}

	for(i=nrl;i<=nrh;i++)
		for(j=ncl;j<=nch;j++) m[i][j] = 0.0;
	return m;
}

/********************************************************/
void initialize(char* inFileName)
{
	int i;
	//long n;
	FILE * input_file_ptr;
	double th;
	double n;  
	short nr,nz,na,nt,nx, ny;
	char tmp_name[256];

	printf("string entered is: %s\n", inFileName);
	/* Allocate 4 global structures */
	photptr=(struct Photon *)malloc(sizeof(struct Photon));
	tissptr=(struct Tissue *)malloc(sizeof(struct Tissue));
	outptr=(struct Output *)malloc(sizeof(struct Output));
	pertptr=(struct perturb *)malloc(sizeof(struct perturb));
	histptr=(struct History *)malloc(sizeof(struct History));
	flagptr=(struct Flags *)malloc(sizeof(struct Flags));

	// CKH 09jan31 malloc those structures that were changed to pointers
	tissptr->layerprops=(struct Layer *)malloc(MAX_NUM_LAYERS*sizeof(struct Layer));
	source->beamtype=malloc(10*sizeof(char));
	photptr->num_photons_written=malloc(MAX_DET*sizeof(double));
	outptr->in_side_allvox=malloc(6*sizeof(int));
	outptr->out_side_allvox=malloc(6*sizeof(int));
	histptr->xh=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->yh=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->zh=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->uxh=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->uyh=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->uzh=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->weight=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->pert_wt=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->path_length=malloc(MAX_HISTORY_PTS*sizeof(double));
	histptr->boundary_col=malloc(MAX_HISTORY_PTS*sizeof(int));

	input_file_ptr = fopen(inFileName, "r");
	DisplayIntro();
	ReadInput(input_file_ptr);

	n = tissptr->layerprops[1].n;  
	nr=detector->nr;
	nz=detector->nz;
	na=detector->na;
	nt=detector->nt;  
	
	nx=detector->nx;  //  ==================================
	ny=detector->ny;

	photptr->sleft = 0.0;
	outptr->Rd = 0.0;
	outptr->Rtot = 0.0;
	outptr->Td = 0.0;
	outptr->Atot = 0.0;
	photptr->Rspec = Specular();

	outptr->A_rz = AllocMatrix(0,nr-1,0,nz-1);
	outptr->A_z = AllocVector(0,nz-1);
	outptr->A_layer=AllocVector(0,tissptr->num_layers+1); 
	outptr->Flu_rz = AllocMatrix(0,nr-1,0,nz-1);
	outptr->Flu_z = AllocVector(0,nz-1);

	outptr->R_ra = AllocMatrix(0,nr-1,0,na-1);
	outptr->R_r = AllocVector(0,nr-1);
	outptr->R_r2 = AllocVector(0,nr-1);

	//outptr->Rev = AllocMatrix(0,1,0,nr-1); /* R for expect value */
	outptr->R_rt = AllocMatrix(0,nr-1,0,nt-1); /* R(r,t) */

	outptr->R_a = AllocVector(0,na-1);
	outptr->T_ra = AllocMatrix(0,nr-1,0,na-1);
	outptr->T_r = AllocVector(0,nr-1);
	outptr->T_a = AllocVector(0,na-1);
	//histptr->xh = AllocVector(0,MAX_HISTORY_PTS);
	//histptr->yh = AllocVector(0,MAX_HISTORY_PTS);
	//histptr->zh = AllocVector(0,MAX_HISTORY_PTS);
	//histptr->uxh = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->uyh = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->uzh = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->weight = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->pert_wt = AllocVector(0,MAX_HISTORY_PTS); // CKH degrade pert_wt from [3,MAX_HISTORY_PTS]
 //   histptr->path_length = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->boundary_col = ivector(0,MAX_HISTORY_PTS);

	//DCFIX (again later)
	//	todo: the following is a bad idea. allocation logic
            // should be done from higher-level constructs. here, it should 
            // just use nx, ny, etc...
	outptr->R_xy = AllocMatrix(0,2*nx,0,2*ny); 

	//detector->nx=2*detector->nr+1;
	/*  outptr->Banana= d3tensor(0,detector->nx-1,0,detector->nz-1,0,detector->nt-1);*/

	/* Note: All arrays are initialized to 0.0 in AllocVector() */

	/* compute beam radius! */
	/*   if (detector->nA/n > 1.0) { */
	/*     printf("NA > too big!\n exiting ...\n"); */
	/*     exit(0); */
	/*   } */
	/*   th=asin(detector->nA/n); */
	/*   source->beam_diam=tissptr->z_focus*tan(th); */


	/*   printf("NA= %e\n",detector->nA); */
	/*   printf("z_f= %f\n",tissptr->z_focus); */
	printf("beam radius= %f\n",source->beam_radius);
	printf("beam type = %c\n",source->beamtype[0]);

	for (i=0;i<detector->nr;++i)
		photptr->num_photons_written[i]=0;

	/* initialize perturbation */
	init_pert();
	init_banana_allvox(); /* FIX added call */

	/* create output binary datafile */
	//for (i=0;i<detector->nr;++i)
	//{
	//	sprintf(tmp_name,"%s%5.3f",pertptr->output_filename,
	//		detector->det_ctr[i]);
	//	outptr->binary_file[i]=
	//		fopen(tmp_name,"wb");
	//}
}

/*****************************************************************/
void DisplayStatus(long n, long num_phot)
{
	/* fraction of photons completed */
	double frac=100*n/num_phot;
	time_t t;
	time(&t);
	printf("%5.0f percent complete, %s", frac, ctime(&t)); 
}

/*****************************************************************/

void Scatter()
{
	double ux = photptr->ux;
	double uy = photptr->uy;
	double uz = photptr->uz;
	short curr_layer = photptr->curr_layer;
	double g = tissptr->layerprops[curr_layer].g;
	double cost, sint;    /* cosine and sine of theta */
	double cosp, sinp;    /* cosine and sine of phi */
	double psi;

	if(g == 0.0)
		cost = 2*RandomNum() -1;
	else {
		double temp = (1-g*g)/(1-g+2*g*RandomNum());
		cost = (1+g*g - temp*temp)/(2*g);
		if(cost < -1) cost = -1;
		else if(cost > 1) cost = 1;
	}
	sint = sqrt(1.0 - cost*cost);

	psi = 2.0*PI*RandomNum();
	cosp = cos(psi);
	sinp = sin(psi);

	if(fabs(uz) > (1-1e-10))  {               /* normal incident. */
		photptr->ux = sint*cosp;
		photptr->uy = sint*sinp;
		photptr->uz = cost*uz/fabs(uz);
	}
	else  {
		double temp = sqrt(1.0 - uz*uz);
		photptr->ux = sint*(ux*uz*cosp - uy*sinp)
			/temp + ux*cost;
		photptr->uy = sint*(uy*uz*cosp + ux*sinp)
			/temp + uy*cost;
		photptr->uz = -sint*cosp*temp + uz*cost;
	}
}

/*****************************************************************/
void init_photon()
{
	double x, y, z_f, denom;
	double RN1, RN2;
	double cosRN2, sinRN2;
	double theta,cost,sint,cosp,sinp;  /* cos/sin of theta/phi */
	photptr->w=1.0-photptr->Rspec; 

	/* Assume Gaussian beam w\ starting coordinates as given */
	/*  on p24 in Ch4 of AJW book  */
	/* photptr->x = 0.0; */

	if ( source->beam_radius == 0.0 )
	{
		photptr->x = 0.0;
		photptr->y = 0.0;
	}
	else 
	{
		RN1=RandomNum();
		RN2=RandomNum();
		//printf("RN1=%f RN2=%f\n",RN1,RN2);
		cosRN2=cos(2*PI*RN2);
		sinRN2=sin(2*PI*RN2);

		if ( (source->beamtype[0] =='f') || (source->beamtype[0] =='F'))
		{
			/* flat beam */
			photptr->x = source->beam_radius*sqrt(RN1)*cosRN2; 
			photptr->y = source->beam_radius*sqrt(RN1)*sinRN2; 
		}
		else if ( (source->beamtype[0] =='r') || (source->beamtype[0] =='R'))//==================new
		{
		  /* flat rectangular beam */
		  photptr->x = source->beam_radius*(RN2-0.5)+source->beam_center_x; 
		  //x=[-beam_radius/2+beam_center_x:beam_radius/2+beam_center_x]
		  photptr->y = RN1*8-4; //y=[-3:3 cm]==========================must be changed if you need a different field of view
		}
		else  /* Gaussian beam */
		{
		  if (RN1==1.0) RN1=RandomNum();
 		  photptr->x=source->beam_radius*sqrt(-log(1.0-RN1)/2.0)*cosRN2; 
 		  photptr->y=source->beam_radius*sqrt(-log(1.0-RN1)/2.0)*sinRN2; 
		}
		
	}
	photptr->z = 0.0;
	photptr->dead = 0;

	x=photptr->x;
	y=photptr->y;
	z_f=tissptr->z_focus;
	/*  denom=sqrt(x*x + y*y + z_f*z_f);*/

	/*   photptr->ux = -x/denom; */
	/*   photptr->uy = -y/denom; */
	/*   photptr->uz = z_f/denom; */
	/* photptr->ux=0.0;
	photptr->uy=0.0;
	photptr->uz=1.0;  */
	theta=2.0*PI*RandomNum();
	cost=cos(theta);
	sint=sin(theta);
	if (source->src_NA==0.0) {
		photptr->ux=0;
		photptr->uy=0;
		photptr->uz=1;
	}
	else {
		do {
			cosp=RandomNum();
			sinp=sqrt(1.0-cosp*cosp);
		} while (sinp>(source->src_NA/tissptr->layerprops[1].n)); 
		photptr->ux=cost*sinp;
		photptr->uy=sint*sinp;
		photptr->uz=cosp; 
	}

	photptr->curr_layer = 1;   /* photon starts in first tissue layer */
	photptr->s = 0.0;
	photptr->sleft = 0.0;

	/* start recording history */
	histptr->num_pts_stored=1;
	histptr->xh[0]=photptr->x;
	histptr->yh[0]=photptr->y;
	histptr->zh[0]=photptr->z;

	histptr->uxh[0]=photptr->ux;
	histptr->uyh[0]=photptr->uy;
	histptr->uzh[0]=photptr->uz;
	histptr->boundary_col[0]=0;

	histptr->weight[0]=photptr->w;
	histptr->path_length[0]=0.0;
	histptr->cum_path_length=0.0;
}
/*****************************************************************/
void SetStepSize()
{
	short curr_layer = photptr->curr_layer;
	double mua = tissptr->layerprops[curr_layer].mua;
	double mus = tissptr->layerprops[curr_layer].mus;
	double RN;
	if (photptr->sleft == 0.0) {
		do RN = RandomNum();
		while ((RN <=0.0) || (RN>ONE));
		photptr->s = -log(RN)/(mua+mus);  
	}
	else {
		photptr->s = photptr->sleft/(mua+mus);  
		photptr->sleft = 0.0;
	}
}
/*****************************************************************/
void Move_Photon()
{
	int index;
	double delta_x,delta_y,delta_z;
	double tmp,l,xsurf,ysurf,rsurf,amt;
	int irsurf;

	double ux = photptr->ux;
	double uy = photptr->uy;
	double uz = photptr->uz;
	double dr = detector->dr;
	double mut=tissptr->layerprops[1].mus+tissptr->layerprops[1].mua;

	photptr->x += photptr->s*ux;
	photptr->y += photptr->s*uy;
	photptr->z += photptr->s*uz; 

	/* record history */
	histptr->num_pts_stored++;
	index=histptr->num_pts_stored-1;
	histptr->xh[index]=photptr->x;
	histptr->yh[index]=photptr->y;
	histptr->zh[index]=photptr->z;

	//if ((photptr->curr_n==1)&&(histptr->num_pts_stored>120))
	//printf("Move: N=%d photptr->x[%d],y,z=%15.10f,%15.10f,%15.10f\n",
	//	photptr->curr_n,index,histptr->xh[index],histptr->yh[index],
	//	histptr->zh[index]);

	histptr->uxh[index]=photptr->ux;
	histptr->uyh[index]=photptr->uy;
	histptr->uzh[index]=photptr->uz;
	if (photptr->hit_bdry==1)
	{
		histptr->boundary_col[index]=1;
		photptr->hit_bdry=0;  /* reset for next hit */
	}
	else
		histptr->boundary_col[index]=0;

	delta_x=photptr->x-histptr->xh[index-1];
	delta_y=photptr->y-histptr->yh[index-1];
	delta_z=photptr->z-histptr->zh[index-1];
	tmp=sqrt(delta_x*delta_x+delta_y*delta_y+delta_z*delta_z);

	pertptr->pathlen_in_layer[photptr->curr_layer] += tmp;
	if (histptr->boundary_col[index]==0)
		++pertptr->col_in_layer[photptr->curr_layer];

	/* path_length = length from the prev point to the curr point */
	histptr->path_length[index]=tmp;
	histptr->cum_path_length += histptr->path_length[index];

}
//DCFIX

/***************************************************************/
short HitBoundary()  //-------------------------------------------------------new
{
	short hit=0;        /* Determines if we've hit the boundary of
						a layer (hit=1) 
						or ellipse from outside (hit=2) 
						or ellipse from inside (hit=4)
						or nothing but we're in the ellipse (hit=3)
						or nothing and we're in the hom. medium (hit=0).*/ 
	if (tissptr->do_ellip_layer==3)
		hit = HitEllip();
	else 
		hit = HitLayer();
	return hit;
}
/**************************************************************************/
short HitLayer() // returns 1 if hit layer, returns 0 if not-------------------copy and paste from original "HitBoundary()"
{
  short curr_layer = photptr->curr_layer;
  double zbegin = tissptr->layerprops[curr_layer].zbegin;
  double zend = tissptr->layerprops[curr_layer].zend;
  double dbound;  /* distance to boundary */
  double uz = photptr->uz;
  double s = photptr->s;
  double z = photptr->z;
  double mus = tissptr->layerprops[curr_layer].mus;
  double mua = tissptr->layerprops[curr_layer].mua;
  short hit;
  
  if (uz<0.0)
    dbound = (zbegin-z)/uz;
  else if (uz>0.0)
    dbound = (zend-z)/uz;
  if ((uz != 0.0) && (s>dbound)) {
    hit = 1;
    photptr->hit_bdry=1;
    photptr->sleft = (photptr->s - dbound)*(mua+mus); 
    photptr->s = dbound;
  }
  else hit = 0;
  return(hit);
}
/********************************************************************************/


short HitEllip()    //-------------------------------------------------------new, based on "Ray_Intersect_Ellip" in pert.h and on "HitLayer"
/* returns	2 if hit the ellipsoid from outside, 
			3 if didn't hit nothing but ray is in the ellipsoid, 
			4 if hit ellip from inside, 
			0 else */
{
	double A,B,C,root1,root2,root,xto,yto,zto;
	int one_in,two_in;
	int numint;
	short hit = 0; 
	double x1= photptr->x;    
	double y1= photptr->y;
	double z1= photptr->z;
	double x2= photptr->x + photptr->s * photptr->ux;
	double y2= photptr->y + photptr->s * photptr->uy;
	double z2= photptr->z + photptr->s * photptr->uz;
	double dbound;  /* distance to boundary */
	double s = photptr->s;
	short curr_layer = photptr->curr_layer; 
	double mus = tissptr->layerprops[curr_layer].mus; /*layer [2] represents ellipse optical properties*/
	double mua = tissptr->layerprops[curr_layer].mua;

  if (z2<0||z2>tissptr->layerprops[1].d){  // if hits upper or lower boundary (with air)    
	  hit=HitLayer();
  }
  else {

	/* determine intersection with ellipsoid*/ 
	one_in=InEllipsoid(x1,y1,z1);//is (x1,y1,z1)in the ellip? 0=no, 1=yes, 3=on the boundary, 2=ellip doesn't exist
	two_in=InEllipsoid(x2,y2,z2);

	if ((one_in==1 || one_in==2 || one_in==3)&& 
		(two_in==1 || two_in==2))  /* ray within ellipsoid */
    {
		hit = 3;
	}
	
	else   //ray may cross the ellipsoid once, twice or never. 
		   //ph MUST stop at first intersection. finds number of intersections numint
    { 
      // sanity check dimensions of ellipsoid 
      if ((tissptr->ellip_rad_x == 0.0) ||
          (tissptr->ellip_rad_y == 0.0) ||
          (tissptr->ellip_rad_z == 0.0))
        {
          printf("PERT ERROR: one ellipsoid radial dimension = 0.0\n");
        }
      else
        {// finds intersections
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
          if (B*B-4*A*C > 0)  // roots are real 
            {
              root1=(-B-sqrt(B*B-4*A*C))/(2*A);
              root2=(-B+sqrt(B*B-4*A*C))/(2*A);
              numint=0;             //number of intersections
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
			  switch (numint) 
                {
                  case 0: /* roots real but no intersection */         
					 hit=0;
					 break;  					  
                  case 1:
					if ((one_in==3)&&(fabs(root)<1e-7))  // CKH FIX 11/10/08
					{
						 hit=0;
					}
					else
					{
						if (one_in==1)
						{
							hit=4;       //exiting the ellipse 
						} 
						else 
						{
							hit=2;       //entering it    
						}
						/*entering or exiting ellipsoid. It's the same*/
						xto=x1+root*(x2-x1); 
						yto=y1+root*(y2-y1);
						zto=z1+root*(z2-z1);

						/*distance to the boundary*/
						dbound=sqrt((xto-x1)*(xto-x1)+
                              (yto-y1)*(yto-y1)+
                              (zto-z1)*(zto-z1));
					
						photptr->hit_bdry=1;        
						photptr->sleft = (photptr->s - dbound)*(mua+mus); 
						photptr->s = dbound;
                        
					 }
					 break;
					 
                  case 2:  /* went through ellipsoid: must stop at nearest intersection */
                    /*wich is nearest?*/
					if (one_in==3) {
					  if (root1>root2) // CKH FIX 11/11/08
					    root=root1;
					  else root=root2;
					}
					else {
					  if (root1<root2) // CKH FIX 11/11/08
					    root=root1;
					  else root=root2;
					}
					xto=x1+root*(x2-x1);  
                    yto=y1+root*(y2-y1);
                    zto=z1+root*(z2-z1);
                    /*distance to the nearest boundary*/
					dbound=sqrt((xto-x1)*(xto-x1)+
                            (yto-y1)*(yto-y1)+
                            (zto-z1)*(zto-z1));
                    photptr->hit_bdry=1;        
					photptr->sleft = (photptr->s - dbound)*(mua+mus); 
					photptr->s = dbound;
					
					hit=2;
					break;
					
			  } /* end switch */
    
            } /* BB-4AC>0 */
		  else /* roots imaginary -> no intersection */
            {
				hit=0;
            } 
	  } /* check on radial axis of ellipsoid */
	} /* intersection with ellipsoid */
  }
  return hit;
  }
///***************************************************************/
//short HitBoundary()
//{
//	short curr_layer = photptr->curr_layer;
//	double zbegin = tissptr->layerprops[curr_layer].zbegin;
//	double zend = tissptr->layerprops[curr_layer].zend;
//	double dbound;  /* distance to boundary */
//	double uz = photptr->uz;
//	double s = photptr->s;
//	double z = photptr->z;
//	double mus = tissptr->layerprops[curr_layer].mus;
//	double mua = tissptr->layerprops[curr_layer].mua;
//	short hit;
//
//	if (uz<0.0)
//		dbound = (zbegin-z)/uz;
//	else if (uz>0.0)
//		dbound = (zend-z)/uz;
//
//	if ((uz != 0.0) && (s>dbound)) {
//		hit = 1;
//		photptr->hit_bdry=1;
//		photptr->sleft = (photptr->s - dbound)*(mua+mus); 
//		photptr->s = dbound;
//	}
//	else hit = 0;
//	return(hit);
//	printf("HitBound: hit=%i\n",photptr->hit_bdry);
//}

/*****************************************************************/
double Fresnel(double n1, double n2, double ci, double * uz_snell)
{
	/* n1 is the index of the current layer */
	/* n2 is the index of the next layer */
	double ct, si, st;  /* cos(thi), cos(tht), sin(thi), sin(tht) */
	double cd, cs, sd, ss; /* cos(thi+tht), cos(thi-tht), etc */
	double r;

	if (n1==n2) {
		*uz_snell = ci;
		r = 0.0;
	}
	else if (ci>COSZERO) {     /* normal incidence */
		*uz_snell = ci;
		r = (n2-n1)*(n2-n1)/((n2+n1)*(n2+n1));
	}
	else if (ci<COS90D) {
		*uz_snell = 0.0;
		r = 1.0;
	}
	else {
		si = sqrt(1-ci*ci);
		st = n1/n2*si;
		ct = sqrt(1-st*st);
		*uz_snell = ct;

		/* Use trig identities */
		sd = si*ct - ci*st;
		ss = si*ct + ci*st;
		cd = ci*ct + si*st;
		cs = ci*ct - si*st;
		/* printf("cd= %12.4e    ss = %12.4e\n"); */
		r = 0.5*(sd*sd/(ss*ss)+sd*sd*cs*cs/(cd*cd*ss*ss));
	}

	return(r);
}

/*****************************************************************/
void Transmit(double r)
{
	short ir,ia;
	double x = photptr->x;
	double y = photptr->y;
	double dr = detector->dr;
	double da = detector->da;

	ir=(short)(sqrt(x*x+y*y)/dr);
	if ( ir > detector->nr-1 )
		ir = detector->nr-1;
	ia=(short)(acos(photptr->uz)/da);
	if ( ia > detector->na-1 )
		ia = detector->na-1;

	if ( photptr->uz <0 ) printf(">0!\n");

	outptr->T_ra[ir][ia] += photptr->w*(1-r);
	photptr->w *= r;
}

/*****************************************************************/
void Reflect(double r)// for index-mismatched reflections 
{
	double amt_out;
	short ir,ia,it,nt=detector->nt; /* FIXED-DC added it,nt */ 

	//DCFIX
	short ix, iy; 

	double w = photptr->w;
	double x = photptr->x;
	double y = photptr->y;
	double dr = detector->dr;
	double da = detector->da;  
	
	//DCFIX
	double nx = detector->nx;  //=========================================
	double ny = detector->ny;
	double dx = detector->dx;
	double dy = detector->dy;

	double t_delay,dt=detector->dt;  /* FIXED-DC added t_delay,dt */

	ir=(short)(sqrt(x*x+y*y)/dr);
	if ( ir > detector->nr-1 )
		ir = detector->nr-1;
	ia=(short)(acos(photptr->uz)/da);
	if ( ia > detector->na-1 )
		ia = detector->na-1;

	amt_out = (1-r)*w;
	outptr->R_r[ir] += amt_out;
	outptr->R_ra[ir][ia] += amt_out;
	outptr->R_r2[ir] += amt_out*amt_out;
	photptr->w *= r;  /* w=w*r is the amt internally reflected */
	
	/* FIXED-DC save R(r,t) */
	t_delay=histptr->cum_path_length/(.03/
		tissptr->layerprops[1].n);  /* -> ps */
	it=(int)floor(t_delay/dt); /* assumes tmin=0 */
	if ((it>nt-1)||(it<0)) it=-1; /* if outside [tmin,tmax] */
	if (it!=-1) {
		outptr->R_rt[ir][it]+=amt_out;
	} 
	/* END FIX */

	//DCFIX (cartesian reflectance)
	ix=(short)((x+nx*dx)/dx); /* added and checked*/
	iy=(short)((y+ny*dy)/dy); /* added and checked*/

  //printf("Reflect: x=%f ix=%d y=%f iy=%d\n",x,ix,y,iy); //=============    cancella
  if ((ix < detector->nx*2-1) && (ix >= 0) &&
      (iy < detector->ny*2-1) && (iy >= 0)) {
    //printf("Reflect: ix=%d iy=%d amt_out=%f\n",ix,iy,amt_out);//=================    cancella
    outptr->R_xy[ix][iy] += amt_out; /* added*/
  }
	photptr->dead=1;
}
/*****************************************************************/
void CrossDown()
{
	double r, uz_snell;
	short curr_layer = photptr->curr_layer;
	double uz = photptr->uz;
	double n_curr = tissptr->layerprops[curr_layer].n;
	double n_next = tissptr->layerprops[curr_layer+1].n;
	double coscrit;

	if (n_curr > n_next)
		coscrit = sqrt(1.0-(n_next/n_curr)*(n_next/n_curr));
	else coscrit = 0.0;

	if (uz <= coscrit)
		r = 1.0;
	else {
		r = Fresnel(n_curr, n_next, uz, &uz_snell);
	}
	//printf("CrossDown: curr_layer=%d\n",curr_layer);
	/* Decide whether or not photon goes to next layer */
	if (RandomNum() > r) {
		/* transmitted to next layer */  // CKH FIX 11/11/08
		if (((tissptr->do_ellip_layer!=3)&&(curr_layer == tissptr->num_layers))
		    ||((tissptr->do_ellip_layer==3)&&(curr_layer == 1))) 
		{
			/* call reflect with fixed weight photons! */
			Transmit(0.0); 
			photptr->dead = 1;
			photptr->ux *= n_curr/n_next;
			photptr->uy *= n_curr/n_next;
			photptr->uz = uz_snell;
		}
		else {
			photptr->curr_layer++;
			photptr->ux *= n_curr/n_next;
			photptr->uy *= n_curr/n_next;
			photptr->uz = uz_snell;
		}
	} /* end if(RandomNum() */
	else {

		photptr->uz = -uz;
	}
}

/*****************************************************************/
void CrossUp()
{
	double r, uz_snell;
	short curr_layer = photptr->curr_layer,index;
	double uz = photptr->uz;
	double n_curr = tissptr->layerprops[curr_layer].n;
	double n_next = tissptr->layerprops[curr_layer-1].n;
	double coscrit,x_curr,y_curr,z_curr;

	if (n_curr > n_next)
		coscrit = sqrt(1.0-(n_next/n_curr)*(n_next/n_curr));
	else coscrit = 0.0;

	if (-uz <= coscrit) {
		r = 1.0;
	}
	else {
		r = Fresnel(n_curr, n_next, -uz, &uz_snell);
	}

	/* Decide on whether photon crosses into next layer */
	if (RandomNum() > r) {    /* moves into next layer */
		if (curr_layer == 1) { /* top layer-move out of tissue */
			photptr->ux *= n_curr/n_next;
			photptr->uy *= n_curr/n_next;
			photptr->uz = uz_snell;

			/* call reflect with fixed weight photons! */
			Reflect(0.0); /* 0.0 b/c all or none refl */
			photptr->dead = 1;
		}
		else {
			photptr->curr_layer--;  /* moves across interface */
			photptr->ux *= n_curr/n_next;
			photptr->uy *= n_curr/n_next;
			photptr->uz = -uz_snell;
		}
	}  /* end if(RandomNum() */
	else
		photptr->uz = -uz;
}

/*****************************************************************/
void CrossLayer(void)
{
	double uz = photptr->uz;
	if (uz<0.0)
		CrossUp();
	else
		CrossDown();
}

/*****************************************************************/
void CrossEllip()   //-----------------------------------------------------new
/*based on CrossLayer, but without Fresnel (n doesn't change)=>no reflections at the boundary.*/
{   
  short curr_layer = photptr->curr_layer;
  double n_curr, n_next;

  if (curr_layer==2) 
	  photptr->curr_layer--;
  else
	  photptr->curr_layer++;

}

/*****************************************************************/

void Absorb(void)
{
	double dw;
	short ir,iz;
	short curr_layer = photptr->curr_layer;
	double mua = tissptr->layerprops[curr_layer].mua;
	double mus = tissptr->layerprops[curr_layer].mus;
	double w = photptr->w;
	double x = photptr->x;
	double y = photptr->y;
	int index=histptr->num_pts_stored-1;

	if (photptr->sleft==0.0)  // only deweight if real not pseudo collision
	{
		/* Compute array indices from r and z */
		iz=(short)(photptr->z/detector->dz);
		if (iz>detector->nz-1) iz=detector->nz-1;
		ir=(short)(sqrt(x*x+y*y)/detector->dr);
		if (ir>detector->nr-1) ir=detector->nr-1;

		/* no cont abs wt change here since weight in post */
		dw = w*mua/(mua+mus); 
		photptr->w -= dw;
		if (curr_layer==3)
			printf("curr_layer==3\n");
		outptr->A_layer[curr_layer] += dw;
		outptr->A_rz[ir][iz] += dw; 

		/* update weight for history */
		histptr->weight[index]=photptr->w;

		if (flagptr->AbsWtType==0) // ANALOG=1;
			photptr->dead=1;
	}
}

/*****************************************************************/
void Scatter_Or_Absorb()
{
	double RN;

	RN=RandomNum();
	if (RN<tissptr->layerprops[photptr->curr_layer].albedo)
		Scatter();
	else
		Absorb();
}

/*****************************************************************/
void Test_Distance(void)
{
	/* kill photon if it has gone too far */
	if(histptr->cum_path_length>=photptr->max_path_length)
		photptr->dead=1;
}

/*****************************************************************/
void Roulette()
{
	if (photptr->w == 0.0)
		photptr->dead = 1;
	else if (RandomNum() < CHANCE)
		photptr->w /= CHANCE;
	else photptr->dead = 1;
}

/*****************************************************************/
void TestWeight()
{
	/*   if (photptr->w < Weight_Limit) */
	/*     Roulette();  */
	if(histptr->num_pts_stored >= MAX_HISTORY_PTS-4)
	{
		photptr->dead=1;
		printf("WARNING: MAX_HISTORY_PTS reached. Killing this photon\n"); 
	}
}

/*****************************************************************/


/********************************************************/
void FreeVector(double *v,short nl,short nh)
{
	free((char*) (v+nl));
}

/***********************************************************
*      Release the memory.
****/
void FreeMatrix(double **m,short nrl,short nrh,
				short ncl,short nch)
{
	short i;

	for(i=nrh;i>=nrl;i--) free((char*) (m[i]+ncl));
	free((char*) (m+nrl));
}

/********************************************************/
void FreeMemory()
{
	FreeMatrix(outptr->A_rz,0,detector->nr-1,0,detector->nz-1);
	FreeVector(outptr->A_z,0,detector->nz-1);
	FreeVector(outptr->A_layer,0,tissptr->num_layers+1);
	FreeMatrix(outptr->Flu_rz,0,detector->nr-1,0,detector->nz-1);
	FreeVector(outptr->Flu_z,0,detector->nz-1);
	FreeMatrix(outptr->R_ra,0,detector->nr-1,0,detector->na-1);
	FreeVector(outptr->R_r,0,detector->nr-1);
	FreeVector(outptr->R_r2,0,detector->nr-1);
	
	FreeMatrix(outptr->R_rt,0,detector->nr-1,0,detector->nt-1);
	FreeVector(outptr->R_a,0,detector->na-1);
	FreeMatrix(outptr->T_ra,0,detector->nr-1,0,detector->na-1);
	FreeVector(outptr->T_r,0,detector->nr-1);
	FreeVector(outptr->T_a,0,detector->na-1);

	//histptr->xh = AllocVector(0,MAX_HISTORY_PTS);
	//histptr->yh = AllocVector(0,MAX_HISTORY_PTS);
	//histptr->zh = AllocVector(0,MAX_HISTORY_PTS);
	//histptr->uxh = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->uyh = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->uzh = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->weight = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->pert_wt = AllocVector(0,MAX_HISTORY_PTS); // CKH degrade pert_wt from [3,MAX_HISTORY_PTS]
 //   histptr->path_length = AllocVector(0,MAX_HISTORY_PTS);
 //   histptr->boundary_col = ivector(0,MAX_HISTORY_PTS);
 
	/*  free_d3tensor(outptr->Banana,0,detector->nx-1,0,detector->nz-1,
	*	0,detector->nt-1);*/
}

/*********************************************************/
void Scatter1D()
{
	short curr_layer = photptr->curr_layer;
	double g = tissptr->layerprops[curr_layer].g;

	if(RandomNum() < ((1+g)/2.0) )
		photptr->uz *= 1.0;
	else
		photptr->uz *= -1.0;
}

