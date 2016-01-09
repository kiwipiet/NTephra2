using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTephra2.Core;

namespace NTephra2.Tests
{
    [TestClass]
    public class WindFactoryCerroNegroTests
    {
        private static readonly Config Config = TestResources.GetConfiguration(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.conf"));
        private static readonly Wind[][] Wind = TestResources.GetWind(EmbeddedResource.GetStringArray("NTephra2.Tests.Resources.CerroNegro.wind"), Config);

        /*
         * First dimension is days, currently only supports one day
         */
        [TestMethod]
        public void Wind_1D_length_1_Test()
        {
            Assert.AreEqual(1, Config.WindDays);
            Assert.AreEqual(1, Wind.Length);
        }
        /*
         * Second Dimension is the length of the ColumnIntegrationSteps
         */
        [TestMethod]
        public void Wind_2D_length_201_Test()
        {
            // COL_STEPS
            Assert.AreEqual(101, Config.ColumnIntegrationSteps + 1);
            Assert.AreEqual(101, Wind[0].Length);
        }

        [TestMethod]
        public void WindIntervalTest()
        {
            // WIND_INTERVAL
            Assert.AreEqual(57.2, WindFactory.GetWindInterval(Config), 0.1);
        }

        [TestMethod]
        public void FirstWindValuesTest()
        {
            var windLocal = Wind[0][0];
            Assert.AreEqual(678, windLocal.GetWindHeight(), 0.000001);
            Assert.AreEqual(3.286950, windLocal.GetWindSpeed(), 0.000001);
            Assert.AreEqual(1.959229, windLocal.GetWindDir(), 0.000001);
        }

        [TestMethod]
        public void LastWindValuesTest()
        {
            var windLocal = Wind[0][100];
            Assert.AreEqual(6400, windLocal.GetWindHeight(), 0.000001);
            Assert.AreEqual(4.414636, windLocal.GetWindSpeed(), 0.000001);
            Assert.AreEqual(4.016226, windLocal.GetWindDir(), 0.000001);
        }
    }
}