using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestAgrin.Framesoft
{
    [TestClass]
    public class FramesoftTest
    {
        [TestMethod]
        public void Test()
        {
            try
            {
                var guid = Guid.NewGuid();
                var loginData = Agrin.Framesoft.Helper.UserManagerHelper.LoginUser(new Agrin.Framesoft.UserInfoData() { UserName = "ali yousefi", Password = ".", ApplicationGuid = guid });
                //Agrin.Framesoft.Helper.UserManagerHelper.GetYoutubeVideoList(new Agrin.Framesoft.UserFileInfoData() { UserName = "ali yousefi", Password = ".", Link = "https://youtu.be/uz30vx2zxZc" });
            }
            catch// (Exception e)
            {

            }
        }
    }
}
