namespace Vts.MonteCarlo
{
    //[ServiceContract(Namespace = "http://Vts.MonteCarlo.TallyActions")]
    /// <summary>
    /// Defines a contract for Monte Carlo history tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IHistoryTally<T> : IHistoryTally
    {
        T Mean { get; }
        T SecondMoment { get; }
    }
}
