using System;
using Vts.Gui.Silverlight.ViewModel;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// MG-RTE simulation parameters
    /// </summary>
    public class FemSimulationInputDataViewModel : BindableObject
    {
        private SimulationInput _simulationInput;
        private OptionViewModel<string> _tissueTypeViewModel;
        
        private object _tissueInputVM;

        /// <summary>
        /// FEM Simulation Input Data View Model
        /// </summary>
        /// <param name="input"></param>
        public FemSimulationInputDataViewModel(Vts.FemModeling.MGRTE._2D.SimulationInput input)
        {
            _simulationInput = input;

            TissueTypeVM = new OptionViewModel<string>("Tissue Type:", true, _simulationInput.TissueInput.TissueType);

            _tissueTypeViewModel.PropertyChanged += (sender, args) =>
            {
                switch (_tissueTypeViewModel.SelectedValue)
                {
                    case "MultiLayer":
                        _simulationInput.TissueInput = new MultiLayerTissueInput();
                        break;
                    
                        throw new ArgumentOutOfRangeException();
                }
                UpdateTissueTypeVM(_simulationInput.TissueInput.TissueType);
            };
            
            UpdateTissueInputVM(_simulationInput.TissueInput);
        }

        /// <summary>
        /// Default constructor for MeshInputView model
        /// </summary>
        public FemSimulationInputDataViewModel()
            : this(new SimulationInput()) { }


        public SimulationInput CurrentInput
        {
            get { return _simulationInput; }
        }

        /// <summary>
        /// The residual value of the iteration for stopping criterion
        /// </summary>
        public double ConvTolerance
        {
            get { return _simulationInput.SimulationOptionsInput.ConvTolerance; }
            set
            {
                _simulationInput.SimulationOptionsInput.ConvTolerance = value;
                this.OnPropertyChanged("ConvTolerance");
            }
        }

        /// <summary>
        /// The choice of multigrid method
        /// </summary>
        public int MethodMg
        {
            get { return _simulationInput.SimulationOptionsInput.MethodMg; }
            set
            {
                if ((value >= 4)  && (value <= 7))
                {
                    _simulationInput.SimulationOptionsInput.MethodMg = value;
                    this.OnPropertyChanged("MethodMg");
                }
                else
                {
                    _simulationInput.SimulationOptionsInput.MethodMg = 6;  // default
                    this.OnPropertyChanged("MethodMg"); 
                }
            }
        }

        /// <summary>
        /// The maximum number of iteration on the finest level in FMG
        /// </summary>
        public int NIterations
        {
            get { return _simulationInput.SimulationOptionsInput.NIterations; }
            set
            {
                _simulationInput.SimulationOptionsInput.NIterations = value;
                this.OnPropertyChanged("NIterations");
            }
        }

        

        /// <summary>
        /// The finest layer of angular mesh generation
        /// </summary>
        public int AMeshLevel
        {
            get { return _simulationInput.MeshDataInput.AMeshLevel; }
            set 
            {
                if ((value >= 2) && (value <= 8))
                {
                    _simulationInput.MeshDataInput.AMeshLevel = value;
                    this.OnPropertyChanged("AMeshLevel");
                }
                else
                {
                    _simulationInput.MeshDataInput.AMeshLevel = 5;   // default
                    this.OnPropertyChanged("AMeshLevel");
                }
            }
        }

        /// <summary>
        /// The finest layer of spatial mesh generation
        /// </summary>
        public int SMeshLevel
        {
            get { return _simulationInput.MeshDataInput.SMeshLevel; }
            set
            {
                if ((value >= 2) && (value <= 8))
                {
                    _simulationInput.MeshDataInput.SMeshLevel = value;
                    this.OnPropertyChanged("SMeshLevel");
                }
                else
                { 
                    _simulationInput.MeshDataInput.SMeshLevel = 3;   // default
                    this.OnPropertyChanged("SMeshLevel");
                }
                
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public OptionViewModel<string> TissueTypeVM
        {
            get { return _tissueTypeViewModel; }
            set
            {
                _tissueTypeViewModel = value;
                OnPropertyChanged("TissueTypeVM");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object TissueInputVM
        {
            get { return _tissueInputVM; }
            set
            {
                _tissueInputVM = value;
                OnPropertyChanged("TissueInputVM");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tissueType"></param>
        private void UpdateTissueTypeVM(string tissueType)
        {
            switch (tissueType)
            {
                case "MultiLayer":
                    _simulationInput.TissueInput = new MultiLayerTissueInput();
                    break;
               default:
                    throw new ArgumentOutOfRangeException();
            }

            TissueTypeVM.Options[tissueType].IsSelected = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tissueInput"></param>
        private void UpdateTissueInputVM(ITissueInput tissueInput)
        {
            switch (tissueInput.TissueType)
            {
                case "MultiEllipsoid":
                    TissueInputVM = new FemMultiRegionTissueViewModel(tissueInput);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    

      

    }
}
