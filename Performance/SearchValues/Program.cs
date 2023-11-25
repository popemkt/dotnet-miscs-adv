// See https://aka.ms/new-console-template for more information

using System.Buffers;

Console.WriteLine(Main.IsBase64_Span("SGVsbG8gV29ybGQ="));
Console.WriteLine(Main.IsBase64_Span("SGVsbG8g^29ybGQ="));

static class Main
{
    private const string Base64Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    private static readonly Lazy<SearchValues<char>> Base64SearchValues =
        //Create is heavily optimized
        new(() => SearchValues.Create(Base64Characters));

    public static bool IsBase64_Span(string text)
    {
        return text.AsSpan().IndexOfAnyExcept(Base64SearchValues.Value) == -1;
    }
}