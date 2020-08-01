using ExcelLibrary.SpreadSheet;
using Seat.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Exir.Framework.Common;

namespace Seat.SeatEngine.DataProvider.Tsetmc
{
    // TseClient.FileService


    public class FileService
    {
        public static void CheckAppFolder()
        {
            string files_path = Path.Combine(Environment.CurrentDirectory, "data\\files");

            if (!Directory.Exists(files_path))
                Directory.CreateDirectory(files_path);

            if (!Directory.Exists(Path.Combine(files_path, "instruments")))
                Directory.CreateDirectory(Path.Combine(files_path, "instruments"));

            string[] files3 = Directory.GetFiles("files");
            foreach (string text3 in files3)
            {
                if (!File.Exists(Path.Combine(files_path, Path.GetFileName(text3))))
                {
                    File.Copy(text3, Path.Combine(files_path, Path.GetFileName(text3)));
                }
            }
            Settings settings = new Settings();
            if (string.IsNullOrEmpty(settings.StorageLocation) || !HasAccessToWrite(settings.StorageLocation))
            {
                try
                {
                    string folderPath = Environment.CurrentDirectory;
                    if (!Directory.Exists(folderPath + "\\data"))
                    {
                        Directory.CreateDirectory(folderPath + "\\data");
                    }
                    if (!HasAccessToWrite(folderPath + "\\data"))
                    {
                        throw new Exception("Access to Storage Location denied");
                    }
                    settings.StorageLocation = folderPath + "\\data";
                    settings.Save();
                }
                catch
                {
                }
            }
            if (!string.IsNullOrEmpty(settings.AdjustedStorageLocation) && HasAccessToWrite(settings.AdjustedStorageLocation))
            {
                return;
            }
            try
            {
                string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                if (!Directory.Exists(folderPath2 + "\\data\\Adjusted"))
                {
                    Directory.CreateDirectory(folderPath2 + "\\data\\Adjusted");
                }
                if (!HasAccessToWrite(folderPath2 + "\\data\\Adjusted"))
                {
                    throw new Exception("Access to Storage Location denied");
                }
                settings.AdjustedStorageLocation = folderPath2 + "\\data\\Adjusted";
                settings.Save();
            }
            catch
            {
            }
        }

