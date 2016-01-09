using System.IO;
using NTephra2.Core;

namespace NTephra2.Tests
{
    public static class TestResources
    {
        public static Config GetConfiguration(string[] configFileContent)
        {
            var configReader = new ConfigReader(configFileContent);
            try
            {
                return configReader.Read();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
            }
            return null;
        }

        public static Point[] GetPoints(string[] configFileContent)
        {
            var configReader = new PointReader(configFileContent);
            try
            {
                return configReader.Read();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
            }
            return null;
        }

        public static WindData[] GetWindData(string[] configFileContent)
        {
            var configReader = new WindDataReader(configFileContent);
            try
            {
                return configReader.Read();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
            }
            return null;
        }

        public static Wind[][] GetWind(string[] configFileContent, Config config)
        {
            var wind = new WindFactory();
            try
            {
                return wind.get_wind(config, GetWindData(configFileContent));
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
            }
            return null;
        }
    }
}