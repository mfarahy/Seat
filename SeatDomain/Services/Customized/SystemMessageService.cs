using Exir.Framework.Common;
using Exir.Framework.Common.Caching;
using SeatDomain.Models;
using System;
using System.Linq;

namespace SeatDomain.Services
{
    public partial interface ISystemMessageService
    {
        SystemMessage[] GetByCode(string code, string media, string culture);
        string GetByCode(string code);
        SystemMessage GetEntityByCode(string code);
        string[] GetValidPrefixes(string[] prefixes, string culture);
        string GetTextMessage(string code, object parameters);
        string GetRawContent(string content, object parameters);
    }

    public partial class SystemMessageService
    {
        public SystemMessageService(IRepository<SystemMessage> repository, IReadOnlyRepository<SystemMessage> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }
        [CacheableEntityInvalidator(CacheName = CacheConstants.InMemoryCache)]
        public override object Save(SystemMessage entity)
        {
            if (entity.Media == Constants.SystemMessage.Media.Sms)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(entity.Text);
                entity.Text = doc.DocumentNode.InnerText;
            }

            return base.Save(entity);
        }

        //[Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public SystemMessage[] GetByCode(string code, string media, string culture)
        {
            return This.GetAll()
                .Where(x => x.Code == code && (String.IsNullOrEmpty(media) || x.Media == media) && (string.IsNullOrEmpty(culture) || culture.Equals(x.Culture, StringComparison.CurrentCultureIgnoreCase)) && x.IsEnable)
                .ToArray();
        }

        public string GetTextMessage(string code, object parameters)
        {
            var messages = This.IgnoreSecurity().GetDefaultQuery().Where(e => e.Code == code);
            if (!messages.Any()) return "";
            var msg = messages.FirstOrDefault();
            if (msg == null || string.IsNullOrEmpty(msg.Text)) return "";
            return GetRawContent(msg.Text, parameters);

        }
        public string GetRawContent(string content, object parameters)
        {
            if (string.IsNullOrEmpty(content)) return "";
            if (parameters == null) return content;
            bool hasParameter = content.IndexOf("[[") >= 0;
            if (!hasParameter) return content;
            return StringFormatUtility.ReplaceParams(content, parameters, "[[", "]]");

        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public string[] GetValidPrefixes(string[] prefixes, string culture)
        {
            var messages = This.GetAll();
            return prefixes
                .Where(y => messages.Any(x => x.Code.StartsWith(y) &&
                (string.IsNullOrEmpty(culture) || culture.Equals(x.Culture, StringComparison.CurrentCultureIgnoreCase)) && x.IsEnable))
                .ToArray();
        }


        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public string GetByCode(string code)
        {
            return This.GetAll()
                .Where(x => x.Code == code)
                .Select(x => x.Text)
                .FirstOrDefault() ?? "";
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public SystemMessage GetEntityByCode(string code)
        {
            return This.GetAll().FirstOrDefault(x => x.Code == code);
        }

       
    }

}
