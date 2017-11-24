using System.Linq;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Matcher;

namespace Zuehlke.Hades.SqlServer.Matcher
{
    /// <summary>
    /// Handles the matching of attributes for SQL Server in due consideration of T-SQL like patterns
    /// </summary>
    public class SqlServerRegexMatcher : RegexMatcherBase
    {
        private readonly char[] _patternChars = { '%', '_', '[', ']' };

        /// <summary>
        /// The <see cref="IRegexConverter"/> to use for converting the regex
        /// </summary>
        protected override IRegexConverter RegexConverter => new SqlServerRegexConverter();

        /// <summary>
        /// Checks if the given string contains a T-SQL like pattern
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true if it is a like pattern / false if not</returns>
        public override bool IsRegexLikePattern(string str)
        {
            return _patternChars.Any(str.Contains);
        }
    }
}
