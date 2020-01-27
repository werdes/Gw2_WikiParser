using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Gw2_WikiParser.Tasks;
using System;
using System.Collections.Generic;
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
                //tasks.Add(new BulkIngredientTask(new string[] { "Category:Bulk_foods" }));
                tasks.Add(new FoodEffectTask(new string[] { "Category:Foods" }));


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
