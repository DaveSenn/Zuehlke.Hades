using System;
using System.Collections.Generic;
using System.Text;

namespace Zuehlke.Hades.Interfaces
{
    /// <summary>
    /// Converts a regex like pattern to a C# regex pattern
    /// </summary>
    public interface IRegexConverter
    {
        /// <summary>
        /// Converts the given pattern to a C# regex pattern
        /// </summary>
        /// <param name="likePattern">Pattern to convert</param>
        /// <returns>C# regex pattern string</returns>
        string Convert(string likePattern);
    }
}
