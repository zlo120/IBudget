using System.Globalization;
using System.Text;

namespace Core.Model
{
    public abstract class DataEntry : BaseModel
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public Frequency? Frequency { get; set; }
        public virtual List<Tag>? Tags { get; set; }

        public override string ToString()
        {
            return $"ID: {ID,-5} Date: {Date.ToString("dd/MM/yyyy"),-15} Amount: {Amount.ToString("C", CultureInfo.GetCultureInfo("en-US")),-10}\n\tTags: {GetTags()}\n";
        }
        protected string GetTags()
        {
            if (Tags is null) return "No tags assigned.";
            var sb = new StringBuilder();
            foreach (var tag in Tags)
            {
                if (tag == Tags[Tags.Count - 1])
                {
                    sb.Append(tag.Name);
                    break;
                }

                sb.Append(tag.Name);
                sb.Append(", ");
            }

            return sb.ToString();
        }
    }
}