using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class ConfigReaderTests
    {
        private readonly Config _config = TestResources.GetConfiguration(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.tephra2.conf"));

        [TestMethod]
        public void ConfigReader_AirDensityTest()
        {
            Assert.AreEqual(1.293, _config.AirDensity, 0.001);
        }
        [TestMethod]
        public void ConfigReader_AirViscosityTest()
        {
            Assert.AreEqual(1.8325E-5, _config.AirViscosity, 0.0001E-5);
        }
        [TestMethod]
        public void ConfigReader_ColumnIntegrationStepsTest()
        {
            Assert.AreEqual(200, _config.ColumnIntegrationSteps);
        }
        [TestMethod]
        public void ConfigReader_DiffusionCoefficientTest()
        {
            Assert.AreEqual(142.6, _config.DiffusionCoefficient, 0.1);
        }
        [TestMethod]
        public void ConfigReader_EddyDiffTest()
        {
            Assert.AreEqual(0.04, _config.EddyDiff, 0.01);
        }
        [TestMethod]
        public void ConfigReader_FallTimeThresholdTest()
        {
            Assert.AreEqual(188.6, _config.FallTimeThreshold, 0.1);
        }
        [TestMethod]
        public void ConfigReader_GravityTest()
        {
            Assert.AreEqual(9.81, _config.Gravity, 0.01);
        }
        [TestMethod]
        public void ConfigReaderPlume_PlumeModelTest()
        {
            Assert.AreEqual(PlumeModel.UniformDistribution, _config.Plume.PlumeModel);
        }
        [TestMethod]
        public void ConfigReaderPlume_RatioTest()
        {
            Assert.AreEqual(0.1, _config.Plume.Ratio, 0.1);
        }
        [TestMethod]
        public void ConfigReaderEruption_PlumeHeightTest()
        {
            Assert.AreEqual(23795.0, _config.Eruption.PlumeHeight, 0.1);
        }
        [TestMethod]
        public void ConfigReaderEruption_EruptionMassTest()
        {
            Assert.AreEqual(1.08258E11, _config.Eruption.EruptionMass, 0.000001E11);
        }
        [TestMethod]
        public void ConfigReaderEruption_GrainSizeMaxTest()
        {
            Assert.AreEqual(-4.0, _config.Eruption.GrainSize.Max, 0.1);
        }
        [TestMethod]
        public void ConfigReaderGrainSize_MinTest()
        {
            Assert.AreEqual(4.0, _config.Eruption.GrainSize.Min, 0.1);
        }
        [TestMethod]
        public void ConfigReaderGrainSize_MedianTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.GrainSize.Median, 0.1);
        }
        [TestMethod]
        public void ConfigReaderGrainSize_StandardTest()
        {
            Assert.AreEqual(1.1, _config.Eruption.GrainSize.Standard, 0.1);
        }
        [TestMethod]
        public void ConfigReaderVent_EastingTest()
        {
            Assert.AreEqual(645110.0, _config.Vent.Easting, 0.1);
        }
        [TestMethod]
        public void ConfigReaderVent_NorthingTest()
        {
            Assert.AreEqual(2158088.0, _config.Vent.Northing, 0.1);
        }
        [TestMethod]
        public void ConfigReaderVent_ElevationTest()
        {
            Assert.AreEqual(3850.0, _config.Vent.Elevation, 0.1);
        }
        [TestMethod]
        public void ConfigReaderDensity_LithicDensityTest()
        {
            Assert.AreEqual(2600.0, _config.Density.LithicDensity, 0.1);
        }
        [TestMethod]
        public void ConfigReaderDensity_PumiceDensityTest()
        {
            Assert.AreEqual(1000.0, _config.Density.PumiceDensity, 0.1);
        }
    }
}