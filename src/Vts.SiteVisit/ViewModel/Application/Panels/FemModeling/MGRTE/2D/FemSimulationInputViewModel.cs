using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.Common;
using Vts.FemModeling.MGRTE._2D.SourceInputs;
using Vts.MonteCarlo;
using Vts.SiteVisit.ViewModel;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// MG-RTE simulation parameters
    /// </summary>
    public class FemSimulationInputDataViewModel : BindableObject
    {
        private SimulationInput _simulationInput;
        private OptionViewModel<Vts.MonteCarlo.TissueType> _tissueTypeViewModel;
        //private OpticalPropertyViewModel _mediumOpticalPropertyViewModel;
        //private OpticalPropertyViewModel _inclusionOpticalPropertyViewModel;

        private object _tissueInputVM;

        /// <summary>
        /// FEM Simulation Input Data View Model
        /// </summary>
        /// <param name="input"></param>
        public FemSimulationInputDataViewModel(Vts.FemModeling.MGRTE._2D.SimulationInput input)
        {
            _simulationInput = input;

            TissueTypeVM = new OptionViewModel<Vts.MonteCarlo.TissueType>("Tissue Type:", true, _simulationInput.TissueInput.TissueType);

            _tissueTypeViewModel.PropertyChanged += (sender, args) =>
            {
                switch (_tissueTypeViewModel.SelectedValue)
                {
                    case Vts.MonteCarlo.TissueType.MultiLayer:
                        _simulationInput.TissueInput = new MultiLayerTissueInput();
                        break;
                    
                        throw new ArgumentOutOfRangeException();
                }
                UpdateTissueTypeVM(_simulationInput.TissueInput.TissueType);
            };
            
            UpdateTissueInputVM(_simulationInput.TissueInput);

            //MediumOpticalPropertyVM = new OpticalPropertyViewModel(
            //    opticalProperties: input.TissueInput.Regions[1].RegionOP,
            //    units: IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(),
            //    title: "Medium Optical Properties:",
            //    enableMua: true,
            //    enableMusp: true,
            //    enableG: true,
            //    enableN: false);

            //InclusionOpticalPropertyVM = new OpticalPropertyViewModel(
            //    opticalProperties: input.InclusionInput.Regions[1].RegionOP,
            //    units: IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(),
            //    title: "Inclusion Optical Properties:",
            //    enableMua: true,
            //    enableMusp: true,
            //    enableG: true,
            //    enableN: false);
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
            get { return _simulationInput.SimulationParameterInput.ConvTolerance; }
            set
            {
                _simulationInput.SimulationParameterInput.ConvTolerance = value;
                this.OnPropertyChanged("ConvTolerance");
            }
        }

        /// <summary>
        /// The choice of multigrid method
        /// </summary>
        public int MethodMg
        {
            get { return _simulationInput.SimulationParameterInput.MethodMg; }
            set
            {
                if ((value >= 4)  && (value <= 7))
                {
                    _simulationInput.SimulationParameterInput.MethodMg = value;
                    this.OnPropertyChanged("MethodMg");
                }
                else
                {
                    this.OnPropertyChanged("MethodMg"); 
                }
            }
        }

        /// <summary>
        /// The maximum number of iteration on the finest level in FMG
        /// </summary>
        public int NIterations
        {
            get { return _simulationInput.SimulationParameterInput.NIterations; }
            set
            {
                _simulationInput.SimulationParameterInput.NIterations = value;
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
                    this.OnPropertyChanged("SMeshLevel");
                }
                
            }
        }

       

        ///// <summary>
        ///// Optical Property of medium
        ///// </summary>
        //public OpticalPropertyViewModel MediumOpticalPropertyVM
        //{
        //    get { return _mediumOpticalPropertyViewModel; }
        //    set
        //    {
        //        _mediumOpticalPropertyViewModel = value;
        //        OnPropertyChanged("MediumOpticalPropertyVM");
        //    }
        //}

        ///// <summary>
        ///// Optical Property of inclusion
        ///// </summary>
        //public OpticalPropertyViewModel InclusionOpticalPropertyVM
        //{
        //    get { return _inclusionOpticalPropertyViewModel; }
        //    set
        //    {
        //        _inclusionOpticalPropertyViewModel = value;
        //        OnPropertyChanged("InclusionOpticalPropertyVM");
        //    }
        //}
        

        /// <summary>
        /// 
        /// </summary>
        public OptionViewModel<Vts.MonteCarlo.TissueType> TissueTypeVM
        {
            get { return _tissueTypeViewModel; }
            set
            {
                _tissueTypeViewModel = value;
                OnPropertyChanged("TissueTypeVM");
            }
        }

        public object TissueInputVM
        {
            get { return _tissueInputVM; }
            set
            {
                _tissueInputVM = value;
                OnPropertyChanged("TissueInputVM");
            }
        }

        private void UpdateTissueTypeVM(Vts.MonteCarlo.TissueType tissueType)
        {
            switch (tissueType)
            {
                case Vts.MonteCarlo.TissueType.MultiLayer:
                    _simulationInput.TissueInput = new MultiLayerTissueInput();
                    break;
               default:
                    throw new ArgumentOutOfRangeException();
            }

            TissueTypeVM.Options[tissueType].IsSelected = true;

        }

        private void UpdateTissueInputVM(ITissueInput tissueInput)
        {
            switch (tissueInput.TissueType)
            {
                case Vts.MonteCarlo.TissueType.MultiEllipsoid:
                    TissueInputVM = new FemMultiRegionTissueViewModel(tissueInput);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    

      

    }
}
