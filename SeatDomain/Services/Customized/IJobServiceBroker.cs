using Exir.Framework.Common;
using SeatDomain.Models.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SeatDomain.Services
{
    [ServiceContract]

    public interface IJobServiceBroker
    {
        [OperationContract]

        BackstageJobThreadState GetState();
        [OperationContract]

        string GetVersion();
        [OperationContract]

        ActionResponse CheckConnections();
        [OperationContract]

        ActionResponse Run(string jobKey, DateTime fromdate);
        [OperationContract]

        Stream GetFile(string file_name);
        [OperationContract]

        void StopService(string secret_key);
    }
}
