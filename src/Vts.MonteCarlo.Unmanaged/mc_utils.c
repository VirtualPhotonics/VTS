#include <stdio.h>
#include <math.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#include "mc_main.h"
#include "mc_utils.h"
#include "protos.h"

#define Boolean char
//#define STANDARDTEST 1 // field in flags structure now
#define ONE (1.0-1e-12)
#define NR_END 1
#define FREE_ARG char*

extern struct Photon *photptr;
extern struct Flags *flagptr;
extern struct Tissue *tissptr;

/*****************************************************************/

/* random number generator */
#define MBIG 1000000000
#define MSEED 161803398
#define MZ 0
#define FAC 1.0E-9

double ran3(int *idum)
{
  static int inext,inextp;
  static long ma[56];
  static int iff=0;
  long mj,mk;
  int i,ii,k;

  if (*idum < 0 || iff == 0) {
    iff=1;
    mj=MSEED-(*idum < 0 ? -*idum : *idum);
    mj %= MBIG;
    ma[55]=mj;
    mk=1;
    for (i=1;i<=54;i++) {
      ii=(21*i) % 55;
      ma[ii]=mk;
      mk=mj-mk;
      if (mk < MZ) mk += MBIG;
      mj=ma[ii];
    }
    for (k=1;k<=4;k++)
      for (i=1;i<=55;i++) {
        ma[i] -= ma[1+(i+30) % 55];
        if (ma[i] < MZ) ma[i] += MBIG;
      }
    inext=0;
    inextp=31;
    *idum=1;
  }
  if (++inext == 56) inext=1;
  if (++inextp == 56) inextp=1;
  mj=ma[inext]-ma[inextp];
  if (mj < MZ) mj += MBIG;
  ma[inext]=mj;
  return (double)mj*FAC;  
}

#undef MBIG
#undef MSEED
#undef MZ
#undef FAC

/*****************************************************************/
/* generate Random number using ran3(). */
/*      Taken from Numerical Recipes in C */
__declspec(dllexport) double RandomNum(void)
{
  static Boolean first_time=1;
  static int idum;      /* seed for ran3. */
  double RN;

  if(first_time) {
//#if STANDARDTEST /* Use fixed seed to test the program. */
  if (flagptr->Seed==0) 
    idum = - 1;
//#else
  else
    idum = -(int)time(NULL)%(1<<15);
    /* use 16-bit integer as the seed. */
//#endif
    //ran3(&idum);  // CKH FIX to match managed
    first_time = 0;
    idum = 1;
  }
  RN = ran3(&idum);
  return( RN ); 
  //return( (double)ran3(&idum) ); 
}

/*****************************************************************/
double ***d3tensor(long nrl, long nrh, long ncl, long nch, long ndl, long ndh)
/* allocate a double 3tensor with range t[nrl..nrh][ncl..nch][ndl..ndh] */
/* from f3tensor in nrutil.c */
{
	long i,j,nrow=nrh-nrl+1,ncol=nch-ncl+1,ndep=ndh-ndl+1;
	double ***t;

	/* allocate pointers to pointers to rows */
	t=(double ***) malloc((size_t)((nrow+NR_END)*sizeof(double**)));
	if (!t) nrerror("allocation failure 1 in d3tensor()");
	t += NR_END;
	t -= nrl;

	/* allocate pointers to rows and set pointers to them */
	t[nrl]=(double **) malloc((size_t)((nrow*ncol+NR_END)*sizeof(double*)));
	if (!t[nrl]) nrerror("allocation failure 2 in d3tensor()");
	t[nrl] += NR_END;
	t[nrl] -= ncl;

	/* allocate rows and set pointers to them */
	t[nrl][ncl]=(double *) malloc((size_t)((nrow*ncol*ndep+NR_END)*sizeof(double)));
	if (!t[nrl][ncl]) nrerror("allocation failure 3 in d3tensor()");
	t[nrl][ncl] += NR_END;
	t[nrl][ncl] -= ndl;

	for(j=ncl+1;j<=nch;j++) t[nrl][j]=t[nrl][j-1]+ndep;
	for(i=nrl+1;i<=nrh;i++) {
		t[i]=t[i-1]+ncol;
		t[i][ncl]=t[i-1][ncl]+ncol*ndep;
		for(j=ncl+1;j<=nch;j++) t[i][j]=t[i][j-1]+ndep;
	}

	/* return pointer to array of pointers to rows */
	return t;
}
/*****************************************************************/
/* THIS NEEDS WORK DO NOT USE! */
double ****d4tensor(long nrl, long nrh, long ncl, long nch, 
       long ndl, long ndh, long nel, long neh)
