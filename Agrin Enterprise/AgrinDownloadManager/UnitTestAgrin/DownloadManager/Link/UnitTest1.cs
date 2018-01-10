using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestAgrin.DownloadManager.Link
{
    [TestClass]
    public class LinkUnitTest
    {
        [TestMethod]
        public void PlayLink()
        {
            try
            {
                Agrin.Download.EntireModels.Link.LinkInfo linkInfo = new Agrin.Download.EntireModels.Link.LinkInfo();
                linkInfo.Play();
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
