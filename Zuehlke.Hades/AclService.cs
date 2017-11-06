using System.Collections.Generic;
using System.Threading.Tasks;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Manager;

namespace Zuehlke.Hades
{
    /// <summary>
    /// Handles the access control checks
    /// </summary>
    public class AclService
    {
        /// <summary>
        /// Handles the matching of attributes
        /// </summary>
        public IAclManager Manager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AclService"/>
        /// </summary>
        /// <param name="manager">A <see cref="IAclManager"/> for the policies (default: in memory)</param>
        public AclService(IAclManager manager = null)
        {
            if (manager == null)
            {
                manager = new InMemoryManager();
            }
            Manager = manager;
        }

        /// <summary>
        /// Checks if the provided <see cref="AccessRequest"/> is granted or not
        /// </summary>
        /// <param name="request">The <see cref="AccessRequest"/> that should be checked</param>
        /// <returns>The access check result</returns>
        public async Task<AccessRequestResult> CheckAccessAsync(AccessRequest request)
        {
            var candidates = await Manager.GetRequestCandidatesAsync(request);
            if (candidates == null || candidates.Count <= 0)
            {
                return await Task.FromResult(AccessRequestResult.Denied);
            }
            return await DoPoliciesAllow(request, candidates);
        }

        /// <summary>
        /// Checks if the policy candidates allow the request
        /// </summary>
        /// <param name="request">The <see cref="AccessRequest"/> inquiry</param>
        /// <param name="candidates">A list of all policies that might be relevant to the request</param>
        /// <returns>The access check result</returns>
        private async Task<AccessRequestResult> DoPoliciesAllow(AccessRequest request, List<Policy> candidates)
        {
            bool isAllowed = false;
            foreach (var policy in candidates)
            {
                if (!Manager.Matcher.Matches(policy.Actions, request.Action))
                {
                    continue;
                }
                if (!Manager.Matcher.Matches(policy.Subjects, request.Subject))
                {
                    continue;
                }
                if (!Manager.Matcher.Matches(policy.Resources, request.Resource))
                {
                    continue;
                }
                if (!PassesConditions(policy.Conditions, request))
                {
                    continue;
                }
                if (policy.Effect == RequestEffect.Deny)
                {
                    return await Task.FromResult(AccessRequestResult.ExplicitlyDenied);
                }
                isAllowed = true;
            }
            if (isAllowed)
            {
                return await Task.FromResult(AccessRequestResult.Granted);
            }
            return await Task.FromResult(AccessRequestResult.Denied);
        }

        /// <summary>
        /// Checks if the conditions of the policy can be fulfilled
        /// </summary>
        /// <param name="conditions">List of conditions that the policy provides</param>
        /// <param name="request">The request inquiry</param>
        /// <returns>true if the conditions can be fulfilled / false if not</returns>
        private bool PassesConditions(List<ICondition> conditions, AccessRequest request)
        {
            if (conditions != null)
            {
                foreach (var cond in conditions)
                {
                    if (!cond.FulfillsCondition(request))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
