using Gw2_WikiParser.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
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
        private RdfXmlParser _rdfParser = new RdfXmlParser(RdfXmlParserMode.DOM);

        private WikiClient _client;
        private WikiSite _site;

        public static WikiWrapper Instance
        {
            get => _instance.Value;
        }

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

        /// <summary>
        /// Gets the content of said page
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<(string, string)> GetPageContent(string title)
        {
            _log.Info($"Retrieving page {title}");
            _ratelimitHandler.Wait();

            WikiPage page = new WikiPage(_site, title);
            await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
            _ratelimitHandler.Set();

            return (page.Title, page.Content);
        }

        /// <summary>
        /// Returns a list of pages with content contained in a category
        /// </summary>
        /// <param name="categoryTitle"></param>
        /// <returns></returns>
        public async Task<List<(string, string)>> GetCategoryMembersWithContent(string categoryTitle)
        {
            List<(string, string)> lstMember = new List<(string, string)>();
            List<WikiPage> pages = await GetCategoryMembers(categoryTitle);

            foreach (WikiPage page in pages)
            {
                _ratelimitHandler.Wait();
                await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
                lstMember.Add((page.Title, page.Content));

                _log.Debug($"Retrieving page {page.Title} {pages.IndexOf(page) + 1}/{pages.Count}");
                _ratelimitHandler.Set();
            }

            return lstMember;
        }


        /// <summary>
        /// Returns a list of pages with content and its rdf graph contained in a category
        /// </summary>
        /// <param name="categoryTitle"></param>
        /// <returns></returns>
        public async Task<List<(string, string, Graph)>> GetCategoryMembersWithContentAndRdfGraph(string categoryTitle)
        {
            List<(string, string, Graph)> lstMember = new List<(string, string, Graph)>();
            List<WikiPage> pages = await GetCategoryMembers(categoryTitle);
            pages = pages.Where(x => x.Title.Contains("Revelry Starcake") || x.Title.Contains("Bowl of Chocolate Chip Ice Cream")).ToList();

            foreach (WikiPage page in pages)
            {
                _ratelimitHandler.Wait();
                await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
                
                _log.Debug($"Retrieving page {page.Title} {pages.IndexOf(page) + 1}/{pages.Count}");
                _ratelimitHandler.Set();

                Graph graph = await GetRdfGraph(page.Title);
                lstMember.Add((page.Title, page.Content, graph));
            }

            return lstMember;
        }

        /// <summary>
        /// Retrieves the RDF Graph of a Page
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        private async Task<Graph> GetRdfGraph(string pageName)
        {
            _ratelimitHandler.Wait();
            string url = ConfigurationManager.AppSettings["wiki_rdf_url"] + pageName;
            Graph graph = new Graph();
            using (TextReader reader = new StringReader(new WebClient().DownloadString(url)))
            {
                _rdfParser.Load(graph, reader);
                _ratelimitHandler.Set();
            }

            return graph;
        }

        /// <summary>
        /// Retrieves a list of WikiPages contained in a category
        /// also handles Subcategories
        /// </summary>
        /// <param name="categoryTitle"></param>
        /// <param name="level"></param>
        /// <returns></returns>
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
            return lstPageTitles.GroupBy(x => x.Title).Select(x => x.First()).ToList();
        }
    }
}
