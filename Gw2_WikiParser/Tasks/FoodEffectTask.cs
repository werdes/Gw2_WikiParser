using Gw2_WikiParser.Exceptions;
using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Gw2_WikiParser.Model.Output.FoodEffectTask;
using Gw2_WikiParser.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace Gw2_WikiParser.Tasks
{
    public class FoodEffectTask : IWikiTask
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string[] _categories;

        public FoodEffectTask(string[] categories)
        {
            _categories = categories;
        }

        public async Task<bool> Run()
        {
            bool wasSuccessful = false;
            List<Food> foods = new List<Food>();
            List<string> lstErrorPageNames = new List<string>();
            try
            {
                List<(string, string, RdfGraphContainer)> lstPages = new List<(string, string, RdfGraphContainer)>();
                foreach (string category in _categories)
                {
                    lstPages.AddRange(await WikiWrapper.Instance.GetCategoryMembersWithContentAndRdfGraph(category));
                }

                foreach ((string pageTitle, string pageContent, RdfGraphContainer rdfGraph) in lstPages)
                {
                    _log.Info($"Parsing {pageTitle}");
                    Food food = ParseFood(pageTitle, pageContent, rdfGraph);
                    if(food != null)
                    {
                        foods.Add(food);
                    }
                    else
                    {
                        lstErrorPageNames.Add(pageTitle);
                    }
                }


                WriteOuput(foods);
                File.WriteAllLines("food_errors.txt", lstErrorPageNames.ToArray());
                wasSuccessful = true;


            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return wasSuccessful;
        }

        /// <summary>
        /// Parses the page for food
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="pageContent"></param>
        /// <param name="graphContainer"></param>
        /// <returns></returns>
        private Food ParseFood(string pageTitle, string pageContent, RdfGraphContainer graphContainer)
        {
            Food food = null;
            int id;
            try
            {
                Triple tripleNourishmentBonus = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_nourishment_bonus"))).FirstOrDefault();
                Triple tripleId = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_game_id"))).FirstOrDefault();

                //Page RDF information has Nourishment Bonus and ID
                if (tripleNourishmentBonus != null &&
                    tripleNourishmentBonus.Object is LiteralNode &&
                    tripleId != null &&
                    tripleId.Object is LiteralNode &&
                    int.TryParse(((LiteralNode)tripleId.Object).Value, out id))
                {
                    string[] nourishmentBonusLines = ((LiteralNode)tripleNourishmentBonus.Object).Value.Split("<br>", StringSplitOptions.RemoveEmptyEntries);
                    food = new Food()
                    {
                        Id = id,
                        Name = pageTitle
                    };

                    foreach (string line in nourishmentBonusLines)
                    {
                        FoodEffect effect = FoodEffect.GetEffect(line);
                        food.Effects.Add(effect);
                    }

                }
                else
                {
                    _log.Warn($"{pageTitle}: RDF does not contain Nourishment or ID");
                }
            }
            catch(UnmatchedFoodEffectException ex)
            {
                _log.Warn(ex);
                food = null;
            }

            return food;
        }

        /// <summary>
        /// Writes the output files (separated by ;, from AppConfig key bulk_output_files)
        /// </summary>
        /// <param name="foods"></param>
        /// <returns></returns>
        private bool WriteOuput(List<Food> foods)
        {
            bool wasSuccessful = true;
            string[] paths = ConfigurationManager.AppSettings["food_effects_output_files"].Split(';');

            foreach (string path in paths)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(foods, Formatting.Indented), Encoding.UTF8);
                }
                catch (IOException ex)
                {
                    _log.Error(ex);
                    wasSuccessful = false;
                }
            }

            return wasSuccessful;
        }

        public string GetProperties()
        {
            return _categories.Join(", ");
        }

    }
}
