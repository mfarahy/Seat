using Exir.Framework.Common;
using SeatDomain.Models;
using SeatDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public partial interface IShortLinkService: IBaseOfService<ShortLink, IShortLinkService> 
    {
        string Refer(string code);
        string Register(string link, string title, bool checkExists);
    }

    [IgnoreT4Template]
    public partial class ShortLinkService : BaseOfService<ShortLink, IShortLinkService>, IShortLinkService
{
    protected new IShortLinkService This { get { return This<IShortLinkService>(); } }
    public ShortLinkService(IRepository<ShortLink> repository) : base(repository)
    {
    }

    public string Refer(string code)
        {
            var entry = IgnoreSecurity()
                   .GetDefaultQuery()
                   .Where(x => x.Code == code)
                   .FirstOrDefault();

            if (entry == null)
                return null;

            Repository.BulkUpdate(false, x => x.ShortLinkPK == entry.ShortLinkPK, x => new ShortLink { ReferCount = x.ReferCount + 1 });

            return entry.OrginalLink;
        }

        [Restfull]

        public override object Save(ShortLink entity)
        {
            if (!entity.HasKey())
                entity.Code = Register(entity.OrginalLink, entity.Title, false);
            else
                base.Save(entity);
            return entity;
        }


        private readonly char[] _base = { 'g', 'P', 'V', '9', 'A', '4', 'C', 'y', 'K', 'R', '1', 't', '8', 'X', 's', 'F', 'e', 'c', 'f', 'L', 'm',
            'T', 'I', 'U', '7', '6', 'r', '2', 'j', 'd', 'h', 'D', '0', 'p', 'l', 'a', 'M', 'u', 'x', '3', 'O', 'W', 'k', '5', 'Y', 'E', 'Q', 'G',
            'H', 'S', 'w', 'n', 'i', 'z', 'b', 'v', 'Z', 'J', 'N', 'B', 'q', 'o', };
        public string Register(string link, string title, bool checkExists)
        {
            if (checkExists)
            {
                var existing_link = IgnoreSecurity()
                    .GetDefaultQuery()
                    .Where(x => x.OrginalLink == link)
                    .FirstOrDefault();

                if (existing_link != null)
                    return existing_link.Code;
            }

            var entry = new ShortLink()
            {
                Title = title,
                OrginalLink = link,
                Code = Guid.NewGuid().ToString(),
                CreateDate = DateTime.Now,
                ReferCount = 0
            };
            IgnoreSecurity();
            base.Save(entry);

            var number = entry.ShortLinkPK;
            StringBuilder sb = new StringBuilder();
            while (number > _base.Length)
            {
                var mod = number % _base.Length;
                sb.Append(_base[mod]);
                number = number / _base.Length;
            }
            sb.Append(_base[number]);

            entry.StartTracking();
            entry.Code = sb.ToString();
            IgnoreSecurity();
            base.Save(entry);

            return entry.Code;
        }
    }
}
