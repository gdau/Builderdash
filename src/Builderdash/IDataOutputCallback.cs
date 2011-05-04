using System.ServiceModel;

namespace Builderdash
{
    public interface IDataOutputCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendDataPacket(string data);
    }
}
