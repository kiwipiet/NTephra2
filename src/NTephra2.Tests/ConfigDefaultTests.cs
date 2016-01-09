using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class ConfigDefaultTests
    {
        private readonly Config _config = new Config();

        [TestMethod]
        public void Default_AirDensityTest()
        {
            Assert.AreEqual(1.293, _config.AirDensity, 0.001);
        }
        [TestMethod]
        public void Default_AirViscosityTest()
        {
            Assert.AreEqual(1.8325E-5, _config.AirViscosity, 0.0001E-5);
        }
        [TestMethod]
        public void Default_ColumnIntegrationStepsTest()
        {
            Assert.AreEqual(100, _config.ColumnIntegrationSteps);
        }
        [TestMethod]
        public void Default_DiffusionCoefficientTest()
        {
            Assert.AreEqual(200, _config.DiffusionCoefficient, 0.1);
        }
        [TestMethod]
        public void Default_EddyDiffTest()
        {
            Assert.AreEqual(0.04, _config.EddyDiff, 0.01);
        }
        [TestMethod]
        public void Default_FallTimeThresholdTest()
        {
            Assert.AreEqual(180, _config.FallTimeThreshold, 0.1);
        }
        [TestMethod]
        public void Default_GravityTest()
        {
            Assert.AreEqual(9.81, _config.Gravity, 0.01);
        }
        [TestMethod]
        public void DefaultPlume_PlumeModelTest()
        {
            Assert.AreEqual(PlumeModel.UniformDistribution, _config.Plume.PlumeModel);
        }
        [TestMethod]
        public void DefaultPlume_RatioTest()
        {
            Assert.AreEqual(0.1, _config.Plume.Ratio, 0.1);
        }
        [TestMethod]
        public void DefaultEruption_PlumeHeightTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.PlumeHeight, 0.1);
        }
        [TestMethod]
        public void DefaultEruption_EruptionMassTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.EruptionMass, 0.1);
        }
        [TestMethod]
        public void DefaultEruption_GrainSizeMaxTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.GrainSize.Max, 0.1);
        }
        [TestMethod]
        public void DefaultGrainSize_MinTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.GrainSize.Min, 0.1);
        }
        [TestMethod]
        public void DefaultGrainSize_MedianTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.GrainSize.Median, 0.1);
        }
        [TestMethod]
        public void DefaultGrainSize_StandardTest()
        {
            Assert.AreEqual(0.0, _config.Eruption.GrainSize.Standard, 0.1);
        }
        [TestMethod]
        public void DefaultVent_EastingTest()
        {
            Assert.AreEqual(645110.0, _config.Vent.Easting, 0.1);
        }
        [TestMethod]
        public void DefaultVent_NorthingTest()
        {
            Assert.AreEqual(2158088.0, _config.Vent.Northing, 0.1);
        }
        [TestMethod]
        public void DefaultVent_ElevationTest()
        {
            Assert.AreEqual(3850.0, _config.Vent.Elevation, 0.1);
        }
        [TestMethod]
        public void DefaultDensity_LithicDensityTest()
        {
            Assert.AreEqual(2600.0, _config.Density.LithicDensity, 0.1);
        }
        [TestMethod]
        public void DefaultDensity_PumiceDensityTest()
        {
            Assert.AreEqual(1000.0, _config.Density.PumiceDensity, 0.1);
        }
    }
}