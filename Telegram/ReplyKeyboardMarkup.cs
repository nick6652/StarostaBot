using System;
using Newtonsoft.Json;

namespace Telegram
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ReplyKeyboardMarkup
    {
        [JsonProperty(PropertyName = "keyboard")]
        public string[][] Keyboard { get; set; }

        [JsonProperty(PropertyName = "resize_keyboard")]
        public bool ResizeKeyboard { get; set; }

        [JsonProperty(PropertyName = "one_time_keyboard")]
        public bool OneTimeKeyboard { get; set; }

        [JsonProperty(PropertyName = "selective")]
        public bool Selective { get; set; }
        
        public override string ToString()
        {


            return JsonConvert.SerializeObject(this);
        }
    }
 
}
