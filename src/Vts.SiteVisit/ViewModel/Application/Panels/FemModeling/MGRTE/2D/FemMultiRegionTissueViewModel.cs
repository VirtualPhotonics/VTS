using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vts.Extensions;
using Vts.FemModeling.MGRTE._2D;


namespace Vts.SiteVisit.ViewModel
{
    public class FemMultiRegionTissueViewModel : BindableObject
    {
        private ITissueInput _input;

        private ObservableCollection<object> _regionsVM;

        private int _currentRegionIndex;

        public FemMultiRegionTissueViewModel(ITissueInput input)
        {
            _input = input;

            switch (input.TissueType)
            {
                case FemModeling.MGRTE._2D.TissueType.MultiLayer:
                    var multiLayerTissueInput = ((MultiLayerTissueInput)_input);
                    _regionsVM = new ObservableCollection<object>(
                        multiLayerTissueInput.Regions.Select((r, i) => (object)new FemLayerRegionViewModel(
                            (LayerTissueRegion)r,
                            "Layer " + i + (r.IsAir() ? " (Air)" : " (Tissue)"))));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentRegionIndex = 0;
        }

        public FemMultiRegionTissueViewModel() 
            : this(new MultiLayerTissueInput())
        {
        }

        public ObservableCollection<object> RegionsVM
        {
            get { return _regionsVM; }
            set
            {
                _regionsVM = value;
                OnPropertyChanged("LayerRegionsVM");
            }
        }

        public int CurrentRegionIndex
        {
            get { return _currentRegionIndex; }
            set
            {
                //if(value<_layerRegionsVM.Count && value >=0)
                //{
                    _currentRegionIndex = value;
                    OnPropertyChanged("CurrentRegionIndex");
                    OnPropertyChanged("MinimumRegionIndex");
                    OnPropertyChanged("MaximumRegionIndex");
                //}
            }
        }

        public int MinimumRegionIndex { get { return 0; } }
        public int MaximumRegionIndex { get { return _regionsVM != null ? _regionsVM.Count - 1 : 0; } }

        public ITissueInput GetTissueInput()
        {
            return _input;
        }
    }
}
