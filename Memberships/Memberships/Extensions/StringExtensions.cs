namespace Memberships.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str) => 
            str == null || str.Equals(string.Empty);
    }
}