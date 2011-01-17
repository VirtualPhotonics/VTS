using System;

namespace Vts.Common
{
    public class UIntRange : Range<uint>
    {
        public UIntRange(uint start, uint stop, int number)
            : base(start, stop, number)
        {
        }

        public UIntRange(uint start, uint stop)
            : this(start, stop, (int)(stop - start) + 1)
        {
        }

        public UIntRange()
            : this(0U, 1U, 2)
        {
        }

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
