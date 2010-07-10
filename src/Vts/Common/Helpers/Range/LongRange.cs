using System;

namespace Vts.Common
{
    public class LongRange : Range<long>
    {
        public LongRange(long start, long stop, int number)
            : base(start, stop, number)
        {
        }

        public LongRange(int start, int stop)
            : this(start, stop, (stop - start) + 1)
        {
        }

        public LongRange()
            : this(0L, 1L, 2)
        {
        }

        protected override long GetDelta()
        {
            if (Count == 1)
            {
                return 0L;
            }

            return (Stop - Start) / (Count - 1);
        }

        protected override int GetNewCount()
        {
            if (Delta == 0)
            {
                return 1;
            }

            return (int)((Stop - Start) / Delta + 1L);
        }

        protected override Func<long, long> GetIncrement()
        {
            return d => d + Delta;
        }
    }
}
