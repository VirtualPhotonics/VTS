using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    //[ServiceContract(Namespace = "http://Vts.MonteCarlo.TallyActions")]
    /// <summary>
    /// Defines a contract for Monte Carlo tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IDetector<T> : IDetector
    {
        T Mean { get; }
        T SecondMoment { get; }
    }

    public interface IDetector
    {
        TallyType TallyType{ get; set; }
        void Normalize(long numPhotons);
        bool ContainsPoint(PhotonDataPoint dp);
    }
}