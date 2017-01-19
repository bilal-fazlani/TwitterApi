using System;

namespace TwitterWebApi.Models
{
    public class Tweet
    {
        public string Text { get; set; }

        public string Name { get; set; }

        public ulong StatusID { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ProfilePicUrl { get; set; }
    }
}