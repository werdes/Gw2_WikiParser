using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Gw2_WikiParser.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


                _log.Info("Running tasks:");
                tasks.ForEach(x => _log.Info($"- {x.GetType().Name}"));
                _log.Info("-------------------------");

                tasks.ForEach(x => x.Run().Wait());
                
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
    }
}
