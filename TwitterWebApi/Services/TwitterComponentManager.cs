using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using TwitterWebApi.Extensions;
using TwitterWebApi.Models;

namespace TwitterWebApi.Services
{
    public class TwitterComponentManager
    {
        private readonly TweetDivider _tweetDivider;

        public TwitterComponentManager(TweetDivider tweetDivider)
        {
            _tweetDivider = tweetDivider;
        }

        public IEnumerable<TweetComponentBase> CreateComponentsFromStatus(Status status)
        {
            List<EntityBase> entities = GetInputEntities(status.Entities);

            List<int> divisions = _tweetDivider.CreateDivisions(entities, status.Text.Length);

            for (int i = 0; i < divisions.Count; i++)
            {
                if (i % 2 != 0) continue;

                int start = divisions[i];
                int end = divisions[i + 1];

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

        private static List<EntityBase> GetInputEntities(Entities entities)
        {
            return entities.UrlEntities
                .Union<EntityBase>(entities.HashTagEntities)
                .Union(entities.UserMentionEntities)
                .Union(entities.SymbolEntities)
                .Union(entities.MediaEntities)
                .OrderBy(x => x.Start)
                .ToList();
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
                    Text = "@"+userMentionEntity.ScreenName,
                    Id = userMentionEntity.Id,
                    ScreenName = userMentionEntity.ScreenName,
                    TweetComponentType = TweetComponentType.UserMention
                });
            }

            throw new NotImplementedException($"code for ${entityBase.GetType().Name} is not implemented");
        }


    }
}