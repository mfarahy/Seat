using Exir.Framework.Common.Logging;
using Seat.DataStore;
using Seat.SeatEngine;
using Seat.SeatEngine.DataProvider;
using Seat.SeatEngine.DataProvider.Tsetmc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Seat
{
    public partial class Form1 : Form
    {
        private ILogger _logger;
        public Form1()
        {
            InitializeComponent();
        }

        private SeatMediatorEngine _engine;
        private void Form1_Load(object sender, EventArgs e)
        {
            FormLogingAdapter.ListView = lsvLogs;

            _engine = new SeatMediatorEngine(new TsetmcOnlineDataProvider(),
                new TsetmcWebServiceDataProvider(),
                new CodalDataProvider(),
                new TsetmcStorage());

            _engine.OnOperationStart += _engine_OnOperationStart;
            _engine.OnOperationStep += _engine_OnOperationStep;
            _engine.OnOperationCompleted += _engine_OnOperationCompleted;

            _refreshTradesThread = new Thread(new ThreadStart(refreshTrades));
            _refreshWorksThread = new Thread(new ThreadStart(refreshWorks));
            _stop = false;

            _logger = LogManager.Instance.GetLogger<Form1>();
        }

        private Thread _refreshTradesThread;
        private Thread _refreshWorksThread;
        private bool _stop;

        private void refreshTrades()
        {
            _logger.Info("refresh trade thread started");

            var periods = new Periods
            {
                TradeStart = TimeSpan.FromHours(8.5),
                TradeEnd = TimeSpan.FromHours(17),
                FastWaitTime = TimeSpan.FromMilliseconds(500),
                SlowWaitTime = TimeSpan.FromSeconds(15)
            };
            var u = new string[] { "🌕", "🌖", "🌗", "🌘", "🌑", "🌒", "🌓", "🌔" };
            int ui = 0;
            while (!_stop)
            {
                bool isHoliday = DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday;
                bool isInTime = !isHoliday && DateTime.Now.TimeOfDay >= periods.TradeStart && DateTime.Now.TimeOfDay <= periods.TradeEnd;

                if (isInTime)
                {
                    _engine.RefreshTrades().Wait();
                }
                if (isHoliday)
                {
                    Thread.Sleep((int)TimeSpan.FromMinutes(10).TotalMilliseconds);
                }
                else
                {
                    if (isInTime)
                        Thread.Sleep((int)periods.FastWaitTime.TotalMilliseconds);
                    else
                        Thread.Sleep((int)periods.SlowWaitTime.TotalMilliseconds);
                }
                lblTradeUpdate.Invoke(new MethodInvoker(delegate
                {
                    ui = (ui + 1) % u.Length;
                    lblTradeUpdate.Text = u[ui];
                }));
            }
        }
        private void refreshWorks()
        {
            _logger.Info("main thread started");

            var periods = new Periods
            {
                RefreshInstruments = TimeSpan.FromMinutes(5),
                TradeStart = TimeSpan.FromHours(8.5),
                TradeEnd = TimeSpan.FromHours(17),
                RefreshObserverMessages = TimeSpan.FromSeconds(15),
                RefreshClosingPrices = TimeSpan.FromHours(23),
                RefreshCodalMessages = TimeSpan.FromMinutes(1),
                UpdateDayTradesStart = TimeSpan.FromHours(0),
                UpdateDayTradesEnd = TimeSpan.FromHours(6),
                UpdateDayTrades = TimeSpan.FromMinutes(5),
                FastWaitTime = TimeSpan.FromMilliseconds(500),
                SlowWaitTime = TimeSpan.FromSeconds(15)
            };


            var lastUpdate = new LastUpdate
            {
                RefreshInstruments = DateTime.MinValue,
                RefreshClosingPrices = DateTime.MinValue,
                RefreshObserverMessages = DateTime.MinValue,
                RefreshCodalMessages = DateTime.MinValue,
                UpdateDayTrades = DateTime.MinValue,
            };
            while (!_stop)
            {
                bool isHoliday = DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday;
                bool isInTime = !isHoliday && DateTime.Now.TimeOfDay >= periods.TradeStart && DateTime.Now.TimeOfDay <= periods.TradeEnd;

                if (isInTime && DateTime.Now - lastUpdate.RefreshInstruments > periods.RefreshInstruments)
                {
                    _logger.Trace("start refresh instruments");
                    _engine.RefreshInstruments().Wait();
                    lastUpdate.RefreshInstruments = DateTime.Now;
                }

                if (isInTime && DateTime.Now - lastUpdate.RefreshObserverMessages > periods.RefreshObserverMessages)
                {
                    _logger.Trace("start refresh observer messages");
                    _engine.RefreshObserverMessages().Wait();
                    lastUpdate.RefreshObserverMessages = DateTime.Now;
                }

                if (!isInTime && DateTime.Now - lastUpdate.RefreshClosingPrices > periods.RefreshClosingPrices)
                {
                    _logger.Trace("start refresh closing prices");
                    _engine.RefreshClosingPrices().Wait();
                    lastUpdate.RefreshClosingPrices = DateTime.Now;
                }

                if (DateTime.Now - lastUpdate.RefreshCodalMessages > periods.RefreshCodalMessages)
                {
                    _logger.Trace("start refresh codal messages");
                    _engine.RefreshCodalMessages().Wait();
                    lastUpdate.RefreshCodalMessages = DateTime.Now;
                }

                if (DateTime.Now.TimeOfDay > periods.UpdateDayTradesStart &&
                    DateTime.Now - lastUpdate.UpdateDayTrades > periods.UpdateDayTrades &&
                   (DateTime.Now.TimeOfDay < periods.UpdateDayTradesEnd || isHoliday))
                {
                    _logger.Trace("start refresh day trade details");
                    _engine.UpdateDayTrades(Settings.Default.UpdateDayTradesCount).Wait();
                    lastUpdate.UpdateDayTrades = DateTime.Now;
                }

                Thread.Sleep((int)periods.SlowWaitTime.TotalMilliseconds);
            }
        }

        #region progress bar
        private void _engine_OnOperationCompleted(object sender, EventArgs e)
        {
            pbProgressBar.Invoke(new MethodInvoker(delegate ()
            {
                pbProgressBar.Value = pbProgressBar.Maximum;
                pbProgressBar.Refresh();
            }));
        }

        private void _engine_OnOperationStep(object sender, EventArgs e)
        {
            pbProgressBar.Invoke(new MethodInvoker(delegate ()
            {
                pbProgressBar.Value++;
                pbProgressBar.Refresh();
            }));
        }

        private void _engine_OnOperationStart(object sender, int e)
        {
            pbProgressBar.Invoke(new MethodInvoker(delegate ()
            {
                pbProgressBar.Value = 0;
                pbProgressBar.Maximum = e;
                pbProgressBar.Minimum = 0;
                pbProgressBar.Refresh();
            }));
        }


        #endregion
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await _engine.WarmupAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _stop = true;
            _engine.Dispose();
            _engine = null;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _refreshWorksThread.Start();
            _refreshTradesThread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _refreshWorksThread?.Abort();
            _refreshTradesThread?.Abort();
        }
    }
}
