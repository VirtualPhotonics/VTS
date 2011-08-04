using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.FemModeling.MGRTE._2D
{
    public class Parameters : BindableObject
    {
        private double nTissue, nExt;
        private int aMeshLevel, aMeshLevel0;
        private int sMeshLevel, sMeshLevel0;
        private int mgMethod, fullMg;
        private int nPreIteration, nPostIteration, nMgCycle, nIterations;
        private double convTol;
        private double mua, musp, g;
        private double length;

        /// <summary>
        /// Absorption coefficient of the tissue
        /// </summary>
        public double Mua
        {
            get { return mua; }
            set { mua = value; this.OnPropertyChanged("Mua"); }
        }

        /// <summary>
        /// Reduced scattering coefficient of the tissue
        /// </summary>
        public double Musp
        {
            get { return musp; }
            set { musp = value; this.OnPropertyChanged("Musp"); }
        }
          
        /// <summary>
        /// Anisotropy factor of the tissue
        /// </summary>
        public double G
        {
            get {return g;}
            set{g = value;this.OnPropertyChanged("G");}
        }                

        /// <summary>
        /// Refractive index of the tissue
        /// </summary>
        public double NTissue
        {
            get{return nTissue;}
            set{nTissue = value;this.OnPropertyChanged("NTissue");}
        }

        /// <summary>
        /// Refractive index of the external medium
        /// </summary>
        public double NExt
        {
            get{return nExt;}
            set{nExt = value;this.OnPropertyChanged("NExt");}
        }

        /// <summary>
        /// The finest layer of angular mesh generation
        /// </summary>
        public int AMeshLevel
        {
            get{return aMeshLevel;}
            set{aMeshLevel = value;this.OnPropertyChanged("AMeshLevel");}
        }

        /// <summary>
        /// The coarsest layer of angular mesh in RTE computation
        /// </summary>
        public int AMeshLevel0
        {
            get{return aMeshLevel0;}
            set{aMeshLevel0 = value;this.OnPropertyChanged("AMeshLevel0");}
        }        

        /// <summary>
        /// The finest layer of spatial mesh generation
        /// </summary>
        public int SMeshLevel
        {
            get{return sMeshLevel;}
            set { sMeshLevel = value; this.OnPropertyChanged("SMeshLevel"); }
        }

        /// <summary>
        /// The coarsest layer of spatial mesh in RTE computation
        /// </summary>
        public int SMeshLevel0
        {
            get{return sMeshLevel0;}
            set { sMeshLevel0 = value; this.OnPropertyChanged("SMeshLevel0"); }
        }

        /// <summary>
        /// The choice of multigrid method,
        /// </summary>
        public int MgMethod
        {
            get{return mgMethod;}
            set{mgMethod = value;this.OnPropertyChanged("MgMethod");}
        }

        /// <summary>
        /// The number of pre-iterations with the suggested value "3", see the paper.
        /// </summary>
        public int NPreIteration
        {
            get{return nPreIteration;}
            set{nPreIteration = value;this.OnPropertyChanged("NPreIteration");}
        }

        /// <summary>
        /// The number of post-iterations with the suggested value "3", see the paper.
        /// </summary>
        public int NPostIteration
        {
            get{return nPostIteration;}
            set{nPostIteration = value; this.OnPropertyChanged("NPostIteration");}
        }

        /// <summary>
        /// The number of multigrid cycles on each level except the finest level in 
        /// FMG with the suggested value "1", see the paper.
        /// </summary>
        public int NMgCycle
        {
            get{return nMgCycle;}
            set{nMgCycle = value; this.OnPropertyChanged("NMgCycle");}
        }

        /// <summary>
        /// The indicator of full multigrid method (FMG) with the suggested value "1"
        /// </summary>
        public int FullMg
        {
            get{ return fullMg;}
            set{fullMg = value; this.OnPropertyChanged("FullMg");}
        }

        /// <summary>
        /// The maximum number of iteration on the finest level in FMG
        /// </summary>
        public int NIterations
        {
            get {return nIterations;}
            set {nIterations = value; this.OnPropertyChanged("NIterations");}            
        }
        
        /// <summary>
        /// The residual value of the iteration for stopping criterion
        /// </summary>
        public double ConvTol   
        {
            get {return convTol;}
            set {convTol = value; this.OnPropertyChanged("ConvTol");}
        }

        /// <summary>
        /// Length of the square mesh
        /// </summary>
        public double Length
        {
            get{return length;}
            set{length = value;this.OnPropertyChanged("Length");}
        }
    }
}
