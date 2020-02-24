using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Gw2_WikiParser.Model.Output.BulkIngredientTask;
using Gw2_WikiParser.Utility;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace Gw2_WikiParser.Tasks
{
    public class BulkIngredientTask : IWikiTask
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string[] _invalidPhrases = new string[]
        {
            "status = historical",
            "status = discontinued"
        };

        private readonly Dictionary<string, int> _itemLookup = null;
        private string[] _categories;

        public BulkIngredientTask(string[] categories)
        {
            _categories = categories;
            //_itemLookup = ApiWrapper.Instance.GetItemIdLookup();
        }


        public async Task<bool> Run()
        {
            _log.Info($"Running Bulk Ingredient task for {_categories.Join(", ")}");
            bool wasSuccessful = false;

            try
            {
                List<(string, string)> lstCategoryPages = new List<(string, string)>();

                foreach (string category in _categories)
                {
                    lstCategoryPages.AddRange(WikiWrapper.Instance.GetCategoryMembersWithContent(category).Result);
                }

                List<BulkItem> items = await GetItems(lstCategoryPages);

                if (items != null &&
                    items.Count > 0)
                {
                    wasSuccessful = WriteOuput(items);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return wasSuccessful;
        }

        /// <summary>
        /// Returns the items from the submitted pages
        /// </summary>
        /// <param name="lstCategoryPages"></param>
        /// <returns></returns>
        private async Task<List<BulkItem>> GetItems(List<(string, string)> lstCategoryPages)
        {
            List<BulkItem> bulkItems = new List<BulkItem>();
            foreach ((string title, string bulkContent) in lstCategoryPages)
            {
                if (!bulkContent.ContainsAny(_invalidPhrases, StringComparison.OrdinalIgnoreCase) &&
                    bulkContent.Contains("{{contains", StringComparison.OrdinalIgnoreCase))
                {

                    string lineBulkItemId = Regex.Match(bulkContent, "(\\| *id *= *).*\\n").Value.Replace(" ", "");

                    string lineBulkContent = Regex.Match(bulkContent, "(\\{\\{contains).*\\n").Value;
                    string outputItemName = lineBulkContent.Split('|')[1].Split("}}")[0];

                    (string outputItemPageName, string outputItemPageContent) = await WikiWrapper.Instance.GetPageContent(outputItemName);
                    string lineOutputItemId = Regex.Match(outputItemPageContent, "(\\| *id *= *).*\\n").Value.Replace(" ", "");

                    int outputItemQuantity = 0;
                    int outputItemId = 0;
                    int bulkItemId = 0;

                    if (int.TryParse(Regex.Match(lineBulkContent, "\\([0-9]+\\)").Value.Replace("(", "").Replace(")", ""), out outputItemQuantity) &&
                        int.TryParse(Regex.Match(lineOutputItemId, "([0-9]+)").Value, out outputItemId) &&
                        int.TryParse(Regex.Match(lineBulkItemId, "([0-9]+)").Value, out bulkItemId))
                    {
                        bulkItems.Add(new BulkItem()
                        {
                            BulkItemId = bulkItemId,
                            BulkItemName = title,
                            OutputItemId = outputItemId,
                            OutputItemQuantity = outputItemQuantity,
                            OutputItemName = outputItemName
                        });

                    }
                }
            }
            return bulkItems;
        }

        /// <summary>
        /// Writes the output files (separated by ;, from AppConfig key bulk_output_files)
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool WriteOuput(List<BulkItem> items)
        {
            bool wasSuccessful = true;
            string[] paths = ConfigurationManager.AppSettings["bulk_output_files"].Split(';');

            foreach (string path in paths)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(items, Formatting.Indented), Encoding.UTF8);
                }
                catch (IOException ex)
                {
                    _log.Error(ex);
                    wasSuccessful = false;
                }
            }

            return wasSuccessful;
        }

        /// <summary>
        /// Returns the Categories for displaying
        /// </summary>
        /// <returns></returns>
        public string GetProperties()
        {
            return _categories.Join(", ");
        }
    }
}
