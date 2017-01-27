using System.Collections.Generic;
using LinqToTwitter;

namespace TwitterWebApi.Models
{
    public class TweetComponentBase
    {
        public string Text { get; set; }
        public TweetComponentType TweetComponentType { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class TextComponent : TweetComponentBase
    {

    }

    public class UrlComponent : TweetComponentBase
    {
        public string Url { get; set; }
    }

    public class UserMentionCompponent : TweetComponentBase
    {
        public ulong Id { get; set; }

        public string ScreenName { get; set; }
    }

    public class HashTagComponent : TweetComponentBase
    {

    }

    public class SymbolComponent : TweetComponentBase
    {

    }

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