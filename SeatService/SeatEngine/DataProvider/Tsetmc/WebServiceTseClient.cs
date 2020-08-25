namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    // TseClient.ServerSerive.WebServiceTseClient
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Web.Services;
    using System.Web.Services.Description;
    using System.Web.Services.Protocols;

    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    public delegate void InstrumentCompletedEventHandler(object sender, InstrumentCompletedEventArgs e);

    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    public delegate void DecompressAndGetInsturmentClosingPriceCompletedEventHandler(object sender, DecompressAndGetInsturmentClosingPriceCompletedEventArgs e);
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    public delegate void InstrumentAndShareCompletedEventHandler(object sender, InstrumentAndShareCompletedEventArgs e);

    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    public delegate void LastPossibleDevenCompletedEventHandler(object sender, LastPossibleDevenCompletedEventArgs e);
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    public delegate void LogErrorCompletedEventHandler(object sender, LogErrorCompletedEventArgs e);

    [WebServiceBinding(Name = "WebServiceTseClientSoap", Namespace = "http://tsetmc.com/")]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    [DebuggerStepThrough]
    public class WebServiceTseClient : SoapHttpClientProtocol
    {
        private bool useDefaultCredentialsSetExplicitly;

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if (IsLocalFileSystemWebService(base.Url) && !useDefaultCredentialsSetExplicitly && !IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                useDefaultCredentialsSetExplicitly = true;
            }
        }
        public WebServiceTseClient()
        {
            Url = Settings.Default.TseClient_ServerSerive_TseClient;
            if (IsLocalFileSystemWebService(Url))
            {
                UseDefaultCredentials = true;
                useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                useDefaultCredentialsSetExplicitly = true;
            }
        }

        #region LastPossibleDevenAsync
        [SoapDocumentMethod("http://tsetmc.com/LastPossibleDeven", RequestNamespace = "http://tsetmc.com/", ResponseNamespace = "http://tsetmc.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string LastPossibleDeven()
        {
            object[] array = Invoke("LastPossibleDeven", new object[0]);
            return (string)array[0];
        }

        public void LastPossibleDevenAsync(Action<string> onSuccess, Action<Exception> onError)
        {
            InvokeAsync("LastPossibleDeven", new object[0], new SendOrPostCallback(delegate (object arg)
            {
                InvokeCompletedEventArgs invokeCompleted = (InvokeCompletedEventArgs)arg;

                if (invokeCompleted.Error != null)
                    onError(invokeCompleted.Error);
                else
                    onSuccess((string)invokeCompleted.Results[0]);

            }), null);
        }
        #endregion


        #region Instrument
        [SoapDocumentMethod("http://tsetmc.com/Instrument", RequestNamespace = "http://tsetmc.com/", ResponseNamespace = "http://tsetmc.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string Instrument(int DEven)
        {
            object[] array = Invoke("Instrument", new object[1]
            {
            DEven
            });
            return (string)array[0];
        }

        public void InstrumentAsync(int DEven, Action<string> onSuccess, Action<Exception> onError)
        {
            InvokeAsync("Instrument", new object[1]
          {
            DEven
          }, new SendOrPostCallback(delegate (object arg)
          {
              InvokeCompletedEventArgs invokeCompleted = (InvokeCompletedEventArgs)arg;

              if (invokeCompleted.Error != null)
                  onError(invokeCompleted.Error);
              else
                  onSuccess((string)invokeCompleted.Results[0]);

          }), null);
        }

        #endregion

        #region DecompressAndGetInsturmentClosingPrice
        [SoapDocumentMethod("http://tsetmc.com/DecompressAndGetInsturmentClosingPrice", RequestNamespace = "http://tsetmc.com/", ResponseNamespace = "http://tsetmc.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DecompressAndGetInsturmentClosingPrice(string insCodes)
        {
            object[] array = Invoke("DecompressAndGetInsturmentClosingPrice", new object[1]
            {
            insCodes
            });
            return (string)array[0];
        }

        public void DecompressAndGetInsturmentClosingPriceAsync(string insCodes, Action<string> onSuccess, Action<Exception> onError)
        {
            InvokeAsync("DecompressAndGetInsturmentClosingPrice", new object[1]
                       {
            insCodes
                       }, new SendOrPostCallback(delegate (object arg)
                       {
                           InvokeCompletedEventArgs invokeCompleted = (InvokeCompletedEventArgs)arg;

                           if (invokeCompleted.Error != null)
                               onError(invokeCompleted.Error);
                           else
                               onSuccess((string)invokeCompleted.Results[0]);

                       }), null);
        }
        #endregion


        #region InstrumentAndShare
        [SoapDocumentMethod("http://tsetmc.com/InstrumentAndShare", RequestNamespace = "http://tsetmc.com/", ResponseNamespace = "http://tsetmc.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string InstrumentAndShare(int DEven, long LastID)
        {
            object[] array = Invoke("InstrumentAndShare", new object[2]
            {
            DEven,
            LastID
            });
            return (string)array[0];
        }

        public void InstrumentAndShareAsync(int DEven, long LastID, Action<string> onSuccess, Action<Exception> onError)
        {
            InvokeAsync("InstrumentAndShare", new object[2]
           {
            DEven,
            LastID
           }, new SendOrPostCallback(delegate (object arg)
           {
               InvokeCompletedEventArgs invokeCompleted = (InvokeCompletedEventArgs)arg;

               if (invokeCompleted.Error != null)
                   onError(invokeCompleted.Error);
               else
                   onSuccess((string)invokeCompleted.Results[0]);

           }), null);
        }

        #endregion
 
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
            {
                return false;
            }
            Uri uri = new Uri(url);
            if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            return false;
        }
    }

}
