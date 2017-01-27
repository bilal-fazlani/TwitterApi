using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinqToTwitter;
using TwitterWebApi.Extensions;

namespace TwitterWebApi.Models.AutomapperProfiles
{
    public class TweetProfile : Profile
    {
        public TweetProfile()
        {
            CreateMap<Status, Tweet>()
                .ForMember(x=>x.Name, y=>y.MapFrom(z=>z.User.Name))
                .ForMember(x=>x.ProfilePicUrl, y=>y.MapFrom(z=>z.User.ProfileImageUrlHttps))
                .ForMember(x=>x.VerifiedUser, y=>y.MapFrom(z=>z.User.Verified))
                .ForMember(x=>x.IncludeRetweets, y=>y.MapFrom(z=>z.RetweetedStatus.StatusID != 0))
                .ForMember(x=>x.RetweetedTweet, y=>y.MapFrom(z=> z.RetweetedStatus.StatusID != 0 ? z.RetweetedStatus : null)).MaxDepth(4)
                .ForMember(x=>x.TweetComponents, y=>y.MapFrom(z=> ConvertToTweetComponents(z.Entities, z.Text)))
                ;
        }

        private List<TweetComponent> ConvertToTweetComponents(Entities entities, string text)
        {
            if (!entities.UrlEntities.Any())
            {
                return new List<TweetComponent>()
                {
                    new TweetComponent
                    {
                        Start = 0,
                        End = text.Length -1,
                        Text = text,
                        TweetComponentType = TweetComponentType.Text
                    }
                };
            }

            List<TweetComponent> components = new List<TweetComponent>();

            foreach (UrlEntity entity in entities.UrlEntities)
            {
                if (!components.Any())//is first
                {
                    if (entity.Start == 0) // is url
                    {
                        AddUrlComponent(components, entity);
                        continue;
                    }
                    else // is text
                    {
                        AddTextComponent(text, components, 0, entity);
                    }
                }

                bool isSpecial = components.Last().End + 1 == entity.Start;
                if (!isSpecial)
                {
                    AddTextComponent(text, components,components.Last().End +1, entity);

                }
                AddUrlComponent(components, entity);
            }

            return components;
        }

        private static void AddTextComponent(string text, List<TweetComponent> components, int start, UrlEntity entity)
        {
            components.Add(new TweetComponent
            {
                Start = start,
                End = entity.Start - 1,
                Text = text.Substring(0, entity.Start - 1).HtmlDecode(),
                TweetComponentType = TweetComponentType.Text
            });
        }

        private static void AddUrlComponent(List<TweetComponent> components, UrlEntity entity)
        {
            components.Add(new TweetComponent
            {
                Start = entity.Start,
                End = entity.End,
                Text = entity.DisplayUrl,
                Url = entity.ExpandedUrl,
                TweetComponentType = TweetComponentType.Url
            });
        }
    }
}