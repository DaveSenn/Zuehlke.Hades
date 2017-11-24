using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuehlke.Hades.SqlServer.Manager;

namespace Zuehlke.Hades.Test.AclService
{
    [TestClass]
    public class AclServiceSqlServerTest : AclServiceTestBase
    {
        protected override async Task<Hades.AclService> GetAclServiceAsync()
        {
            var manager = new SqlServerManager(AppSettings.ConnectionString);
            await manager.CreateDatabaseSchemaAsync();
            return new Hades.AclService(manager);
        }
    }
}
