namespace IBudget.Core.Model
{
    public class Tag : BaseModel
    {
        public string Name { get; set; }
        public bool IsTracked { get; set; }
        public virtual List<Income>? Incomes { get; set; }
        public virtual List<Expense>? Expenses { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not Tag) return false;
            var other = obj as Tag;
            if (other.Name != Name) return false;
            return true;
        }
    }
}