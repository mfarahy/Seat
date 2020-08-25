using Exir.Framework.Common;
using Exir.Framework.Common.Entity;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SeatDomain.Models;
using SeatDomain.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class TsetmcOnlineDataProvider : DataProvider, ITsetmcOnlineDataProvider
    {
        private bool _isMarketWatchInit;
        private long _heven;
        private long _refid;

        public long Heven { get { return _heven; } }
        public string IndexIncreament { get; set; }
        public ObserverMessage LastObserverMessage { get; set; }

        public TsetmcOnlineDataProvider() : base(
            //"http://www.tsetmc.com",
            //"http://cdn1.tsetmc.com",
            "http://cdn2.tsetmc.com",
            "http://cdn3.tsetmc.com",
            "http://cdn4.tsetmc.com",
            "http://cdn5.tsetmc.com",
            "http://cdn6.tsetmc.com",
            "http://cdn7.tsetmc.com")
        {
            Data = new ConcurrentDictionary<long, TsetmcDataRow>();
        }

        public ConcurrentDictionary<long, TsetmcDataRow> Data { get; private set; }

        private async Task LoadClientTypeAsync(TimeSpan timeout)
        {
            var content = await try_do_async(async () =>
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
                cts.CancelAfter(timeout);

                var response = await Http.GetAsync("tsev2/data/ClientTypeAll.aspx", cts.Token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });

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

            if (change_count > 0)
                logger.InfoFormat("the {0} number of client type was changed base on backup value", change_count);
        }

        public async Task LoadInstHistoryAsync()
        {
            var content = await try_do_async(async () =>
            {
                var response = await Http.GetAsync("tsev2/data/ClosingPriceAll.aspx");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });

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

            var content = await try_do_async(async () =>
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
                cts.CancelAfter(timeout);

                var response = await Http.GetAsync(url + "?" + nvc.SerializeToString(), cts.Token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });

            var all = content.ApplyCorrectYeKe().Split('@');

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
                if (!_isMarketWatchInit)
                {
                    await UpdateMarketWatchAsync(timeout);
                    await LoadClientTypeAsync(timeout);
                }
                else
                {
                    var t1 = UpdateMarketWatchAsync(timeout);
                    var t2 = LoadClientTypeAsync(timeout);
                    await Task.WhenAll(t1, t2);
                }

                if (Environment.UserInteractive)
                    Console.Write(".");
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

        private double AdvRound(double b, double a)
        {
            return Math.Round(b * Math.Pow(10, a)) / Math.Pow(10, a);
        }

        public async Task LoadDataAsync()
        {
            await try_do_async(async () =>
            {
                _isMarketWatchInit = false;
                await UpdateMarketWatchAsync(TimeSpan.FromSeconds(10));
                await LoadClientTypeAsync(TimeSpan.FromSeconds(10));
                logger.InfoFormat("successfuly {0} instance was loaded by LoadData", Data.Count);
            });
        }
        private static ConcurrentBag<string> _blackList = new ConcurrentBag<string>();
        public async Task<IEnumerable<ObserverMessage>> GetNewMessagesAsync()
        {
            var html = await try_do_async(async () =>
            {
                var response = await Http.GetAsync("/Loader.aspx?ParTree=151313&Flow=0");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content.ApplyCorrectYeKe();
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

                        if (_blackList.Contains(org_name)) continue;

                        var instance_name = org_name.TrimEnd('1', '2', '3', '4');

                        var instance = StaticData.Instruments.FirstOrDefault(x => x.Symbol == org_name);
                        if (instance == null)
                            instance = StaticData.Instruments.FirstOrDefault(x => x.Symbol == instance_name);

                        if (instance != null)
                        {
                            if (prev_message.RelativeInstances == null)
                                prev_message.RelativeInstances = new List<long>();

                            if (!prev_message.RelativeInstances.Contains(instance.InsCode))
                                prev_message.RelativeInstances.Add(instance.InsCode);
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
        public async Task<InstrumentLastInfo> FindAsync(string symbole, long? insCode)
        {
            InstrumentLastInfo result = null;

            if (insCode != null && Data.ContainsKey(insCode.Value))
            {
                var row = Data[insCode.Value];
                result = new InstrumentLastInfo();
                result.Symbole = row.l18;
                result.LSoc30 = row.l30;
                result.InsCode = row.RowId;
                result.CSecVal = row.cs;
            }

            if (result == null && insCode != null)
            {
                var ins = StaticData.Instruments.Where(x => x.InsCode == insCode.Value).FirstOrDefault();
                if (ins != null)
                {
                    result = new InstrumentLastInfo();
                    result.Symbole = ins.Symbol;
                    result.LSoc30 = ins.LSoc30;
                    result.InsCode = ins.InsCode;
                    int cSecVal;
                    if (int.TryParse(ins.CSecVal, out cSecVal))
                        result.CSecVal = cSecVal;
                }
            }


            if (result == null && !String.IsNullOrEmpty(symbole))
            {
                var itemFromData = Data.Values.FirstOrDefault(x => x.l18 == symbole);
                if (itemFromData != null)
                {
                    result = new InstrumentLastInfo();
                    result.Symbole = itemFromData.l18;
                    result.LSoc30 = itemFromData.l30;
                    result.InsCode = itemFromData.RowId;
                    result.CSecVal = itemFromData.cs;
                }
            }

            if (result == null)
            {
                var result_array = await try_do_async(async () =>
                {
                    var response = await Http.GetAsync("/tsev2/data/search.aspx?skey=" + symbole);
                    response.EnsureSuccessStatusCode();
                    var search_result = await response.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(search_result)) return null;
                    return search_result.ApplyCorrectYeKe().Split(';')[0]?.Split(',');
                });

                if (result_array != null && result_array.Length > 0)
                {
                    var item = result_array.FirstOrDefault(x => x == symbole);
                    if (item == null) return null;
                    result = new InstrumentLastInfo();
                    result.Symbole = result_array[0];
                    result.LSoc30 = result_array[1]?.MaxLength(30);
                    result.InsCode = TryParseLong(result_array[2]);
                }
            }

            if (result != null)
            {
                //http://www.tsetmc.com/Loader.aspx?ParTree=151311&i=50792786683910016

                var t1 = try_do_async(async () =>
                {
                    var response = await Http.GetAsync("/Loader.aspx?ParTree=151311&i=" + result.InsCode);

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                });

                string loader_result = null;
                if (result.CSecVal == 0)
                {
                    await Task.WhenAll(t1);
                    loader_result = t1.Result;
                    result.CSecVal = TryParseInt(extract(loader_result, "CSecVal"));
                }

                var t2 = try_do_async(async () =>
                {
                    var response = await Http.GetAsync(String.Format("/tsev2/data/instinfofast.aspx?i={0}&c={1}", result.InsCode, result.CSecVal));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                });

                if (String.IsNullOrEmpty(loader_result))
                {
                    await Task.WhenAll(t1, t2);
                    loader_result = t1.Result;
                }
                else
                    await Task.WhenAll(t2);

                result.InstrumentID = extract(loader_result, "InstrumentID");
                result.EstimatedEPS = (float)TryParseDouble(extract(loader_result, "EstimatedEPS"));
                result.BaseVol = TryParseLong(extract(loader_result, "BaseVol"));
                result.CSecVal = TryParseInt(extract(loader_result, "CSecVal"));
                result.ZTitad = TryParseLong(extract(loader_result, "ZTitad"));
                result.Flow = (FlowTypes)TryParseInt(extract(loader_result, "Flow"));
                result.PSGelStaMin = TryParseInt(extract(loader_result, "PSGelStaMin"));
                result.PSGelStaMax = TryParseInt(extract(loader_result, "PSGelStaMax"));
                result.MinYear = TryParseInt(extract(loader_result, "MinYear"));
                result.MaxYear = TryParseInt(extract(loader_result, "MaxYear"));
                result.QTotTran5JAvg = TryParseLong(extract(loader_result, "QTotTran5JAvg"));
                result.SectorPE = (float)TryParseDouble(extract(loader_result, "SectorPE"));
                result.KAjCapValCpsIdx = TryParseLong(extract(loader_result, "KAjCapValCpsIdx"));
                result.PriceMin = TryParseInt(extract(loader_result, "PriceMin"));
                result.PriceMax = TryParseInt(extract(loader_result, "PriceMax"));


                var data = t2.Result;
                var AllData = data.Split(';');
                var Ov = AllData[0].Split(',');
                result.Status = InsState(Ov[1]);
                result.PriceYesterday = TryParseInt(Ov[5]);
                result.PdrCotVal = TryParseInt(Ov[2]);
                result.PClosing = TryParseInt(Ov[3]);
                result.Last = TryParseInt(Ov[11]);
                result.Count = TryParseInt(Ov[8]);
                result.Vol = TryParseLong(Ov[9]);
                result.Val = TryParseDecimal(Ov[10]);
                if (result.Flow != FlowTypes.Ati)
                    result.BVal = TryParseDecimal(Ov[11]);

                if (!String.IsNullOrEmpty(result.InstrumentID) &&
                    result.InstrumentID.Length > 2 && result.InstrumentID[2] == 'T' &&
                    Ov.Length > 14 && Ov[15].Length != 0)
                {
                    result.NAV = TryParseInt(Ov[15]);
                    result.NAVDate = Typing.ChangeType<DateTime>(Ov[14]);
                }

                DateTime date;
                if (DateTime.TryParseExact(extract(loader_result, "DEven"), "yyyyMMdd", Thread.CurrentThread.CurrentCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None, out date))
                    result.DEven = date.Date;
                return result;
            }
            return null;
        }
        private InstrumentStates InsState(string a)
        {
            switch (a)
            {
                case "I ": return InstrumentStates.I_Impermissible;
                case "A ": return InstrumentStates.A_Allow;
                case "AG": return InstrumentStates.AG;
                case "AS": return InstrumentStates.AS_AllowStoped;
                case "AR": return InstrumentStates.AR_AllowReserved;
                case "IG": return InstrumentStates.IG;
                case "IS": return InstrumentStates.IS_ImpermissibleStoped;
                case "IR": return InstrumentStates.IR_ImpermissibleReserved;
            }
            return InstrumentStates.Unknown;
        }
        private static string extract(string text, string name)
        {
            return extract(text, name, String.Format(@"{0}\s*=\s*'(?<{0}>[^']*)'", name));
        }
        private static string extract(string text, string name, string regex)
        {
            var match = Regex.Match(text, regex).Groups[name];
            if (match != null)
                return match.Value;
            return null;
        }
        public async Task<DayTradeDetails> ExtractDayDetailsAsync(long insCode, DateTime date)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var html = await try_do_async(async () =>
                {
                    var response = await Http.GetAsync(String.Format("/Loader.aspx?ParTree=15131P&i={0}&d={1}", insCode, date.ToString("yyyyMMdd")));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                });
                watch.Stop();
                List<BestLimit> bestLimits = ExtractBestLimits(insCode, date, html);
                List<Trade> trades = ExtractIntraTradeData(insCode, date, html);
                List<ShareHolderChange> shareHolderStates = ExtractShareHolderData(insCode, date, html);
                List<ClosingPrice> closingPriceData = ExtractClosingPriceData(insCode, date, html);

                logger.InfoFormat("extract day trade details for {0} for {1} in {2}ms with {3}BL {4}T {5}SHS",
                    insCode, date.ToString("yyyyMMdd"), watch.ElapsedMilliseconds, bestLimits.Count, trades.Count, shareHolderStates.Count);

                return new DayTradeDetails
                {
                    BestLimits = bestLimits,
                    DayDate = date,
                    InsCode = insCode,
                    ShareHolderStates = shareHolderStates,
                    Trades = trades,
                    ClosingPriceData = closingPriceData
                };
            }
            catch (Exception ex)
            {
                logger.WarnFormat("cannot get day trade details for {0} in {1}", ex, insCode, date);
                return null;
            }
        }
        private List<ClosingPrice> ExtractClosingPriceData(long insCode, DateTime date, string html)
        {
            var m = Regex.Match(html, @"ClosingPriceData=([^;]*]);");
            var dataArray = JsonConvert.DeserializeObject<string[][]>(m.Groups[1].Value);

            var result = new List<ClosingPrice>();
            for (var i = 0; i < dataArray.Length; ++i)
            {
                try
                {
                    result.Add(new ClosingPrice
                    {
                        DateTime = date + TryParseTime(dataArray[i][12]),
                        InsCode = insCode,
                        PdrCotVal = TryParseInt(dataArray[i][2]),
                        PClosing = TryParseInt(dataArray[i][3]),
                        First = TryParseInt(dataArray[i][4]),
                        Ystrdy = TryParseInt(dataArray[i][5]),
                        Max = TryParseInt(dataArray[i][6]),
                        Min = TryParseInt(dataArray[i][7]),
                        TradeCount = TryParseInt(dataArray[i][8]),
                        Vol = TryParseLong(dataArray[i][9]),
                        Val = TryParseDecimal(dataArray[i][10]),
                        BVal = TryParseDecimal(dataArray[i][11]),
                    });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return result;
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
                try
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
                catch (Exception)
                {

                    throw;
                }
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
                try
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
                catch (Exception)
                {

                    throw;
                }
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
                    DateTime = date + TryParseTime(bestLimitDataArray[i][0]),
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

        public async Task<Index> GetIndex(long indexCode, bool includeHistory)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var html = await try_do_async(async () =>
            {
                var response = await Http.GetAsync(String.Format("/Loader.aspx?ParTree=15131J&i={0}", indexCode));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
            watch.Stop();

            var index = new Index();

            index.LVal18AFC = extract(html, "LVal18AFC");
            index.Code = indexCode;
            index.Title = extract(html, "Title", @"<div class=""header bigheader"">(?<Title>[^<]*)<");
            index.LastValue = (float)TryParseDouble(extract(html, "LastValue", @"<td>آخرین مقدار شاخص</td>\s*<td>(?<LastValue>[^<]*)</td>"));
            index.MaxValue = (float)TryParseDouble(extract(html, "MaxValue", @"<td>بیشترین مقدار روز</td>\s*<td>(?<MaxValue>[^<]*)</td>"));
            index.MinValue = (float)TryParseDouble(extract(html, "MinValue", @"<td>کمترین مقدار روز</td>\s*<td>(?<MinValue>[^<]*)</td>"));
            var chartIndexLastDayData = extract(html, "ChartIndexLastDay");
            index.LastDayTimeValue = chartIndexLastDayData.Split(';')
                .Select(x => new IndexLastDayTimeValue(x)
                {
                    InsCode = indexCode
                }).ToArray();
            index.RelatedCompanies = Regex.Match(html, @"<tr id='(\d{17})'>")
                .Groups[1].Captures.Cast<Capture>()
                .Select(x => TryParseLong(x.Value))
                .ToList();

            if (includeHistory)
            {
                html = await try_do_async(async () =>
                {
                    var response = await Http.GetAsync(String.Format("tsev2/chart/data/Index.aspx?i={0},t={1}", indexCode, "value"));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                });

                List<DayValue> result = new List<DayValue>();
                var o = html.Split(';');
                for (var i = 0; i < o.Length; i++)
                {
                    var h = o[i].Split(',');
                    var deven = Typing.ChangeType<DateTime>(h[0]);
                    result.Add(new DayValue(deven, TryParseDecimal(h[1])));
                }
                index.History = result;
            }

            return index;
        }

        public async Task<List<Index>> GetIndexLastValue()
        {
            var html = await try_do_async(async () =>
            {
                var response = await Http.GetAsync(String.Format("/Loader.aspx?Partree=151315&Flow=1"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
            List<Index> result = new List<Index>();
            int last_tr_index = 0;
            bool is_header = true;
            do
            {
                var tr_start = html.IndexOf("<tr>", last_tr_index);
                if (tr_start < 0) break;
                var tr_end = html.IndexOf("</tr>", tr_start);
                last_tr_index = tr_end;
                if (is_header)
                {
                    is_header = false;
                    continue;
                }
                string tr = html.Substring(tr_start, tr_end - tr_start);
                var insCode = extract(tr, "insCode", @"Loader.aspx\?ParTree=15131J&i=(?<insCode>\d*)");
                var tds = tr.Split(new string[] { "<td>" }, StringSplitOptions.RemoveEmptyEntries);
                var publish_time = extract(tds[2], "time", @"(?<time>\d+:\d+)");
                var lastValue = extract(tds[3], "number", @"(?<number>[,0-9.]+)");
                var changeValue = extract(tds[4], "number", @"(?<number>[-,0-9.]+)");
                if (tds[4].IndexOf("mn") > 0) changeValue = "-" + changeValue;
                var changePercent = extract(tds[5], "number", @"(?<number>[-,0-9.]+)");
                if (tds[5].IndexOf("mn") > 0) changePercent = "-" + changePercent;
                var maxValue = extract(tds[6], "number", @"(?<number>[,0-9.]+)"); ;
                var minValue = extract(tds[7], "number", @"(?<number>[,0-9.]+)"); ;
                result.Add(new Index
                {
                    Code = TryParseLong(insCode),
                    PublishTime = TimeSpan.ParseExact(publish_time, @"hh\:mm", Thread.CurrentThread.CurrentCulture.DateTimeFormat),
                    LastValue = (float)TryParseDouble(lastValue),
                    ChangeValue = (float)TryParseDouble(changeValue),
                    ChangePercent = (float)TryParseDouble(changePercent),
                    MaxValue = (float)TryParseDouble(changePercent),
                    MinValue = (float)TryParseDouble(changePercent),
                });
            } while (true);

            return result;
        }
    }
}
