using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.Manager;

namespace Zuehlke.Hades.Test.Manager
{
    [TestClass]
    public class InMemoryManagerTest : AclManagerTest
    {
        protected override async Task<IAclManager> GetManagerAsync()
        {
            return await Task.FromResult(new InMemoryManager());
        }
    }
}
