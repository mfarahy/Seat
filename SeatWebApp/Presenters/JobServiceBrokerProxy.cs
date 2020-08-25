using System;
using Exir.Framework.Common;
using System.IO;
using SeatDomain.Services;

namespace SeatWebApp.Presenters
{
    public class JobServiceBrokerProxy : IJobServiceBroker
    {
        public ActionResponse CheckConnections()
        {
            using (var jobServiceClient = new JobServiceBrokerClient())
            {
                return jobServiceClient.CheckConnections();
            }
        }

        public Stream GetFile(string file_name)
        {
            using (var jobServiceClient = new JobServiceBrokerClient())
            {
                return jobServiceClient.GetFile(file_name);
            }
        }

        public BackstageJobThreadState GetState()
        {
            using (var jobServiceClient = new JobServiceBrokerClient())
            {
                return jobServiceClient.GetState();
            }
        }

        public string GetVersion()
        {
            using (var jobServiceClient = new JobServiceBrokerClient())
            {
                return jobServiceClient.GetVersion();
            }
        }

        public ActionResponse Run(string jobKey, DateTime rundate)
        {
            using (var jobServiceClient = new JobServiceBrokerClient())
            {
                return jobServiceClient.Run(jobKey, rundate);
            }
        }

        public void StopService(string secret_key)
        {
            using (var jobServiceClient = new JobServiceBrokerClient())
            {
                jobServiceClient.StopService(secret_key);
            }
        }
    }
}