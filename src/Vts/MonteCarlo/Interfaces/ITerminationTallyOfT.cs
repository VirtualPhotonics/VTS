namespace Vts.MonteCarlo
{
    //[ServiceContract(Namespace = "http://Vts.MonteCarlo.TallyActions")]
    /// <summary>
    /// Defines a contract for Monte Carlo tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface ITerminationTally<T> : ITerminationTally
    {
        T Mean { get; }
        T SecondMoment { get; }
    }
}
