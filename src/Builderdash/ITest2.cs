using System;
using System.ServiceModel;

namespace WcfShared
{
    [ServiceContract]
    public interface ITest2
    {
        [OperationContract]
        string GetCert(string input);
    }
}