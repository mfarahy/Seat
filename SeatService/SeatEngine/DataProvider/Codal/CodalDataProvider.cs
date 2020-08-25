using Exir.Framework.Common;
using Newtonsoft.Json;
using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider
{
    public class CodalDataProvider : DataProvider, ICodalDataProvider
    {
        public CodalDataProvider() : base("https://search.codal.ir")
        {
        }

        public CodalMessage Last { get; set; }
        public async Task<List<CodalMessage>> GetNewMessages()
        {
            bool stop = false;
            List<CodalMessage> result = new List<CodalMessage>();
            for (int page = 1; page <= 10 && !stop; ++page)
            {
                var codalResult = await try_do_async(async () =>
                {
                    var response = await Http.GetAsync(String.Format("/api/search/v2/q?&Audited=true&AuditorRef=-1&Category=-1&Childs=true&CompanyState=-1&CompanyType=-1&Consolidatable=true&IsNotAudited=false&Length=-1&LetterType=-1&Mains=true&NotAudited=true&NotConsolidatable=true&PageNumber={0}&Publisher=false&TracingNo=-1&search=false", page));
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    json = json.ReplacePersianNumbers();
                    return JsonConvert.DeserializeObject<CodalSearchResponse>(json);
                }, 3);

                if (codalResult.Letters != null)
                    for (int i = 0; i < codalResult.Letters.Count; ++i)
                    {
                        var letter = codalResult.Letters[i];
                        if (Last != null && letter.TracingNo == Last.TracingNo)
                        {
                            stop = true;
                            break;
                        }
                        else
                        {
                            var msg = new CodalMessage
                            {
                                TracingNo = letter.TracingNo,
                                CompanyName = letter.CompanyName,
                                PublishDateTime = Typing.ChangeType<DateTime>(letter.PublishDateTime),
                                SentDateTime = Typing.ChangeType<DateTime>(letter.SentDateTime),
                                Symbol = letter.Symbol,
                                Title = letter.Title
                            };
                            result.Add(msg);
                        }
                    }
            }
            if (result.Count > 0)
            {
                Last = result[0];
                logger.InfoFormat("successfuly {0} new codal messages was detected.", result.Count);
            }

            return result;
        }
    }
}
