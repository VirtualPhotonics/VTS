#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#include<stdio.h>

#define MAX_HISTORY_PTS 300000
#define PI 3.141592654
#define C_CM 3e10 /* speed of light in cm/s */
#define MAX_COORD 30.0
#define FACTOR 1e3 /* The resolution is defined as (1e-2)/FACTOR */
#define MAX_NUM_LAYERS 14
#define MAX_DET 10

  struct Layer{
    double n;
    double mua;
    double mus;
    double albedo;
    double g;
    double d;
    double zbegin, zend;
	double scatter_length; // DAW/CAW add
  };

    struct Flags{
	  int Seed;
	  int AbsWtType;
  };

  struct History{
    double *xh; /* CKH FIX */
	double *yh;
    double *zh;
    double *uxh;
    double *uyh;
    double *uzh;
    double *weight;
	double *pert_wt; // CKH degrade pert_wt from [3,MAX_HISTORY_PTS]
    double *path_length;
    int *boundary_col;
    int num_pts_stored;
    double cum_path_length;
  };

  struct Photon{
    double x,y,z;
    double ux, uy, uz;
    double s;
    double sleft; /* remaining step size when photon hits boundary */
    short dead;
    double w;
    double Rspec;			/* Specular reflectance */   
    double max_path_length;
    //struct str_history history;
    short hit_bdry;
	short curr_layer;		/* index for layerprops */
	//struct Layer *layerprops;	/* CKH FIX pointer to Layer structure */
    double *num_photons_written; /* CKH FIX */
	int curr_n; // CKH added 12/6
  };
  
  struct DetectorDefinition{	  
    double dz, dr, da, dt, dx, dy;
    short nz, nr, na, nt, nx, ny;

	/* detector data */
	int reflect_flag;  /* 1=reflect 0=transmit */
	/* number of x-axis bins */
	double det_ctr[MAX_DET];
	double det_rad;
	double det_NA;  
  };

  struct SourceDefinition{	  
    int num_photons;  /* CKH FIX */
	double beam_radius;
    double beam_center_x;
    char *beamtype; /* CKH FIX */
    double src_NA;
  };

  struct Tissue{
	int do_ellip_layer;  /* 0=nopert 1=pert:ellip 2=pert:layer 3=nopert:ellip */ 
	/* ellipsoid data */
	/* center position */
	double ellip_x, ellip_y, ellip_z;
	/* radius dimensions */
	double ellip_rad_x, ellip_rad_y, ellip_rad_z;
	/* layer info */
	double layer_z_min, layer_z_max;
    short num_layers;
    double z_focus;    /* focus depth */
    double NA;         /* NA */
    double cylinder_radius,cylinder_height;
    double zup,zlow;
	struct Layer *layerprops;	/* CKH FIX pointer to Layer structure *
  };
  
  struct OutputHeaderInfo{
    struct Layer layerprops[MAX_NUM_LAYERS];
    long num_photons;
    short num_layers;
    double det_rmin,det_rmax,det_radius;
    short Absorption_Weighting_Used; /* flag for absorption weighting */
  };
  
  struct Output{
    double **A_rz;
    double *A_z;
    double *A_layer;
    double Atot;
    double **Flu_rz;
    double *Flu_z;
    double **R_ra;
    double *R_r, *R_a, *R_r2; 
    double **R_xy,**T_ra;
    double *T_r, *T_a;
    double Rd,Rtot;
    double Td;
    double **R_rt;
    double wt_pathlen_out_top;
    double wt_pathlen_out_bot;
    double wt_pathlen_out_sides;
    double ****in_side_allvox,****out_side_allvox; /* FIX added */
    //double ****in_side2_allvox,****out_side2_allvox; /* FIX added */ 
	double** Amp_rw, Phase_rw, re_rw, im_rw;
	double* path_length_in_layer;
	double** R0_rt; // Method DC
    double*** D_rt_layer; // Method DC

    double nomega;
    double cramer_wt;
    double Rconv,Rconv2;
    int num_visit_conv,num_visit;
    //FILE *cramer_start;
    FILE *binary_file[MAX_DET];
    //FILE *text_file;
  };

//typedef struct Photon PHOTON;
//typedef struct Tissue TISSUE;
//typedef struct Output OUTPUT;
//typedef struct perturb PERTURB;

  /*void UpdateBar(GtkWidget *, long);*/
void ReadInput(FILE *);
void DisplayInput();
void initialize(char* inFileName);
void DisplayIntro(void);
void DisplayStatus(long, long);
void Scatter(void);
void init_photon(void);
void init_photon_cramer(void);
void SetStepSize(void);
short HitBoundary(void);
void CrossLayer(void);

//DCFIX
short HitLayer(void);
short HitEllip(void);
void CrossEllip(void); //========================

void Move_Photon(void);
void Absorb(void);
void Scatter_Or_Absorb(void);
void TestWeight(void);
void FreeMemory(void);
void Test_Distance(void);
void TestWeight(void);
void Scatter1D(void);

void SaveResults(void);

//__declspec(dllexport) void initialize_from_external(PHOTON *photptr_ex, TISSUE *tissptr_ex, OUTPUT *outptr_ex, PERTURB *pertptr_ex);
void RunMCCHInternal(char* inFileName);
void RunMCLoop(void);
__declspec(dllexport) void RunMCLoopExternal(struct Photon *photptr_ex, struct Tissue *tissptr_ex, struct perturb *pertptr_ex, struct Output *outptr_ex);
//__declspec(dllexport) void RunMCLoop();

#ifdef __cplusplus
}
#endif /* __cplusplus */

