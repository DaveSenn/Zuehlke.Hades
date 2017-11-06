using System.Collections.Generic;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Matcher
{
    /// <summary>
    /// Handles the matching of attributes with an exact match search 
    /// </summary>
    public class ExactMatcher : IMatcher
    {
        /// <summary>
        /// Checks if there is a needle (attribute) in the haystack which is exactly the same
        /// </summary>
        /// <param name="haystack">List of attributes</param>
        /// <param name="needle">Attribute to look for</param>
        /// <returns>true if found / false if not found</returns>
        public bool Matches(List<string> haystack, string needle)
        {
            return haystack.Contains(needle);
        }
    }
}
