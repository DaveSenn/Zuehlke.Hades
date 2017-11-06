using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zuehlke.Hades.Interfaces
{
    /// <summary>
    /// Manages the policies for the access control
    /// </summary>
    public interface IAclManager
    {
        /// <summary>
        /// Handles the matching of attributes
        /// </summary>
        IMatcher Matcher { get; }

        /// <summary>
        /// Adds a policy asynchronously with the information provided as a <see cref="PolicyCreationRequest"/>.
        /// </summary>
        /// <param name="policyCreationRequest">Information to create the new <see cref="Policy"/> with</param>
        /// <returns>The newly created <see cref="Policy"/></returns>
        Task<Policy> AddPolicyAsync(PolicyCreationRequest policyCreationRequest);

        /// <summary>
        /// Updates an existing policy (with the provided id) to the given policy information asynchronously
        /// </summary>
        /// <param name="policy">Updated policy</param>
        /// <returns>The updated <see cref="Policy"/> (should be the same as the one that was passed into the method)</returns>
        /// <exception cref="KeyNotFoundException">If there is no policy with the id provided in the passed policy</exception>
        Task<Policy> UpdatePolicyAsync(Policy policy);

        /// <summary>
        /// Get a specific policy by its id asynchronously
        /// </summary>
        /// <param name="id">The id of the policy</param>
        /// <returns>The <see cref="Policy"/> with the given id</returns>
        /// <exception cref="KeyNotFoundException">If there is no policy with the given id</exception>
        Task<Policy> GetPolicyByIdAsync(string id);

        /// <summary>
        /// Deletes a policy by its id asynchronously
        /// </summary>
        /// <param name="id">The id of the policy</param>
        /// <returns>true on sucess / false if unsuccessful</returns>
        Task<bool> DeletePolicyAsync(string id);

        /// <summary>
        /// Get all policies asynchronously
        /// </summary>
        /// <returns>A list of all policies</returns>
        Task<List<Policy>> GetAllPoliciesAsync();

        /// <summary>
        /// Get all policies that might be applicable for the given request asynchronously
        /// </summary>
        /// <param name="request">The access request</param>
        /// <returns>A list of policies that might be applicable</returns>
        Task<List<Policy>> GetRequestCandidatesAsync(AccessRequest request);
    }
}
