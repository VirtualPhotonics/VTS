
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
