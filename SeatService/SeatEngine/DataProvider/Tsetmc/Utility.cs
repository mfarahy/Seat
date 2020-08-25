// TseClient.Utility
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using SeatService.SeatServiceEngine.DataProvider.Tsetmc;

namespace SeatService.SeatServiceEngine.DataProvider
{

    public class Utility
    {
        public static string Today()
        {
            DateTime now = DateTime.Now;
            PersianCalendar persianCalendar = new PersianCalendar();
            string text = persianCalendar.GetYear(now).ToString();
            string text2 = persianCalendar.GetMonth(now).ToString().PadLeft(2, '0');
            string text3 = persianCalendar.GetDayOfMonth(now).ToString().PadLeft(2, '0');
            return text + "/" + text2 + "/" + text3;
        }

        public static string ConvertDateTimeToJalaliString(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            string text = persianCalendar.GetYear(dateTime).ToString();
            string text2 = persianCalendar.GetMonth(dateTime).ToString().PadLeft(2, '0');
            string text3 = persianCalendar.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0');
            return text + "/" + text2 + "/" + text3;
        }

        public static int ConvertDateTimeToJalaliInt(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            string str = persianCalendar.GetYear(dateTime).ToString();
            string str2 = persianCalendar.GetMonth(dateTime).ToString().PadLeft(2, '0');
            string str3 = persianCalendar.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0');
            return Convert.ToInt32(str + str2 + str3);
        }

        public static DateTime ConvertJalaliStringToDateTime(string input)
        {
            var cDate = new cDate(1);
            cDate.fromJalali(input);
            return cDate.ToDateTime();
        }

        public static int ConvertGregorianIntToJalaliInt(int input)
        {
            string text = input.ToString();
            DateTime dateTime = new DateTime(Convert.ToInt32(text.Substring(0, 4)), Convert.ToInt32(text.Substring(4, 2)), Convert.ToInt32(text.Substring(6, 2)));
            return ConvertDateTimeToJalaliInt(dateTime);
        }

        public static int ConvertJalaliStringToGregorianInt(string input)
        {
            cDate cDate = new cDate(1);
            cDate.fromJalali(input);
            DateTime dateTime = cDate.ToDateTime();
            return Convert.ToInt32(dateTime.Year + dateTime.Month.ToString("00") + dateTime.Day.ToString("00"));
        }

        public static string Compress(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
            {
                gZipStream.Write(bytes, 0, bytes.Length);
            }
            memoryStream.Position = 0L;
            new MemoryStream();
            byte[] array = new byte[memoryStream.Length];
            memoryStream.Read(array, 0, array.Length);
            byte[] array2 = new byte[array.Length + 4];
            Buffer.BlockCopy(array, 0, array2, 4, array.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, array2, 0, 4);
            return Convert.ToBase64String(array2);
        }

        public static string Decompress(string compressedText)
        {
            byte[] array = Convert.FromBase64String(compressedText);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int num = BitConverter.ToInt32(array, 0);
                memoryStream.Write(array, 4, array.Length - 4);
                byte[] array2 = new byte[num];
                memoryStream.Position = 0L;
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(array2, 0, array2.Length);
                }
                return Encoding.UTF8.GetString(array2);
            }
        }

        public static int ConvertDateTimeToGregorianInt(DateTime dateTime)
        {
            return Convert.ToInt32(dateTime.Year + dateTime.Month.ToString("00") + dateTime.Day.ToString("00"));
        }

        public static DateTime ConvertGregorianIntToDateTime(int input)
        {
            string text = input.ToString();
            return new DateTime(Convert.ToInt32(text.Substring(0, 4)), Convert.ToInt32(text.Substring(4, 2)), Convert.ToInt32(text.Substring(6, 2)));
        }
    }

}
