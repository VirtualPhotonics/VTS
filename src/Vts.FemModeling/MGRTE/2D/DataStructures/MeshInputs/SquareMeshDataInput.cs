namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Angular and spatial mesh data for a square mesh
    /// </summary>
    public class SquareMeshDataInput
    {
        /// <summary>
        /// Constructor for mesh input data
        /// </summary>
        /// <param name="aMeshLevel">The finest layer of angular mesh generation</param>
        /// <param name="sMeshLevel">The finest layer of spatial mesh generation</param>
        /// <param name="sideLength">Length of the square mesh</param>
        public SquareMeshDataInput(int aMeshLevel, int sMeshLevel, double sideLength)
        {
            AMeshLevel = aMeshLevel;
            SMeshLevel = sMeshLevel;
            SideLength = sideLength;
        }

        /// <summary>
        /// Default constructor for mesh input data
        /// </summary>
        public SquareMeshDataInput()
            : this(5, 3, 10.0) { }

        /// <summary>
        /// The finest layer of angular mesh generation
        /// </summary>
        public int AMeshLevel { get; set; }

        /// <summary>
        /// The finest layer of spatial mesh generation
        /// </summary>
        public int SMeshLevel { get; set; }

        /// <summary>
        /// Length of the square mesh
        /// </summary>
        public double SideLength { get; set; }
    }
}
