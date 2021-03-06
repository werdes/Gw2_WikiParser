﻿using Gw2_WikiParser.Extensions;
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
using WikiClientLibrary;
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
        private CacheHandler<(string, string, RdfGraphContainer, WikiPage)> _cache = new Utility.CacheHandler<(string, string, RdfGraphContainer, WikiPage)>(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private CacheHandler<string> _cacheUrls = new CacheHandler<string>(MethodBase.GetCurrentMethod().DeclaringType.Name + "_url");

        private WikiClient _client;
        private WikiSite _site;

        public static WikiWrapper Instance
        {
            get => _instance.Value;
        }

        private WikiWrapper()
        {
            string wikiUrl = ConfigurationManager.AppSettings["wiki_api_url"];
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
        public async Task<(string, string)> GetPageContent(string title, bool caching = true)
        {
            _log.Info($"Retrieving page {title}");
            _ratelimitHandler.Wait();

            if (_cache.Contains(title) && caching)
            {
                (string pageTitle, string content, RdfGraphContainer rdfGraph, WikiPage wikiPage) = _cache.Get(title);
                if (!string.IsNullOrEmpty(pageTitle) && !string.IsNullOrEmpty(content))
                {
                    return (pageTitle, content);
                }
            }

            WikiPage page = new WikiPage(_site, title);

            await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);

            _ratelimitHandler.Set();
            _cache.Set(title, (page.Title, page.Content, null, page));

            return (page.Title, page.Content);
        }


        /// <summary>
        /// Gets the said page
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public string GetPageUrl(string title)
        {
            _log.Info($"Retrieving page url for {title}");

            if (_cacheUrls.Contains(title))
            {
                return _cacheUrls.Get(title);
            }
            else
            {

                _ratelimitHandler.Wait();
                WikiLink link = WikiLink.Parse(_site, title);
                _cacheUrls.Set(title, link.TargetUrl);
                _ratelimitHandler.Set();
                return link.TargetUrl;
            }
        }


        /// <summary>
        /// Returns a list of pages with content contained in a category
        /// </summary>
        /// <param name="categoryTitle"></param>
        /// <returns></returns>
        public async Task<List<(string, string)>> GetCategoryMembersWithContent(string categoryTitle, bool caching = true)
        {
            List<(string, string)> lstMember = new List<(string, string)>();
            List<WikiPage> pages = await GetCategoryMembers(categoryTitle);


            foreach (WikiPage page in pages)
            {
                if (_cache.Contains(page.Title) && caching)
                {
                    (string title, string content, RdfGraphContainer rdfGraph, WikiPage wikiPage) = _cache.Get(page.Title);
                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(content))
                    {
                        lstMember.Add((title, content));
                        continue;
                    }
                }

                _ratelimitHandler.Wait();
                await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);
                lstMember.Add((page.Title, page.Content));
                _cache.Set(page.Title, (page.Title, page.Content, null, null));

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
        public async Task<List<(string, string, RdfGraphContainer)>> GetCategoryMembersWithContentAndRdfGraph(string categoryTitle, bool caching = true)
        {
            List<(string, string, RdfGraphContainer)> lstMember = new List<(string, string, RdfGraphContainer)>();
            List<WikiPage> pages = await GetCategoryMembers(categoryTitle);

            foreach (WikiPage page in pages)
            {
                if (_cache.Contains(page.Title) && caching)
                {
                    (string title, string content, RdfGraphContainer rdfGraph, WikiPage wikiPage) = _cache.Get(page.Title);
                    if (!string.IsNullOrEmpty(title) &&
                        !string.IsNullOrEmpty(content) &&
                        rdfGraph != null)
                    {
                        lstMember.Add((title, content, rdfGraph));
                        continue;
                    }
                }

                _ratelimitHandler.Wait();
                await page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects);

                _log.Debug($"Retrieving page {page.Title} {pages.IndexOf(page) + 1}/{pages.Count}");
                _ratelimitHandler.Set();

                RdfGraphContainer graph = GetRdfGraph(page.Title);
                lstMember.Add((page.Title, page.Content, graph));

                _cache.Set(page.Title, (page.Title, page.Content, graph, null));
            }

            return lstMember;
        }

        /// <summary>
        /// Retrieves the RDF Graph of a Page
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public RdfGraphContainer GetRdfGraph(string pageName)
        {
            _ratelimitHandler.Wait();
            string url = ConfigurationManager.AppSettings["wiki_rdf_url"] + pageName;
            RdfGraphContainer graphContainer = new RdfGraphContainer(new WebClient().DownloadString(url));

            return graphContainer;
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
                List<WikiPage> pages = await categoryMembersGenerator.EnumPagesAsync().ToList();

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
