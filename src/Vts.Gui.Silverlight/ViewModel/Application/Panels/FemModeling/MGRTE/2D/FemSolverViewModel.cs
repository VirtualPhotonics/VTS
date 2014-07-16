using GalaSoft.MvvmLight.Command;
using Vts.Common.Logging;
using Vts.FemModeling.MGRTE._2D;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// FEM Solver view model
    /// </summary>
    public class FemSolverViewModel : BindableObject
    {
        
        private FemSimulationInputDataViewModel _MeshInputVM;
        //private OpticalPropertyViewModel _MediumOpticalPropertyVM;
        //private OpticalPropertyViewModel _InclusionOpticalPropertyVM;

        ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(FemSolverViewModel));   

        /// <summary>
        /// constructor for FEM forward solver model
        /// </summary>
        public FemSolverViewModel()
        {
            _MeshInputVM = new FemSimulationInputDataViewModel();
            //_MediumOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "Medium Optical Properties:", G = 0.8, N = 1.0, EnableG = true, EnableN = false };
            //_InclusionOpticalPropertyVM = new OpticalPropertyViewModel() { Title ="Inclusion Optical Properties:", G = 0.8, N = 1.0, EnableG = true, EnableN = false };
            
            ExecuteFemSolverCommand = new RelayCommand(() => FemExecuteFemSolverExecuted(null, null));
        }

        
        /// <summary>
        /// Execute FEM Forward Solver command
        /// </summary>
        public RelayCommand ExecuteFemSolverCommand { get; private set; }

        /// <summary>
        /// Update FEM Simulation input data
        /// </summary>
        public FemSimulationInputDataViewModel MeshInputVM
        {
            get { return _MeshInputVM; }
            set
            {
                _MeshInputVM = value;
                OnPropertyChanged("MeshInputVM");
            }
        }
        
       
        /// <summary>
        /// Execute FEM Sovler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FemExecuteFemSolverExecuted(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            ExecuteSolver();
        }

        /// <summary>
        // This is the main function for RTE_2D.
        // Note: we assume the spatial domain has "nt" intervals,
        // starting from "-x" to "+x" with increasing "x" coordinate;
        // starting from "0" to "+z" with increasing "z" coordinate;   
        /// </summary>
        private void ExecuteSolver()
        {
            if ((_MeshInputVM.AMeshLevel > 8) || (_MeshInputVM.SMeshLevel > 8))
                logger.Info(() => "Angular or Spatial mesh level is larger than 8\n");
            else
            {
               Measurement measurement = SolverMGRTE.ExecuteMGRTE(_MeshInputVM.CurrentInput);
               var meshData = new MapData(measurement.inten, measurement.xloc, measurement.zloc, measurement.dx, measurement.dz);
                Commands.Mesh_PlotMap.Execute(meshData);
            }
        }
    }
}
