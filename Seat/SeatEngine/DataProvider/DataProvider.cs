using Exir.Framework.Common.Logging;
using Seat.SeatEngine.DataProvider.Tsetmc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Seat.SeatEngine.DataProvider
{
    public abstract class DataProvider : IDisposable
    {
        protected ILogger logger;
        protected HttpClient client;
        public int TryCount { get; set; } = 100;

        public DataProvider(string baseUrl)
        {
            logger = LogManager.Instance.GetLogger<TsetmcOnlineDataProvider>();

            if (baseUrl != null)
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };


                client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(10000);
                client.BaseAddress = new Uri(baseUrl);
            }
        }


        protected static int TryParseInt(string v)
        {
            if (String.IsNullOrEmpty(v) || v == "0") return 0;
            if (v.EndsWith(".00"))
                v = v.Substring(0, v.Length - 3);

            int number;
            if (!int.TryParse(v, out number))
                throw new Exception();
            return number;
        }
        protected static long TryParseLong(string v)
        {
            if (String.IsNullOrEmpty(v) || v == "0") return 0;
            if (v.EndsWith(".00"))
                v = v.Substring(0, v.Length - 3);

            long number;
            if (!long.TryParse(v, out number))
                throw new Exception();
            return number;
        }
        protected static double TryParseDouble(string v)
        {
            if (String.IsNullOrEmpty(v) || v == "0") return 0d;

            double number;
            if (!double.TryParse(v, out number))
                throw new Exception();
            return number;
        }
        protected static decimal TryParseDecimal(string v)
        {
            if (String.IsNullOrEmpty(v) || v == "0") return (decimal)0;

            decimal number;
            if (!decimal.TryParse(v, out number))
                throw new Exception();
            return number;
        }

        protected async Task try_do_async(Func<Task> func)
        {
            for (var i = 0; i < TryCount; ++i)
            {
                try
                {
                    await func();
                    break;
                }
                catch (Exception ex)
                {
                    if (i == TryCount - 1)
                        throw;

                    Thread.Sleep(1000);
                    logger.ErrorFormat("cannot load data for {0}th trying", ex, i);
                }
            }
        }

        protected T try_do<T>(Func<T> func)
        {
            for (var i = 0; i < TryCount; ++i)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    if (i == TryCount - 1)
                        throw;

                    Thread.Sleep(1000);
                    logger.ErrorFormat("cannot load data for {0}th trying", ex, i);
                }
            }
            return default(T);
        }

        protected async Task<T> try_do_async<T>(Func<Task<T>> func)
        {
            for (var i = 0; i < TryCount; ++i)
            {
                try
                {
                    return await func();
                }
                catch (Exception ex)
                {
                    if (i == TryCount - 1)
                        throw;

                    Thread.Sleep(1000);
                    logger.ErrorFormat("cannot load data for {0}th trying", ex, i);
                }
            }
            return default(T);
        }

        public virtual void Dispose()
        {
            client?.Dispose();
        }
    }
}
