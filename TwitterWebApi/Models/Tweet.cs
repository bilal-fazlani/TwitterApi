﻿using System;
using System.Collections.Generic;
using TwitterWebApi.Models.Components;

namespace TwitterWebApi.Models
{
    public class Tweet
    {
        public string Name { get; set; }

        public ulong? StatusId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string ProfilePicUrl { get; set; }

        public bool? IncludeRetweets { get; set; }

        public Tweet RetweetedTweet { get; set; }

        public bool VerifiedUser { get; set; }

        public List<TweetComponentBase> TweetComponents { get; set; }

        public string Text { get; set; }
    }
}