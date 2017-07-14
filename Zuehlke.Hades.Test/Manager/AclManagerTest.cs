using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Test.Manager
{
    [TestClass]
    public abstract class AclManagerTest
    {
        public abstract Task<IAclManager> GetManagerAsync();
        private List<Policy> _policiesToDelete = new List<Policy>();
        private IAclManager _manager;

        [TestMethod]
        public async Task AddPolicy_AddTestPolicy_ReturnsCorrectPolicy()
        {
            _manager = await GetManagerAsync();
            var policy = await _manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            _policiesToDelete.Add(policy);
            var pcr = policy as PolicyCreationRequest;
            AssertPolicyCreationRequestsAreEqual(TestData.PolicyCreationRequest, pcr);
            Assert.IsTrue(Guid.TryParse(policy.Id, out var guid));
        }

        [TestMethod]
        public async Task AddPolicy_AddTestPolicy_Succeeds()
        {
            _manager = await GetManagerAsync();
            var policy = await _manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            _policiesToDelete.Add(policy);
            var result = await _manager.GetPolicyByIdAsync(policy.Id);
            AssertPoliciesAreEqual(policy, result);
        }

        [TestMethod]
        public async Task DeletePolicy_DeleteTestPolicy_ReturnsTrue()
        {
            _manager = await GetManagerAsync();
            var policy = await _manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            _policiesToDelete.Add(policy);
            Assert.IsTrue(await _manager.DeletePolicyAsync(policy.Id));
        }

        [TestMethod]
        public async Task DeletePolicy_DeleteNonExistentPolicy_ReturnsFalse()
        {
            Assert.IsFalse(await(await GetManagerAsync()).DeletePolicyAsync("123456789"));
        }

        [TestMethod]
        public async Task UpdatePolicy_UpdateTestPolicy_Succeeds()
        {
            _manager = await GetManagerAsync();
            var policy = await _manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            _policiesToDelete.Add(policy);
            var updatePolicy = new Policy(TestData.UpdatePolicyCreationRequest) { Id = policy.Id };
            var updatedPolicy = await _manager.UpdatePolicyAsync(updatePolicy);
            var result = await _manager.GetPolicyByIdAsync(policy.Id);
            AssertPoliciesAreEqual(updatePolicy, result);
        }

        [TestMethod]
        public async Task UpdatePolicy_UpdateTestPolicy_ReturnsUpdatedPolicy()
        {
            _manager = await GetManagerAsync();
            var policy = await _manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            _policiesToDelete.Add(policy);
            var updatePolicy = new Policy(TestData.UpdatePolicyCreationRequest) { Id = policy.Id };
            var updatedPolicy = await _manager.UpdatePolicyAsync(updatePolicy);
            AssertPoliciesAreEqual(updatePolicy, updatedPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdatePolicy_UpdateNonExistentPolicy_ThrowsKeyNotFound()
        {
            var manager = await GetManagerAsync();
            await manager.UpdatePolicyAsync(new Policy(TestData.PolicyCreationRequest)
            {
                Id = "1234"
            });
        }

        [TestMethod]
        public async Task GetPolicyById_GetTestPolicy_ReturnsTestPolicy()
        {
            _manager = await GetManagerAsync();
            var policy = await _manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            _policiesToDelete.Add(policy);
            var result = await _manager.GetPolicyByIdAsync(policy.Id);
            AssertPoliciesAreEqual(policy, result);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetPolicyById_GetNonExistentPolicy_ThrowsKeyNotFound()
        {
            var manager = await GetManagerAsync();
            await manager.GetPolicyByIdAsync("123456789");
        }

        [TestMethod]
        public async Task GetAllPolicies_GetFullList_ReturnsCompleteList()
        {
            _manager = await GetManagerAsync();
            foreach(var pcr in TestData.PolicyCreationRequests)
            {
                var policy = await _manager.AddPolicyAsync(pcr);
                _policiesToDelete.Add(policy);
            }
            var policies = await _manager.GetAllPoliciesAsync();
            Assert.AreEqual(TestData.PolicyCreationRequests.Count, policies.Count);
        }

        [TestMethod]
        public async Task GetRequestCandidates_GetTestCandidates_ReturnsCompleteList()
        {
            _manager = await GetManagerAsync();
            foreach (var pcr in TestData.PolicyCreationRequests)
            {
                var policy = await _manager.AddPolicyAsync(pcr);
                _policiesToDelete.Add(policy);
            }
            var result = await _manager.GetRequestCandidatesAsync(new AccessRequest()
            {
                Subject = TestData.CandidatesForSubject.Key,
                Resource = TestData.PolicyCreationRequests.First().Resources.First(),
                Action = TestData.PolicyCreationRequests.First().Actions.First()
            });
            Assert.IsTrue(result.Count > 0);
            CollectionAssert.AllItemsAreNotNull(result);
        }

        private void AssertPolicyCreationRequestsAreEqual(PolicyCreationRequest expected, PolicyCreationRequest actual)
        {
            if (expected.Actions != null && actual.Actions != null)
            {
                expected.Actions.Sort();
                actual.Actions.Sort();
            }
            CollectionAssert.AreEqual(expected.Actions, actual.Actions);
            if (expected.Subjects != null && actual.Subjects != null)
            {
                expected.Subjects.Sort();
                actual.Subjects.Sort();
            }
            CollectionAssert.AreEqual(expected.Subjects, actual.Subjects);
            if (expected.Resources != null && actual.Resources != null)
            {
                expected.Resources.Sort();
                actual.Resources.Sort();
            }
            CollectionAssert.AreEqual(expected.Resources, actual.Resources);
            if (expected.Conditions != null && actual.Conditions != null)
            {
                expected.Conditions.Sort();
                actual.Conditions.Sort();
            }
            CollectionAssert.AreEqual(expected.Conditions, actual.Conditions);
            Assert.AreEqual(expected.Effect, actual.Effect);
        }
        private void AssertPoliciesAreEqual(Policy expected, Policy actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            AssertPolicyCreationRequestsAreEqual(expected as PolicyCreationRequest, actual as PolicyCreationRequest);
        }
        [TestCleanup]
        public async Task Cleanup()
        {
            foreach(var policy in _policiesToDelete)
            {
                await _manager.DeletePolicyAsync(policy.Id);
            }
        }
    }
}
