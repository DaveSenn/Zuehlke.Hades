using System.Collections.Generic;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades
{
    /// <summary>
    /// An inquiry to request access for the specified subject with the specified action on the specified resource
    /// </summary>
    public class AccessRequest
    {
        /// <summary>
        /// The subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// The action
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// The resource
        /// </summary>
        public string Resource { get; set; }
        /// <summary>
        /// Context information, which can be used by <see cref="ICondition"/>
        /// </summary>
        public Dictionary<string, string> Context { get; set; }
    }
}
