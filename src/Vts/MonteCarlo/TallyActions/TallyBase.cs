using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Vts.MonteCarlo.TallyActions
{
    public abstract class TallyBase
    {
        protected AbsorptionWeightingType _awt;

        public TallyBase(AbsorptionWeightingType awt)
        {
            _awt = awt;

            SetAbsorbAction(awt);
        }

        protected abstract void SetAbsorbAction(AbsorptionWeightingType awt);
    }
}
