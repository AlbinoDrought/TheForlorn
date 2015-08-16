using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForlornStub;

namespace ForlornTest
{
    [TestClass]
    public class TestEOF
    {
        [TestMethod]
        public void TestWriteReadEOF()
        {
            if(System.IO.File.Exists("test.exe"))
            {
                System.IO.File.Delete("test.exe");
                System.Threading.Thread.Sleep(100);
            }

            StubSettings ss = StubSettings.Default;
            ss.HostName = Environment.MachineName;
            EOF.SetSettings("test.exe", ss);
            StubSettings ssx = EOF.GetSettings("test.exe");

            Assert.AreEqual(ss.HostName, ssx.HostName);
        }
    }
}
