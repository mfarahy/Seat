using Exir.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public partial class BackstageJob
    {
        public Dictionary<string, object> GetArguments()
        {
            if (String.IsNullOrEmpty(SerializedArgs))
                return new Dictionary<string, object>();

            var d = (Dictionary<string, string>)Serializer.FastFullDeserialize(SerializedArgs);
            var ddic = new Dictionary<string, object>();
            foreach (var entry in d)
            {
                if (entry.Value == "__NULL__")
                    ddic.Add(entry.Key, null);
                else
                {
                    ddic.Add(entry.Key, Serializer.FastFullDeserialize(entry.Value));
                }
            }
            return ddic;
        }

        public string GetExecutionLog()
        {
            if (ExecutionLog == null) return String.Empty;

            using (var mem = new MemoryStream(ExecutionLog))
            using (var zipStream = new GZipStream(mem, CompressionMode.Decompress))
            using (var reader = new StreamReader(zipStream))
            {
                mem.Seek(0, SeekOrigin.Begin);

                return reader.ReadToEnd();
            }
        }
    }
}
