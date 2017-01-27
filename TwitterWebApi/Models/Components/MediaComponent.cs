using System.Collections.Generic;
using LinqToTwitter;

namespace TwitterWebApi.Models.Components
{
    public class MediaComponent : TweetComponentBase
    {
        public ulong ID { get; set; }

        public string MediaUrl { get; set; }

        public string MediaUrlHttps { get; set; }

        public List<PhotoSize> Sizes { get; set; }

        public string Type { get; set; }

        public List<int> Indices { get; set; }

        public VideoInfo VideoInfo { get; set; }
    }
}