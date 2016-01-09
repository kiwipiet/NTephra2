using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Logging;

namespace NTephra2.Core
{
    public class PointReader
    {
        private static readonly ILog Logger = LogManager.GetLogger<PointReader>();
        private readonly string[] _pointFileData;

        public PointReader(FileSystemInfo pointFile)
        {
            _pointFileData = File.ReadAllLines(pointFile.FullName);
        }
        public PointReader(string[] pointFileData)
        {
            _pointFileData = pointFileData;
        }
        public Point[] Read()
        {
            Logger.Info("Reading Point Data");

            var result = _pointFileData
                .Where(configLine => !string.IsNullOrWhiteSpace(configLine))
                .Select(configLine => configLine.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries))
                .Select(ReadTokensIntoWind)
                .ToList();

            Debug.Assert(result.Count > 0);

            return result.ToArray();
        }
        private static Point ReadTokensIntoWind(IReadOnlyList<string> tokens)
        {
            Debug.Assert(tokens.Count == 3);

            var point = new Point();
            point.SetEasting(double.Parse(tokens[0]));
            point.SetNorthing(double.Parse(tokens[1]));
            point.SetElevation(double.Parse(tokens[2]));
            return point;
        }
    }

}
