using Newtonsoft.Json;
using System.Collections.Generic;

namespace SeatService.SeatServiceEngine.DataProvider
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SuperVision
    {
        [JsonProperty("UnderSupervision")]
        public int UnderSupervision { get; set; }
        [JsonProperty("AdditionalInfo")]
        public string AdditionalInfo { get; set; }
        [JsonProperty("Reasons")]
        public List<string> Reasons { get; set; }

    }

    public class Letter
    {
        [JsonProperty("SuperVision")]
        public SuperVision SuperVision { get; set; }
        [JsonProperty("TracingNo")]
        public int TracingNo { get; set; }
        [JsonProperty("Symbol")]
        public string Symbol { get; set; }
        [JsonProperty("CompanyName")]
        public string CompanyName { get; set; }
        [JsonProperty("UnderSupervision")]
        public int UnderSupervision { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("LetterCode")]
        public string LetterCode { get; set; }
        [JsonProperty("SentDateTime")]
        public string SentDateTime { get; set; }
        [JsonProperty("PublishDateTime")]
        public string PublishDateTime { get; set; }
        [JsonProperty("HasHtml")]
        public bool HasHtml { get; set; }
        [JsonProperty("Url")]
        public string Url { get; set; }
        [JsonProperty("HasExcel")]
        public bool HasExcel { get; set; }
        [JsonProperty("HasPdf")]
        public bool HasPdf { get; set; }
        [JsonProperty("HasXbrl")]
        public bool HasXbrl { get; set; }
        [JsonProperty("HasAttachment")]
        public bool HasAttachment { get; set; }
        [JsonProperty("AttachmentUrl")]
        public string AttachmentUrl { get; set; }
        [JsonProperty("PdfUrl")]
        public string PdfUrl { get; set; }
        [JsonProperty("ExcelUrl")]
        public string ExcelUrl { get; set; }
        [JsonProperty("XbrlUrl")]
        public string XbrlUrl { get; set; }
        [JsonProperty("TedanUrl")]
        public string TedanUrl { get; set; }

    }

    public class CodalSearchResponse
    {
        [JsonProperty("Total")]
        public int Total { get; set; }
        [JsonProperty("Page")]
        public int Page { get; set; }
        [JsonProperty("Letters")]
        public List<Letter> Letters { get; set; }

    }


}
