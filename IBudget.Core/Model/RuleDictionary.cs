using System.Diagnostics.CodeAnalysis;

namespace IBudget.Core.Model
{
    public class RuleDictionary
    {

        public string rule { get; set; }
        public string[] tags { get; set; }
        public static bool operator ==(RuleDictionary rD1, RuleDictionary rD2) => rD1.Equals(rD2);
        public static bool operator !=(RuleDictionary rD1, RuleDictionary rD2) => !rD1.Equals(rD2);
        public override bool Equals(object obj) => Equals(obj as RuleDictionary);
        public bool Equals(RuleDictionary? other)
        {
            if (other is null) return false;
            if (rule.Equals(other.rule)) return true;
            return false;
        }
        public bool Equals(RuleDictionary? x, RuleDictionary? y)
        {
            if (x is null || y is null) return false;
            if (x.rule.Equals(y.rule)) return true;
            return false;
        }
        public int GetHashCode([DisallowNull] RuleDictionary obj) => (obj.rule, obj.tags).GetHashCode();
        public override int GetHashCode() => (rule, tags).GetHashCode();
        public override string ToString()
        {
            var output = $"{{\n            rule: {rule},\n            tags: {{";
            if (tags.Length == 0) output += "}";
            foreach (var tag in tags)
                output += $"\n                {tag},";
            if (tags.Length > 0) output += "\n            }";
            output += "\n        },";

            return output;
        }
    }
}
