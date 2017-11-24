using System.Collections.Generic;
using Zuehlke.Hades.Condition;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Test.AclService
{
    public static class TestData
    {
        public static IEnumerable<PolicyCreationRequest> PolicyCreationRequests => new List<PolicyCreationRequest>
        {
            new PolicyCreationRequest
            {
                Subjects = new List<string> { "user:2", "user:3" },
                Actions = new List<string> { "read", "write" },
                Resources = new List<string> { "qwertz" },
                Description = "test description",
                Effect = RequestEffect.Allow
            },
            new PolicyCreationRequest
            {
                Subjects = new List<string> { "user:1" },
                Actions = new List<string> { "read"},
                Conditions = new List<ICondition>
                {
                    new StringEqualsCondition("key", "value"),
                    new CidrCondition("192.168.0.1/16")
                },
                Resources = new List<string> { "abc" },
                Effect = RequestEffect.Allow
            },
            new PolicyCreationRequest
            {
                Subjects = new List<string> { "role:2", "user:3" },
                Actions = new List<string> { "write" },
                Resources = new List<string> { "qwertz" },
                Effect = RequestEffect.Deny
            },
            new PolicyCreationRequest
            {
                Subjects = new List<string> { "user:%" },
                Actions = new List<string> { "read" },
                Resources = new List<string> { "a" },
                Effect = RequestEffect.Allow
            },
            new PolicyCreationRequest
            {
                Subjects = new List<string> { "role:[0-5]" },
                Actions = new List<string> { "write" },
                Resources = new List<string> { "a" },
                Effect = RequestEffect.Allow
            }
        };
        public static readonly List<AccessRequest> AccessGrantedRequests = new List<AccessRequest>
        {
            new AccessRequest
            {
                Subject = "user:2",
                Action = "write",
                Resource = "qwertz"
            },
            new AccessRequest
            {
                Subject = "user:1",
                Action = "read",
                Resource = "a"
            },
            new AccessRequest
            {
                Subject = "user:1",
                Action = "read",
                Context = new Dictionary<string, string>
                {
                    {"key", "value" },
                    {"cidr_ip", "192.168.0.5" }
                },
                Resource = "abc"
            },
            new AccessRequest
            {
                Subject = "role:4",
                Action = "write",
                Resource = "a"
            }
        };
        public static readonly List<AccessRequest> AccessDeniedRequests = new List<AccessRequest>
        {
            new AccessRequest
            {
                Subject = "role:99",
                Action = "write",
                Resource = "qwertz"
            },
            new AccessRequest
            {
                Subject = "role:6",
                Action = "write",
                Resource = "a"
            },
            new AccessRequest
            {
                Subject = "user:1",
                Action = "read",
                Context = new Dictionary<string, string>
                {
                    {"key", "wrongvalue" },
                    {"cidr_ip", "192.168.0.5" }
                },
                Resource = "abc"
            },
            new AccessRequest
            {
                Subject = "user:1",
                Action = "read",
                Context = new Dictionary<string, string>
                {
                    {"key", "value" },
                    {"cidr_ip", "192.169.0.1" }
                },
                Resource = "abc"
            }
        };
        public static readonly List<AccessRequest> AccessExplicitlyDeniedRequest = new List<AccessRequest>
        {
            new AccessRequest
            {
                Subject = "role:2",
                Action = "write",
                Resource = "qwertz"
            }
        };
    }
}
