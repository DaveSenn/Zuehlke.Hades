using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zuehlke.Hades.Manager;
using Zuehlke.Hades.Matcher;

namespace Zuehlke.Hades.Test.AclService
{
    [TestClass]
    public class AclServiceInMemoryTest : AclServiceTestBase
    {
        protected async override Task<Hades.AclService> GetAclServiceAsync()
        {
            return await Task.FromResult(new Hades.AclService(new InMemoryManager(new SqlServerRegexMatcher())));
        }
    }
}
