namespace Core.Model
{
    public static class FrequencyMethods
    {
        public static string ConvertToString(Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.Daily:
                    return "Daily";

                case Frequency.Weekly:
                    return "Weekly";

                case Frequency.BiWeekly:
                    return "BiWeekly";

                case Frequency.Monthy:
                    return "Monthly";

                case Frequency.Yearly:
                    return "Yearly";

                default:
                    throw new Exception("Incorrect method parameter");
            }
        }

        public static Frequency ConvertToFrequency(string frequency)
        {
            switch (frequency.ToUpper())
            {
                case "DAILY":
                    return Frequency.Daily;

                case "WEEKLY":
                    return Frequency.Weekly;

                case "BIWEEKLY":
                    return Frequency.BiWeekly;

                case "MONTHLY":
                    return Frequency.Monthy;

                case "YEARLY":
                    return Frequency.Yearly;

                default:
                    throw new Exception("Incorrect Frequency string");
            }
        }
    }
}
