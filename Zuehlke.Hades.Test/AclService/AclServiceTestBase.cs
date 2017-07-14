using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zuehlke.Hades;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Manager;
using Zuehlke.Hades.Matcher;

namespace Zuehlke.Hades.Test.AclService
{
    [TestClass]
    public abstract class AclServiceTestBase
    {
        private Hades.AclService _acl;
        private List<Policy> _policiesToDelete = new List<Policy>();
        [TestMethod]
        public async Task CheckAccess_CheckTestRequests_ReturnsGranted()
        {
            foreach(var request in TestData.AccessGrantedRequests)
            {
                var result = await _acl.CheckAccessAsync(request);
                Assert.AreEqual(AccessRequestResult.Granted, result);
            }
        }

        [TestMethod]
        public async Task CheckAccess_CheckTestRequests_ReturnsDenied()
        {
            foreach(var request in TestData.AccessDeniedRequests)
            {
                var result = await _acl.CheckAccessAsync(request);
                Assert.AreEqual(AccessRequestResult.Denied, result);
            }
        }

        [TestMethod]
        public async Task CheckAccess_CheckTestRequests_ReturnsExplicitlyDenied()
        {
            foreach (var request in TestData.AccessExplicitlyDeniedRequest)
            {
                var result = await _acl.CheckAccessAsync(request);
                Assert.AreEqual(AccessRequestResult.ExplicitlyDenied, result);
            }
        }

        [TestInitialize]
        public async Task SetupTestPolicies()
        {
            _acl = await GetAclServiceAsync();
            foreach (var pcr in TestData.PolicyCreationRequests)
            {
                var policy = await _acl.Manager.AddPolicyAsync(pcr);
                _policiesToDelete.Add(policy);
            }
        }
        [TestCleanup]
        public async Task Cleanup()
        {
            foreach (var policy in _policiesToDelete)
            {
                await _acl.Manager.DeletePolicyAsync(policy.Id);
            }
        }

        protected abstract Task<Hades.AclService> GetAclServiceAsync();
    }
}
