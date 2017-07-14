using System;
using System.Collections.Generic;
using System.Text;
using Zuehlke.Hades.Condition;

namespace Zuehlke.Hades.Test.Manager
{
    public static class TestData
    {
        public static PolicyCreationRequest PolicyCreationRequest => new PolicyCreationRequest()
        {
            Subjects = new List<string>() { "user:2", "user:3" },
            Actions = new List<string>() { "read", "write" },
            Resources = new List<string>() { "qwertz" },
            Effect = RequestEffect.Allow
        };
        public static PolicyCreationRequest UpdatePolicyCreationRequest => new PolicyCreationRequest()
        {
            Subjects = new List<string>() { "user:1" },
            Actions = new List<string>() { "read", "add_permissions" },
            Resources = new List<string>() { "abc" },
            Effect = RequestEffect.Deny
        };

        public static List<PolicyCreationRequest> PolicyCreationRequests => new List<PolicyCreationRequest>()
        {
            new PolicyCreationRequest()
            {
                Subjects = new List<string>() { "user:2", "user:3" },
                Actions = new List<string>() { "read", "write" },
                Resources = new List<string>() { "qwertz" },
                Conditions = new List<Interfaces.ICondition>(){
                    new StringEqualsCondition("testkey","testvalue"),
                    new CidrCondition("192.168.0.1/16")
                },
                Effect = RequestEffect.Allow
            },
            new PolicyCreationRequest()
            {
                Subjects = new List<string>() { "user:1", },
                Actions = new List<string>() { "read"},
                Resources = new List<string>() { "abc" },
                Effect = RequestEffect.Allow
            },
            new PolicyCreationRequest()
            {
                Subjects = new List<string>() { "role:2", "user:3" },
                Actions = new List<string>() { "write" },
                Resources = new List<string>() { "qwertz" },
                Effect = RequestEffect.Deny
            }
        };
        public static KeyValuePair<string, int> CandidatesForSubject = new KeyValuePair<string, int>("user:3", 2);
    }
}
