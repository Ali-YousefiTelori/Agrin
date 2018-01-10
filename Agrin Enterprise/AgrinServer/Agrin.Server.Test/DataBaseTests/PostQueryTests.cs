using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Agrin.Server.DataBaseLogic;
using Agrin.Server.Models.Filters;
using Agrin.Server.DataBase.Contexts;
using System.ComponentModel;

namespace Agrin.Server.Test.DataBaseTests
{
    [TestClass]
    public class PostQueryTests
    {
        [TestMethod]
        public void FilterVirtualPostCategories()
        {
            try
            {
                var posts = PostExtension.FilterVirtualPostCategories(new FilterBaseInfo());
                Assert.IsTrue(posts.IsSuccess, posts.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
