using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using TwitterWebApi.Models;

namespace TwitterWebApi.Services
{
    public class TwitterComponentManager
    {
        private readonly TweetDivider _tweetDivider;
        private readonly ComponentFactory _componentFactory;

        public TwitterComponentManager(TweetDivider tweetDivider, ComponentFactory componentFactory)
        {
            _tweetDivider = tweetDivider;
            _componentFactory = componentFactory;
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
                    yield return _componentFactory.GetTextComponent(status.Text, start, end);
                }
                else
                {
                    yield return _componentFactory.GetSpecialComponent(entity);
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
    }
}