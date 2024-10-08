﻿namespace IBudget.Core.Model
{
    public class Tag : BaseModel
    {
        public string Name { get; set; }
        public bool IsTracked { get; set; }
        public virtual List<Income>? Incomes { get; set; }
        public virtual List<Expense>? Expenses { get; set; }
    }
}