using System.Text.RegularExpressions;

namespace Domain.Utilities;

public class ValidationUtility
{
    public static bool IsValidEmailFormat(string email)
    {
        const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
        return Regex.IsMatch(email, pattern);
    }
}