using System;
using System.Collections.Generic;
using System.Text;

namespace Zuehlke.Hades
{
    /// <summary>
    /// An inquiry to request access for the specified subject with the specified action on the specified resource
    /// </summary>
    public class AccessRequest
    {
        public string Subject { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
        public Dictionary<string, string> Context { get; set; }
    }
}
