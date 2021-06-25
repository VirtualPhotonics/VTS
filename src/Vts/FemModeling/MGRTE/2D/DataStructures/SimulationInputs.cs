using Vts.FemModeling.MGRTE._2D.SourceInputs;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// FEM 2D MG_RTE Simulation inputs
    /// </summary>
    public class SimulationInput
    {
        private SquareMeshDataInput _meshDataInput;
        private MeshSimulationOptions _simulationOptionsInput;
        private IExtFemSourceInput _extSourceInput;
        private IIntFemSourceInput _intSourceInput;
        private ITissueInput _tissueInput;
        /// <summary>
        /// Input data for spatial and angular mesh
        /// </summary>
        public SquareMeshDataInput MeshDataInput { get { return _meshDataInput; } set { _meshDataInput = value; } }
        /// <summary>
        ///  Mesh simulation parameters
        /// </summary>
        public MeshSimulationOptions SimulationOptionsInput { get { return _simulationOptionsInput; } set { _simulationOptionsInput = value; } }
        /// <summary>
        /// Specifying external source
        /// </summary>
        public IExtFemSourceInput ExtSourceInput { get { return _extSourceInput; } set { _extSourceInput = value; } }
        /// <summary>
        /// Specifying internal source
        /// </summary>
        public IIntFemSourceInput IntSourceInput { get { return _intSourceInput; } set { _intSourceInput = value; } }
        /// <summary>
        /// Specifying tissue definition
        /// </summary>
        public ITissueInput TissueInput { get { return _tissueInput; } set { _tissueInput = value; } }

        /// <summary>
        /// General constructor for simulation inputs 
        /// </summary>
        /// <param name="meshDataInput">Input data for spatial and angular mesh</param>
        /// <param name="simulationOptionsInput">Mesh simulation options input</param> 
        /// <param name="extSourceInput">Specifying external source</param>
        /// <param name="intSourceInput">Specifying internal source</param>
        /// <param name="tissueInput">Specifying tissue definition</param>
        public SimulationInput(
            SquareMeshDataInput meshDataInput,
            MeshSimulationOptions simulationOptionsInput,
            IExtFemSourceInput extSourceInput,
            IIntFemSourceInput intSourceInput,
            ITissueInput tissueInput
            )
        {
            MeshDataInput = meshDataInput;
            SimulationOptionsInput = simulationOptionsInput;
            ExtSourceInput = extSourceInput;
            IntSourceInput = intSourceInput;
            TissueInput = tissueInput;
        }

        /// <summary>
        /// Default constructor for simulation inputs
        /// </summary>
        public SimulationInput()
            : this(new SquareMeshDataInput(),
                  new MeshSimulationOptions(),
                  new ExtPointSourceInput(),
                  null,
                  new MultiEllipsoidTissueInput()) { }
    }
}
