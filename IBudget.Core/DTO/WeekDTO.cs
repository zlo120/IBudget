using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.DTO
{
    public class WeekDTO
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Label { get; }
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public List<Income> Income { get; set; } = new List<Income>();
        private readonly ICalendarService _calendarService;
        public WeekDTO(DateTime start, DateTime end, string label)
        {
            Label = label;
            Start = start;
            End = end;
        }
    }
}