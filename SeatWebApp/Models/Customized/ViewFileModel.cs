using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeatWebApp.Models
{
    public class ViewFileModel
    {
        public string Command { get; set; }
        public string PkValue { get; set; }
        public string Service { get; set; }
        public string Property { get; set; }
        public bool IsRaw{ get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
    }

    public class ViewThumbnailModel: ViewFileModel
    {
        public string Extension { get; set; }
        public Guid Key { get; set; }
        public int MaxWith { get; set; }
        public int MaxHeight { get; set; }
        public DateTime Audit_LastModifyDate { get; set; }
    }
}