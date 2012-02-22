using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Complete input parameters
    /// </summary>
    public class SimulationInputs : BindableObject
    {
        private double nTissue, nExt;
        private int aMeshLevel;
        private int sMeshLevel;
        private int mgMethod, fullMg;
        private int nPreIteration, nPostIteration, nMgCycle, nIterations;
        private double convTol;
        private double medMua, medMusp, medG;
        private double length;
        private double inMua, inMusp, inG;
        private double inRad, inX, inZ;

        /// <summary>
        /// Absorption coefficient of the tissue
        /// </summary>
        public double MedMua
        {
            get { return medMua; }
            set { medMua = value; this.OnPropertyChanged("MedMua"); }
        }

        /// <summary>
        /// Reduced scattering coefficient of the tissue
        /// </summary>
        public double MedMusp
        {
            get { return medMusp; }
            set { medMusp = value; this.OnPropertyChanged("MedMusp"); }
        }
          
        /// <summary>
        /// Anisotropy factor of the tissue
        /// </summary>
        public double MedG
        {
            get {return medG;}
            set { medG = value; this.OnPropertyChanged("MedG"); }
        }

        /// <summary>
        /// Absorption coefficient of the inclusion
        /// </summary>
        public double InMua
        {
            get { return inMua; }
            set { inMua = value; this.OnPropertyChanged("InMua"); }
        }

        /// <summary>
        /// Reduced scattering coefficient of the inclusion
        /// </summary>
        public double InMusp
        {
            get { return inMusp; }
            set { inMusp = value; this.OnPropertyChanged("InMusp"); }
        }

        /// <summary>
        /// Anisotropy factor of the inclusion
        /// </summary>
        public double InG
        {
            get { return inG; }
            set { inG = value; this.OnPropertyChanged("InG"); }
        }      

        /// <summary>
        /// Radius of the inclusion
        /// </summary>
        public double InRad
        {
            get { return inRad; }
            set { inRad = value; this.OnPropertyChanged("InRad"); }
        }

        /// <summary>
        /// X coordinate of the inclusion
        /// </summary>
        public double InX
        {
            get { return inX; }
            set { inX = value; this.OnPropertyChanged("InX"); }
        }

        /// <summary>
        /// Z coordinate of the inclusion
        /// </summary>
        public double InZ
        {
            get { return inZ; }
            set { inZ = value; this.OnPropertyChanged("InZ"); }
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
        /// The finest layer of spatial mesh generation
        /// </summary>
        public int SMeshLevel
        {
            get{return sMeshLevel;}
            set { sMeshLevel = value; this.OnPropertyChanged("SMeshLevel"); }
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
