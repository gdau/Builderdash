using System;
using System.ServiceModel;

namespace Builderdash
{
    [ServiceContract(CallbackContract = typeof(IDataOutputCallback), SessionMode = SessionMode.Required)]
    public interface IJobService
    {
        [OperationContract]
        Guid RunJob();

        [OperationContract]
        Job GetJob(Guid jobId);

        [OperationContract]
        void SubscribeToStatus(Guid jobId);

        [OperationContract]
        void UnsubscribeToStatus(Guid jobId);
    }
}