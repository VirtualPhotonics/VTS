using System;

namespace Vts.Common
{
    public class UintRange : Range<uint>
    {
        public UintRange(uint start, uint stop, int number)
            : base(start, stop, number) { }

        public UintRange()
            : this(0U, 1U, 2) { }

        protected override uint GetDelta()
        {
            if (Count == 1)
            {
                return 0U;
            }

            return (uint)((Stop - Start) / (Count - 1));
        }

        protected override int GetNewCount()
        {
            return (int)((Stop - Start) / Delta + 1);
        }

        protected override Func<uint, uint> GetIncrement()
        {
            return d => (d + Delta);
        }
    }
}
