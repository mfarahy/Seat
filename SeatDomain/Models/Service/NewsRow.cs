using System;
using Exir.Framework.Common;

namespace SeatDomain.Models
{
    public class NewsRow : VirtualEntityBase
    {
        public NewsRow() : base(new BaseField()
        {
            Name = nameof(NewsPk),
            PropertyType = typeof(int),
            Kind = FieldKinds.PrimaryKey
        })
        { }


        public int NewsPk { get; set; }
        public string Subject { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }
        public int CategoryPk { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? NewsArchiveDate { get; set; }
        public DateTime? NewsExpireDate { get; set; }
        public DateTime? Audit_CreateDate { get; set; }
        public string Audit_CreatorUserName { get; set; }
        public string Audit_CreatorIP { get; set; }
        public DateTime Audit_LastModifyDate { get; set; }
        public string Audit_LastModifierUserName { get; set; }
        public string Audit_LastModifierIP { get; set; }
        public string Metadata { get; set; }
        public byte[] Image { get; set; }
        public int? SortNumber { get; set; }
        public string CategoryTitle { get; set; }
        
    }
}