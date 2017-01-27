using System;
using LinqToTwitter;
using TwitterWebApi.Extensions;
using TwitterWebApi.Models;
using TwitterWebApi.Models.Components;

namespace TwitterWebApi.Services
{
    public class ComponentFactory
    {
        public TextComponent GetTextComponent(string text, int start, int end)
        {
            return(new TextComponent
            {
                Start = start,
                End = end,
                Text = text.Substring(start, end - start).HtmlDecode(),
                TweetComponentType = TweetComponentType.Text
            });
        }

        public TweetComponentBase GetSpecialComponent(EntityBase entityBase)
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