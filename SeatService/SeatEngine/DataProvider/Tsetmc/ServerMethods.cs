using System.IO;
using System.Net;
using System.Threading.Tasks;
using Exir.Framework.Common;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class ServerMethods
    {
        public static string Instrument(int lastDEven, bool useWebService)
        {
            try
            {
                Settings settings = new Settings();
                if (useWebService)
                {
                    using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                    {
                        webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                        return webServiceTseClient.Instrument(lastDEven).ApplyCorrectYeKe();
                    }
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=Instrument&a=" + lastDEven);
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = streamReader.ReadToEnd().ApplyCorrectYeKe();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                return result;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> InstrumentAsync(int lastDEven, bool useWebService)
        {
            try
            {
                Settings settings = new Settings();
                if (useWebService)
                {
                    using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                    {
                        webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                        TaskCompletionSource<string> taskCompletion = new TaskCompletionSource<string>();
                        webServiceTseClient.InstrumentAsync(lastDEven, x =>
                        {
                            taskCompletion.SetResult(x.ApplyCorrectYeKe());
                        }, x =>
                        {
                            taskCompletion.SetException(x);
                        });
                        return await taskCompletion.Task;
                    }
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=Instrument&a=" + lastDEven);
                httpWebRequest.Method = "GET";
                WebResponse response = await httpWebRequest.GetResponseAsync();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = await streamReader.ReadToEndAsync();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                return result.ApplyCorrectYeKe();
            }
            catch
            {
                throw;
            }
        }

        public static string LastPossibleDeven(bool useWebService)
        {
            try
            {
                Settings settings = new Settings();
                if (useWebService)
                {
                    using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                    {
                        webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                        return webServiceTseClient.LastPossibleDeven();
                    }
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=LastPossibleDeven");
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                return result;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> LastPossibleDevenAsync(bool useWebService)
        {
            try
            {
                Settings settings = new Settings();
                if (useWebService)
                {
                    using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                    {
                        webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                        TaskCompletionSource<string> taskCompletion = new TaskCompletionSource<string>();
                        webServiceTseClient.LastPossibleDevenAsync(x =>
                        {
                            taskCompletion.SetResult(x.ApplyCorrectYeKe());
                        }, x =>
                        {
                            taskCompletion.SetException(x);
                        });

                        return await taskCompletion.Task;
                    }
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=LastPossibleDeven");
                httpWebRequest.Method = "GET";
                WebResponse response = await httpWebRequest.GetResponseAsync();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = await streamReader.ReadToEndAsync();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                return result;
            }
            catch
            {
                throw;
            }
        }

        #region GetInsturmentClosingPrice
        public static string GetInsturmentClosingPrice(string insCodes, bool useWebService)
        {
            Settings settings = new Settings();
            if (useWebService)
            {
                using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                {
                    webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                    string insCodes2 = Utility.Compress(insCodes);
                    return webServiceTseClient.DecompressAndGetInsturmentClosingPrice(insCodes2);
                }
            }
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=ClosingPrices&a=" + insCodes);
            httpWebRequest.Method = "GET";
            WebResponse response = httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            response.Close();
            return result;
        }

        public static async Task<string> GetInsturmentClosingPriceAsync(string insCodes, bool useWebService)
        {
            Settings settings = new Settings();
            if (useWebService)
            {
                using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                {
                    TaskCompletionSource<string> taskCompletion = new TaskCompletionSource<string>();

                    webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                    string insCodes2 = Utility.Compress(insCodes);
                    webServiceTseClient.DecompressAndGetInsturmentClosingPriceAsync(insCodes2, x =>
                    {
                        taskCompletion.SetResult(x);
                    }, exception =>
                    {
                        taskCompletion.SetException(exception);
                    });

                    return await taskCompletion.Task;
                }
            }
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=ClosingPrices&a=" + insCodes);
            httpWebRequest.Method = "GET";
            WebResponse response = await httpWebRequest.GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string result = await streamReader.ReadToEndAsync();
            streamReader.Close();
            responseStream.Close();
            response.Close();
            return result;
        }

        #endregion

        #region InstrumentAndShare
        public static string InstrumentAndShare(int lastDEven, long lastId, bool useWebService)
        {
            try
            {
                Settings settings = new Settings();
                if (useWebService)
                {
                    using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                    {
                        TaskCompletionSource<string> taskCompletion = new TaskCompletionSource<string>();

                        webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                        return webServiceTseClient.InstrumentAndShare(lastDEven, lastId).ApplyCorrectYeKe();
                    }
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=InstrumentAndShare&a=" + lastDEven + "&a2=" + lastId);
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                return result.ApplyCorrectYeKe();
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> InstrumentAndShareAsync(int lastDEven, long lastId, bool useWebService)
        {
            try
            {
                Settings settings = new Settings();
                if (useWebService)
                {
                    using (WebServiceTseClient webServiceTseClient = new WebServiceTseClient())
                    {
                        TaskCompletionSource<string> taskCompletion = new TaskCompletionSource<string>();

                        webServiceTseClient.EnableDecompression = settings.EnableDecompression;
                        webServiceTseClient.InstrumentAndShareAsync(lastDEven, lastId, x =>
                        {
                            taskCompletion.SetResult(x.ApplyCorrectYeKe());
                        }, x =>
                        {
                            taskCompletion.SetException(x);
                        });

                        return await taskCompletion.Task;
                    }
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(settings.TseClientServerUrl + "?t=InstrumentAndShare&a=" + lastDEven + "&a2=" + lastId);
                httpWebRequest.Method = "GET";
                WebResponse response = await httpWebRequest.GetResponseAsync();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = await streamReader.ReadToEndAsync();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                return result.ApplyCorrectYeKe();
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }

}
