using System.Text.RegularExpressions;

namespace IBudget.GUI.Utils
{
    public static class LabelUtils
    {
        public static string AddSpacesBeforeCapitals(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        }
    }
}
