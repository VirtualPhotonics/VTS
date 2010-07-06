using System.ServiceModel;

namespace Vts.MonteCarlo.Wcf
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
    [ServiceContract]
    public interface IMonteCarloService
    {
        [OperationContract]
        bool[] ExecuteBatch(SimulationInput[] input);
    }
}