/* allocate a double 3tensor with range */
/* t[nrl..nrh][ncl..nch][ndl..ndh][nel..neh] */
/* from f3tensor in nrutil.c */
{
	long i,j,k,nrow=nrh-nrl+1,ncol=nch-ncl+1,ndep=ndh-ndl+1;
        long ndep2=neh-nel+1;
	double ****t;

	/* allocate pointers to pointers to pointers to rows */
	t=(double ****) malloc((size_t)((nrow+NR_END)*sizeof(double***)));
	if (!t) nrerror("allocation failure 1 in d4tensor()");
	t += NR_END;
	t -= nrl;

	/* allocate pointers to pointers to rows and set pointers to them */
	t[nrl]=(double ***) malloc((size_t)((nrow*ncol+NR_END)*
          sizeof(double**)));
	if (!t[nrl]) nrerror("allocation failure 2 in d4tensor()");
	t[nrl] += NR_END;
	t[nrl] -= ncl;

	/* allocate pointers to rows and set pointers to them */
	t[nrl][ncl]=(double **) malloc((size_t)
          ((nrow*ncol*ndep+NR_END)*sizeof(double*)));
	if (!t[nrl][ncl]) nrerror("allocation failure 3 in d4tensor()");
	t[nrl][ncl] += NR_END;
	t[nrl][ncl] -= ndl;

	/* allocate rows and set pointers to them */
	t[nrl][ncl][ndl]=(double *) malloc((size_t)
          ((nrow*ncol*ndep*ndep2+NR_END)*sizeof(double)));
	if (!t[nrl][ncl][ndl]) nrerror("allocation failure 4 in d4tensor()");
	t[nrl][ncl][ndl] += NR_END;
	t[nrl][ncl][ndl] -= nel;

	for(j=ndl+1;j<=ndh;j++) t[nrl][ncl][j]=t[nrl][ncl][j-1]+ndep2;
	for(i=nrl+1;i<=nrh;i++) {
		t[i]=t[i-1]+ncol;
		t[i][ncl]=t[i-1][ncl]+ncol*ndep;
		t[i][ncl][ndl]=t[i-1][ncl][ndl]+ncol*ndep*ndep2;
		for(j=ncl+1;j<=nch;j++) {
                   t[i][j]=t[i][j-1]+ndep*ndep2;
		   for(k=ndl+1;k<=ndh;k++) 
                     t[i][j][k]=t[i][j][k-1]+ndep2;
                }
	}

	/* return pointer to array of pointers to rows */
	return t;
}

/*****************************************************************/
void free_d3tensor(double ***t, long nrl, long nrh, long ncl, long nch,
	long ndl, long ndh)
/* free a double d3tensor allocated by d3tensor() */
{
	free((FREE_ARG) (t[nrl][ncl]+ndl-NR_END));
	free((FREE_ARG) (t[nrl]+ncl-NR_END));
	free((FREE_ARG) (t+nrl-NR_END));
}
/*****************************************************************/
void free_d4tensor(double ****t, long nrl, long nrh, long ncl, long nch,
	long ndl, long ndh, long nel, long neh)
/* free a double d4tensor allocated by d4tensor() */
{
	free((FREE_ARG) (t[nrl][ncl][ndl]+nel-NR_END));
	free((FREE_ARG) (t[nrl][ncl]+ndl-NR_END));
	free((FREE_ARG) (t[nrl]+ncl-NR_END));
	free((FREE_ARG) (t+nrl-NR_END));
}
/*****************************************************************/
void Write_3_Bytes(double double_var, FILE * fptr)
{
  unsigned char *out_array_ptr;
  unsigned char out_array[3];
  unsigned char res;
  int int_var;
  unsigned int in, abs_int_var;
  
  /* convert double to an  integer */
  if(double_var<0)
    int_var = (int)(double_var*1e5-0.5);
  else
    int_var = (int)(double_var*1e5+0.5);

  if (int_var<0)
    abs_int_var= -1*int_var;
  else
    abs_int_var=int_var;
  
  /* first 8 bits */
  in=abs_int_var & 0xff;
  res=in;
  out_array_ptr=out_array;
  *out_array_ptr=res;
  
  /* 2nd 8 bits */
  in=abs_int_var & 0xff00;
  res=in >> 8;
  out_array_ptr++;
  *out_array_ptr=res;
  
  /* last 8 bits */
  in=abs_int_var & 0xff0000;
  res=in >> 16;
  if(int_var<0)
    res=res|128;
  out_array_ptr++;
  *out_array_ptr=res;
  
  /* now write 3 bytes to output file */
  fwrite(out_array,3,1,fptr);
}
/************************************************************/
double Char3_To_Double(unsigned char * in_array_ptr)
{
  unsigned int rest_int1,rest_int2,rest_int3,restored_int;
  unsigned char res;
  double restored_double, sign;
  
  res= *in_array_ptr;
  rest_int1= res;
  
  in_array_ptr++;
  res= *in_array_ptr;
  rest_int2= res << 8;
  
  in_array_ptr++;
  res= *in_array_ptr;
  if(res&128)
    { res = res-128;
    sign= -1.0;
    }
  else
    sign=1.0;
  rest_int3= res << 16;
  
  restored_int=rest_int1+rest_int2+rest_int3;
  restored_double= sign*((double)(restored_int))*1e-5;
  /*printf("%.6f\n",restored_double);*/
  return(restored_double);
}

/************************************************************/

double Specular()
{
  double R;
  double n_air = tissptr->layerprops[0].n;
  double n_tiss = tissptr->layerprops[1].n;
  
  R = (n_air-n_tiss)*(n_air-n_tiss)/((n_air+n_tiss)*(n_air+n_tiss));
  /* printf("Specular: n_air=%f n_tiss=%f R=%f\n",n_air,n_tiss,R); */
  return(R);
}

/********************************************************/
