using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Complete input parameters
    /// </summary>
    public class MeshDataInput : BindableObject
    {

        private int _aMeshLevel;
        private int _sMeshLevel;
        private double _sideLength;

        /// <summary>
        /// Constructor for mesh input data
        /// </summary>
        /// <param name="aMeshLevel">The finest layer of angular mesh generation</param>
        /// <param name="sMeshLevel">The finest layer of spatial mesh generation</param>
        /// <param name="sideLength">Length of the square mesh</param>
        public MeshDataInput(int aMeshLevel, int sMeshLevel, double sideLength)
        {
            AMeshLevel = aMeshLevel;
            SMeshLevel = sMeshLevel;
            SideLength = sideLength;
        }

        /// <summary>
        /// Default constructor for mesh input data
        /// </summary>
        public MeshDataInput()
            : this(5, 3, 1.0) { }

        /// <summary>
        /// The finest layer of angular mesh generation
        /// </summary>
        public int AMeshLevel
        {
            get { return _aMeshLevel; }
            set { _aMeshLevel = value; this.OnPropertyChanged("AMeshLevel"); }
        }

        /// <summary>
        /// The finest layer of spatial mesh generation
        /// </summary>
        public int SMeshLevel
        {
            get { return _sMeshLevel; }
            set { _sMeshLevel = value; this.OnPropertyChanged("SMeshLevel"); }
        }

        /// <summary>
        /// Length of the square mesh
        /// </summary>
        public double SideLength
        {
            get { return _sideLength; }
            set { _sideLength = value; this.OnPropertyChanged("SideLength"); }
        }
    }
}
