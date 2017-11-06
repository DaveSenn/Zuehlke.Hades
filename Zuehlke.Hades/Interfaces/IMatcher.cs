using System.Collections.Generic;

namespace Zuehlke.Hades.Interfaces
{
    /// <summary>
    /// Handles the matching of attributes
    /// </summary>
    public interface IMatcher
    {
        /// <summary>
        /// Checks if there is a needle (attribute) in the haystack
        /// </summary>
        /// <param name="haystack">List of attributes</param>
        /// <param name="needle">Attribute to look for</param>
        /// <returns>true if found / false if not found</returns>
        bool Matches(List<string> haystack, string needle);
    }
}
