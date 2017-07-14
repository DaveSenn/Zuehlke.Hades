using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Manager;
using System.Threading.Tasks;

namespace Zuehlke.Hades.Test.Manager
{
    [TestClass]
    public class InMemoryManagerTest : AclManagerTest
    {
        public async override Task<IAclManager> GetManagerAsync()
        {
            return await Task.FromResult(new InMemoryManager());
        }
    }
}
