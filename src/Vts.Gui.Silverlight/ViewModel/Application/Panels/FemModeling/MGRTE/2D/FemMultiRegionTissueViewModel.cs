//using Vts.FemModeling.MGRTE._2D;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.Gui.Silverlight.ViewModel
{
    public class FemMultiRegionTissueViewModel : BindableObject
    {
        private ITissueInput _input;

        private ObservableCollection<object> _tissueRegionsVM;
        private ObservableCollection<object> _inclusionRegionsVM;

        private int _currentTissueRegionIndex;
        private int _currentInclusionRegionIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public FemMultiRegionTissueViewModel(ITissueInput input)
        {
            _input = input;

            switch (input.TissueType)
            {
                case Vts.MonteCarlo.TissueType.MultiEllipsoid:
                    var multiEllipsoidTissueInput = ((MultiEllipsoidTissueInput)_input);
                    _tissueRegionsVM = new ObservableCollection<object>(
                        multiEllipsoidTissueInput.LayerRegions.Select((r, i) => (object)new LayerRegionViewModel(
                            (LayerRegion)r,
                            "Layer " + i + (r.IsAir() ? " (Air)" : " (Tissue)"))));
                    _inclusionRegionsVM = new ObservableCollection<object>(
                        multiEllipsoidTissueInput.EllipsoidRegions.Select((r, i) => (object)new EllipsoidRegionViewModel(
                            (EllipsoidRegion)r,
                            "Inclusion " + i + (r.IsAir() ? " (Air)" : " (Tissue)"))));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentTissueRegionIndex = 0;
            _currentInclusionRegionIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public FemMultiRegionTissueViewModel() 
            : this(new MultiLayerTissueInput())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<object> TissueRegionsVM
        {
            get { return _tissueRegionsVM; }
            set
            {
                _tissueRegionsVM = value;
                OnPropertyChanged("TissueRegionsVM");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<object> InclusionRegionsVM
        {
            get { return _inclusionRegionsVM; }
            set
            {
                _inclusionRegionsVM = value;
                OnPropertyChanged("InclusionRegionsVM");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentTissueRegionIndex
        {
            get { return _currentTissueRegionIndex; }
            set
            {
                //if(value<_layerRegionsVM.Count && value >=0)
                //{
                    _currentTissueRegionIndex = value;
                    OnPropertyChanged("CurrentTissueRegionIndex");
                    OnPropertyChanged("MinimumTissueRegionIndex");
                    OnPropertyChanged("MaximumTissueRegionIndex");
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentInclusionRegionIndex
        {
            get { return _currentInclusionRegionIndex; }
            set
            {
                //if(value<_layerRegionsVM.Count && value >=0)
                //{
                _currentInclusionRegionIndex = value;
                OnPropertyChanged("CurrentInclusionRegionIndex");
                OnPropertyChanged("MinimumInclusionRegionIndex");
                OnPropertyChanged("MaximumInclusionRegionIndex");
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinimumTissueRegionIndex { get { return 0; } }
        public int MaximumTissueRegionIndex { get { return _tissueRegionsVM != null ? _tissueRegionsVM.Count - 1 : 0; } }

        /// <summary>
        /// 
        /// </summary>
        public int MinimumInclusionRegionIndex { get { return 0; } }
        public int MaximumInclusionRegionIndex { get { return _inclusionRegionsVM != null ? _inclusionRegionsVM.Count - 1 : 0; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITissueInput GetTissueInput()
        {
            return _input;
        }
    }
}
