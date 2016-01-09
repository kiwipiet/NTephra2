using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Logging;

namespace NTephra2.Core
{
    public class GridReader
    {
        private static readonly ILog Logger = LogManager.GetLogger<GridReader>();
        private readonly FileInfo _pointFile;

        public GridReader(FileInfo pointFile)
        {
            _pointFile = pointFile;
        }
        public Point[] Read()
        {
            Logger.InfoFormat("Reading {0}", _pointFile);

            var result = File.ReadAllLines(_pointFile.FullName)
                .Where(configLine => !string.IsNullOrWhiteSpace(configLine))
                .Select(configLine => configLine.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries))
                .Select(ReadTokensIntoPoint)
                .ToArray();

            Debug.Assert(result.Length > 0);

            return result;
        }
        private static Point ReadTokensIntoPoint(string[] tokens)
        {
            Debug.Assert(tokens.Length == 3);

            var point = new Point();
            point.SetEasting(double.Parse(tokens[0]));
            point.SetNorthing(double.Parse(tokens[1]));
            point.SetElevation(double.Parse(tokens[2]));
            return point;
        }
    }
}
