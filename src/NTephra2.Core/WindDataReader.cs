using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Logging;

namespace NTephra2.Core
{
    public class WindDataReader
    {
        private static readonly ILog Logger = LogManager.GetLogger<WindDataReader>();
        private readonly string[] _windFileContent;

        public WindDataReader(FileSystemInfo windFile)
        {
            _windFileContent = File.ReadAllLines(windFile.FullName);
        }
        public WindDataReader(string[] windFileContent)
        {
            _windFileContent = windFileContent;
        }
        public WindData[] Read()
        {
            Logger.Info("Reading Wind Data");

            var result = _windFileContent
                .Where(configLine => !string.IsNullOrWhiteSpace(configLine))
                .Select(configLine => configLine.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries))
                .Select(ReadTokensIntoWind)
                .ToArray();

            Debug.Assert(result.Length > 0);

            return result;
        }
        private static WindData ReadTokensIntoWind(string[] tokens)
        {
            Debug.Assert(tokens.Length == 3);
            var wind = new WindData();
            wind.SetWindHeight(double.Parse(tokens[0]));
            wind.SetWindSpeed(double.Parse(tokens[1]));
            wind.SetWindDir(double.Parse(tokens[2]));

            //logger.info("Wind line: Height: {}\tSpeed: {}\tDir: {}", new Object[] { wind.getWindHeight(), wind.getWindSpeed(), wind.getWindDir() });
            return wind;
        }
    }
}
