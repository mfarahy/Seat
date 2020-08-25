using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SeatDomain.Helpers
{
    public static class FileHelper
    {
        public static List<FileInfo> GetDeserializedFiles(this byte[] file)
        {
            Dictionary<string, object>[] dic;
            using (var mem = new MemoryStream(file))
            {
                IFormatter formatter = new BinaryFormatter();
                dic = (Dictionary<string, object>[])formatter.Deserialize(mem);
            }
            var files = new List<FileInfo>();

            for (var index = 0; index < dic.Length; index++)
            {
                var item = new FileInfo
                {
                    Content = (byte[]) dic[index]["Content"],
                    FileName = (string) dic[index]["FileName"],
                    Extension = (string) dic[index]["Extension"],
                    Guid = (Guid) dic[index]["Guid"],
                    RegistrationDt = (DateTime) dic[index]["RegistrationDt"],
                    Tooltip = (string) dic[index]["Tooltip"]
                };
                item.ExtensionTypeCode  = item.Extension.GetFileExtensionTypeCode();
                files.Add(item);
            }
            return files;
        }
        public static byte GetFileExtensionTypeCode(this string extension)
        {
            var extention = Constants.ExtentionTypes.Unknown;
            if (extension.Equals(Constants.FileExtentions.JPG, StringComparison.InvariantCultureIgnoreCase))
                extention = Constants.ExtentionTypes.JPG;
            else if (extension.Equals(Constants.FileExtentions.JPEG, StringComparison.InvariantCultureIgnoreCase))
                extention = Constants.ExtentionTypes.JPEG;
            else if (extension.Equals(Constants.FileExtentions.PNG, StringComparison.InvariantCultureIgnoreCase))
                extention = Constants.ExtentionTypes.PNG;
            else if (extension.Equals(Constants.FileExtentions.PDF, StringComparison.InvariantCultureIgnoreCase))
                extention = Constants.ExtentionTypes.PDF;
            return extention;
        }

    }

    public class FileInfo
    {
        public byte[] Content { get; set; }
        public string Extension { get; set; }
        public byte? ExtensionTypeCode { get; set; }
        public string FileName { get; set; }
        public Guid  Guid { get; set; }
        public DateTime? RegistrationDt { get; set; }
        public string Tooltip { get; set; }
    }
}
