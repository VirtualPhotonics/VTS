using System;
using System.Collections.Generic;
using System.Linq;
namespace Vts.MonteCarlo.TallyActions
{
    public abstract class TallyBase
    {
        protected ITissue _tissue;
        protected IList<OpticalProperties> _ops;

        public TallyBase(ITissue tissue)
            //: base(tissue.AbsorptionWeightingType)
        {
            _tissue = tissue;
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }
        //protected AbsorptionWeightingType _awt;

        //public TallyBase(AbsorptionWeightingType awt)
        //{
        //    _awt = awt;

        //    SetAbsorbAction(awt);
        //}

        //protected abstract void SetAbsorbAction(AbsorptionWeightingType awt);
    }
}
