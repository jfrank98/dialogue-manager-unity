using System.Collections.Generic;

public static class StringExtensions
{
    public static List<string> SplitWithEscape(this string input, char delimiter, char escapeCharacter = '\\')
    {
        List<string> result = new List<string>();
        bool inEscape = false;
        var currentSegment = new System.Text.StringBuilder();

        foreach (char c in input)
        {
            if (inEscape)
            {
                currentSegment.Append(c);
                inEscape = false;
            }
            else if (c == escapeCharacter)
            {
                inEscape = true;
            }
            else if (c == delimiter)
            {
                result.Add(currentSegment.ToString());
                currentSegment.Clear();
            }
            else
            {
                currentSegment.Append(c);
            }
        }

        // Add the last segment
        result.Add(currentSegment.ToString());

        return result;
    }
}
