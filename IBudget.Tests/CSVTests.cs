using IBudget.Core.Utils;

namespace IBudget.Tests
{
    public class CSVTests
    {
        [Fact]
        public void DateFormatterTest()
        {
            // arrange
            var workingDateString = "12/02/2024";
            var leapYearString = "29/02/2024";
            var incorrectDateString1 = "31/04/2024";
            var incorrectDateString2 = "32/04/2024";
            var incorrectDateString3 = "32/30/2024";
            var incorrectDateString4 = "incorrect string";

            var workingDateTime1 = DateTime.Parse(workingDateString);
            var workingDateOnly1 = DateOnly.FromDateTime(workingDateTime1);

            var workingDateTime2 = DateTime.Parse(leapYearString);
            var workingDateOnly2 = DateOnly.FromDateTime(workingDateTime2);

            Assert.Equal(CsvFormatter.FormatDate(workingDateString), workingDateOnly1);
            Assert.Equal(CsvFormatter.FormatDate(leapYearString), workingDateOnly2);

            Assert.Throws<Exception>(() => CsvFormatter.FormatDate(incorrectDateString1));
            Assert.Throws<Exception>(() => CsvFormatter.FormatDate(incorrectDateString2));
            Assert.Throws<Exception>(() => CsvFormatter.FormatDate(incorrectDateString3));
            Assert.Throws<Exception>(() => CsvFormatter.FormatDate(incorrectDateString4));
        }

        [Fact]
        public void DescriptionFormatterTest()
        {
            var workingExample1 = "Drinking Place Inc. AUS Card xx9284 Value Date: 18/04/2024";
            var workingExample2 = "My New Cool Business Card xx3725 Value Date: 22/01/2024";
            var workingExample3 = "Cool Cafe AUS Card xx4065 Value Date: 01/12/2024";

            var completeExample1 = "Drinking Place Inc. AUS";
            var completeExample2 = "My New Cool Business";
            var completeExample3 = "Cool Cafe AUS";

            Assert.Equal(CsvFormatter.FormatDescription(workingExample1), completeExample1);
            Assert.Equal(CsvFormatter.FormatDescription(workingExample2), completeExample2);
            Assert.Equal(CsvFormatter.FormatDescription(workingExample3), completeExample3);
        }
    }
}
