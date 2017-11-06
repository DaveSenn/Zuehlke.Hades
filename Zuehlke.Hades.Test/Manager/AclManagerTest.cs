using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Test.Manager
{
    [TestClass]
    public abstract class AclManagerTest
    {
        protected abstract Task<IAclManager> GetManagerAsync();
        protected readonly List<Policy> PoliciesToDelete = new List<Policy>();
        protected IAclManager Manager;

        [TestMethod]
        public async Task AddPolicy_AddTestPolicy_ReturnsCorrectPolicy()
        {
            Manager = await GetManagerAsync();
            var policy = await Manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            PoliciesToDelete.Add(policy);
            AssertPolicyCreationRequestsAreEqual(TestData.PolicyCreationRequest, policy);
            Assert.IsTrue(Guid.TryParse(policy.Id, out var guid));
        }

        [TestMethod]
        public async Task AddPolicy_AddTestPolicy_Succeeds()
        {
            Manager = await GetManagerAsync();
            var policy = await Manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            PoliciesToDelete.Add(policy);
            var result = await Manager.GetPolicyByIdAsync(policy.Id);
            AssertPoliciesAreEqual(policy, result);
        }

        [TestMethod]
        public async Task DeletePolicy_DeleteTestPolicy_ReturnsTrue()
        {
            Manager = await GetManagerAsync();
            var policy = await Manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            PoliciesToDelete.Add(policy);
            Assert.IsTrue(await Manager.DeletePolicyAsync(policy.Id));
        }

        [TestMethod]
        public async Task DeletePolicy_DeleteNonExistentPolicy_ReturnsFalse()
        {
            Assert.IsFalse(await(await GetManagerAsync()).DeletePolicyAsync("123456789"));
        }

        [TestMethod]
        public async Task UpdatePolicy_UpdateTestPolicy_Succeeds()
        {
            Manager = await GetManagerAsync();
            var policy = await Manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            PoliciesToDelete.Add(policy);
            var updatePolicy = new Policy(TestData.UpdatePolicyCreationRequest) { Id = policy.Id };
            var updatedPolicy = await Manager.UpdatePolicyAsync(updatePolicy);
            var result = await Manager.GetPolicyByIdAsync(policy.Id);
            AssertPoliciesAreEqual(updatePolicy, result);
        }

        [TestMethod]
        public async Task UpdatePolicy_UpdateTestPolicy_ReturnsUpdatedPolicy()
        {
            Manager = await GetManagerAsync();
            var policy = await Manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            PoliciesToDelete.Add(policy);
            var updatePolicy = new Policy(TestData.UpdatePolicyCreationRequest) { Id = policy.Id };
            var updatedPolicy = await Manager.UpdatePolicyAsync(updatePolicy);
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
            Manager = await GetManagerAsync();
            var policy = await Manager.AddPolicyAsync(TestData.PolicyCreationRequest);
            PoliciesToDelete.Add(policy);
            var result = await Manager.GetPolicyByIdAsync(policy.Id);
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
            Manager = await GetManagerAsync();
            foreach(var pcr in TestData.PolicyCreationRequests)
            {
                var policy = await Manager.AddPolicyAsync(pcr);
                PoliciesToDelete.Add(policy);
            }
            var policies = await Manager.GetAllPoliciesAsync();
            Assert.AreEqual(TestData.PolicyCreationRequests.Count, policies.Count);
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
            Assert.AreEqual(expected.Description, actual.Description);
        }

        private void AssertPoliciesAreEqual(Policy expected, Policy actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            AssertPolicyCreationRequestsAreEqual(expected, actual);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            foreach(var policy in PoliciesToDelete)
            {
                await Manager.DeletePolicyAsync(policy.Id);
            }
        }
    }
}
