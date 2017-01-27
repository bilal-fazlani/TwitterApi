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

        public List<TweetComponentBase> TweetComponents { get; set; }

        public string Text { get; set; }
    }
}