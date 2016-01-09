using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class PointReaderCerroNegroTests
    {
        private readonly Point[] _points = TestResources.GetPoints(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.pos"));

        [TestMethod]
        public void pointData_length_15721()
        {
            Assert.AreEqual(15721, _points.Length);
        }

        [TestMethod]
        public void pointData_15721()
        {
            const int index = 15720;
            Assert.AreEqual(632290, _points[index].GetEasting(), 0.1);
            Assert.AreEqual(1482690, _points[index].GetNorthing(), 0.1);
            Assert.AreEqual(300, _points[index].GetElevation(), 0.1);
        }
    }
}