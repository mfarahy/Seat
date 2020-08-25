using Exir.Framework.Common.Logging;
using SeatService.SeatServiceEngine.DataProvider.Tsetmc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SeatService.SeatServiceEngine.DataProvider
{
    public abstract class DataProvider : IDisposable
    {
        protected ILogger logger;

        protected ThreadLocal<List<HttpClient>> _clients;
        protected ThreadLocal<int> _activeClientIndex;

        public int TryCount { get; set; } = 10;
        private Uri[] _baseUrls;

        public HttpClient Http
        {
            get
            {
                return _clients?.Value[_activeClientIndex.Value];
            }
        }

        public DataProvider(params string[] baseUrls)
        {
            logger = LogManager.Instance.GetLogger<TsetmcOnlineDataProvider>();

            if (baseUrls != null && baseUrls.Length > 0)
            {
                _baseUrls = baseUrls.Select(x => new Uri(x)).ToArray();
                _clients = new ThreadLocal<List<HttpClient>>(() =>
                {
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    };
                    return _baseUrls.Select(x =>
                    {
                        var client = new HttpClient(handler);
                        client.Timeout = TimeSpan.FromSeconds(10000);
                        client.BaseAddress = x;
                        return client;
                    }).ToList();
                });
                _activeClientIndex = new ThreadLocal<int>(() => 0);
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

        protected static TimeSpan TryParseTime(string stime)
        {
            var c = stime.Length == 5 ? "0" + stime : stime;
            return TimeSpan.Parse(c.Substring(0, 2) + ":" + c.Substring(2, 2) + ":" + c.Substring(4, 2));
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
                catch (Exception hex)
                {
                    handle(i, hex);
                }
            }
        }
        protected T try_do<T>(Func<T> func)
        {
            return try_do(func, TryCount, null);
        }
        protected T try_do<T>(Func<T> func, int tryCount, Func<Exception, bool> onError)
        {
            for (var i = 0; i < TryCount; ++i)
            {
                try
                {
                    return func();
                }
                catch (Exception hex)
                {
                    handle(i, hex);
                    if (onError != null && !onError(hex))
                        break;
                }

            }
            return default(T);
        }

        private void handle(int i, Exception exception)
        {
            if (Environment.UserInteractive)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(".");
                Console.ResetColor();
            }

            string baseAddress = Http?.BaseAddress.ToString();
            if (i == TryCount - 1)
            {
                logger.ErrorFormat("cannot load data for {0}th trying from {1} because {2}", exception, i + 1, baseAddress, exception.Message);
                throw exception;
            }

            if (_clients != null)
            {
                _activeClientIndex.Value = i % _clients.Value.Count;
            }

            if (_clients == null || _clients.Value.Count == 1)
                Thread.Sleep(1000);
        }

        protected async Task<T> try_do_async<T>(Func<Task<T>> func, int? tryCount = null)
        {
            for (var i = 0; i < (tryCount ?? TryCount); ++i)
            {
                try
                {
                    return await func();
                }
                catch (Exception hex)
                {
                    handle(i, hex);
                }
            }
            return default(T);
        }

        public virtual void Dispose()
        {
            if (_clients != null)
            {
                foreach (var httpClient in _clients.Value)
                    httpClient.Dispose();
                _clients?.Dispose();
                _activeClientIndex.Dispose();
            }
        }
    }
}
