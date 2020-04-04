using Gw2_WikiParser.Extensions;
using Gw2Sharp.WebApi.Caching;
using Gw2Sharp.WebApi.V2.Clients;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2_WikiParser.Utility
{
    public class ApiWrapper
    {
        private const int API_BULK_MAX_COUNT = 200;

        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Gw2Sharp.Gw2Client _client = null;
        private Gw2Sharp.IConnection _connection = null;
        private int _version;

        private RatelimitHandler _ratelimitHandler = new RatelimitHandler(400, nameof(ApiWrapper));

        private static Lazy<ApiWrapper> _instance = new Lazy<ApiWrapper>(() => new ApiWrapper());
        public static ApiWrapper Instance
        {
            get => _instance.Value;
        }

        private ApiWrapper()
        {
            _version = GetBuildId();
            _connection = new Gw2Sharp.Connection()
            {
                //CacheMethod = new ArchiveCacheMethod(ConfigurationManager.AppSettings["api_cache"].Format(_version))
                CacheMethod = new MemoryCacheMethod()
            };

            _client = new Gw2Sharp.Gw2Client(_connection);
        }

        /// <summary>
        /// Returns a Dictionary with all items
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetItemIdLookup()
        {
            Dictionary<string, int> lookup = new Dictionary<string, int>();

            IItemsClient itemsClient = _client.WebApi.V2.Items;
            List<List<int>> idChunks = itemsClient.IdsAsync().Result.SplitIntoChunks(API_BULK_MAX_COUNT).Take(1).ToList();
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

            _client.Dispose();
            return lookup;

        }

        public async Task<string> GetSingleIngredientNameForRecipeId(int recipeId)
        {
            Recipe recipe = await _client.WebApi.V2.Recipes.GetAsync(recipeId);
            if(recipe != null &&
               recipe.Ingredients.Count == 1)
            {
                Item singleIngredient = await _client.WebApi.V2.Items.GetAsync(recipe.Ingredients.First().ItemId);

                if(singleIngredient != null)
                {
                    return singleIngredient.Name;
                }
            }

            return null;
        }

        public int GetBuildId()
        {
            IBuildClient buildClient = new Gw2Sharp.Gw2Client().WebApi.V2.Build;
            return buildClient.GetAsync().Result.Id;
        }

    }
}
