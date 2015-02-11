using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExactTarget.TriggeredEmail.Creation.Creators
{
    public class LayoutHtmlReplacementFieldNameParser
    {
        public static IEnumerable<string> Parse(string layoutHtml)
        {
            var replacementFields = new List<string>();
            if (string.IsNullOrWhiteSpace(layoutHtml))
            {
                return replacementFields;
            }
            var regex = new Regex(@"(?<=%%)[a-zA-Z0-9].*?[a-zA-Z0-9]?(?=%%)");
            var matches = regex.Matches(layoutHtml);

            for (var i = 0; i < matches.Count; i++)
            {
                replacementFields.Add(matches[i].Value);
            }
            return replacementFields;
        }
    }
}