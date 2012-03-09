using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.Common;
using Vts.FemModeling.MGRTE._2D.SourceInputs;
using Vts.SiteVisit.ViewModel;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// MG-RTE simulation parameters
    /// </summary>
    public class FemSimulationInputDataViewModel : BindableObject
    {
        private SimulationInput _simulationInput;
        private OpticalPropertyViewModel _mediumOpticalPropertyViewModel;
        private OpticalPropertyViewModel _inclusionOpticalPropertyViewModel;
        private Position _inclusionLocationViewModel;
        private double _inclusionRadiusViewModel;

        /// <summary>
        /// FEM Simulation Input Data View Model
        /// </summary>
        /// <param name="input"></param>
        public FemSimulationInputDataViewModel(SimulationInput input)
        {
            _simulationInput = input;

            MediumOpticalPropertyVM = new OpticalPropertyViewModel(
                opticalProperties: input.TissueInput.Regions[1].RegionOP,
                units: IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(),
                title: "Medium Optical Properties:",
                enableMua: true,
                enableMusp: true,
                enableG: true,
                enableN: false);

            InclusionOpticalPropertyVM = new OpticalPropertyViewModel(
                opticalProperties: input.InclusionInput.Regions[1].RegionOP,
                units: IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(),
                title: "Inclusion Optical Properties:",
                enableMua: true,
                enableMusp: true,
                enableG: true,
                enableN: false);

            InclusionLocationVM = new Position(
                input.InclusionInput.Regions[1].Location.X,
                0.0,
                input.InclusionInput.Regions[1].Location.Z);

            InclusionRadiusVM = input.InclusionInput.Regions[1].Radius;
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
                _simulationInput.SimulationParameterInput.MethodMg = value;
                this.OnPropertyChanged("MethodMg");
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
                this.OnPropertyChanged("CurrentInput");
            }
        }

        /// <summary>
        /// Refractive index of the external medium
        /// </summary>
        public double NExternal
        {
            get { return _simulationInput.SimulationParameterInput.NExternal; }
            set
            {
                _simulationInput.SimulationParameterInput.NExternal = value;
                this.OnPropertyChanged("CurrentInput");
                this.OnPropertyChanged("NExternal");
            }
        }

        /// <summary>
        /// The finest layer of angular mesh generation
        /// </summary>
        public int AMeshLevel
        {
            get { return _simulationInput.MeshDataInput.AMeshLevel; }
            set {
                _simulationInput.MeshDataInput.AMeshLevel = value;
                this.OnPropertyChanged("AMeshLevel");
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
                _simulationInput.MeshDataInput.SMeshLevel = value;
                this.OnPropertyChanged("SMeshLevel");
            }
        }

        /// <summary>
        /// Length of the square mesh
        /// </summary>
        public double SideLength
        {
            get { return _simulationInput.MeshDataInput.SideLength; }
            set { _simulationInput.MeshDataInput.SideLength = value;
            this.OnPropertyChanged("SideLength");
            }
        }

        /// <summary>
        /// Optical Property of medium
        /// </summary>
        public OpticalPropertyViewModel MediumOpticalPropertyVM
        {
            get { return _mediumOpticalPropertyViewModel; }
            set
            {
                _mediumOpticalPropertyViewModel = value;
                OnPropertyChanged("MediumOpticalPropertyVM");
            }
        }

        /// <summary>
        /// Optical Property of inclusion
        /// </summary>
        public OpticalPropertyViewModel InclusionOpticalPropertyVM
        {
            get { return _inclusionOpticalPropertyViewModel; }
            set
            {
                _inclusionOpticalPropertyViewModel = value;
                OnPropertyChanged("InclusionOpticalPropertyVM");
            }
        }

        /// <summary>
        /// Location of inclusion
        /// </summary>
        public Position InclusionLocationVM
        {
            get { return _inclusionLocationViewModel; }
            set
            {
                _inclusionLocationViewModel = value;
                OnPropertyChanged("InclusionLocationVM");
            }
        }

        /// <summary>
        /// Radius of inclusion
        /// </summary>
        public double InclusionRadiusVM
        {
            get { return _inclusionRadiusViewModel; }
            set
            {
                _inclusionRadiusViewModel = value;
                OnPropertyChanged("InclusionRadiusVM");
            }
        }

    }
}
