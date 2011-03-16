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
        {
            _tissue = tissue;
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }
    }
}
