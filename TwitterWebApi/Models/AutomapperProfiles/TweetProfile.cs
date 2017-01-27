using System;
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
            CreateMap<Status, List<TweetComponentBase>>()
                .ConstructUsing(ConvertToTweetComponents);


            CreateMap<Status, Tweet>()
                .ForMember(x=>x.Name, y=>y.MapFrom(z=>z.User.Name))
                .ForMember(x=>x.ProfilePicUrl, y=>y.MapFrom(z=>z.User.ProfileImageUrlHttps))
                .ForMember(x=>x.VerifiedUser, y=>y.MapFrom(z=>z.User.Verified))
                .ForMember(x=>x.IncludeRetweets, y=>y.MapFrom(z=>z.RetweetedStatus.StatusID != 0))
                .ForMember(x=>x.RetweetedTweet, y=>y.MapFrom(z=> z.RetweetedStatus.StatusID != 0 ? z.RetweetedStatus : null)).MaxDepth(4)
                .ForMember(x=>x.TweetComponents, y=>y.MapFrom(z=> z))
                ;
        }

        private List<EntityBase> GetInputEntities(Entities entities)
        {
            return entities.UrlEntities
                .Union<EntityBase>(entities.HashTagEntities)
                .Union(entities.UserMentionEntities)
                .Union(entities.SymbolEntities)
                .Union(entities.MediaEntities)
                .OrderBy(x=>x.Start)
                .ToList();
        }

        private List<TweetComponentBase> ConvertToTweetComponents(Status status)
        {
            List<EntityBase> inputEntities = GetInputEntities(status.Entities);

            if (!inputEntities.Any())
            {
                return new List<TweetComponentBase>()
                {
                    new TextComponent
                    {
                        Start = 0,
                        End = status.Text.Length -1,
                        Text = status.Text,
                        TweetComponentType = TweetComponentType.Text
                    }
                };
            }

            List<TweetComponentBase> outputComponents = new List<TweetComponentBase>();

            foreach (EntityBase entity in inputEntities)
            {
                if (!outputComponents.Any())//is first
                {
                    if (entity.Start == 0) // is special
                    {
                        AddSpecialComponent(outputComponents, entity);
                        continue;
                    }
                    else // is text
                    {
                        AddTextComponent(status.Text, outputComponents, 0, entity);
                    }
                }

                bool isSpecial = outputComponents.Last().End + 1 == entity.Start;
                if (!isSpecial)
                {
                    AddTextComponent(status.Text, outputComponents,outputComponents.Last().End +1, entity);
                }
                AddSpecialComponent(outputComponents, entity);
            }

            return outputComponents;
        }

        private static void AddTextComponent(string text, ICollection<TweetComponentBase> components, int start, EntityBase entity)
        {
            components.Add(new TextComponent
            {
                Start = start,
                End = entity.Start - 1,
                Text = text.Substring(0, entity.Start - 1).HtmlDecode(),
                TweetComponentType = TweetComponentType.Text
            });
        }

        private static void AddSpecialComponent(ICollection<TweetComponentBase> components, EntityBase entity)
        {
            if (entity is MediaEntity)
            {
                MediaEntity mediaEntity = (MediaEntity) entity;

                components.Add(new MediaComponent
                {
                    Start = mediaEntity.Start,
                    End = mediaEntity.End,
                    ID = mediaEntity.ID,
                    Indices = mediaEntity.Indices,
                    MediaUrl = mediaEntity.MediaUrl,
                    MediaUrlHttps = mediaEntity.MediaUrlHttps,
                    Sizes = mediaEntity.Sizes,
                    Type = mediaEntity.Type,
                    VideoInfo = mediaEntity.VideoInfo,
                    TweetComponentType = TweetComponentType.Media
                });
            }
            else if (entity is SymbolEntity)
            {
                SymbolEntity urlEntity = (SymbolEntity) entity;

                components.Add(new SymbolComponent
                {
                    Start = urlEntity.Start,
                    End = urlEntity.End,
                    Text = urlEntity.Text,
                    TweetComponentType = TweetComponentType.Symbol
                });
            }
            else if (entity is UrlEntity)
            {
                UrlEntity urlEntity = (UrlEntity) entity;

                components.Add(new UrlComponent
                {
                    Start = urlEntity.Start,
                    End = urlEntity.End,
                    Text = urlEntity.DisplayUrl,
                    Url = urlEntity.ExpandedUrl,
                    TweetComponentType = TweetComponentType.Url
                });
            }
            else if (entity is HashTagEntity)
            {
                HashTagEntity hashTagEntity = (HashTagEntity) entity;

                components.Add(new HashTagComponent
                {
                    Start = hashTagEntity.Start,
                    End = hashTagEntity.End,
                    Text = hashTagEntity.Tag,
                    TweetComponentType = TweetComponentType.HashTag
                });
            }
            else if (entity is UserMentionEntity)
            {
                UserMentionEntity userMentionEntity = (UserMentionEntity) entity;

                components.Add(new UserMentionCompponent()
                {
                    Start = userMentionEntity.Start,
                    End = userMentionEntity.End,
                    Text = userMentionEntity.Name,
                    Id = userMentionEntity.Id,
                    ScreenName = userMentionEntity.ScreenName,
                    TweetComponentType = TweetComponentType.UserMention
                });
            }
            else
            {
                throw new NotImplementedException($"case for ${entity.GetType().Name} is not implemented");
            }
        }
    }
}