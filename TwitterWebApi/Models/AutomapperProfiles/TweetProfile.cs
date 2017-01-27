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
            CreateMap<Status, Tweet>()
                .ForMember(x=>x.Name, y=>y.MapFrom(z=>z.User.Name))
                .ForMember(x=>x.ProfilePicUrl, y=>y.MapFrom(z=>z.User.ProfileImageUrlHttps))
                .ForMember(x=>x.VerifiedUser, y=>y.MapFrom(z=>z.User.Verified))
                .ForMember(x=>x.IncludeRetweets, y=>y.MapFrom(z=>z.RetweetedStatus.StatusID != 0))
                .ForMember(x=>x.RetweetedTweet, y=>y.MapFrom(z=> z.RetweetedStatus.StatusID != 0 ? z.RetweetedStatus : null)).MaxDepth(4)
                .ForMember(x=>x.TweetComponents, y=>y.MapFrom(z=> CreateComponents(z).ToList()))
                ;
        }

        private static IEnumerable<EntityBase> GetInputEntities(Entities entities)
        {
            return entities.UrlEntities
                .Union<EntityBase>(entities.HashTagEntities)
                .Union(entities.UserMentionEntities)
                .Union(entities.SymbolEntities)
                .Union(entities.MediaEntities)
                .OrderBy(x => x.Start);
        }


        private static List<int> CreateDivisions(Status status)
        {
            List<EntityBase> inputEntities = GetInputEntities(status.Entities).ToList();

            bool? isLastSpecial = null;

            List<int> divisions = new List<int>();

            foreach (var inspection in Enumerable.Range(0, status.Text.Length)
                .Select(index => new{IsSpecial = IsCharacterSpecial(index, inputEntities), Index = index}))
            {
                if (isLastSpecial != inspection.IsSpecial)
                {
                    if(divisions.Count > 0)
                    {
                        divisions.Add(inspection.Index-1); //end previous text block
                    }

                    divisions.Add(inspection.Index); // start new text block
                }

                if (inspection.Index+1 == status.Text.Length) //end
                {
                    divisions.Add(status.Text.Length); //end last text block
                }

                isLastSpecial = inspection.IsSpecial;
            }

            return divisions;
        }

        private static IEnumerable<TweetComponentBase> CreateComponents(Status status)
        {
            List<int> divisions = CreateDivisions(status);

            for (int i = 0; i < divisions.Count; i++)
            {
                if (i % 2 != 0) continue;

                int start = divisions[i];
                int end = divisions[i + 1];

                IEnumerable<EntityBase> entities = GetInputEntities(status.Entities);
                EntityBase entity = entities.SingleOrDefault(x => x.Start == start && x.End == end);
                if (entity == null)
                {
                    yield return GetTextComponent(status.Text, start, end);
                }
                else
                {
                    yield return GetSpecialComponent(entity);
                }
            }
        }

        private static bool IsCharacterSpecial(int index, IEnumerable<EntityBase> entities)
        {
            return entities.Any(e => index >= e.Start && index <= e.End);
        }

        private static TextComponent GetTextComponent(string text, int start, int end)
        {
            return(new TextComponent
            {
                Start = start,
                End = end,
                Text = text.Substring(start, end - start).HtmlDecode(),
                TweetComponentType = TweetComponentType.Text
            });
        }

        private static TweetComponentBase GetSpecialComponent(EntityBase entityBase)
        {
            MediaEntity mediaEntity = entityBase as MediaEntity;
            if (mediaEntity != null)
            {
                return (new MediaComponent
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

            SymbolEntity symbolEntity = entityBase as SymbolEntity;
            if (symbolEntity != null)
            {
                return (new SymbolComponent
                {
                    Start = symbolEntity.Start,
                    End = symbolEntity.End,
                    Text = symbolEntity.Text,
                    TweetComponentType = TweetComponentType.Symbol
                });
            }

            UrlEntity urlEntity = entityBase as UrlEntity;
            if (urlEntity != null)
            {
                return (new UrlComponent
                {
                    Start = urlEntity.Start,
                    End = urlEntity.End,
                    Text = urlEntity.DisplayUrl,
                    Url = urlEntity.ExpandedUrl,
                    TweetComponentType = TweetComponentType.Url
                });
            }

            HashTagEntity tagEntity = entityBase as HashTagEntity;
            if (tagEntity != null)
            {
                return(new HashTagComponent
                {
                    Start = tagEntity.Start,
                    End = tagEntity.End,
                    Text = "#"+tagEntity.Tag,
                    TweetComponentType = TweetComponentType.HashTag
                });
            }

            UserMentionEntity userMentionEntity = entityBase as UserMentionEntity;
            if (userMentionEntity != null)
            {
                return(new UserMentionCompponent()
                {
                    Start = userMentionEntity.Start,
                    End = userMentionEntity.End,
                    Text = userMentionEntity.Name,
                    Id = userMentionEntity.Id,
                    ScreenName = userMentionEntity.ScreenName,
                    TweetComponentType = TweetComponentType.UserMention
                });
            }

            throw new NotImplementedException($"code for ${entityBase.GetType().Name} is not implemented");
        }
    }
}