using Gw2_WikiParser.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Pages.Queries.Properties;
using WikiClientLibrary.Sites;

namespace Gw2_WikiParser.Utility
{
    public class WikiWrapper
    {
        private static Lazy<WikiWrapper> _instance = new Lazy<WikiWrapper>(() => new WikiWrapper());
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private RatelimitHandler _ratelimitHandler = new RatelimitHandler(100, nameof(WikiWrapper));

        private WikiClient _client;
        private WikiSite _site;

        public static WikiWrapper Instance { get { return _instance.Value; } }

        private WikiWrapper()
        {
            string wikiUrl = ConfigurationManager.AppSettings["wiki_url"];
            string userAgent = $"Werdes.WikiParser.{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            _client = new WikiClient()
            {
                ClientUserAgent = userAgent
            };

            _site = new WikiSite(_client, wikiUrl);
            _site.Initialization.Wait();

        }

        public async Task<(string, string)> GetPageContent(string title)
        {
            _log.Info($"Retrieving page {title}");
            _ratelimitHandler.Wait();

            WikiPage page = new WikiPage(_site, title);
            await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
            _ratelimitHandler.Set();

            return (page.Title, page.Content);
        }

        public async Task<List<(string, string)>> GetCategoryMembersWithContent(string categoryTitle)
        {
            List<(string, string)> lstMember = new List<(string, string)>();
            List<WikiPage> pages = await GetCategoryMembers(categoryTitle);

            foreach (WikiPage page in pages)
            {
                _ratelimitHandler.Wait();
                await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
                lstMember.Add((page.Title, page.Content));
                _ratelimitHandler.Set();
            }

            return lstMember;
        }

        private async Task<List<WikiPage>> GetCategoryMembers(string categoryTitle, int level = 0)
        {
            List<WikiPage> lstPageTitles = new List<WikiPage>();
            
            _ratelimitHandler.Wait();
            WikiPage catPage = new WikiPage(_site, categoryTitle);
            catPage.RefreshAsync().Wait();
            _ratelimitHandler.Set();

            CategoryInfoPropertyGroup mainCategoryInfo = catPage.GetPropertyGroup<CategoryInfoPropertyGroup>();

            if (mainCategoryInfo != null)
            {
                _log.Info($"Retrieving Category {catPage.Title}: {mainCategoryInfo.MembersCount} pages, {mainCategoryInfo.SubcategoriesCount} categories");
                CategoryMembersGenerator categoryMembersGenerator = new CategoryMembersGenerator(catPage);
                IEnumerable<WikiPage> pages = await categoryMembersGenerator.EnumPagesAsync().ToList();

                foreach (WikiPage page in pages)
                {
                    CategoryInfoPropertyGroup categoryInfo = page.GetPropertyGroup<CategoryInfoPropertyGroup>();
                    FileInfoPropertyGroup fileInfo = page.GetPropertyGroup<FileInfoPropertyGroup>();
                    if (categoryInfo != null)
                    {
                        _log.Info($"{new string('-', level)}{page.Title} has {categoryInfo.MembersCount} pages, {categoryInfo.SubcategoriesCount} categories");
                        lstPageTitles.AddRange(await GetCategoryMembers(page.Title, level + 1));
                    }
                    //no Files
                    else if (fileInfo == null)
                    {
                        //no category
                        lstPageTitles.Add(page);
                    }
                }
            }
            return lstPageTitles.Distinct().ToList();
        }
    }
}
