using Gw2_WikiParser.Extensions;
using Gw2Sharp.WebApi.Caching;
using Gw2Sharp.WebApi.V2.Clients;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Gw2_WikiParser.Utility
{
    public class ApiWrapper
    {
        private const int API_BULK_MAX_COUNT = 200;

        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Gw2Sharp.Gw2Client _client = null;

        private RatelimitHandler _ratelimitHandler = new RatelimitHandler(60, nameof(ApiWrapper));

        private static Lazy<ApiWrapper> _instance = new Lazy<ApiWrapper>(() => new ApiWrapper());
        public static ApiWrapper Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private ApiWrapper()
        {
            _client = new Gw2Sharp.Gw2Client(new Gw2Sharp.Connection());
        }

        public Dictionary<string, int> GetItemIdLookup()
        {
            Dictionary<string, int> lookup = new Dictionary<string, int>();

            IItemsClient itemsClient = _client.WebApi.V2.Items;
            List<List<int>> idChunks = itemsClient.IdsAsync().Result.SplitIntoChunks(API_BULK_MAX_COUNT).ToList();
            List<Item> items = new List<Item>();

            
            foreach (List<int> idsChunk in idChunks)
            {
                _log.DebugFormat("Item lookup loading chunk[{0}/{1}], Ratelimit: {2} free calls",
                    idChunks.IndexOf(idsChunk) + 1,
                    idChunks.Count,
                    _ratelimitHandler.GetCurrentFreeCalls());
                _ratelimitHandler.Wait();

                items.AddRangeIfNotNull(itemsClient.ManyAsync(idsChunk).Result);
                lookup.AddRangeIfNotContainsKey(items.Select(x => new KeyValuePair<string, int>(x.Name, x.Id)));
                _ratelimitHandler.Set();
            }
            return lookup;
        }
    }
}
