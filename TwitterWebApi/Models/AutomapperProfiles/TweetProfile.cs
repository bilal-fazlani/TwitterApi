using AutoMapper;
using LinqToTwitter;

namespace TwitterWebApi.Models.AutomapperProfiles
{
    public class TweetProfile : Profile
    {
        public TweetProfile()
        {
            CreateMap<Status, Tweet>()
                .ForMember(x=>x.Name, y=>y.MapFrom(z=>z.User.Name))
                .ForMember(x=>x.ProfilePicUrl, y=>y.MapFrom(z=>z.User.ProfileImageUrlHttps))
                .ForMember(x=>x.IncludeRetweets, y=>y.MapFrom(z=>z.RetweetedStatus.StatusID != 0))
                .ForMember(x=>x.RetweetedTweet, y=>y.MapFrom(z=> z.RetweetedStatus.StatusID != 0 ? z.RetweetedStatus : null)).MaxDepth(4)
                ;
        }
    }
}