using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zuehlke.Hades.Manager;

namespace Zuehlke.Hades.Test.AclService
{
    [TestClass]
    public class AclServiceSqlServerTest : AclServiceTestBase
    {
        protected async override Task<Hades.AclService> GetAclServiceAsync()
        {
            var manager = new SqlServerManager("{your-connection-string-here}");
            await manager.CreateDatabaseSchemaAsync();
            return new Hades.AclService(manager);
        }
    }
}
