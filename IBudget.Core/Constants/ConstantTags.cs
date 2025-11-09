using IBudget.Core.Model;

namespace IBudget.Core.Constants
{
    public static class ConstantTags
    {
        public static readonly Tag IgnoredTag = new() { Name = "Ignored", CreatedAt = DateTime.Now, IsTracked = false };
    }
}
