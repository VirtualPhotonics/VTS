namespace Vts
{
    public class Tuple<TFirst,TSecond>
    {
        public TFirst First { get; set; }
        public TSecond Second { get; set; }

        public Tuple(TFirst first, TSecond second) { First = first; Second = second; }
    }

    public class Tuple<TFirst, TSecond, TThird>
    {
        public TFirst First { get; set; }
        public TSecond Second { get; set; }
        public TThird Third { get; set; }

        public Tuple(TFirst first, TSecond second, TThird third) { First = first; Second = second; Third = third; }
    }
}
