using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Output.BulkIngredientTask
{
    public class BulkItem
    {
        [JsonProperty("bulk_item_id")]
        public int BulkItemId { get; set; }

        [JsonProperty("bulk_item_name")]
        public string BulkItemName { get; set; }

        [JsonProperty("output_item_id")]
        public int OutputItemId { get; set; }

        [JsonProperty("output_item_quantity")]
        public int OutputItemQuantity { get; set; }

        [JsonProperty("output_item_name")]
        public string OutputItemName { get; set; }
    }
}
