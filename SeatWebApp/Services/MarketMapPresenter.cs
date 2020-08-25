using Exir.Framework.Common;
using Exir.Framework.Uie.Adapter;
using Exir.Framework.Uie.Contracts;
using Exir.Framework.Uie.Contracts.Support;
using Newtonsoft.Json;
using SeatDomain.Models;
using SeatDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeatWebApp.Services
{
    public class MarketMapPresenter : VirtualPresenter
    {
        public MarketMapPresenter() : base(typeof(MarketMapModel))
        {
        }
        public override IModel CreateNew()
        {
            return new MarketMapModel(new MarketMap());
        }
        public override IModel Get(string key)
        {
            MarketMap map = new MarketMap();

            var catSrv = StaticServiceFactory.Create<IInstrumentCategoryService>();
            var insSrv = StaticServiceFactory.Create<IInstrumentService>();
            var liveSrv = StaticServiceFactory.Create<ILiveInstDataService>();

            insSrv.MakeFriend(catSrv);
            liveSrv.MakeFriend(catSrv);

            var cats = catSrv.GetAll().Where(x => x.CSecVal != "X1").ToList();
            var instruments = insSrv.GetAll().Where(x => x.InstrumentType == SeatDomain.Constants.InstrumentTypes.Saham);

            DateTime date = DateTime.Now.Date;
            var hSrv = ServiceFactory.Create<IHolidayService>();
            if (!hSrv.IsWorkingDay(DateTime.Now))
            {
                date = liveSrv.GetDefaultQuery()
                    .OrderByDescending(x => x.DEven)
                    .Select(x => x.DEven)
                    .FirstOrDefault().Date;
            }
            var lives = liveSrv.GetDefaultQuery()
                .Where(x => x.DEven == date)
                .ToList();

            var data = new List<MarketNode>();

            foreach (var cat in cats)
            {
                MarketNode section = new MarketNode()
                {
                    Id = cat.CSecVal,
                    name = cat.Name,
                    data = new Dictionary<string, object>()
                };
                var children = new List<MarketNode>();
                var childs = instruments.Where(x => x.CSecVal == cat.CSecVal).ToList();
                long total_vol = 0;
                foreach (var ins in childs)
                {
                    var live = lives.Where(x => x.InsCode == ins.InsCode).FirstOrDefault();
                    if (live == null) continue;
                    var percent = (live.PdrCotVal- live.PriceYesterday) * 1.0 / live.PriceYesterday * 100;
                    total_vol += live.Vol;
                    children.Add(new MarketNode
                    {
                        Id = ins.LatinSymbol,
                        name = ins.Symbol,
                        data = new Dictionary<string, object> { { "$area", live.Vol }, { "$color", getColor(percent) } }
                    });
                }
                if (total_vol > 0)
                {
                    data.Add(section);
                    section.children = children.ToArray();
                    section.data.Add("$area", total_vol);
                }
            }

            MarketMap result = new MarketMap()
            {
                Data = new MarketNode
                {
                    children = data.ToArray(),
                    Id = "root"
                }
            };


            return new MarketMapModel(result);
        }
        private static string getColor(double percent)
        {
            if (percent >= 4)
                return "#139D52";
            if (percent >= 1.25)
                return "#207151";
            if (percent >= -1.25)
                return "#3F4553";
            if (percent > -4)
                return "#853447";
            return "#C7203A";
        }

    }
    public class MarketMapModel : EntityViewModel<MarketMap>
    {
        public MarketMapModel(MarketMap entity) : base(entity)
        {
        }

        public MarketMapModel(object obj, string keyProperty, Type keyType, string versionProperty, bool isKeyIdentity) : base(obj, keyProperty, keyType, versionProperty, isKeyIdentity)
        {
        }
    }

    public class MarketMap : VirtualEntityBase<int>
    {
        [JsonProperty]
        public MarketNode Data { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MarketNode
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public Dictionary<string, object> data { get; set; }
        [JsonProperty]
        public MarketNode[] children { get; set; }

    }
}