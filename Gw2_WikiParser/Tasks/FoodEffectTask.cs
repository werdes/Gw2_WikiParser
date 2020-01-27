using Gw2_WikiParser.Exceptions;
using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Gw2_WikiParser.Model.Output.FoodEffectTask;
using Gw2_WikiParser.Utility;
using System;
using System.Collections.Generic;
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
            try
            {
                List<(string, string, Graph)> lstPages = new List<(string, string, Graph)>();
                foreach (string category in _categories)
                {
                    lstPages.AddRange(await WikiWrapper.Instance.GetCategoryMembersWithContentAndRdfGraph(category));
                }

                foreach ((string pageTitle, string pageContent, Graph rdfGraph) in lstPages)
                {
                    ParseFood(pageTitle, pageContent, rdfGraph);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return wasSuccessful;
        }

        private bool ParseFood(string pageTitle, string pageContent, Graph rdfGraph)
        {
            bool validParse = false;
            int id;

            try
            {

                Triple tripleNourishmentBonus = rdfGraph.Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_nourishment_bonus"))).FirstOrDefault();
                Triple tripleId = rdfGraph.Triples.Where(x => x.Nodes.Any(y => y.NodeType == NodeType.Uri && ((UriNode)y).Uri.ToString().EndsWith("Property-3AHas_game_id"))).FirstOrDefault();

                if (tripleNourishmentBonus != null &&
                    tripleNourishmentBonus.Object is LiteralNode &&
                    tripleId != null &&
                    tripleId.Object is LiteralNode &&
                    int.TryParse(((LiteralNode)tripleId.Object).Value, out id))
                {
                    string[] nourishmentBonusLines = ((LiteralNode)tripleNourishmentBonus.Object).Value.Split("<br>", StringSplitOptions.RemoveEmptyEntries);
                    Food food = new Food()
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
            }
            catch (UnmatchedFoodEffectException ex)
            {
                _log.Error(ex);
            }

            return validParse;
        }

        public string GetProperties()
        {
            return _categories.Join(", ");
        }

    }
}
