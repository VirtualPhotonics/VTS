#define MAX_DET 10
void pert(void);
void init_pert(void);

struct perturb
{
  /* USER SPECIFIED DATA */
  //int do_ellip_layer;  /* 0=nopert 1=pert:ellip 2=pert:layer 3=nopert:ellip */ 
  ///* ellipsoid data */
  ///* center position */
  //double ellip_x, ellip_y, ellip_z;
  ///* radius dimensions */
  //double ellip_rad_x, ellip_rad_y, ellip_rad_z;
  ///* layer info */
  //double layer_z_min, layer_z_max;
  
  ///* detector data */
  //int reflect_flag;  /* 1=reflect 0=transmit */
  ///* number of x-axis bins */
  //int nr;
  //double det_ctr[MAX_DET];
  //double det_rad;
  //double det_NA;
  
  /* russian roulette data */
  double wt_cut;
  double prr;
  /* END USER */
  int tot_phot;
  int col_in_layer[14];
  double pathlen_in_layer[14];
  int tot_out_top;
  int tot_out_bot;
  char output_filename[256]; 
  int col_hit_bdry;

};

/* prototypes */
int In_Detector(double, double, double, double, double);
int InEllipsoid(double, double, double);
void Ray_Intersect_Ellip(double, double, double,
                         double, double, double,
                         double *, double *);
void Ray_Intersect_Layer(double, double, double,
                         double, double, double,
                         double *, double *);
//void Pert_Col(double, double, double);
//void Pert_Path_Len(double, double, double,
//                     double, double, double,int);

