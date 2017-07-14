using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Matcher
{
    /// <summary>
    /// Converts a t-sql like pattern to a C# regex pattern
    /// </summary>
    public class SqlServerRegexConverter : IRegexConverter
    {
        /// <summary>
        /// Converts the given t-sql like pattern to a C# regex pattern
        /// </summary>
        /// <param name="likePattern">Pattern to convert</param>
        /// <returns>C# regex pattern string</returns>
        public string Convert(string likePattern)
        {
            return Regex.Replace(
                likePattern,
                @"[%_]|\[[^]]*\]|[^%_[]+",
                match =>
                {
                    if (match.Value == "%")
                    {
                        return ".*";
                    }
                    if (match.Value == "_")
                    {
                        return ".";
                    }
                    if (match.Value.StartsWith("[") && match.Value.EndsWith("]"))
                    {
                        return match.Value;
                    }
                    return Regex.Escape(match.Value);
                });
        }
    }
}
