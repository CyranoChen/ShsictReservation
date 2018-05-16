using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Reservation.Mvc.Entities.SecureNode;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class CheckListTest
    {
        [TestMethod]
        public void CleanCheckList_Test()
        {
            CheckList.Clean(-30);

            Assert.IsTrue(true);
        }
    }
}
