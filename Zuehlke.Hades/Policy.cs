using System.Collections.Generic;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades
{
    /// <summary>
    /// A policy describes a rule to assign positive or negative permissions to a subject
    /// </summary>
    public class Policy : PolicyCreationRequest
    {
        /// <summary>
        /// The id of the policy
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Initializes a new instance of a <see cref="Policy"/>
        /// </summary>
        public Policy(PolicyCreationRequest pcr): base(pcr)
        {
            Id = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Policy"/> from an existing one
        /// </summary>
        /// <param name="p">The <see cref="Policy"/> to clone</param>
        public Policy(Policy p): base(p)
        {
            Id = p.Id;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Policy"/>
        /// </summary>
        public Policy() { }
    }

    /// <summary>
    /// An inquiry to request the creation of a policy
    /// </summary>
    public class PolicyCreationRequest
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="PolicyCreationRequest"/>
        /// </summary>
        public PolicyCreationRequest() { }

        /// <summary>
        /// Create a new <see cref="PolicyCreationRequest"/> from an existing <see cref="PolicyCreationRequest"/>
        /// </summary>
        /// <param name="pcr">The <see cref="PolicyCreationRequest"/> to clone</param>
        protected PolicyCreationRequest(PolicyCreationRequest pcr)
        {
            Subjects = pcr.Subjects;
            Actions = pcr.Actions;
            Resources = pcr.Resources;
            Effect = pcr.Effect;
            Description = pcr.Description;
            Conditions = pcr.Conditions;
        }

        /// <summary>
        /// A description of the policy
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// The subjects this policy applies to
        /// </summary>
        public List<string> Subjects { get; set; }
        /// <summary>
        /// The actions the policy applies to
        /// </summary>
        public List<string> Actions { get; set; }
        /// <summary>
        /// The resources the policy applies to
        /// </summary>
        public List<string> Resources { get; set; }
        /// <summary>
        /// Any additional <see cref="ICondition"/> to include
        /// </summary>
        public List<ICondition> Conditions { get; set; }
        /// <summary>
        /// Whether matching requests are allowed or denied
        /// </summary>
        public RequestEffect Effect { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum RequestEffect
    {
        /// <summary>
        /// Allow matching requests
        /// </summary>
        Allow,
        /// <summary>
        /// Deny matchine requests
        /// </summary>
        Deny
    }
}
