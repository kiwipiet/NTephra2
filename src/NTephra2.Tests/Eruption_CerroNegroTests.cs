using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class EruptionCerroNegroTests
    {
        private static readonly Config Config = TestResources.GetConfiguration(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.conf"));

        private static Eruption _eruption;

        [TestInitialize]
        public void TestInitialize()
        {
            _eruption = new Eruption(Config);
        }

        // ColumnBeta is not being used!!
        [TestMethod]
        public void ColumnBetaTest()
        {
            Assert.AreEqual(0.0, _eruption.ColumnBeta);//, 0.1);
        }

        [TestMethod]
        public void MaxPhiTest()
        {
            Assert.AreEqual(4.0, _eruption.MaxPhi);
        }
        [TestMethod]
        public void MaxPlumeHeightTest()
        {
            Assert.AreEqual(6400.0, _eruption.MaxPlumeHeight);
        }
        [TestMethod]
        public void MeanPhiTest()
        {
            Assert.AreEqual(0.0, _eruption.MeanPhi);
        }
        [TestMethod]
        public void MinPhiTest()
        {
            Assert.AreEqual(-4.0, _eruption.MinPhi);
        }
        [TestMethod]
        public void SigmaPhiTest()
        {
            Assert.AreEqual(1.5, _eruption.SigmaPhi);
        }
        [TestMethod]
        public void TotalAshMassTest()
        {
            Assert.AreEqual(3.5E10, _eruption.TotalAshMass);
        }
        [TestMethod]
        public void VentHeightTest()
        {
            Assert.AreEqual(678.0, _eruption.VentHeight);
        }
        [TestMethod]
        public void VolcanoEastingTest()
        {
            Assert.AreEqual(532290.0, _eruption.VolcanoEasting);
        }
        [TestMethod]
        public void VolcanoNorthingTest()
        {
            Assert.AreEqual(1382690.0, _eruption.VolcanoNorthing);
        }
    }
}
