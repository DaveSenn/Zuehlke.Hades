using Newtonsoft.Json;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Condition
{
    /// <summary>
    /// Checks if two string are equal
    /// </summary>
    public class StringEqualsCondition : ICondition
    {
        /// <summary>
        /// The key of the condition is used to get a value from the <see cref="AccessRequest.Context"/>
        /// </summary>
        public string Key { get; private set; }
        
        /// <summary>
        /// The string that should be checked against 
        /// </summary>
        public string Value { get; private set; }

        [JsonConstructor]
        private StringEqualsCondition() { }

        /// <summary>
        /// Initializes a new instance of <see cref="StringEqualsCondition"/> with the given key and value
        /// </summary>
        /// <param name="key">The key of the condition is used to get a value from the <see cref="AccessRequest.Context"/></param>
        /// <param name="value">The string that should be checked against</param>
        public StringEqualsCondition(string key, string value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Checks if the Context of the <see cref="AccessRequest"/> has an entry with the key and value provided
        /// </summary>
        /// <param name="request">The <see cref="AccessRequest"/> inquiry</param>
        /// <returns>true if the string is equal / false if not</returns>
        public bool FulfillsCondition(AccessRequest request)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                if (request.Context.ContainsKey(Key))
                {
                    if (request.Context[Key].Equals(Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
