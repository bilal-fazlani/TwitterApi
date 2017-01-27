namespace TwitterWebApi.Models.Components
{
    public class TweetComponentBase
    {
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
        HashTag = 3,
        Media =4,
        Symbol =5
    }
}