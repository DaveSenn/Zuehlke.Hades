using System.Collections.Generic;
using System.Text.RegularExpressions;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Matcher
{
    /// <summary>
    /// Handles the matching of attributes in due consideration of regular expressions or similar pattern expressions
    /// </summary>
    public abstract class RegexMatcherBase : IMatcher
    {
        protected abstract IRegexConverter RegexConverter { get; }
        private readonly LRUCache<string, string> _cache;
        private readonly IRegexConverter _regexConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcherBase"/> class with the given cache capacity.
        /// </summary>
        /// <param name="cacheCapacity">The maximum amout of items the cache should keep</param>
        protected RegexMatcherBase(int cacheCapacity = 512)
        {
            _cache = new LRUCache<string, string>(cacheCapacity);
            _regexConverter = RegexConverter;
        }

        /// <summary>
        /// Checks if there is a needle (attribute) in the haystack which is either exactly the same or matches through a regular expression
        /// </summary>
        /// <param name="haystack">List of attributes</param>
        /// <param name="needle">Attribute to look for</param>
        /// <returns>true if found / false if not found</returns>
        public bool Matches(List<string> haystack, string needle)
        {
            foreach(var str in haystack)
            {
                //check if does not contain a regex like pattern
                if(!IsRegexLikePattern(str))
                {
                    if (str.Equals(needle))
                    {
                        return true;
                    }
                }
                else
                {
                    //check if pattern is cached
                    var cached = _cache.Get(str);
                    if (cached != default(string))
                    {
                        if (new Regex(cached).IsMatch(needle))
                        {
                            return true;
                        }
                        continue;
                    }
                    //not cached --> convert --> add to cache
                    var regexString = _regexConverter.Convert(str);
                    _cache.Add(str, regexString);
                    if(new Regex(regexString).IsMatch(needle))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if the given string has a regex/similar pattern
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>true if it is a regex / false if not</returns>
        public abstract bool IsRegexLikePattern(string str);
    }
}
