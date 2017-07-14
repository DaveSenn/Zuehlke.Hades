using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Manager;

namespace Zuehlke.Hades.Test.Manager
{
    [TestClass]
    public class SqlServerManagerTest : AclManagerTest
    {
        private bool _isSchemaCreated = false;

        public async override Task<IAclManager> GetManagerAsync()
        {
            var manager = new SqlServerManager("{your-connection-string-here}");
            if (!_isSchemaCreated)
            {
                await (manager as SqlServerManager).CreateDatabaseSchemaAsync();
                _isSchemaCreated = true;
            }
            return manager;
        }
    }
}
