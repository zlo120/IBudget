using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public class Tag : BaseModel
    {
        public required string Name { get; set; }
        public required bool IsTracked { get; set; }
        public required DateTime CreatedAt { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Tag) return false;
            var other = obj as Tag;
            if (other?.Name == Name) return true;
            return false;
        }

        public int GetHashCode(Tag obj) => (obj.Name, obj.IsTracked).GetHashCode();
        public override int GetHashCode() => (Name, IsTracked).GetHashCode();
        public override string ToString() => WriteObject.ToJsonString(this);
    }
}