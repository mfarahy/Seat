using Exir.Framework.Common.Logging;
using SeatDomain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class TsetmcWebServiceDataProvider : DataProvider, ITsetmcWebServiceDataProvider
    {
        private ILogger _logger;

        public TsetmcWebServiceDataProvider() : base(null)
        {
            _logger = LogManager.Instance.GetLogger<TsetmcWebServiceDataProvider>();
        }

        public async Task FillDataAsync()
        {
            await StaticData.FillStaticDataAsync();
        }

        public ConcurrentBag<Instrument> Instruments
        {
            get
            {
                return StaticData.Instruments;
            }
        }

        public IEnumerable<TseShareInfo> TseShares
        {
            get
            {
                return StaticData.TseShares;
            }
        }



        public async Task<bool> UpdateInstrumentsAsync()
        {
            Settings settings = new Settings();
            if (Utility.ConvertDateTimeToGregorianInt(DateTime.Now) == settings.LastInstrumentReceiveDate)
            {
                return true;
            }
            try
            {
                int num = 0;
                foreach (Instrument instrument in StaticData.Instruments)
                {
                    if (instrument.DEven > num)
                    {
                        num = instrument.DEven.GetValueOrDefault();
                    }
                }
                long num2 = 0L;
                foreach (TseShareInfo tseShare in StaticData.TseShares)
                {
                    if (tseShare.Idn > num2)
                    {
                        num2 = tseShare.Idn;
                    }
                }

                string text = await try_do(async () =>
                  {
                      return await ServerMethods.InstrumentAndShareAsync(num, num2, Settings.Default.UseWebService);
                  });

                string text2 = text.Split('@')[0];
                if (!string.IsNullOrEmpty(text2))
                {
                    if (text2.Equals("*"))
                    {
                        //بروز رسانی اطلاعات در حد فاصل ساعت هشت صبح تا یک بعد از ظهر روزهای شنبه تا چهارشنبه امکان پذیر نمی باشد. \nجهت مشاهده لیست فعلی نمادها روی دکمه مرحله بعد کلیک کنید.");
                        return false;
                    }
                    string[] array = text2.Split(';');
                    for (int i = 0; i < array.Length; i++)
                    {
                        string[] row2 = array[i].Split(',');
                        var ins = StaticData.Instruments.FirstOrDefault((Instrument p) => p.InsCode == Convert.ToInt64(row2[0]));
                        if (ins == null)
                        {
                            Instrument instrumentInfo = new Instrument();
                            instrumentInfo.InsCode = Convert.ToInt64(row2[0].ToString());
                            instrumentInfo.InstrumentID = row2[1].ToString();
                            instrumentInfo.LatinSymbol = row2[2].ToString();
                            instrumentInfo.LatinName = row2[3].ToString();
                            instrumentInfo.CompanyCode = row2[4].ToString();
                            instrumentInfo.Symbol = row2[5].ToString();
                            instrumentInfo.Name = row2[6].ToString();
                            instrumentInfo.CIsin = row2[7].ToString();
                            instrumentInfo.DEven = Convert.ToInt32(row2[8].ToString());
                            instrumentInfo.Flow = Convert.ToByte(row2[9].ToString());
                            instrumentInfo.LSoc30 = row2[10].ToString();
                            instrumentInfo.CGdSVal = row2[11].ToString();
                            instrumentInfo.CGrValCot = row2[12].ToString();
                            instrumentInfo.YMarNSC = row2[13].ToString();
                            instrumentInfo.CComVal = row2[14].ToString();
                            instrumentInfo.CSecVal = row2[15].ToString()?.Trim();
                            instrumentInfo.CSoSecVal = row2[16].ToString().Trim();
                            instrumentInfo.YVal = row2[17].ToString();
                            StaticData.Instruments.Add(instrumentInfo);
                        }
                        else
                        {
                            ins.InstrumentID = row2[1].ToString();
                            ins.LatinSymbol = row2[2].ToString();
                            ins.LatinName = row2[3].ToString();
                            ins.CompanyCode = row2[4].ToString();
                            ins.Symbol = row2[5].ToString();
                            ins.Name = row2[6].ToString();
                            ins.CIsin = row2[7].ToString();
                            ins.DEven = Convert.ToInt32(row2[8].ToString());
                            ins.Flow = Convert.ToByte(row2[9].ToString());
                            ins.LSoc30 = row2[10].ToString();
                            ins.CGdSVal = row2[11].ToString();
                            ins.CGrValCot = row2[12].ToString();
                            ins.YMarNSC = row2[13].ToString();
                            ins.CComVal = row2[14].ToString();
                            ins.CSecVal = row2[15].ToString()?.Trim();
                            ins.CSoSecVal = row2[16].ToString()?.Trim();
                            ins.YVal = row2[17].ToString();
                        }
                    }
                    await FileService.WriteInstrumentsAsync();
                    settings.LastInstrumentReceiveDate = Utility.ConvertDateTimeToGregorianInt(DateTime.Now);
                    settings.Save();
                }
                string text3 = text.Split('@')[1];
                if (!string.IsNullOrEmpty(text3))
                {
                    string[] array2 = text3.Split(';');
                    for (int j = 0; j < array2.Length; j++)
                    {
                        string[] row = array2[j].Split(',');
                        var share = StaticData.TseShares.FirstOrDefault((TseShareInfo p) => p.Idn == Convert.ToInt64(row[0]));
                        if (share == null)
                        {
                            TseShareInfo tseShareInfo = new TseShareInfo();
                            tseShareInfo.Idn = Convert.ToInt64(row[0].ToString());
                            tseShareInfo.InsCode = Convert.ToInt64(row[1].ToString());
                            tseShareInfo.DEven = Convert.ToInt32(row[2].ToString());
                            tseShareInfo.NumberOfShareNew = Convert.ToDecimal(row[3].ToString());
                            tseShareInfo.NumberOfShareOld = Convert.ToDecimal(row[4].ToString());
                            StaticData.TseShares.Add(tseShareInfo);
                        }
                        else
                        {
                            share.InsCode = Convert.ToInt64(row[1].ToString());
                            share.DEven = Convert.ToInt32(row[2].ToString());
                            share.NumberOfShareNew = Convert.ToDecimal(row[3].ToString());
                            share.NumberOfShareOld = Convert.ToDecimal(row[4].ToString());
                        }
                    }
                    await FileService.WriteTseSharesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The magic number in GZip header is not correct") && settings.EnableDecompression)
                {
                    settings.EnableDecompression = false;
                    settings.Save();
                    return false;
                }
                _logger.Error("UpdateInstruments", ex);
                return false;
            }
        }

        private static ConcurrentBag<long> _fuckingSymbols = new ConcurrentBag<long>();
        public async Task<List<ClosingPriceInfo>> UpdateClosingPricesAsync(Action<int> onStart, Action<int> onPageDone)
        {
            Settings settings = new Settings();

            string text = "";
            try
            {
                text = await try_do(async () =>
                  {
                      return await ServerMethods.LastPossibleDevenAsync(Settings.Default.UseWebService);
                  });
            }
            catch (Exception ex)
            {
                _logger.Error("lastPossibleDEvens", ex);
            }
            if (text.Equals("*"))
            {
                //بروز رسانی اطلاعات در حد فاصل ساعت هشت صبح تا یک بعد از ظهر روزهای شنبه تا چهارشنبه امکان پذیر نمی باشد.");
                return null;
            }

            var instruments = StaticData.Instruments.Where(x => !_fuckingSymbols.Contains(x.InsCode)).ToList();

            const int PAGE_SIZE = 50;
            int pageCount = (int)Math.Ceiling(instruments.Count * 1.0 / PAGE_SIZE);
            ConcurrentQueue<List<Instrument>> queue = new ConcurrentQueue<List<Instrument>>();
            for (int i = 0; i < pageCount; ++i)
                queue.Enqueue(instruments.Skip(i * PAGE_SIZE).Take(PAGE_SIZE).ToList());

            onStart?.Invoke(pageCount);
            List<ClosingPriceInfo> result = new List<ClosingPriceInfo>();
            int proccessed_page_count = 0, total_page_count = queue.Count;
            while (queue.Count > 0)
            {
                List<Instrument> page;
                if (!queue.TryDequeue(out page))
                {
                    Thread.Sleep(500); continue;
                }
                int pageSize = page.Count;
                if (pageSize == 0) continue;

                long[][] array3 = new long[pageSize][];
                for (int j = 0; j < pageSize; j++)
                {
                    array3[j] = new long[3];
                    array3[j][0] = page[j].InsCode;
                    array3[j][1] = Convert.ToInt64(page[j].LastDbDeven);
                    array3[j][2] = ((!(page[j].YMarNSC == "NO")) ? 1 : 0);
                }
                string text2 = "";
                long[][] array4 = array3;
                foreach (long[] array5 in array4)
                {
                    object obj = text2;
                    text2 = string.Concat(obj, array5[0], ",", array5[1], ",", array5[2], ";");
                }
                text2 = text2.Substring(0, text2.Length - 1);

                string insturmentClosingPrice = null;

                _logger.InfoFormat("start get {0}% of closing price from server", Math.Round(proccessed_page_count * 1.0 / total_page_count * 100));

                bool first_error = true;
                insturmentClosingPrice = try_do(() =>
                          {
                              return ServerMethods.GetInsturmentClosingPrice(text2, Settings.Default.UseWebService);
                          }, 3, exception =>
                           {
                               if (first_error)
                               {
                                   Settings.Default.UseWebService = !Settings.Default.UseWebService;
                                   Settings.Default.Save();

                                   first_error = !first_error;
                                   _logger.ErrorFormat("error while getting closing price for these instruments {0}", text2);
                               }
                               if (exception.Message.IndexOf("Forbidden") > 0)
                               {
                                   if (page.Count > 1)
                                   {
                                       var sub_page_size = page.Count / 2;
                                       queue.Enqueue(page.Take(sub_page_size).ToList());
                                       queue.Enqueue(page.Skip(sub_page_size).Take(sub_page_size).ToList());
                                       total_page_count += 2;
                                   }
                                   else
                                   {
                                       _fuckingSymbols.Add(page[0].InsCode);
                                   }
                                   return false;
                               }
                               if (exception.Message.Contains("The magic number in GZip header is not correct") && settings.EnableDecompression)
                               {
                                   settings.EnableDecompression = false;
                                   settings.Save();
                               }
                               return true;
                           });


                if (insturmentClosingPrice == null) continue;

                if (insturmentClosingPrice.Equals("*"))
                {
                    _logger.ErrorFormat("update closing price server return *");
                    //   روز رسانی اطلاعات در حد فاصل ساعت هشت صبح تا یک بعد از ظهر روزهای شنبه تا چهارشنبه امکان پذیر نمی باشد.");
                    return null;
                }
                string[] array6 = insturmentClosingPrice.Split('@');
                string[] array7 = array6;
                foreach (string text3 in array7)
                {
                    if (string.IsNullOrEmpty(text3))
                    {
                        continue;
                    }
                    List<ClosingPriceInfo> cpList = new List<ClosingPriceInfo>();
                    string[] array8 = text3.Split(';');

                    for (int m = 0; m < array8.Length; m++)
                    {
                        ClosingPriceInfo closingPriceInfo = new ClosingPriceInfo();
                        try
                        {
                            string[] array9 = array8[m].Split(',');
                            closingPriceInfo.InsCode = Convert.ToInt64(array9[0].ToString());
                            closingPriceInfo.DEven = Convert.ToInt32(array9[1].ToString());
                            closingPriceInfo.PClosing = Convert.ToDecimal(array9[2].ToString());
                            closingPriceInfo.PDrCotVal = Convert.ToDecimal(array9[3].ToString());
                            closingPriceInfo.ZTotTran = Convert.ToDecimal(array9[4].ToString());
                            closingPriceInfo.QTotTran5J = Convert.ToDecimal(array9[5].ToString());
                            closingPriceInfo.QTotCap = Convert.ToDecimal(array9[6].ToString());
                            closingPriceInfo.PriceMin = Convert.ToDecimal(array9[7].ToString());
                            closingPriceInfo.PriceMax = Convert.ToDecimal(array9[8].ToString());
                            closingPriceInfo.PriceYesterday = Convert.ToDecimal(array9[9].ToString());
                            closingPriceInfo.PriceFirst = Convert.ToDecimal(array9[10].ToString());
                            cpList.Add(closingPriceInfo);
                        }
                        catch (Exception ex2)
                        {
                            _logger.Error("UpdateClosingPrices[Row:" + array8[m] + "]", ex2);
                            throw ex2;
                        }
                    }

                    if (cpList.Count > 0)
                    {
                        result.AddRange(cpList);
                        //var deven = await FileService.WriteClosingPricesAsync(cpList);
                        Instrument instrumentInfo = StaticData.Instruments.First(p => p.InsCode == cpList[0].InsCode);
                        instrumentInfo.LastDeven = cpList.OrderBy(x => x.DEven).Last().DEven;
                    }

                    cpList.Clear();
                    cpList.TrimExcess();
                    cpList = null;
                }

                ++proccessed_page_count;
            }

            _logger.InfoFormat("get {0} new closing price from server", result.Count);

            return result;
        }


    }
}
