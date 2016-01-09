using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class EruptionValuesCerroNegroTests
    {
        private static readonly Config Config = TestResources.GetConfiguration(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.conf"));

        private static Eruption _eruption;
        private static readonly Wind[][] Wind = TestResources.GetWind(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.wind"), Config);

        private static EruptionValues _eruptionValues;

        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx) 
        {
            _eruption = new Eruption(Config);

            _eruptionValues = new EruptionValues(_eruption, Wind[0], Config);
        }

        [TestMethod]
        public void DoesTephraPiMatchConfigPi()
        {
            const double tephraPi = 3.141592654;
            Assert.AreEqual(tephraPi, Config.Pi, 0.000000001);
        }

        [TestMethod]
        public void PartStepsTest()
        {
            Assert.AreEqual(80, _eruptionValues.PartSteps);
        }
        [TestMethod]
        public void ThresholdTest()
        {
            Assert.AreEqual(1822.4, _eruptionValues.Threshold, 0.1);
        }
        [TestMethod]
        public void SqrtTwoPiTest()
        {
            Assert.AreEqual(2.506628, _eruptionValues.SqrtTwoPi, 0.000001);
        }
        [TestMethod]
        public void BetaSqrtTwoPiTest()
        {
            // Always 0 as Beta is 0
            Assert.AreEqual(0.0, _eruptionValues.BetaSqrtTwoPi, 0.1);
        }
        [TestMethod]
        public void TwoBetaSqrdTest()
        {
            // Always 0 as Beta is 0
            Assert.AreEqual(0.0, _eruptionValues.TwoBetaSqrd, 0.1);
        }
        [TestMethod]
        public void PdfGrainSizeDemon1Test()
        {
            Assert.AreEqual(0.265962, _eruptionValues.PdfGrainSizeDemon1, 0.000001);
        }
        [TestMethod]
        public void TwoPartSigmaSizeTest()
        {
            Assert.AreEqual(4.5, _eruptionValues.TwoPartSigmaSize, 0.000001);
        }
        [TestMethod]
        public void ht_section_width_Test()
        {
            Assert.AreEqual(5722, _eruption.MaxPlumeHeight - _eruption.VentHeight, 0.000001);
        }
        [TestMethod]
        public void part_section_width_Test()
        {
            Assert.AreEqual(8, _eruption.MaxPhi - _eruption.MinPhi, 0.000001);
        }
        [TestMethod]
        public void ht_step_width_Test()
        {
            var htSectionWidth = _eruption.MaxPlumeHeight - _eruption.VentHeight;
            var htStepWidth = htSectionWidth / Config.ColumnIntegrationSteps;
            Assert.AreEqual(57.22, htStepWidth, 0.000001);
        }
        [TestMethod]
        public void part_step_width_Test()
        {
            var partSectionWidth = _eruption.MaxPhi - _eruption.MinPhi;
            var partStepWidth = partSectionWidth / _eruptionValues.PartSteps;
            Assert.AreEqual(0.1, partStepWidth, 0.000001);
        }
        [TestMethod]
        public void TotalPCol_Test()
        {
            var htSectionWidth = _eruption.MaxPlumeHeight - _eruption.VentHeight;
            var htStepWidth = htSectionWidth / Config.ColumnIntegrationSteps;
            var totalPCol = _eruptionValues.GetTotalPCol(_eruption, Config, htSectionWidth, htStepWidth);
            Assert.AreEqual(1.0, totalPCol, 0.000001);
        }
        [TestMethod]
        public void TotalPPart_Test()
        {
            var partSectionWidth = _eruption.MaxPhi - _eruption.MinPhi;
            var partStepWidth = partSectionWidth / _eruptionValues.PartSteps;
            var totalPPart = _eruptionValues.GetTotalPPart(_eruption, partStepWidth);
            Assert.AreEqual(0.992317, totalPPart, 0.000001);
        }
        [TestMethod]
        public void TotalP_Test()
        {
            var htSectionWidth = _eruption.MaxPlumeHeight - _eruption.VentHeight;
            var htStepWidth = htSectionWidth / Config.ColumnIntegrationSteps;
            var totalPCol = _eruptionValues.GetTotalPCol(_eruption, Config, htSectionWidth, htStepWidth);

            var partSectionWidth = _eruption.MaxPhi - _eruption.MinPhi;
            var partStepWidth = partSectionWidth / _eruptionValues.PartSteps;
            var totalPPart = _eruptionValues.GetTotalPPart(_eruption, partStepWidth);
            Assert.AreEqual(0.992317, _eruptionValues.GetTotalP(totalPCol, totalPPart), 0.000001);
        }
        [TestMethod]
        public void CreateTableArray_Test()
        {
            var tableArray = _eruptionValues.CreateTableArray();
            //Assert.AreEqual(80, tableArray.Length );
            //Assert.AreEqual(100, tableArray[0].Length);
            Assert.AreEqual(80 * 100, tableArray.Length);
        }
    }
}