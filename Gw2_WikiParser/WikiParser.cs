using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Gw2_WikiParser.Tasks;
using Gw2_WikiParser.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2_WikiParser
{
    public class WikiParser
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WikiParser()
        {
            List<IWikiTask> tasks = new List<IWikiTask>();
            try
            {
                tasks.Add(new BulkIngredientTask(new string[] { "Category:Bulk_foods" }));
                tasks.Add(new FoodEffectTask(ConfigurationManager.AppSettings["food_effects_categories"].Split(';')));
                tasks.Add(new EventTimerTask(ConfigurationManager.AppSettings["event_timer_page"]));

                _log.Info("Running tasks:");
                tasks.ForEach(x => _log.Info($"- {x.GetType().Name}: {x.GetProperties()}"));
                _log.Info("-------------------------");

                Task.WaitAll(tasks.Select(x => x.Run()).ToArray());
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
    }
}
