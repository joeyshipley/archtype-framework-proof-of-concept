namespace PagePlay.Site.Infrastructure.Application;

public static class StringExtensions
{
    public static string ToLowerFirstCharacter(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (input.Length == 1)
            return input.ToLowerInvariant();

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
}