using System;
using System.Collections.ObjectModel;
using System.Linq;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.Gui.Silverlight.ViewModel
{
    public class MultiRegionTissueViewModel : BindableObject
    {
        private ITissueInput _input;

        private ObservableCollection<object> _regionsVM;

        private int _currentRegionIndex;

        public MultiRegionTissueViewModel(ITissueInput input)
        {
            _input = input;

            switch (input.TissueType)
            {
                case "MultiLayer":
                    var multiLayerTissueInput = ((MultiLayerTissueInput)_input);
                    _regionsVM = new ObservableCollection<object>(
                        multiLayerTissueInput.Regions.Select((r, i) => (object)new LayerRegionViewModel(
                            (LayerTissueRegion)r,
                            "Layer " + i)));
                    break;
                case "SingleEllipsoid":
                    var singleEllipsoidTissueInput = ((SingleEllipsoidTissueInput) _input);
                    _regionsVM = new ObservableCollection<object>(
                        singleEllipsoidTissueInput.LayerRegions
                            .Select((r, i) => (object)new LayerRegionViewModel(
                                (LayerTissueRegion)r,
                                "Layer " + i))
                            .Concat(new EllipsoidRegionViewModel((EllipsoidTissueRegion)singleEllipsoidTissueInput.EllipsoidRegion,
                                "Ellipsoid Region")));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentRegionIndex = 0;
        }

        public MultiRegionTissueViewModel() 
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
