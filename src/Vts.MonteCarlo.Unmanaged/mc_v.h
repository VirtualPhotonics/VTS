#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

struct bvolume{
  double dx,dy,dz;
  int nx,ny,nz;
  int banana_photons;  /* number of photons contributing to banana */
  int num_mu;
  int num_phi;
  int num_sa;
};

struct vox_list{  /* used only by allvox */
  int ix,iy,iz;
  int col;  
  int sa_idx; 
  struct vox_list *next;
};

void init_banana_plane(void);
void init_banana_cube(void);
void init_banana_allvox(void);
void Compute_Banana(void);
void Compute_Prob_plane(void);
void Compute_Prob_cube(void);
void Write_Wt_Table(FILE *, double ***);
void Read_Wt_Table(void);
void Output_Wts_plane(void);
void Output_Wts_cube(void);
void Output_Wts_allvox(void);
void Free_List(struct vox_list *);
void Add_To_List(struct vox_list **,int,int,int,int,int);
void Angular_Bin_plane(double,double,double,double,double,double,
		int,int *,int *);
void Angular_Bin_cube(double,double,double,double,double,double,
		int,int *,int *);
int Angular_Bin_allvox(double,double,double,double,double,double);
void Ray_Intersect_Cube(double,double,double,double,double,double,
		      double,double,double,double,double,double, 
		      int *,int *,int *, int *, int *, int *, int *,
                      double *,double *);
int Ray_Intersect_Plane(double,double,double,double,double,double,
                    double *,double *,double *);


#ifdef __cplusplus
}
#endif /* __cplusplus */
