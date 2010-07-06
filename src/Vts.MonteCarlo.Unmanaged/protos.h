#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*****************************************************************
 *  function prototypes
 *****/
void ReadInput(FILE *);
void initialize(void);
void DisplayIntro(void);
void DisplayStatus(long, long);
void Scatter(void);
void init_photon(void);
void SetStepSize(void);
short HitBoundary(void);
void CrossLayer(void);
void Move_Photon(void);
void Absorb(void);
void Scatter_Or_Absorb(void);
void TestWeight(void);
void FreeMemory(void);
void Test_Distance(void);
void Write_Or_Pert();

/* mc_2_photon_io.c */
void Compute_EV(void);
void Write_3_Bytes(double, FILE *);
short Intersect_Cylinder(double, double, double, double,
			 double, double);
short Heading_Towards_Cylinder(double, double, double, double, double);
short Soln_Exists(double, double, double, double, double *,double *);

void Write_Photon_To_Disk(int);
void Display_Photon_Data(void);

/* save_text.c */
void SaveTextResult(void);
void NormalizeResults(void);

  /* mc_utils.c */
  double ran3(int *idum);
  __declspec(dllexport) double RandomNum(void);
  double ***d3tensor(long,long,long,long,long,long);
  void free_d3tensor(double ***,long,long,long,long,long,long);
  double ****d4tensor(long,long,long,long,long,long,long,long);
  void free_d4tensor(double ****,long,long,long,long,long,long,long,long);

#ifdef __cplusplus
}
#endif /* __cplusplus */
