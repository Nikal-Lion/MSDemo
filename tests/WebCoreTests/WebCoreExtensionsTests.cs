using MS.Entities.Core;
using MS.WebCore;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace MS.WebCoreTests
{
    public class WebCoreExtensionsTests
    {
        private readonly ITestOutputHelper outputHelper;

        public WebCoreExtensionsTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void ShouldGetThePath()
        {
            string pathName = WebCoreExtensions.GetXmlAbsolutePath(Assembly.GetExecutingAssembly().GetName().Name);
            outputHelper.WriteLine(pathName);
            bool result = File.Exists(pathName);
            Assert.True(result);
        }
    }
}
