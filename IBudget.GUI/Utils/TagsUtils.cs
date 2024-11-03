using IBudget.Core.Model;
using System.Collections.Generic;

namespace IBudget.GUI.ExtensionMethods
{
    public static class TagsUtils
    {
        public static List<Tag> ToTags(List<string> tags)
        {
            var tagsList = new List<Tag>();

            foreach(var tag in tags)
            {
                tagsList.Add(new Tag()
                {
                    Name = tag
                });
            }

            return tagsList;
        }
    }
}
