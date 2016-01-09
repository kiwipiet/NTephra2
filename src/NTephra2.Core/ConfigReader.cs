using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;

namespace NTephra2.Core
{
    public class ConfigReader
    {

        private static readonly ILog Logger = LogManager.GetLogger<ConfigReader>();

        private readonly string[] _configFileContent;

        public ConfigReader(FileSystemInfo configFile)
        {
            _configFileContent = File.ReadAllLines(configFile.FullName);
        }

        public ConfigReader(string[] configFileContent)
        {
            _configFileContent = configFileContent;
        }
        public Config Read()
        {
            Logger.Info("Reading Configuration");

            var result = new Config();
            foreach (var tokens in _configFileContent
                .Where(configLine => !string.IsNullOrWhiteSpace(configLine))
                .Select(configLine => configLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                ReadTokensIntoConfig(tokens, result);
            }
            return result;
        }

        private static void ReadTokensIntoConfig(IReadOnlyList<string> tokens, Config config)
        {
            if (tokens.Count != 2)
            {
                return;
            }
            var configName = tokens[0];
            //logger.info("Config: {}: {}", configName, tokens[1]);
            switch (configName)
            {
                case "PLUME_HEIGHT":
                    config.Eruption.PlumeHeight = double.Parse(tokens[1]);
                    break;

                case "ERUPTION_MASS":
                    config.Eruption.EruptionMass = double.Parse(tokens[1]);
                    break;

                case "MAX_GRAINSIZE":
                    config.Eruption.GrainSize.Max = double.Parse(tokens[1]);
                    break;

                case "MIN_GRAINSIZE":
                    config.Eruption.GrainSize.Min = double.Parse(tokens[1]);
                    break;

                case "MEDIAN_GRAINSIZE":
                    config.Eruption.GrainSize.Median = double.Parse(tokens[1]);
                    break;

                case "STD_GRAINSIZE":
                    config.Eruption.GrainSize.Standard = double.Parse(tokens[1]);
                    break;

                case "VENT_EASTING":
                    config.Vent.Easting = double.Parse(tokens[1]);
                    break;

                case "VENT_NORTHING":
                    config.Vent.Northing = double.Parse(tokens[1]);
                    break;

                case "VENT_ELEVATION":
                    config.Vent.Elevation = double.Parse(tokens[1]);
                    break;

                case "EDDY_CONST":
                    config.EddyDiff = double.Parse(tokens[1]);
                    break;

                case "DIFFUSION_COEFFICIENT":
                    config.DiffusionCoefficient = double.Parse(tokens[1]);
                    break;

                case "FALL_TIME_THRESHOLD":
                    config.FallTimeThreshold = double.Parse(tokens[1]);
                    break;

                case "LITHIC_DENSITY":
                    config.Density.LithicDensity = double.Parse(tokens[1]);
                    break;

                case "PUMICE_DENSITY":
                    config.Density.PumiceDensity = double.Parse(tokens[1]);
                    break;

                case "COL_STEPS":
                    config.ColumnIntegrationSteps = int.Parse(tokens[1]);
                    break;

                case "PLUME_MODEL":
                    config.Plume.PlumeModel = (PlumeModel)Enum.Parse(typeof(PlumeModel), tokens[1]);
                    break;

                case "PLUME_RATIO":
                    config.Plume.Ratio = double.Parse(tokens[1]);
                    break;

                case "WIND_DAYS":
                    config.WindDays = int.Parse(tokens[1]);
                    break;

                case "WIND_COLUMNS":
                    config.WindColumns = int.Parse(tokens[1]);
                    break;

                default:
                    Logger.InfoFormat("Unknown Configuration Settings: {0}", configName);
                    break;
            }
        }
    }
}
