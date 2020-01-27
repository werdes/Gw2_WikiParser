using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Utility
{
    public class CacheHandler
    {
        private class InternalCache
        {
            private static Lazy<InternalCache> _instance = new Lazy<InternalCache>(() => new InternalCache());
            public static InternalCache Instance
            {
                get => _instance.Value;
            }

            private InternalCache()
            {

            }

        }


        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, object> _cache = new Dictionary<string, object>();

        public CacheHandler()
        {

        }

    }

}
