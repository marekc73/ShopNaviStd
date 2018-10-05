using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShopNavi.Data
{

    [JsonObject]
    public class MessageResult
    {
        IList<Google.Apis.Gmail.v1.Data.Message> messages = new List<Google.Apis.Gmail.v1.Data.Message>();


        [JsonProperty("messages")]
        public IList<Google.Apis.Gmail.v1.Data.Message> Messages
        {
            get
            {
                return this.messages;
            }
            set
            {
                this.messages = value;
            }
        }
    }
}
