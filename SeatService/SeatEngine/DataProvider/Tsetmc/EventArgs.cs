using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    // TseClient.ServerSerive.DecompressAndGetInsturmentClosingPriceCompletedEventArgs
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;

    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    [DesignerCategory("code")]
    [DebuggerStepThrough]
    public class DecompressAndGetInsturmentClosingPriceCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[0];
            }
        }

        internal DecompressAndGetInsturmentClosingPriceCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    [DebuggerStepThrough]
    public class InstrumentAndShareCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[0];
            }
        }

        internal InstrumentAndShareCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    public class InstrumentCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[0];
            }
        }

        internal InstrumentCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    [DebuggerStepThrough]
    public class LastPossibleDevenCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[0];
            }
        }

        internal LastPossibleDevenCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
    [GeneratedCode("System.Web.Services", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class LogErrorCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[0];
            }
        }

        internal LogErrorCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }

}
