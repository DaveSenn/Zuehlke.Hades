using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Manager;

namespace Zuehlke.Hades.Test.Manager
{
    [TestClass]
    public class SqlServerManagerTest : AclManagerTest
    {
        private bool _isSchemaCreated;

        protected override async Task<IAclManager> GetManagerAsync()
        {
            var manager = new SqlServerManager(AppSettings.ConnectionString);
            if (!_isSchemaCreated)
            {
                await manager.CreateDatabaseSchemaAsync();
                _isSchemaCreated = true;
            }
            return manager;
        }

        [TestMethod]
        public async Task GetRequestCandidates_GetTestCandidates_ReturnsPolicyForResource()
        {
            Manager = await GetManagerAsync();
            foreach (var pcr in TestData.PolicyCreationRequests)
            {
                var policy = await Manager.AddPolicyAsync(pcr);
                PoliciesToDelete.Add(policy);
            }
            var result = await Manager.GetRequestCandidatesAsync(new AccessRequest
            {
                Subject = TestData.CandidatesForSubject.Key,
                Resource = TestData.CandidatesForResource.Key,
                Action = TestData.PolicyCreationRequests.First().Actions.First()
            });
            Assert.IsTrue(result.Count == TestData.CandidatesForResource.Value);
            CollectionAssert.AllItemsAreNotNull(result);
        }
    }
}
