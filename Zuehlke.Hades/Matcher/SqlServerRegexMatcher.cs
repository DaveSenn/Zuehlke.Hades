using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Matcher
{
    /// <summary>
    /// Handles the matching of attributes for SQL Server in due consideration of T-SQL like patterns
    /// </summary>
    public class SqlServerRegexMatcher : RegexMatcherBase
    {
        private readonly char[] patternChars = new char[] { '%', '_', '[', ']' };

        protected override IRegexConverter RegexConverter => new SqlServerRegexConverter();

        /// <summary>
        /// Checks if the given string contains a T-SQL like pattern
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true if it is a like pattern / false if not</returns>
        public override bool IsRegexLikePattern(string str)
        {
            return patternChars.Any(pc => str.Contains(pc));
        }
    }
}
