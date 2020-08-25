using Exir.Framework.Common;
using Exir.Framework.Security.Cryptography;
using SeatDomain.Models;
using SeatDomain.Services;
using System.Web.Mvc;

namespace SeatWebApp.Controllers
{
    [Authorize]
    public class FeedBackController : Controller
    {
        [HttpPost]
        public ActionResult SaveFeedBack(FeedBack feed)
        {
            var queryString = Request.Form["CategoryType"];
            var dic = queryString.Split(',');
            int feedCategroy = -1;
            switch (dic.Length)
            {
                case 1:
                    {
                        if (int.TryParse(dic[0], out feedCategroy))
                        {
                            break;
                        }
                        throw new ExirException("نوع آیتم انتخابی بازخورد موجود نیست");
                    }
                case 2:
                    {
                        if (int.TryParse(dic[0], out feedCategroy))
                        {
                            break;
                        }
                        else if (int.TryParse(dic[1], out feedCategroy))
                        {
                            break;
                        }
                        throw new ExirException("نوع آیتم انتخابی بازخورد موجود نیست");
                    }
                default:
                    {
                        throw new ExirException("نوع آیتم انتخابی بازخورد موجود نیست");
                    }
            }

            feed.CategoryType = feedCategroy;

            var feedBack = StaticServiceFactory.Create<IFeedBackService>();
            feedBack.IgnoreSecurity().Save(feed);
            return Content("OK");
        }

        [HttpGet]
        public ActionResult UnsubscribeMe(string signature)
        {
            var identity = EmbedRsaCryptoService.XorDecrypt(signature);
            var entity_id = int.Parse(identity.Split('-')[1]);
            var feedbackSrv = StaticServiceFactory.Create<IFeedBackService>();
            var feedback = feedbackSrv.GetEntity(entity_id, null);
            feedback.Status = SeatDomain.Constants.FeedbackStates.InvalidResponse;
            feedbackSrv.Save(feedback);
            return Content("OK");
        }
    }
}