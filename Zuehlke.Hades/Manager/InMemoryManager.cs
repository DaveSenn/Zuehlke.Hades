using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Matcher;

namespace Zuehlke.Hades.Manager
{
    /// <summary>
    /// Manages the policies for the access control in memory without persistence
    /// </summary>
    public class InMemoryManager : IAclManager
    {
        private readonly Dictionary<string, Policy> _activePolicies = new Dictionary<string, Policy>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Handles the matching of attributes
        /// </summary>
        public IMatcher Matcher { get; } = new ExactMatcher();

        /// <summary>
        /// Initializes a new instance of <see cref="InMemoryManager"/> with the provided <see cref="IMatcher"/>
        /// </summary>
        /// <param name="matcher">Matcher for attribute matching</param>
        public InMemoryManager(IMatcher matcher = null)
        {
            if (matcher != null)
            {
                Matcher = matcher;
            }
        }

        /// <summary>
        /// Adds a policy asynchronously with the information provided as a <see cref="PolicyCreationRequest"/>.
        /// </summary>
        /// <param name="policyCreationRequest">Information to create the new <see cref="Policy"/> with</param>
        /// <returns>The newly created <see cref="Policy"/></returns>
        public async Task<Policy> AddPolicyAsync(PolicyCreationRequest policyCreationRequest)
        {
            await _semaphore.WaitAsync();
            try
            {
                var newPolicy = new Policy(policyCreationRequest)
                {
                    Id = Guid.NewGuid().ToString()
                };
                _activePolicies.Add(newPolicy.Id, newPolicy);
                return await Task.FromResult(newPolicy);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Deletes a policy by its id asynchronously
        /// </summary>
        /// <param name="id">The id of the policy</param>
        /// <returns>true on sucess / false if unsuccessful</returns>
        public async Task<bool> DeletePolicyAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await Task.FromResult(_activePolicies.Remove(id));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Get all policies asynchronously
        /// </summary>
        /// <returns>A list of all policies</returns>
        public async Task<List<Policy>> GetAllPoliciesAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return await Task.FromResult(_activePolicies.Select(p => p.Value).ToList());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Get a specific policy by its id asynchronously
        /// </summary>
        /// <param name="id">The id of the policy</param>
        /// <returns>The <see cref="Policy"/> with the given id</returns>
        /// <exception cref="KeyNotFoundException">If there is no policy with the given id</exception>
        public async Task<Policy> GetPolicyByIdAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_activePolicies.TryGetValue(id, out var policy))
                {
                    return await Task.FromResult(policy);
                }
                throw new KeyNotFoundException();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Get all policies that might be applicable for the given request asynchronously !Returns all policies!
        /// </summary>
        /// <param name="request">The access request</param>
        /// <returns>A list of policies that might be applicable</returns>
        public async Task<List<Policy>> GetRequestCandidatesAsync(AccessRequest request)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await Task.FromResult(
                    _activePolicies.Select(p => p.Value).ToList());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Updates an existing policy (with the provided id) to the given policy information asynchronously
        /// </summary>
        /// <param name="policy">Updated policy</param>
        /// <returns>The updated <see cref="Policy"/> (should be the same as the one that was passed into the method)</returns>
        /// <exception cref="KeyNotFoundException">If there is no policy with the id provided in the passed policy</exception>
        public async Task<Policy> UpdatePolicyAsync(Policy policy)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_activePolicies.ContainsKey(policy.Id))
                {
                    _activePolicies[policy.Id] = policy;
                    return await Task.FromResult(policy);
                }
                throw new KeyNotFoundException();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
