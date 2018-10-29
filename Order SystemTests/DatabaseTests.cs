using Microsoft.VisualStudio.TestTools.UnitTesting;
using Order_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_System.Tests
{
    [TestClass()]
    public class DatabaseTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            bool res = Database.Login("dan", "das");
            Assert.AreNotEqual(true,res);
        }

        [TestMethod()]
        public void RegisterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckEmailTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckProductExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckHistoryOrderByIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckHistoryOrderByIDTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMeniuTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckoutTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetHistoryOrdersTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMeniuDataByIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDataCustomerTest()
        {
            Assert.Fail();
        }
    }
}