using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuehlke.Hades.Manager;
using Zuehlke.Hades.SqlServer.Matcher;

namespace Zuehlke.Hades.Test.AclService
{
    [TestClass]
    public class AclServiceInMemoryTest : AclServiceTestBase
    {
        protected override async Task<Hades.AclService> GetAclServiceAsync()
        {
            return await Task.FromResult(new Hades.AclService(new InMemoryManager(new SqlServerRegexMatcher())));
        }
    }
}
