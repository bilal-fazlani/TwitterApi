namespace TwitterWebApi.Models.Components
{
    public class UserMentionCompponent : TweetComponentBase
    {
        public ulong Id { get; set; }

        public string ScreenName { get; set; }
    }
}