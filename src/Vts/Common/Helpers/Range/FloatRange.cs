using System;

namespace Vts.Common
{
    public class FloatRange : Range<float>
    {
        public FloatRange(float start, float stop, int number)
            : base(start, stop, number)
        {
        }

        public FloatRange(float start, float stop)
            : this(start, stop, 2)
        {
        }

        public FloatRange()
            : this(0F, 1F, 2)
        {
        }

        protected override float GetDelta()
        {
            if (Count == 1)
            {
                return 0F;
            }

            return (Stop - Start) / (Count - 1F);
        }

        protected override int GetNewCount()
        {
            if (Delta == 0f)
            {
                return 1;
            }

            return (int)((Stop - Start) / Delta + 1);
        }

        protected override Func<float, float> GetIncrement()
        {
            return d => d + Delta;
        }

        public FloatRange Clone()
        {
            return new FloatRange(Start, Stop, Count);
        }
    }
}
