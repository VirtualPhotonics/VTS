using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.FemModeling.MGRTE._2D.SourceInputs;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// FEM 2D MG_RTE Simulation inputs
    /// </summary>
    public class SimulationInput
    {
        /// <summary>
        /// Input data for spatial and angular mesh
        /// </summary>
        public SquareMeshDataInput MeshDataInput;
        /// <summary>
        ///  Mesh simulation parameters
        /// </summary>
        public MeshSimulationOptions SimulationParameterInput;
        /// <summary>
        /// Specifying external source
        /// </summary>
        public IExtFemSourceInput ExtSourceInput;
        /// <summary>
        /// Specifying internal source
        /// </summary>
        public IIntFemSourceInput IntSourceInput;
        /// <summary>
        /// Specifying tissue definition
        /// </summary>
        public ITissueInput TissueInput;
        /// <summary>
        /// Specifying inclusion definition
        /// </summary>
        public IInclusionInput InclusionInput;


        /// <summary>
        /// General constructor for simulation inputs 
        /// </summary>
        /// <param name="meshDataInput">Input data for spatial and angular mesh</param>
        /// <param name="simulationParameterInput">Mesh simulation parameters</param> 
        /// <param name="extSourceInput">Specifying external source</param>
        /// <param name="intSourceInput">Specifying internal source</param>
        /// <param name="tissueInput">Specifying tissue definition</param>
        /// <param name="inclusionInput">Specifying single tissue inclusion </param>
        public SimulationInput(
            SquareMeshDataInput meshDataInput,
            MeshSimulationOptions simulationParameterInput,
            IExtFemSourceInput extSourceInput,
            IIntFemSourceInput intSourceInput,
            ITissueInput tissueInput,
            IInclusionInput inclusionInput
            )
        {
            MeshDataInput = meshDataInput;
            SimulationParameterInput = simulationParameterInput;
            ExtSourceInput = extSourceInput;
            IntSourceInput = intSourceInput;
            TissueInput = tissueInput;
            InclusionInput = inclusionInput;
        }

        /// <summary>
        /// Default constructor for simulation inputs
        /// </summary>
        public SimulationInput()
            : this(new SquareMeshDataInput(),
                  new MeshSimulationOptions(),
                  new ExtPointSourceInput(),
                  null,
                  new MultiLayerTissueInput(),
                  new MultiLayerInclusionInput()) { }
    }
}
