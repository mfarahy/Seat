using Exir.Framework.Common;
using Exir.Framework.Common.Logging;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Seat.Models;
using Seat.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Seat.SeatEngine.DataProvider.Tsetmc
{
    public class TsetmcOnlineDataProvider : DataProvider, ITsetmcOnlineDataProvider
    {
        private bool _isMarketWatchInit;
        private long _heven;
        private long _refid;

        public long Heven { get { return _heven; } }
        public string Index { get; set; }
        public string IndexIncreament { get; set; }
        public ObserverMessage LastObserverMessage { get; set; }

        public TsetmcOnlineDataProvider() : base("http://www.tsetmc.com")
        {
            Data = new ConcurrentDictionary<long, TsetmcDataRow>();
        }

        public ConcurrentDictionary<long, TsetmcDataRow> Data { get; private set; }



        private async Task LoadClientTypeAsync(TimeSpan timeout)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            cts.CancelAfter(timeout);
            var response = await client.GetAsync("tsev2/data/ClientTypeAll.aspx", cts.Token);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            int change_count = 0;
            var rows = content.Split(';');
            for (var qpos = 0; qpos < rows.Length; qpos++)
            {
                var cols = rows[qpos].Split(',');

                if (cols.Length != 9) continue;

                var RowId = TryParseLong(cols[0]);

                if (!Data.ContainsKey(RowId))
                    Data.TryAdd(RowId, new TsetmcDataRow());

                var row = Data[RowId];

                row.Buy_CountI = TryParseInt(cols[1]);
                row.Buy_CountN = TryParseInt(cols[2]);
                row.Buy_I_Volume = TryParseLong(cols[3]);
                row.Buy_N_Volume = TryParseLong(cols[4]);
                row.Sell_CountI = TryParseInt(cols[5]);
                row.Sell_CountN = TryParseInt(cols[6]);
                row.Sell_I_Volume = TryParseLong(cols[7]);
                row.Sell_N_Volume = TryParseLong(cols[8]);

                if (row.IsTypeClientChanges())
                    change_count++;
            }
        }

        public async Task LoadInstHistoryAsync()
        {
            var response = await client.GetAsync("tsev2/data/ClosingPriceAll.aspx");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var RowId = 0L;
            var rows = content.Split(';');

            for (var qpos = 0; qpos < rows.Length; qpos++)
            {
                var cols = rows[qpos].Split(',');
                int offset = 0;
                if (cols.Length == 11)
                {
                    RowId = TryParseLong(cols[0]);
                    offset = 1;
                }

                var days = TryParseInt(cols[offset]);
                if (!Data.ContainsKey(RowId))
                    Data.TryAdd(RowId, new TsetmcDataRow());

                var row = Data[RowId];

                var insDay = new InstHistory
                {
                    PClosing = TryParseInt(cols[offset + 1]),
                    PDrCotVal = TryParseInt(cols[offset + 2]),
                    ZTotTran = TryParseInt(cols[offset + 3]),
                    QTotTran5J = TryParseLong(cols[offset + 4]),
                    QTotCap = TryParseLong(cols[offset + 5]),
                    PriceMin = TryParseInt(cols[offset + 6]),
                    PriceMax = TryParseInt(cols[offset + 7]),
                    PriceYesterday = TryParseInt(cols[offset + 8]),
                    PriceFirst = TryParseInt(cols[offset + 9])
                };
                if (!row.History.ContainsKey(days))
                    row.History.TryAdd(days, insDay);
                else
                    row.History[days] = insDay;
            }
        }

        private async Task<int> UpdateMarketWatchAsync(TimeSpan timeout)
        {
            string url = "tsev2/data/MarketWatchInit.aspx";
            if (_isMarketWatchInit)
                url = "tsev2/data/MarketWatchPlus.aspx";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("h", (5 * Math.Floor(_heven * 1.0 / 5)).ToString());
            nvc.Add("r", (25 * Math.Floor(_refid * 1.0 / 25)).ToString());

            var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            cts.CancelAfter(timeout);
            var response = await client.GetAsync(url + "?" + nvc.SerializeToString(), cts.Token);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var all = content.ApplyCorrectYeKe().Split('@');

            if (all[1].Length != 0)
                UpdateFastView(all[1].Split(','));

            var instPrice = all[2].Split(';');
            int deal_count = 0;
            for (var ipos = 0; ipos < instPrice.Length; ipos++)
            {
                var col = instPrice[ipos].Split(',');
                var RowID = TryParseLong(col[0]);

                var row = Data.ContainsKey(RowID) ? Data[RowID] : new TsetmcDataRow();
                row.RowId = RowID;
                if (col.Length == 10)
                {
                    var py = Data.ContainsKey(RowID) ? Data[RowID].py : 0;
                    var eps = Data.ContainsKey(RowID) ? Data[RowID].eps : 0;

                    row.heven = TryParseInt(col[1]);
                    row.pf = TryParseInt(col[2]);
                    row.pc = TryParseInt(col[3]);
                    row.pcc = TryParseInt(col[3]) - py;
                    row.pcp = py == 0 ? 0 : (float)AdvRound(100 * (TryParseInt(col[3]) - py) / py, 2);
                    row.pl = TryParseInt(col[4]);
                    row.plc = col[5] == "0" ? 0 : TryParseInt(col[4]) - py;
                    row.plp = col[5] == "0" || py == 0 ? 0 : (float)AdvRound(100.0 * (TryParseInt(col[4]) - py) / py, 2);
                    row.tno = TryParseInt(col[5]);
                    row.tvol = TryParseLong(col[6]);
                    row.tval = TryParseLong(col[7]);
                    row.pmin = TryParseInt(col[8]);
                    row.pmax = TryParseInt(col[9]);
                    row.pe = eps == 0 ? 0f : (float)AdvRound(TryParseInt(col[4]) * 1.0f / eps, 2);

                    if (_heven < TryParseLong(col[1]))
                    {
                        _heven = TryParseLong(col[1]);
                        deal_count++;
                    }
                }
                else if (col.Length == 23)
                {
                    row.inscode = TryParseLong(col[0]);
                    row.iid = col[1];
                    row.l18 = col[2];
                    row.l30 = col[3];
                    row.py = TryParseInt(col[13]);
                    row.bvol = TryParseInt(col[15]);
                    row.visitcount = TryParseInt(col[16]);
                    row.flow = (FlowTypes)TryParseInt(col[17]);
                    row.cs = TryParseInt(col[18]);
                    row.tmax = TryParseInt(col[19]);
                    row.tmin = TryParseInt(col[20]);
                    row.z = TryParseLong(col[21]);
                    row.yval = col[22];

                    row.heven = TryParseInt(col[4]);
                    row.pf = TryParseInt(col[5]);
                    row.pc = TryParseInt(col[6]);
                    row.pcc = TryParseInt(col[6]) - TryParseInt(col[13]);
                    row.pcp = (float)AdvRound(100 * (TryParseInt(col[6]) - TryParseInt(col[13])) / TryParseInt(col[13]), 2);
                    row.pl = TryParseInt(col[7]);
                    row.plc = col[8] == "0" ? 0 : TryParseInt(col[7]) - TryParseInt(col[13]);
                    row.plp = col[8] == "0" ? 0 : (float)AdvRound(100 * (TryParseInt(col[7]) - TryParseInt(col[13])) / TryParseInt(col[13]), 2);
                    row.tno = TryParseInt(col[8]);
                    row.tvol = TryParseLong(col[9]);
                    row.tval = TryParseLong(col[10]);
                    row.pmin = TryParseInt(col[11]);
                    row.pmax = TryParseInt(col[12]);
                    row.eps = TryParseInt(col[14]);
                    row.pe = TryParseDouble(col[14]) == 0 ? 0 : (float)AdvRound(TryParseInt(col[6]) / TryParseInt(col[14]), 2);

                    if (_heven <= TryParseInt(col[4]))
                    {
                        _heven = TryParseInt(col[4]);
                        deal_count++;
                    }

                    Data.TryAdd(RowID, row);
                }
            }
            if (deal_count > 0)
                logger.InfoFormat("number of deals base on heven changes is {0}", deal_count);

            var bestLimit = all[3].Split(';');
            for (var ipos = 0; ipos < bestLimit.Length; ipos++)
            {
                var col = bestLimit[ipos].Split(',');
                var RowID = TryParseLong(col[0]);
                if (!Data.ContainsKey(RowID))
                    continue;
                var row = Data[RowID];
                switch (col[1])
                {
                    case "1":
                        row.zo1 = TryParseInt(col[2]);
                        row.zd1 = TryParseInt(col[3]);
                        row.pd1 = TryParseInt(col[4]);
                        row.po1 = TryParseInt(col[5]);
                        row.qd1 = TryParseLong(col[6]);
                        row.qo1 = TryParseLong(col[7]);
                        break;
                    case "2":
                        row.zo2 = TryParseInt(col[2]);
                        row.zd2 = TryParseInt(col[3]);
                        row.pd2 = TryParseInt(col[4]);
                        row.po2 = TryParseInt(col[5]);
                        row.qd2 = TryParseLong(col[6]);
                        row.qo2 = TryParseLong(col[7]);
                        break;
                    case "3":
                        row.zo3 = TryParseInt(col[2]);
                        row.zd3 = TryParseInt(col[3]);
                        row.pd3 = TryParseInt(col[4]);
                        row.po3 = TryParseInt(col[5]);
                        row.qd3 = TryParseLong(col[6]);
                        row.qo3 = TryParseLong(col[7]);
                        break;
                }
            }

            if (all[4] != "0" && TryParseLong(all[4]) > _refid)
                _refid = TryParseLong(all[4]);

            _isMarketWatchInit = true;

            return deal_count;
        }

        public async Task<bool> RefreshAsync(TimeSpan timeout)
        {
            try
            {
                await UpdateMarketWatchAsync(timeout);
                await LoadClientTypeAsync(timeout);
            }
            catch (TaskCanceledException)
            {
                logger.Error("refresh timeout");
                return false;
            }
            catch (HttpRequestException httpex)
            {
                logger.Error("refresh error", httpex);
                return false;
            }

            return true;
        }

        public void Clear()
        {
            _heven = 0;
            _refid = 0;
            _isMarketWatchInit = false;
            Data.Clear();
        }

        private void UpdateFastView(string[] a)
        {
            Index = a[2];
            IndexIncreament = a[3];
        }

        private double AdvRound(double b, double a)
        {
            return Math.Round(b * Math.Pow(10, a)) / Math.Pow(10, a);
        }

        public async Task LoadDataAsync()
        {
            await try_do_async(async () =>
            {
                await UpdateMarketWatchAsync(TimeSpan.FromSeconds(10));
                await LoadClientTypeAsync(TimeSpan.FromSeconds(10));
                logger.InfoFormat("successfuly {0} instance was loaded by LoadData", Data.Count);
            });
        }

        public async Task<IEnumerable<ObserverMessage>> GetNewMessagesAsync()
        {
            var html = await try_do_async(async () =>
            {
                var response = await client.GetAsync("/Loader.aspx?ParTree=151313&Flow=0");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var trs = doc.DocumentNode.SelectNodes("//tr").ToList();

            var messages = new List<ObserverMessage>();
            ObserverMessage prev_message = null;
            foreach (var tr in trs)
            {
                if (tr.ChildNodes.Count == 4)
                {
                    prev_message = new ObserverMessage()
                    {
                        Subject = tr.ChildNodes[0].InnerText,
                        MessageDt = Typing.ChangeType<DateTime>("13" + tr.ChildNodes[2].InnerText)
                    };
                }
                else
                {
                    if (prev_message != null)
                        prev_message.Description = tr.InnerText;

                    foreach (Match match in Regex.Matches(tr.InnerText, @"\(([^)]*)\)"))
                    {
                        var org_name = match.Groups[1].Value;
                        var instance_name = org_name.TrimEnd('1');

                        var instance = Data.Values.FirstOrDefault(x => x.l18 == org_name);
                        if (instance == null)
                            instance = Data.Values.FirstOrDefault(x => x.l18 == instance_name);

                        if (instance == null)
                        {
                            instance = await FindAsync(instance_name);
                            if (instance != null)
                                Data.TryAdd(instance.RowId, instance);
                        }

                        if (instance != null)
                        {
                            if (prev_message.RelativeInstances == null)
                                prev_message.RelativeInstances = new List<long>();

                            if (!prev_message.RelativeInstances.Contains(instance.RowId))
                                prev_message.RelativeInstances.Add(instance.RowId);
                        }
                    }

                    messages.Add(prev_message);
                }
            }
            if (messages.Count == 0) return new ObserverMessage[0];

            if (LastObserverMessage == null)
            {
                LastObserverMessage = messages[0];
                return messages;
            }

            int new_count = 0;
            for (int i = 0; i < messages.Count; ++i)
            {
                if (LastObserverMessage == messages[i])
                    break;
                else
                    ++new_count;
            }

            LastObserverMessage = messages[0];

            return messages.Take(new_count);
        }

        public override void Dispose()
        {
            Data.Clear();
            base.Dispose();
        }


        public async Task<TsetmcDataRow> FindAsync(string symbole)
        {
            var result_array = await try_do_async(async () =>
            {
                var response = await client.GetAsync("/tsev2/data/search.aspx?skey=" + symbole);
                response.EnsureSuccessStatusCode();
                var search_result = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(search_result)) return null;
                return search_result.ApplyCorrectYeKe().Split(';')[0]?.Split(',');
            });

            if (result_array != null && result_array.Length > 0)
            {
                TsetmcDataRow result = null;
                result = new TsetmcDataRow();
                result.l18 = result_array[0];
                result.l30 = result_array[1]?.MaxLength(30);
                result.RowId = TryParseLong(result_array[2]);

                //http://www.tsetmc.com/Loader.aspx?ParTree=151311&i=50792786683910016

                return await try_do_async(async () =>
                 {
                     var response = await client.GetAsync("/Loader.aspx?ParTree=151311&i=" + result.RowId);
                     response.EnsureSuccessStatusCode();
                     var loader_result = await response.Content.ReadAsStringAsync();

                     result.iid = extract(loader_result, "InstrumentID");
                     result.eps = (float)TryParseDouble(extract(loader_result, "EstimatedEPS"));
                     result.bvol = TryParseLong(extract(loader_result, "BaseVol"));
                     result.cs = TryParseInt(extract(loader_result, "CSecVal"));
                     result.z = TryParseLong(extract(loader_result, "ZTitad"));
                     result.flow = (FlowTypes)TryParseInt(extract(loader_result, "Flow"));
                     
                     return result;
                 });

            }
            return null;
        }

        private static string extract(string text, string name)
        {
            var match = Regex.Match(text, String.Format(@"{0}\s*=\s*'(?<{0}>[^']*)'", name)).Groups[name];
            if (match != null)
                return match.Value;
            return null;
        }

        public async Task<DayTradeDetails> ExtractDayDetailsAsync(long insCode, DateTime date)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var html = await try_do_async(async () =>
            {
                var response = await client.GetAsync(String.Format("/Loader.aspx?ParTree=15131P&i={0}&d={1}", insCode, date.ToString("yyyyMMdd")));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
            watch.Stop();
            List<BestLimit> bestLimits = ExtractBestLimits(insCode, date, html);
            List<Trade> trades = ExtractIntraTradeData(insCode, date, html);
            List<ShareHolderChange> shareHolderStates = ExtractShareHolderData(insCode, date, html);

            logger.InfoFormat("extract day trade details for {0} for {1} in {2}ms with {3}BL {4}T {5}SHS",
                insCode, date.ToString("yyyyMMdd"), watch.ElapsedMilliseconds, bestLimits.Count, trades.Count, shareHolderStates.Count);

            return new DayTradeDetails
            {
                BestLimits = bestLimits,
                DayDate = date,
                InsCode = insCode,
                ShareHolderStates = shareHolderStates,
                Trades = trades
            };
        }
        private static List<ShareHolderChange> ExtractShareHolderData(long insCode, DateTime date, string html)
        {
            Match m;
            List<ShareHolderChange> changes;
            m = Regex.Match(html, @"ShareHolderData=([^;]*]);");
            var dataArray = JsonConvert.DeserializeObject<string[][]>(m.Groups[1].Value);
            changes = new List<ShareHolderChange>();
            for (var i = 0; i < dataArray.Length; ++i)
            {
                changes.Add(new ShareHolderChange
                {
                    DateTime = date,
                    InsCode = insCode,
                    Arrow = String.IsNullOrEmpty(dataArray[i][4]) ? null : dataArray[i][4].Substring(5, 1),
                    Percent = TryParseDouble(dataArray[i][3]),
                    HolderPK = TryParseInt(dataArray[i][0]),
                    InstrumentID = dataArray[i][1],
                    Name = dataArray[i][5],
                    Quoantity = TryParseLong(dataArray[i][2]),
                });
            }
            return changes;
        }
        private static List<Trade> ExtractIntraTradeData(long insCode, DateTime date, string html)
        {
            Match m;
            List<Trade> trades;
            m = Regex.Match(html, @"IntraTradeData=([^;]*]);");
            var intraTradeDataArray = JsonConvert.DeserializeObject<string[][]>(m.Groups[1].Value);
            trades = new List<Trade>();
            for (var i = 0; i < intraTradeDataArray.Length; ++i)
            {
                trades.Add(new Trade
                {
                    Number = TryParseInt(intraTradeDataArray[i][0]),
                    Price = TryParseDecimal(intraTradeDataArray[i][3]),
                    Quantity = TryParseInt(intraTradeDataArray[i][2]),
                    DateTime = date + TimeSpan.Parse(intraTradeDataArray[i][1]),
                    Unknown1 = TryParseInt(intraTradeDataArray[i][4]),
                    InsCode = insCode
                });
            }
            return trades;
        }

        private static List<BestLimit> ExtractBestLimits(long insCode, DateTime date, string html)
        {
            var m = Regex.Match(html, @"BestLimitData=([^;]*]);");
            var bestLimitDataArray = JsonConvert.DeserializeObject<string[][]>(m.Groups[1].Value);

            var data = new List<BestLimit>();
            for (var i = 0; i < bestLimitDataArray.Length; ++i)
            {
                data.Add(new BestLimit
                {
                    DateTime = ReadableHEven(date, bestLimitDataArray[i][0]),
                    Row = int.Parse(bestLimitDataArray[i][1]),
                    Buy_Count = TryParseInt(bestLimitDataArray[i][2]),
                    Buy_Volume = TryParseLong(bestLimitDataArray[i][3]),
                    Buy_Price = TryParseDecimal(bestLimitDataArray[i][4]),
                    Sell_Price = TryParseDecimal(bestLimitDataArray[i][5]),
                    Sell_Volume = TryParseLong(bestLimitDataArray[i][6]),
                    Sell_Count = TryParseInt(bestLimitDataArray[i][7]),
                    InsCode = insCode
                });
            }

            return data;
        }

        private static DateTime ReadableHEven(DateTime date, string stime)
        {
            var c = stime.Length == 5 ? "0" + stime : stime;
            return date + TimeSpan.Parse(c.Substring(0, 2) + ":" + c.Substring(2, 2) + ":" + c.Substring(4, 2));
        }
    }
}
