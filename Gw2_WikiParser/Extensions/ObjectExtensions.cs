using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Extensions
{
    public static class ObjectExtensions
    {
        public static T DeepCopy<T>(this T obj) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
    }
}
