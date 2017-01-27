using System;
using System.Collections.Generic;
using LinqToTwitter;

namespace TwitterWebApi.Models
{
    public class Tweet
    {
        public string Name { get; set; }

        public ulong? StatusID { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string ProfilePicUrl { get; set; }

        public bool? IncludeRetweets { get; set; }

        public Tweet RetweetedTweet { get; set; }

        public bool VerifiedUser { get; set; }

        public List<TweetComponent> TweetComponents { get; set; }

        public string Text { get; set; }
    }

    public class TweetComponent
    {
        public string Url { get; set; }

        public string Text { get; set; }

        public TweetComponentType TweetComponentType { get; set; }

        public int Start { get; set; }

        public int End { get; set; }
    }

    public enum TweetComponentType
    {
        Text = 0,
        Url = 1,
        UserMention = 2,
        HashTag = 3
    }
}