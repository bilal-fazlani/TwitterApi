using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;

namespace TwitterWebApi.Services
{
    public class TweetDivider
    {
        public List<int> CreateDivisions(List<EntityBase> inputEntities, int textLength)
        {
            bool? isLastSpecial = null;

            List<int> divisions = new List<int>();

            foreach (var inspection in Enumerable.Range(0, textLength)
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

                if (inspection.Index+1 == textLength) //end
                {
                    divisions.Add(textLength); //end last text block
                }

                isLastSpecial = inspection.IsSpecial;
            }

            return divisions;
        }

        private static bool IsCharacterSpecial(int index, IEnumerable<EntityBase> entities)
        {
            return entities.Any(e => index >= e.Start && index <= e.End);
        }
    }
}