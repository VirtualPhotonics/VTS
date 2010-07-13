using System;

namespace Vts.Common
{
    public class IntRange : Range<int>
    {
        public IntRange(int start, int stop, int number)
            : base(start, stop, number)
        {
        }

        public IntRange(int start, int stop)
            : base(start, stop, (stop - start) + 1)
        {
        }

        public IntRange()
            : base(0, 1, 2)
        {
        }

        protected override int GetDelta()
        {
            if (Delta == 0d)
            {
                return 1;
            }

            return (Stop - Start) / (Count - 1);
        }

        protected override int GetNewCount()
        {
            if (Delta == 0)
            {
                return 1;
            }

            return ((Stop - Start) / Delta + 1);
        }

        protected override Func<int, int> GetIncrement()
        {
            return d => d + Delta;
        }
    }
}
