using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades
{
    /// <summary>
    /// A policy describes a rule to assign positive or negative permissions to a subject
    /// </summary>
    public class Policy : PolicyCreationRequest
    {
        public string Id { get; set; }
        public Policy(PolicyCreationRequest pcr): base(pcr)
        {
            Id = string.Empty;
        }
        public Policy(Policy p): base(p)
        {
            Id = p.Id;
        }
        public Policy() { }
    }

    /// <summary>
    /// An inquiry to request the creation of a policy
    /// </summary>
    public class PolicyCreationRequest
    {
        public PolicyCreationRequest() { }
        protected PolicyCreationRequest(PolicyCreationRequest pcr)
        {
            Subjects = pcr.Subjects;
            Actions = pcr.Actions;
            Resources = pcr.Resources;
            Effect = pcr.Effect;
            Description = pcr.Description;
            Conditions = pcr.Conditions;
        }
        public string Description { get; set; } = "";
        public List<string> Subjects { get; set; }
        public List<string> Actions { get; set; }
        public List<string> Resources { get; set; }
        public List<ICondition> Conditions { get; set; }
        public RequestEffect Effect { get; set; }

    }
    public enum RequestEffect
    {
        Allow,
        Deny
    }
}
