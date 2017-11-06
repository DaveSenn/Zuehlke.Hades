using Newtonsoft.Json;

namespace Zuehlke.Hades.Interfaces
{
    /// <summary>
    /// A condition is an additional rule a request has to fullfil
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// The key of the condition is used to get a value from the <see cref="AccessRequest.Context"/>
        /// </summary>
        [JsonProperty]
        string Key { get; }

        /// <summary>
        /// A value that the data will be compared with
        /// </summary>
        [JsonProperty]
        string Value { get; }

        /// <summary>
        /// Checks if the condition is fullfilled
        /// </summary>
        /// <param name="request">The <see cref="AccessRequest"/> inquiry</param>
        /// <returns>true if the condition is fulfilled / false if not</returns>
        bool FulfillsCondition(AccessRequest request);
    }
}
