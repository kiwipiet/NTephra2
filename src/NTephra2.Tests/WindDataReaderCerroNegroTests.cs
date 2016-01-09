using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class WindDataReaderCerroNegroTests
    {
        private readonly WindData[] _windData = TestResources.GetWindData(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.wind"));

        [TestMethod]
        public void windData_length_17()
        {
            Assert.AreEqual(17, _windData.Length);
        }
    }
}