        public static bool HasAccessToWrite(string path)
        {
            try
            {
                using (File.Create(Path.Combine(path, "Access.txt"), 1, FileOptions.DeleteOnClose))
                {
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<ColumnInfo>> ColumnsInfoAsync()
        {
            return await ColumnsInfoAsync("Columns.csv");
        }

        public static async Task<List<ColumnInfo>> DefaultColumnsInfoAsync()
        {
            return await ColumnsInfoAsync("DeafultColumns.csv");
        }

        public static async Task<List<ColumnInfo>> ColumnsInfoAsync(string fileName)
        {
            CheckAppFolder();
            List<ColumnInfo> list = new List<ColumnInfo>();
            try
            {
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\" + fileName))))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string text = await streamReader.ReadLineAsync();
                        string[] array = text.Split(',');
                        list.Add(new ColumnInfo
                        {
                            Index = Convert.ToInt32(array[0].ToString()),
                            Type = (ColumnType)Enum.Parse(typeof(ColumnType), array[1].ToString()),
                            Header = array[2].ToString(),
                            Visible = array[3].ToString().Equals("1")
                        });
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ColumnsInfoInString()
        {
            CheckAppFolder();
            string str = "Columns.csv";
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\" + str))))
                {
                    while (!streamReader.EndOfStream)
                    {
                        stringBuilder.Append(streamReader.ReadLine());
                        stringBuilder.Append('\n');
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return stringBuilder.ToString();
        }

        public static void WriteColumnsInfo(string input)
        {
            CheckAppFolder();
            using (TextWriter textWriter = File.CreateText(Path.Combine(Environment.CurrentDirectory, "data\\files\\Columns.csv")))
            {
                textWriter.Write(input);
                textWriter.Flush();
            }
        }

        public static async Task<ConcurrentBag<Instrument>> InstrumentsAsync()
        {
            CheckAppFolder();
            ConcurrentBag<Instrument> list = new ConcurrentBag<Instrument>();
            try
            {
                using (StreamReader streamReader = new StreamReader(File.OpenRead(
                    Path.Combine(Environment.CurrentDirectory, "data\\files\\instruments.csv"))))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string text = await streamReader.ReadLineAsync();
                        string[] array = text.Split(',');
                        list.Add(new Instrument
                        {
                            InsCode = Convert.ToInt64(array[0].ToString()),
                            InstrumentID = array[1].ToString(),
                            LatinSymbol = array[2].ToString(),
                            LatinName = array[3].ToString(),
                            CompanyCode = array[4].ToString(),
                            Symbol = array[5].ToString(),
                            Name = array[6].ToString(),
                            CIsin = array[7].ToString(),
                            DEven = Convert.ToInt32(array[8].ToString()),
                            Flow = Convert.ToByte(array[9].ToString()),
                            LSoc30 = array[10].ToString(),
                            CGdSVal = array[11].ToString(),
                            CGrValCot = array[12].ToString(),
                            YMarNSC = array[13].ToString(),
                            CComVal = array[14].ToString(),
                            CSecVal = array[15].ToString()?.Trim(),
                            CSoSecVal = array[16].ToString()?.Trim(),
                            YVal = array[17].ToString()
                        });
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task WriteInstrumentsAsync()
        {
            CheckAppFolder();
            using (TextWriter textWriter = File.CreateText(Path.Combine(Environment.CurrentDirectory, "data\\files\\Instruments.csv")))
            {
                foreach (Instrument instrument in StaticData.Instruments)
                {
                    textWriter.Write(instrument.InsCode);
                    textWriter.Write(',');
                    textWriter.Write(instrument.InstrumentID);
                    textWriter.Write(',');
                    textWriter.Write(instrument.LatinSymbol);
                    textWriter.Write(',');
                    textWriter.Write(instrument.LatinName);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CompanyCode);
                    textWriter.Write(',');
                    textWriter.Write(instrument.Symbol);
                    textWriter.Write(',');
                    textWriter.Write(instrument.Name);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CIsin);
                    textWriter.Write(',');
                    textWriter.Write(instrument.DEven);
                    textWriter.Write(',');
                    textWriter.Write(instrument.Flow);
                    textWriter.Write(',');
                    textWriter.Write(instrument.LSoc30);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CGdSVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CGrValCot);
                    textWriter.Write(',');
                    textWriter.Write(instrument.YMarNSC);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CComVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CSecVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CSoSecVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.YVal);
                    textWriter.Write('\n');
                }
                await textWriter.FlushAsync();
            }
        }

        public static void CopyInstrumentsToOutput()
        {
            CheckAppFolder();
            Settings settings = new Settings();
            string str = settings.StorageLocation;
            if (settings.AdjustPricesCondition == 1 || settings.AdjustPricesCondition == 2)
            {
                str = settings.AdjustedStorageLocation;
            }
            using (TextWriter textWriter = File.CreateText(str + "\\Instruments." + settings.FileExtension))
            {
                textWriter.Write("InsCode");
                textWriter.Write(',');
                textWriter.Write("InstrumentID");
                textWriter.Write(',');
                textWriter.Write("CValMne");
                textWriter.Write(',');
                textWriter.Write("LVal18");
                textWriter.Write(',');
                textWriter.Write("CSocCSAC");
                textWriter.Write(',');
                textWriter.Write("LVal18AFC");
                textWriter.Write(',');
                textWriter.Write("LVal30");
                textWriter.Write(',');
                textWriter.Write("CIsin");
                textWriter.Write(',');
                textWriter.Write("DTYYYYMMDD");
                textWriter.Write(',');
                textWriter.Write("Flow");
                textWriter.Write(',');
                textWriter.Write("LSoc30");
                textWriter.Write(',');
                textWriter.Write("CGdSVal");
                textWriter.Write(',');
                textWriter.Write("CGrValCot");
                textWriter.Write(',');
                textWriter.Write("YMarNSC");
                textWriter.Write(',');
                textWriter.Write("CComVal");
                textWriter.Write(',');
                textWriter.Write("CSecVal");
                textWriter.Write(',');
                textWriter.Write("CSoSecVal");
                textWriter.Write(',');
                textWriter.Write("YVal");
                textWriter.Write('\n');
                foreach (Instrument instrument in StaticData.Instruments)
                {
                    textWriter.Write(instrument.InsCode);
                    textWriter.Write(',');
                    textWriter.Write(instrument.InstrumentID);
                    textWriter.Write(',');
                    textWriter.Write(instrument.LatinSymbol);
                    textWriter.Write(',');
                    textWriter.Write(instrument.LatinName);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CompanyCode);
                    textWriter.Write(',');
                    textWriter.Write(instrument.Symbol);
                    textWriter.Write(',');
                    textWriter.Write(instrument.Name);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CIsin);
                    textWriter.Write(',');
                    textWriter.Write(instrument.DEven);
                    textWriter.Write(',');
                    textWriter.Write(instrument.Flow);
                    textWriter.Write(',');
                    textWriter.Write(instrument.LSoc30);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CGdSVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CGrValCot);
                    textWriter.Write(',');
                    textWriter.Write(instrument.YMarNSC);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CComVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CSecVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.CSoSecVal);
                    textWriter.Write(',');
                    textWriter.Write(instrument.YVal);
                    textWriter.Write('\n');
                }
                textWriter.Flush();
            }
        }

        public static async Task<List<string>> SelectedInstrumentsAsync()
        {
            CheckAppFolder();
            List<string> list = new List<string>();
            try
            {
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\selectedInstruments.csv"))))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string text = await streamReader.ReadLineAsync();
                        string[] array = text.Split(',');
                        list.Add(array[0].ToString());
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<int> LastDevenAsync(string insCode)
        {
            CheckAppFolder();
            int result = 0;
            try
            {
                if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "data\\files\\instruments\\" + insCode + ".csv")))
                {
                    return 0;
                }
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\instruments\\" + insCode + ".csv"))))
                {
                    string text = "";
                    while (!streamReader.EndOfStream)
                    {
                        text = await streamReader.ReadLineAsync();
                    }
                    string[] array = text.Split(',');
                    result = Convert.ToInt32(array[1].ToString());
                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static async Task<List<ClosingPriceInfo>> ClosingPricesAsync(long insCode)
        {
            CheckAppFolder();
            List<ClosingPriceInfo> list = new List<ClosingPriceInfo>();
            try
            {
                if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "data\\files\\Instruments\\" + insCode + ".csv")))
                {
                    return list;
                }
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\Instruments\\" + insCode + ".csv"))))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string text = await streamReader.ReadLineAsync();
                        string[] array = text.Split(',');
                        list.Add(new ClosingPriceInfo
                        {
                            InsCode = Convert.ToInt64(array[0].ToString()),
                            DEven = Convert.ToInt32(array[1].ToString()),
                            PClosing = Convert.ToDecimal(array[2].ToString()),
                            PDrCotVal = Convert.ToDecimal(array[3].ToString()),
                            ZTotTran = Convert.ToDecimal(array[4].ToString()),
                            QTotTran5J = Convert.ToDecimal(array[5].ToString()),
                            QTotCap = Convert.ToDecimal(array[6].ToString()),
                            PriceMin = Convert.ToDecimal(array[7].ToString()),
                            PriceMax = Convert.ToDecimal(array[8].ToString()),
                            PriceYesterday = Convert.ToDecimal(array[9].ToString()),
                            PriceFirst = Convert.ToDecimal(array[10].ToString())
                        });
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<int> WriteClosingPricesAsync(List<ClosingPriceInfo> input)
        {
            CheckAppFolder();
            if (input.Count == 0)
            {
                return 0;
            }
            string text = input[0].InsCode.ToString();
            List<ClosingPriceInfo> list = new List<ClosingPriceInfo>();
            List<ClosingPriceInfo> list2 = new List<ClosingPriceInfo>();
            list = await ClosingPricesAsync(Convert.ToInt64(text));
            foreach (ClosingPriceInfo item2 in input)
            {
                list2.Add(item2);
            }
            using (List<ClosingPriceInfo>.Enumerator enumerator2 = list.GetEnumerator())
            {
                ClosingPriceInfo item;
                while (enumerator2.MoveNext())
                {
                    item = enumerator2.Current;
                    if (list2.Find((ClosingPriceInfo p) => p.DEven == item.DEven) == null)
                    {
                        list2.Add(item);
                    }
                }
            }
            list2.Sort((ClosingPriceInfo s1, ClosingPriceInfo s2) => s1.DEven.CompareTo(s2.DEven));
            using (TextWriter textWriter = File.CreateText(Path.Combine(Environment.CurrentDirectory, "data\\files\\instruments\\" + text + ".csv")))
            {
                foreach (ClosingPriceInfo item3 in list2)
                {
                    textWriter.Write(item3.InsCode);
                    textWriter.Write(',');
                    textWriter.Write(item3.DEven);
                    textWriter.Write(',');
                    textWriter.Write(item3.PClosing);
                    textWriter.Write(',');
                    textWriter.Write(item3.PDrCotVal);
                    textWriter.Write(',');
                    textWriter.Write(item3.ZTotTran);
                    textWriter.Write(',');
                    textWriter.Write(item3.QTotTran5J);
                    textWriter.Write(',');
                    textWriter.Write(item3.QTotCap);
                    textWriter.Write(',');
                    textWriter.Write(item3.PriceMin);
                    textWriter.Write(',');
                    textWriter.Write(item3.PriceMax);
                    textWriter.Write(',');
                    textWriter.Write(item3.PriceYesterday);
                    textWriter.Write(',');
                    textWriter.Write(item3.PriceFirst);
                    textWriter.Write('\n');
                }
                await textWriter.FlushAsync();

                return list2[list2.Count - 1].DEven;
            }
        }

        public static async Task WriteOutputFileAsync(Instrument instrument, List<ClosingPriceInfo> cp, bool appendExistingFile)
        {
            CheckAppFolder();
            Settings settings = new Settings();
            string text = settings.StorageLocation;
            if (settings.AdjustPricesCondition == 1 || settings.AdjustPricesCondition == 2)
            {
                text = settings.AdjustedStorageLocation;
            }
            string str = settings.Delimeter.ToString();
            string text2 = "_";
            switch (Convert.ToInt32(settings.FileName))
            {
                case 0:
                    text2 = instrument.CIsin;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text2 += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text2 += "-i";
                        }
                    }
                    break;
                case 1:
                    text2 = instrument.LatinName;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text2 += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text2 += "-i";
                        }
                    }
                    break;
                case 2:
                    text2 = instrument.LatinSymbol;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text2 += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text2 += "-i";
                        }
                    }
                    break;
                case 3:
                    text2 = instrument.Name;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text2 += "-ت";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text2 += "-ا";
                        }
                    }
                    break;
                case 4:
                    text2 = instrument.Symbol;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text2 += "-ت";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text2 += "-ا";
                        }
                    }
                    break;
                default:
                    text2 = instrument.CIsin;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text2 += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text2 += "-i";
                        }
                    }
                    break;
            }
            text2 = text2.Replace('\\', ' ');
            text2 = text2.Replace('/', ' ');
            text2 = text2.Replace('*', ' ');
            text2 = text2.Replace(':', ' ');
            text2 = text2.Replace('>', ' ');
            text2 = text2.Replace('<', ' ');
            text2 = text2.Replace('?', ' ');
            text2 = text2.Replace('|', ' ');
            text2 = text2.Replace('^', ' ');
            text2 = text2.Replace('"', ' ');
            int num = 0;
            if (appendExistingFile)
            {
                if (!File.Exists(text + "\\" + text2 + "." + settings.FileExtension))
                {
                    appendExistingFile = false;
                }
                else
                {
                    int indexOfDate = 0;
                    bool isShamsiDate = false;
                    foreach (ColumnInfo item in StaticData.ColumnsInfo)
                    {
                        if (item.Type == ColumnType.Date && item.Visible)
                        {
                            indexOfDate = item.Index - 1;
                            isShamsiDate = false;
                            break;
                        }
                        if (item.Type == ColumnType.ShamsiDate && item.Visible)
                        {
                            indexOfDate = item.Index;
                            isShamsiDate = true;
                        }
                    }
                    num = await OutputFileLastDevenAsync(instrument, indexOfDate, isShamsiDate);
                }
            }
            List<ColumnInfo> list = await ColumnsInfoAsync();
            Encoding uTF = Encoding.UTF8;
            switch (Convert.ToInt32(settings.Encoding))
            {
                case 2:
                    uTF = Encoding.GetEncoding(1256);
                    break;
                case 0:
                    uTF = Encoding.Unicode;
                    break;
                case 1:
                    uTF = Encoding.UTF8;
                    break;
                default:
                    uTF = Encoding.UTF8;
                    break;
            }
            TextWriter textWriter = new StreamWriter(text + "\\" + text2 + "." + settings.FileExtension, appendExistingFile, uTF);
            list.Sort((ColumnInfo s1, ColumnInfo s2) => s1.Index.CompareTo(s2.Index));
            string text3 = "";
            if (settings.ShowHeaders && num == 0)
            {
                foreach (ColumnInfo item2 in list)
                {
                    if (item2.Visible)
                    {
                        text3 += item2.Header;
                        text3 += str;
                    }
                }
                text3 = text3.Substring(0, text3.Length - 1);
                await textWriter.WriteLineAsync(text3);
            }
            foreach (ClosingPriceInfo item3 in cp)
            {
                if ((appendExistingFile && item3.DEven <= num) || (!settings.ExportDaysWithoutTrade && item3.ZTotTran == 0m))
                {
                    continue;
                }
                text3 = "";
                foreach (ColumnInfo item4 in list)
                {
                    if (!item4.Visible)
                    {
                        continue;
                    }
                    switch (item4.Type)
                    {
                        case ColumnType.ClosingPrice:
                            text3 += item3.PClosing;
                            break;
                        case ColumnType.CompanyCode:
                            text3 += instrument.CompanyCode.ToString();
                            break;
                        case ColumnType.Count:
                            text3 += item3.ZTotTran;
                            break;
                        case ColumnType.Date:
                            text3 += item3.DEven;
                            break;
                        case ColumnType.LastPrice:
                            text3 += item3.PDrCotVal;
                            break;
                        case ColumnType.LatinName:
                            text3 += instrument.LatinName.ToString();
                            if (instrument.YMarNSC != "ID")
                            {
                                if (settings.AdjustPricesCondition == 1)
                                {
                                    text3 += "-a";
                                }
                                else if (settings.AdjustPricesCondition == 2)
                                {
                                    text3 += "-i";
                                }
                            }
                            break;
                        case ColumnType.Name:
                            text3 += instrument.Name.Replace(" ", "_").ToString();
                            if (instrument.YMarNSC != "ID")
                            {
                                if (settings.AdjustPricesCondition == 1)
                                {
                                    text3 += "-ت";
                                }
                                else if (settings.AdjustPricesCondition == 2)
                                {
                                    text3 += "-ا";
                                }
                            }
                            break;
                        case ColumnType.Price:
                            text3 += item3.QTotCap;
                            break;
                        case ColumnType.PriceFirst:
                            text3 += item3.PriceFirst;
                            break;
                        case ColumnType.PriceMax:
                            text3 += item3.PriceMax;
                            break;
                        case ColumnType.PriceMin:
                            text3 += item3.PriceMin;
                            break;
                        case ColumnType.PriceYesterday:
                            text3 += item3.PriceYesterday;
                            break;
                        case ColumnType.ShamsiDate:
                            text3 += Utility.ConvertGregorianIntToJalaliInt(item3.DEven);
                            break;
                        case ColumnType.Symbol:
                            text3 += instrument.Symbol.Replace(" ", "_").ToString();
                            if (instrument.YMarNSC != "ID")
                            {
                                if (settings.AdjustPricesCondition == 1)
                                {
                                    text3 += "-ت";
                                }
                                else if (settings.AdjustPricesCondition == 2)
                                {
                                    text3 += "-ا";
                                }
                            }
                            break;
                        case ColumnType.Volume:
                            text3 += item3.QTotTran5J;
                            break;
                    }
                    text3 += str;
                }
                text3 = text3.Substring(0, text3.Length - 1);
                await textWriter.WriteLineAsync(text3);
            }
            await textWriter.FlushAsync();
            textWriter.Dispose();
        }

        public static void RenameOutputFiles()
        {
            CheckAppFolder();
            Settings settings = new Settings();
            string text = settings.StorageLocation;
            if (settings.AdjustPricesCondition == 1 || settings.AdjustPricesCondition == 2)
            {
                text = settings.AdjustedStorageLocation;
            }
            foreach (Instrument instrument in StaticData.Instruments)
            {
                string text2 = "_";
                switch (Convert.ToInt32(settings.FileName))
                {
                    case 0:
                        text2 = instrument.CIsin;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                    case 1:
                        text2 = instrument.LatinName;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                    case 2:
                        text2 = instrument.LatinSymbol;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                    case 3:
                        text2 = instrument.Name;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-ت";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-ا";
                            }
                        }
                        break;
                    case 4:
                        text2 = instrument.Symbol;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-ت";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-ا";
                            }
                        }
                        break;
                    default:
                        text2 = instrument.CIsin;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                }
                text2 = text2.Replace('\\', ' ');
                text2 = text2.Replace('/', ' ');
                text2 = text2.Replace('*', ' ');
                text2 = text2.Replace(':', ' ');
                text2 = text2.Replace('>', ' ');
                text2 = text2.Replace('<', ' ');
                text2 = text2.Replace('?', ' ');
                text2 = text2.Replace('|', ' ');
                text2 = text2.Replace('^', ' ');
                text2 = text2.Replace('"', ' ');
                if (File.Exists(text + "\\" + text2 + "." + settings.FileExtension))
                {
                    File.Copy(text + "\\" + text2 + "." + settings.FileExtension, text + "\\" + text2 + "[" + DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute.ToString("00") + "-" + DateTime.Now.Second.ToString("00") + "]." + settings.FileExtension);
                    File.Delete(text + "\\" + text2 + "." + settings.FileExtension);
                }
            }
        }

        public static async Task<int> OutputFileLastDevenAsync(Instrument instrument, int indexOfDate, bool isShamsiDate)
        {
            CheckAppFolder();
            int result = 0;
            try
            {
                Settings settings = new Settings();
                string text = settings.StorageLocation;
                if (settings.AdjustPricesCondition == 1 || settings.AdjustPricesCondition == 2)
                {
                    text = settings.AdjustedStorageLocation;
                }
                string text2 = "_";
                switch (Convert.ToInt32(settings.FileName))
                {
                    case 0:
                        text2 = instrument.CIsin;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                    case 1:
                        text2 = instrument.LatinName;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                    case 2:
                        text2 = instrument.LatinSymbol;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                    case 3:
                        text2 = instrument.Name;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-ت";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-ا";
                            }
                        }
                        break;
                    case 4:
                        text2 = instrument.Symbol;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-ت";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-ا";
                            }
                        }
                        break;
                    default:
                        text2 = instrument.CIsin;
                        if (instrument.YMarNSC != "ID")
                        {
                            if (settings.AdjustPricesCondition == 1)
                            {
                                text2 += "-a";
                            }
                            else if (settings.AdjustPricesCondition == 2)
                            {
                                text2 += "-i";
                            }
                        }
                        break;
                }
                text2 = text2.Replace('\\', ' ');
                text2 = text2.Replace('/', ' ');
                text2 = text2.Replace('*', ' ');
                text2 = text2.Replace(':', ' ');
                text2 = text2.Replace('>', ' ');
                text2 = text2.Replace('<', ' ');
                text2 = text2.Replace('?', ' ');
                text2 = text2.Replace('|', ' ');
                text2 = text2.Replace('^', ' ');
                text2 = text2.Replace('"', ' ');
                if (!File.Exists(text + "\\" + text2 + "." + settings.FileExtension))
                {
                    return 0;
                }
                using (StreamReader streamReader = new StreamReader(File.OpenRead(text + "\\" + text2 + "." + settings.FileExtension)))
                {
                    string text3 = "";
                    while (!streamReader.EndOfStream)
                    {
                        text3 = await streamReader.ReadLineAsync();
                    }
                    string[] array = text3.Split(',');
                    if (!isShamsiDate)
                    {
                        result = Convert.ToInt32(array[indexOfDate].ToString());
                        return result;
                    }
                    result = Utility.ConvertJalaliStringToGregorianInt(array[indexOfDate].ToString());
                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static async Task WriteOutputExcel(Instrument instrument, List<ClosingPriceInfo> cp)
        {
            CheckAppFolder();
            Settings settings = new Settings();
            string str = settings.StorageLocation;
            if (settings.AdjustPricesCondition == 1 || settings.AdjustPricesCondition == 2)
            {
                str = settings.AdjustedStorageLocation;
            }
            string text = "_";
            switch (Convert.ToInt32(settings.FileName))
            {
                case 0:
                    text = instrument.CIsin;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text += "-i";
                        }
                    }
                    break;
                case 1:
                    text = instrument.LatinName;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text += "-i";
                        }
                    }
                    break;
                case 2:
                    text = instrument.LatinSymbol;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text += "-i";
                        }
                    }
                    break;
                case 3:
                    text = instrument.Name;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text += "-ت";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text += "-ا";
                        }
                    }
                    break;
                case 4:
                    text = instrument.Symbol;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text += "-ت";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text += "-ا";
                        }
                    }
                    break;
                default:
                    text = instrument.CIsin;
                    if (instrument.YMarNSC != "ID")
                    {
                        if (settings.AdjustPricesCondition == 1)
                        {
                            text += "-a";
                        }
                        else if (settings.AdjustPricesCondition == 2)
                        {
                            text += "-i";
                        }
                    }
                    break;
            }
            text = text.Replace('\\', ' ');
            text = text.Replace('/', ' ');
            text = text.Replace('*', ' ');
            text = text.Replace(':', ' ');
            text = text.Replace('>', ' ');
            text = text.Replace('<', ' ');
            text = text.Replace('?', ' ');
            text = text.Replace('|', ' ');
            text = text.Replace('^', ' ');
            text = text.Replace('"', ' ');
            List<ColumnInfo> list = await ColumnsInfoAsync();
            string file = str + "\\" + text + ".xls";
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet(instrument.InstrumentID);
            list.Sort((ColumnInfo s1, ColumnInfo s2) => s1.Index.CompareTo(s2.Index));
            int num = 0;
            int num2 = 0;
            if (settings.ShowHeaders)
            {
                foreach (ColumnInfo item in list)
                {
                    if (item.Visible)
                    {
                        worksheet.Cells[num, num2] = new Cell(item.Header);
                        num2++;
                    }
                }
                num++;
            }
            foreach (ClosingPriceInfo item2 in cp)
            {
                if (!settings.ExportDaysWithoutTrade && item2.ZTotTran == 0m)
                {
                    continue;
                }
                num2 = 0;
                foreach (ColumnInfo item3 in list)
                {
                    if (!item3.Visible)
                    {
                        continue;
                    }
                    string text2 = "";
                    switch (item3.Type)
                    {
                        case ColumnType.ClosingPrice:
                            worksheet.Cells[num, num2] = new Cell(item2.PClosing.ToString());
                            num2++;
                            break;
                        case ColumnType.CompanyCode:
                            worksheet.Cells[num, num2] = new Cell(instrument.CompanyCode.ToString());
                            num2++;
                            break;
                        case ColumnType.Count:
                            worksheet.Cells[num, num2] = new Cell(item2.ZTotTran.ToString());
                            num2++;
                            break;
                        case ColumnType.Date:
                            worksheet.Cells[num, num2] = new Cell(item2.DEven.ToString());
                            num2++;
                            break;
                        case ColumnType.LastPrice:
                            worksheet.Cells[num, num2] = new Cell(item2.PDrCotVal.ToString());
                            num2++;
                            break;
                        case ColumnType.LatinName:
                            text2 = "";
                            if (instrument.YMarNSC != "ID")
                            {
                                if (settings.AdjustPricesCondition == 1)
                                {
                                    text2 = "-a";
                                }
                                else if (settings.AdjustPricesCondition == 2)
                                {
                                    text2 = "-i";
                                }
                            }
                            worksheet.Cells[num, num2] = new Cell(instrument.LatinName.ToString() + text2);
                            num2++;
                            break;
                        case ColumnType.Name:
                            text2 = "";
                            if (instrument.YMarNSC != "ID")
                            {
                                if (settings.AdjustPricesCondition == 1)
                                {
                                    text2 = "-ت";
                                }
                                else if (settings.AdjustPricesCondition == 2)
                                {
                                    text2 = "-ا";
                                }
                            }
                            worksheet.Cells[num, num2] = new Cell(instrument.Name.ToString() + text2);
                            num2++;
                            break;
                        case ColumnType.Price:
                            worksheet.Cells[num, num2] = new Cell(item2.QTotCap.ToString());
                            num2++;
                            break;
                        case ColumnType.PriceFirst:
                            worksheet.Cells[num, num2] = new Cell(item2.PriceFirst.ToString());
                            num2++;
                            break;
                        case ColumnType.PriceMax:
                            worksheet.Cells[num, num2] = new Cell(item2.PriceMax.ToString());
                            num2++;
                            break;
                        case ColumnType.PriceMin:
                            worksheet.Cells[num, num2] = new Cell(item2.PriceMin.ToString());
                            num2++;
                            break;
                        case ColumnType.PriceYesterday:
                            worksheet.Cells[num, num2] = new Cell(item2.PriceYesterday.ToString());
                            num2++;
                            break;
                        case ColumnType.ShamsiDate:
                            worksheet.Cells[num, num2] = new Cell(Utility.ConvertGregorianIntToJalaliInt(item2.DEven).ToString());
                            num2++;
                            break;
                        case ColumnType.Symbol:
                            text2 = "";
                            if (instrument.YMarNSC != "ID")
                            {
                                if (settings.AdjustPricesCondition == 1)
                                {
                                    text2 = "-ت";
                                }
                                else if (settings.AdjustPricesCondition == 2)
                                {
                                    text2 = "-ا";
                                }
                            }
                            worksheet.Cells[num, num2] = new Cell(instrument.Symbol.ToString() + text2);
                            num2++;
                            break;
                        case ColumnType.Volume:
                            worksheet.Cells[num, num2] = new Cell(item2.QTotTran5J.ToString());
                            num2++;
                            break;
                    }
                }
                num++;
            }
            workbook.Worksheets.Add(worksheet);
            workbook.Save(file);
        }

        public static int LogErrorFile(string message)
        {
            string text = "";
            Settings settings = new Settings();
            string text2 = settings.StorageLocation;
            if (settings.AdjustPricesCondition == 1 || settings.AdjustPricesCondition == 2)
            {
                text2 = settings.AdjustedStorageLocation;
            }
            if (!string.IsNullOrEmpty(text2) && Directory.Exists(text2))
            {
                text = text2;
                using (TextWriter textWriter = File.CreateText(text + "\\error.log"))
                {
                    textWriter.Write(message);
                    textWriter.Flush();
                }
                return 1;
            }
            return -1;
        }

        public static void EraseCurrentFiles()
        {
            CheckAppFolder();
            FileInfo[] files = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "data\\files\\Instruments")).GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                fileInfo.Delete();
            }
            FileInfo fileInfo2 = new FileInfo(Path.Combine(Environment.CurrentDirectory, "data\\files\\Instruments.csv"));
            fileInfo2.Delete();
            fileInfo2.Create();
        }

        public static string ReadVersionFileContent()
        {
            CheckAppFolder();
            string result = "";
            try
            {
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\Version.txt"))))
                {
                    result = streamReader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static void WriteVersionFileContent(string version)
        {
            CheckAppFolder();
            using (TextWriter textWriter = File.CreateText(Path.Combine(Environment.CurrentDirectory, "data\\files\\Version.txt")))
            {
                textWriter.Write(version);
                textWriter.Flush();
            }
        }

        public static async Task<ConcurrentBag<TseShareInfo>> TseSharesAsync()
        {
            CheckAppFolder();
            ConcurrentBag<TseShareInfo> list = new ConcurrentBag<TseShareInfo>();
            try
            {
                using (StreamReader streamReader = new StreamReader(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "data\\files\\TseShares.csv"))))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string text = await streamReader.ReadLineAsync();
                        string[] array = text.Split(',');
                        list.Add(new TseShareInfo
                        {
                            Idn = Convert.ToInt64(array[0].ToString()),
                            InsCode = Convert.ToInt64(array[1].ToString()),
                            DEven = Convert.ToInt32(array[2].ToString()),
                            NumberOfShareNew = Convert.ToDecimal(array[3].ToString()),
                            NumberOfShareOld = Convert.ToDecimal(array[4].ToString())
                        });
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task WriteTseSharesAsync()
        {
            CheckAppFolder();
            using (TextWriter textWriter = File.CreateText(Path.Combine(Environment.CurrentDirectory, "data\\files\\TseShares.csv")))
            {
                foreach (TseShareInfo tseShare in StaticData.TseShares)
                {
                    textWriter.Write(tseShare.Idn);
                    textWriter.Write(',');
                    textWriter.Write(tseShare.InsCode);
                    textWriter.Write(',');
                    textWriter.Write(tseShare.DEven);
                    textWriter.Write(',');
                    textWriter.Write(tseShare.NumberOfShareNew);
                    textWriter.Write(',');
                    textWriter.Write(tseShare.NumberOfShareOld);
                    textWriter.Write('\n');
                }
                await textWriter.FlushAsync();
            }
        }
    }

}
