using System;
using System.Collections.Generic;
using System.Linq;
using Exir.Framework.Common;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Exir.Framework.Security.Cryptography;
using System.Threading;
using Exir.Framework.Common.DataAccess;
using SeatDomain.Configs;
using SeatServiceService;
using SeatDomain.Models.Service;

namespace SeatService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "JobServiceBroker" in both code and config file together.
    public class JobServiceBroker
    {
        public ActionResponse CheckConnections()
        {
            List<string> errors = new List<string>();
            var connections = ConfigurationManager.ConnectionStrings;
            foreach (ConnectionStringSettings cs in connections)
            {
                if (cs.ProviderName != "System.Data.SqlClient") continue;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[cs.Name].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            if (errors.Count > 0)
                return new ActionResponse(String.Join(",", errors));
            else
                return new ActionResponse(true);
        }

        private static IConnectionStringEncryptionProvider _encryption;

        public Stream GetFile(string enc_file_path)
        {
            var file_path = EmbedRsaCryptoService.XorDecryptSafeBase64(enc_file_path);
            return FileSystemProvider.Instance.FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public string GetVersion()
        {
            return ApplicationInfo.Version;
        }

        public void StopService(string secret_key)
        {
            if (JobServiceConfig.Instance.SecretKey == secret_key)
            {
                Program.ServiceInstance.TryStop();
            }
        }

        public BackstageJobThreadState GetState()
        {
            var state = new BackstageJobThreadState();
            int workerThreads, completionPortThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            state.AvailableThreads = workerThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            state.MaxThreads = workerThreads;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            state.MinThreads = workerThreads;
            if (Program.ServiceInstance != null)
                state.QueueState = String.Join(Environment.NewLine, Program.ServiceInstance.Queue.Select(x => String.Join("=", new string[] { x.Key, x.Value.ToString() })));
            return state;
        }
    }
}
