namespace PagePlay.Site.Infrastructure.Application;

public static class StringExtensions
{
    public static string ToLowerFirstCharacter(this string input) =>
        string.IsNullOrEmpty(input) 
            ? input 
            : char.ToLowerInvariant(input[0]) + input[1..];
}