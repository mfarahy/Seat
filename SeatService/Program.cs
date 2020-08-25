using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeatServiceService
{
    static class Program
    {
        public static SeatService.SeatService ServiceInstance { get; private set; }    

        static void Main(string[] args)
        {
            ServiceInstance = new SeatService.SeatService();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                ServiceInstance
            };

            if (Environment.UserInteractive)
            {
                ServiceInstance.Start(args);
                Console.ReadLine();
                ServiceInstance.Stop();
                Thread.Sleep(1000);
                ServiceInstance.Dispose();
            }
            else
                ServiceBase.Run(ServicesToRun);
        }
    }
}
