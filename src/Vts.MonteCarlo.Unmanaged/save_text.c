#include <stdlib.h>
#include <math.h>
#include <stdio.h>

#include "mc_main.h"
#include "save_text.h"
#include "pert.h"

extern struct Photon *photptr;
extern struct Tissue *tissptr;
extern struct Output *outptr;
extern struct perturb *pertptr;
extern struct SourceDefinition *source;
extern struct DetectorDefinition *detector;

/************************************************/
void NormalizeResults(void)
{
	short ir,iz,i,ia,i_lay,it; /* FIX added it */	
	//DCFIX
	short  ix, iy;
	
	double z;
	double sumR,sumT,sumA;
	double C1,C2;
	double temp;
	double tmpR,tmpR2,percent_error;
	short nr=detector->nr; 
	short na=detector->na;
	short nz=detector->nz;
	short nt=detector->nt;  /* FIX added nt */
	double dz=detector->dz;
	double dr=detector->dr; 
	double da=detector->da;

	//DCFIX
	double nx=detector->nx;//======================================
	double dx=detector->dx;
	double ny=detector->ny;
	double dy=detector->dy;

	double dt=detector->dt; /* FIX added dt */
	long num_phot = source->num_photons;
	short num_lay = tissptr->num_layers;

	/* Generate data to output */
	/* First  sum arrays, then scale them */

	/* Sum R_ra,T_ra to get Rd,Td */
	sumR=0.0;
	sumT=0.0;
	for (ir=0;ir<nr;ir++) {
		for ( ia=0; ia<na;ia++ )
		{
			sumR += outptr->R_ra[ir][ia];
			sumT += outptr->T_ra[ir][ia];
		}
	}
	outptr->Rd = sumR;
	outptr->Td = sumT;


	/* Generate R_r,T_r */
	sumR=0.0;
	sumT=0.0;
	for ( ir=0;ir<nr ;ir++ )
	{
		sumR=0.0;
		sumT=0.0;
		for ( ia=0;ia<na ;ia++ )
		{
			sumR += outptr->R_ra[ir][ia];
			sumT += outptr->T_ra[ir][ia];
		}
		outptr->R_r[ir] = sumR;
		outptr->T_r[ir] = sumT;
	}

	/* Generate R_a,T_a */
	sumR=0.0;
	sumT=0.0;
	for ( ia=0;ia<na ;ia++ )
	{
		sumR=0.0;
		sumT=0.0;
		for ( ir=0;ir<nr ;ir++ )
		{
			sumR += outptr->R_ra[ir][ia];
			sumT += outptr->T_ra[ir][ia];
		}
		outptr->R_a[ia] = sumR;
		outptr->T_a[ia] = sumT;
	}

	/* Generate A_z */
	for ( iz=0;iz<nz ;iz++ )
	{
		sumA=0.0;
		for ( ir=0;ir<nr ;ir++ ) sumA += outptr->A_rz[ir][iz];
		outptr->A_z[iz] = sumA;
	}

	/* TEST of A_rz! */
	temp = 0.0;
	for ( iz=0;iz<nz ;iz++ )
	{
		temp += outptr->A_z[iz];
	}

	/* Now scale all values */

	/* scalar values */
	outptr->Rd /= num_phot;
	outptr->Td /= num_phot;
	outptr->Rtot = outptr->Rd + photptr->Rspec;

	for ( i=1;i<num_lay+1 ;i++ )
	{
		outptr->A_layer[i] /= num_phot;
	}

	for ( i=1;i<num_lay+1 ;i++ )
	{
		outptr->Atot += outptr->A_layer[i];
	}

	/* Scale R and T */

	/* R_ra, T_ra */
	C1 = 2.0*PI*dr*dr*2.0*PI*da*num_phot;

	for ( ir=0;ir<nr ;ir++ )
	{
		for ( ia=0;ia<na ;ia++ )
		{
			C2=C1*(ir+0.5)*sin((ia+0.5)*da);
			outptr->R_ra[ir][ia] /= C2;
			outptr->T_ra[ir][ia] /= C2;
		}
	}

	//DCFIX
	/* R_xy, T_xy */
	for ( ix=0; ix<(2*nx); ix++ ) /* added ==============================and changed*/
    {
        for ( iy=0;iy<(2*ny); iy++ )
        {
            outptr->R_xy[ix][iy] /= (num_phot*dx*dy);        //it was/= dx*dy*num_phot;
        }
    }

	/* R_r, T_r */
	for ( ir=0;ir<nr ;ir++ ) 
	{ 
		C1=2.0*PI*(ir+0.5)*dr*dr*num_phot; 
		outptr->R_r[ir] /= C1; 
		outptr->T_r[ir] /= C1; 
	} 
	
	/* FIX R_rt */
	for ( it=0;it<nt ; it++ )
	{
		for ( ir=0;ir<nr ;ir++ ) 
		{ 
			C1=2.0*PI*(ir+0.5)*dr*dr*num_phot; 
			outptr->R_rt[ir][it] /= C1; 
		}
	} 
	/* END FIX */

	/* R_a, T_a */
	for ( ia=0;ia<na ;ia++ )
	{
		C1=2.0*PI*sin((ia+0.5)*da)*da*num_phot;
		outptr->R_a[ia] /= C1;
		outptr->T_a[ia] /= C1;
	}

	/* Scale A_rz */
	for ( ir=0;ir<nr ;ir++ )
	{
		for ( iz=0;iz<nz ;iz++ )
		{
			C1=2.0*PI*(ir+0.5)*dr*dr*dz*num_phot;
			outptr->A_rz[ir][iz] /= C1;
		}
	}

	/* Scale A_z */
	for ( iz=0;iz<nz ;iz++ )
	{
		C1=dz*num_phot;
		outptr->A_z[iz] /= C1;
	}

	/* Generate fluence from A_rz by dividing by mua */
	for ( ir=0;ir<nr ;ir++ )
	{
		for ( iz=0; iz<nz;iz++ )
		{
			i=1;
			z=(iz+0.5)*dz;
			while ((z>=tissptr->layerprops[i].zend) && (i<num_lay)) i++;
			i_lay = i;
			outptr->Flu_rz[ir][iz] = outptr->A_rz[ir][iz]/tissptr->layerprops[i_lay].mua;
		}
	}

	/* Get Flu_z from A_z */
	for ( iz=0; iz<nz;iz++ )
	{
		i=1;
		z=(iz+0.5)*dz;
		while ((z>=tissptr->layerprops[i].zend) && (i<num_lay)) i++;
		i_lay = i;
		outptr->Flu_z[iz] = outptr->A_z[iz]/tissptr->layerprops[i_lay].mua;
	}
}
/************************************************/
void SaveTextResult(void)
{
	short ir,iz,i,ia,i_lay,it; /* FIX added it */
	//DCFIX
	short  ix, iy;
	short nr=detector->nr; 
	short na=detector->na;
	short nz=detector->nz;
	short nt=detector->nt;  /* FIX added nt */
	double dz=detector->dz;
	double dr=detector->dr; 
	double da=detector->da;

	//DCFIX
	double nx=detector->nx;//======================================
	double dx=detector->dx;
	double ny=detector->ny;
	double dy=detector->dy;

	double dt=detector->dt; /* FIX added dt */
	char tmp_name[256];
	long num_phot = source->num_photons;
	short num_lay = tissptr->num_layers;

	FILE * file;
	sprintf(tmp_name,"%s%s",pertptr->output_filename,".txt");
	file = fopen(tmp_name, "w"); 

	/* SAVE DATA TO FILE */
	fprintf(file,"Input tissue parameters\n");
	fprintf(file,"Number of layers: %d\n",tissptr->num_layers); 
	fprintf(file,"layer\tn\tmus\tg\tmua\tthickness (cm)\n"); 

	for ( i=1;i<num_lay+1 ;i++ )
	{
		fprintf(file,"%d\t",i);
		fprintf(file,"%G\t",tissptr->layerprops[i].n);
		fprintf(file,"%G\t",tissptr->layerprops[i].mus);
		fprintf(file,"%G\t",tissptr->layerprops[i].g);
		fprintf(file,"%G\t",tissptr->layerprops[i].mua);
		fprintf(file,"%G\n",tissptr->layerprops[i].d);
	}
	fprintf(file,"Input number of photons=%d\n",num_phot);

	fprintf(file,"\n\n\n");
	fprintf(file,"Specular reflection   = %12.4E\n",photptr->Rspec);
	fprintf(file,"Diffuse reflection    = %12.4E\n",outptr->Rd);
	fprintf(file,"Total reflection      = %12.4E\n",outptr->Rtot);
	fprintf(file,"Diffuse transmission  = %12.4E\n",outptr->Td);
	fprintf(file,"Total absorption      = %12.4E\n",outptr->Atot);

	fprintf(file,"\n\n");
	fprintf(file,"Absorption vs layer\n");
	for ( i=1;i<num_lay+1 ;i++ )
	{
		fprintf(file,"Layer %d: \t%f\n",i,outptr->A_layer[i]);
	}
	fprintf(file,"\n\n");

	fprintf(file,"Radially resolved reflection and transmission\n");
	fprintf(file,"r(cm)\tR(r)[W/cm2]\tT(r)[W/cm2]\n");
	for ( ir=0;ir<nr ;ir++ )
	{
		fprintf(file,"%.4e\t%.4e\t%.4e\n",(ir+0.5)*dr,outptr->R_r[ir],
			outptr->T_r[ir]);
	}
	fprintf(file,"\n\n");

	/* FIX ADDED R(r,t) OUTPUT */
	 fprintf(file,"Reflection vs r and time [W/cm2/ps]\n");
	 fprintf(file,"The top row is time (in ps)\n");
	 fprintf(file,"The first column is radius (in cm)\n");
	 fprintf(file,"\t\tincreasing time ------->\n");
	 fprintf(file,"           \t");
	 for ( it=0;it<nt ;it++ )
	 {
		fprintf(file,"%.4e\t",(it+0.5)*dt);
	 }
	 fprintf(file,"\n");
	 for ( ir=0;ir<nr ;ir++ )
	 {
		fprintf(file,"%.4e\t",(ir+0.5)*dr);
		for ( it=0;it<nt ;it++ )
		{
			fprintf(file,"%.4e\t",outptr->R_rt[ir][it]);
		}
		fprintf(file,"\n");
	 }
	 fprintf(file,"\n\n");
	 /* END FIX */

	fprintf(file,"Angular resolved reflection and transmission\n");
	fprintf(file,"a(rad) \t R(a)[W/Sr] \t T(a)[W/Sr]\n");
	for ( ia=0;ia<na ;ia++ )
	{
		fprintf(file,"%.4e\t%.4e\t%.4e\n",(ia+0.5)*da,outptr->R_a[ia],
			outptr->T_a[ia]);
	}
	fprintf(file,"\n\n");

	fprintf(file,"Reflection vs r and angle [W/cm2/Sr]\n");
	fprintf(file,"The top row is angle (in rad)\n");
	fprintf(file,"The first column is radius (in cm)\n");
	fprintf(file,"\t\tincreasing angle ------->\n");
	fprintf(file,"           \t");
	for ( ia=0;ia<na ;ia++ )
	{
		fprintf(file,"%.4e\t",(ia+0.5)*da);
	}
	fprintf(file,"\n");
	for ( ir=0;ir<nr ;ir++ )
	{
		fprintf(file,"%.4e\t",(ir+0.5)*dr);
		for ( ia=0;ia<na ;ia++ )
		{
			fprintf(file,"%.4e\t",outptr->R_ra[ir][ia]);
		}
		fprintf(file,"\n");
	}
	fprintf(file,"\n\n");


	fprintf(file,"Transmission vs r and angle [W/cm2/Sr]\n");
	fprintf(file,"The top row is angle (in rad)\n");
	fprintf(file,"The first column is radius (in cm)\n");
	fprintf(file,"\t\tincreasing angle ------->\n");
	fprintf(file,"           \t");
	for ( ia=0;ia<na ;ia++ )
	{
		fprintf(file,"%.4e\t",(ia+0.5)*da);
	}
	fprintf(file,"\n");
	for ( ir=0;ir<nr ;ir++ )
	{
		fprintf(file,"%.4e\t",(ir+0.5)*dr);
		for ( ia=0;ia<na ;ia++ )
		{
			fprintf(file,"%.4e\t",outptr->T_ra[ir][ia]);
		}
		fprintf(file,"\n");
	}
	fprintf(file,"\n\n");


	/* Save Fluence and absorption */

	fprintf(file,"Depth resolved fluence and absorption\n");
	fprintf(file,"depth (cm)\tfluence[-]\tabsorption[W/cm]\n");
	for ( iz=0;iz<nz ;iz++ )
	{
		fprintf(file,"%.4e\t%.4e\t%.4e\n",(iz+0.5)*dz,
			outptr->Flu_z[iz],outptr->A_z[iz]);
	}
	fprintf(file,"\n\n");

	fprintf(file,"Fluence vs r and z [W/cm2]\n");
	fprintf(file,"The top row is radius (in cm)\n");
	fprintf(file,"The first column is depth (in cm)\n");
	fprintf(file,"\t\tincreasing radius ------->\n");
	fprintf(file,"           \t");
	for ( ir=0;ir<nr ;ir++ )
	{
		fprintf(file,"%.4e\t",(ir+0.5)*dr);
	}
	fprintf(file,"\n");
	for ( iz=0;iz<nz ;iz++ )
	{
		fprintf(file,"%.4e\t",(iz+0.5)*dz);
		for ( ir=0;ir<nr ;ir++ )
		{
			fprintf(file,"%.4e\t",outptr->Flu_rz[ir][iz]);
		}
		fprintf(file,"\n");
	}
	fprintf(file,"\n\n");

	fprintf(file,"Absorption vs r and z [W/cm3]\n");
	fprintf(file,"The top row is radius (in cm)\n");
	fprintf(file,"The first column is depth (in cm)\n");
	fprintf(file,"\t\tincreasing radius ------->\n");
	fprintf(file,"           \t");
	for ( ir=0;ir<nr ;ir++ )
	{
		fprintf(file,"%.4e\t",(ir+0.5)*dr);
	}
	fprintf(file,"\n");
	for ( iz=0;iz<nz ;iz++ )
	{
		fprintf(file,"%.4e\t",(iz+0.5)*dz);
		for ( ir=0;ir<nr ;ir++ )
		{
			fprintf(file,"%.4e\t",outptr->A_rz[ir][iz]);
		}
		fprintf(file,"\n");
	}
	fprintf(file,"\n\n");

	//DCFIX
  /* R_xy added */
 fprintf(file,"Cartesian resolved reflection\n");
 fprintf(file,"x(cm)\t    y(cm)\t    R(r)[W/cm2]\n");
 for ( ix=0;ix<nx*2 ;ix++ )
 {
    for ( iy=0;iy<ny*2 ;iy++ )
    {
		fprintf(file,"%.4e\t",(ix+0.5)*dx-nx*dx);
		fprintf(file,"%.4e\t",(iy+0.5)*dy-ny*dy);
        fprintf(file,"%.4e\n",outptr->R_xy[ix][iy]);
    }
 }
 fprintf(file,"\n\n");



	///* scale expected value and compute % error */
	//for(ir=0;ir<nr;ir++)
	//{
	//	tmpR=outptr->Rev[0][ir];
	//	tmpR2=outptr->Rev[1][ir];
	//	/* computer % error */
	//	percent_error=100.0*sqrt(tmpR2/(tmpR*tmpR)-1/source->num_photons);
	//	outptr->Rev[1][ir]=percent_error;
	//	/* scale reflectance */
	//	outptr->Rev[0][ir] /= 2.0*PI*(ir+0.5)*dr*dr*num_phot;
	//}
}

