using AutoMapper;
using LinqToTwitter;
using TwitterWebApi.Models;
using TwitterWebApi.Services;

namespace TwitterWebApi.AutomapperProfiles
{
    public class TweetProfile : Profile
    {
        public TweetProfile(TwitterComponentManager componentManager)
        {
            CreateMap<Status, Tweet>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.User.Name))
                .ForMember(x => x.StatusId, y => y.MapFrom(z => z.StatusID))
                .ForMember(x => x.ProfilePicUrl, y => y.MapFrom(z => z.User.ProfileImageUrlHttps))
                .ForMember(x => x.VerifiedUser, y => y.MapFrom(z => z.User.Verified))
                .ForMember(x => x.IncludeRetweets, y => y.MapFrom(z => z.RetweetedStatus.StatusID != 0))
                .ForMember(x => x.RetweetedTweet,
                    y => y.MapFrom(z => z.RetweetedStatus.StatusID != 0 ? z.RetweetedStatus : null))
                .MaxDepth(4)
                .ForMember(x => x.TweetComponents,
                    y => y.MapFrom(z => componentManager.CreateComponentsFromStatus(z)))
                ;
        }

    }
}