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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VDS.RDF;

namespace Gw2_WikiParser.Tasks
{
    public class FoodEffectTask : IWikiTask
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string[] _categories;
        private readonly string _errorFile = Path.Combine(ConfigurationManager.AppSettings["food_effects_error_dir"], "unmatched_effects.txt");
        private readonly string _allEffectsFile = Path.Combine(ConfigurationManager.AppSettings["food_effects_error_dir"], "all_effects.txt");

        public FoodEffectTask(string[] categories)
        {
            _categories = categories;

            if (File.Exists(_errorFile))
                File.Delete(_errorFile);

            if (File.Exists(_allEffectsFile))
                File.Delete(_allEffectsFile);
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
                    Food food = await ParseFood(pageTitle, pageContent, rdfGraph);
                    if (food != null)
                    {
                        foods.Add(food);
                    }
                    else
                    {
                        _log.Warn($"{pageTitle} food could not be crawled");
                    }
                }


                WriteOuput(foods);
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
        private async Task<Food> ParseFood(string pageTitle, string pageContent, RdfGraphContainer graphContainer)
        {
            Food food = null;
            int id;
            try
            {
                Triple tripleNourishmentBonus = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_nourishment_bonus"))).FirstOrDefault();
                Triple tripleId = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_game_id"))).FirstOrDefault();

                //Page RDF information has Nourishment Bonus and ID
                if (tripleId != null &&
                    tripleId.Object is LiteralNode &&
                    int.TryParse(((LiteralNode)tripleId.Object).Value, out id))
                {
                    if (tripleNourishmentBonus != null &&
                        tripleNourishmentBonus.Object is LiteralNode)
                    {
                        string[] nourishmentBonusLines = ((LiteralNode)tripleNourishmentBonus.Object).Value.Split("<br>", StringSplitOptions.RemoveEmptyEntries);
                        File.AppendAllLines(_allEffectsFile, nourishmentBonusLines, Encoding.UTF8);
                        food = new Food()
                        {
                            Id = id,
                            Name = pageTitle,
                            DurationSeconds = GetDuration(graphContainer, pageTitle),
                            IsFeast = GetFeastStatus(graphContainer, pageTitle)
                        };

                        foreach (string line in nourishmentBonusLines)
                        {
                            FoodEffect effect = FoodEffect.GetEffect(line);
                            food.Effects.Add(effect);
                        }

                    }
                    else
                    {
                        _log.Warn($"{pageTitle}: RDF does not contain Nourishment");

                        Food feastIngredient = await ResolveFeast(pageTitle, graphContainer);

                        if (feastIngredient != null)
                        {
                            food = new Food()
                            {
                                Id = id,
                                Name = pageTitle,
                                Effects = feastIngredient.Effects,
                                IsFeast = true,
                                DurationSeconds = (int)TimeSpan.FromHours(1).TotalSeconds
                            };
                        }
                        else
                        {
                            _log.Warn($"Feast {pageTitle} could not be resolved");
                            food = null;
                        }
                    }
                }
                else
                {
                    _log.Error($"{pageTitle} does not have ID");
                    food = null;
                }
            }
            catch (InvalidFoodDurationException ex)
            {
                _log.Warn(ex);
                food = null;
            }
            catch (UnmatchedFoodEffectException ex)
            {
                _log.Warn(ex);
                File.AppendAllText(_errorFile, ex.Line + Environment.NewLine, Encoding.UTF8);
                food = null;
            }

            return food;
        }

        private bool GetFeastStatus(RdfGraphContainer graphContainer, string pageTitle)
        {
            Triple tripleItemType = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_item_type"))).FirstOrDefault();

            if (tripleItemType != null &&
                tripleItemType.Object is LiteralNode &&
                ((LiteralNode)tripleItemType.Object).Value.Contains("Feast", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        private int GetDuration(RdfGraphContainer graph, string pageTitle)
        {
            int duration;
            Triple tripleDuration = graph.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_nourishment_duration"))).FirstOrDefault();
            if (tripleDuration != null &&
               tripleDuration.Object is LiteralNode)
            {
                string value = ((LiteralNode)tripleDuration.Object).Value;
                Regex regex = new Regex(@"((\\+|\\-)* *([0-9]+ *))");

                if (regex.IsMatch(value) &&
                    int.TryParse(regex.Match(value).Value, out duration))
                {
                    if (value.Trim().EndsWith("m"))
                    {
                        return (int)TimeSpan.FromMinutes(duration).TotalSeconds;
                    }
                    else if (value.Trim().EndsWith("h"))
                    {
                        return (int)TimeSpan.FromHours(duration).TotalSeconds;
                    }
                    else
                    {
                        throw new InvalidFoodDurationException(pageTitle);
                    }
                }
                else
                {
                    throw new InvalidFoodDurationException(pageTitle);
                }
            }
            else
            {
                throw new InvalidFoodDurationException(pageTitle);
            }
        }

        /// <summary>
        /// Resolves a Feast
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="graphContainer"></param>
        /// <returns></returns>
        private async Task<Food> ResolveFeast(string pageTitle, RdfGraphContainer graphContainer)
        {
            int recipeId;
            string ingredientName = null;


            //Parsing Feasts
            Triple tripleItemType = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_item_type"))).FirstOrDefault();

            if (tripleItemType != null &&
                tripleItemType.Object is LiteralNode &&
                ((LiteralNode)tripleItemType.Object).Value.Contains("Feast", StringComparison.OrdinalIgnoreCase))
            {
                _log.Info($"{pageTitle} has item type: Feast - trying to resolve");

                Triple tripleRecipeId = graphContainer.GetGraph().Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_recipe_id"))).FirstOrDefault();

                if (tripleRecipeId != null &&
                    tripleRecipeId.Object is LiteralNode &&
                    int.TryParse(((LiteralNode)tripleRecipeId.Object).Value, out recipeId))
                {
                    try
                    {
                        ingredientName = await ApiWrapper.Instance.GetSingleIngredientNameForRecipeId(recipeId);
                    }
                    catch (Gw2Sharp.WebApi.Http.NotFoundException)
                    {
                        _log.Error($"Could not resolve recipe for {recipeId}");
                    }

                    if (!string.IsNullOrEmpty(ingredientName))
                    {
                        (string ingredientPageTitle, string ingredientPageContent) = await WikiWrapper.Instance.GetPageContent(ingredientName);
                        RdfGraphContainer ingredientGraphContainer = WikiWrapper.Instance.GetRdfGraph(ingredientPageTitle);
                        return await ParseFood(ingredientPageTitle, ingredientPageContent, ingredientGraphContainer);
                    }
                    else
                    {
                        _log.Warn($"Ingredient/Recipe could not be resolved for {pageTitle}");
                    }
                }


            }
            return null;
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
