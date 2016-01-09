using System;
using System.IO;
using CLAP;
using Common.Logging;
using NTephra2.Core;

namespace NTephra2
{
    public class TephraMain
    {
        private static readonly ILog Logger = LogManager.GetLogger<TephraMain>();

        private static readonly Config DefaultConfig = GetDefaultConfig();

        [Verb(IsDefault = true)]
        public void Main(
            [Aliases("c")]
            [Description("Configuration File")]
            [DefaultValue(@".\Samples\CerroNegro.conf")]
            [Required]
            string configFileName,
            [Aliases("p")]
            [Description("Point File")]
            [DefaultValue(@".\Samples\CerroNegro.pos")]
            [Required]
            string pointFileName,
            [Aliases("w")]
            [Description("Wind File")]
            [DefaultValue(@".\Samples\CerroNegro.wind")]
            [Required]
            string windFileName)
        {
            _configFileInfo = new FileInfo(configFileName);
            _pointFileInfo = new FileInfo(pointFileName);
            _windFileInfo = new FileInfo(windFileName);

            var config = getConfiguration();
            PrintConfig(config);
            var points = GetPoints(_pointFileInfo);
            PrintPoints(points);
            var wind = GetWind(config, _windFileInfo);
            PrintWind(wind);

            var eruption = new Eruption(config);
            PrintEruption(eruption);

            var eruptionValues = new EruptionValues(eruption, wind[0], config);

            /* Calculating an accumulation map */
            var calc = new TephraCalc(config);

            //for (i = 0;i < local_n; i++) {  /* For each location */
            for (var i = 0; i < points.Length; i++)
            {  /* For each location */
               //  /* Note: W[j]  means that if there are multiple eruptions, 
               //  there should be multiple WIND_DAYS in the wind.in file, 
               //  1 WIND_DAY for each eruption line */
               //  tephra_calc(erupt+j, pt+i, W[j], &stats);
                Logger.InfoFormat("points[{0} of {1}] ...", i, points.Length);
                var stats = new Stats(); // TODO : There seems no point to this object!!
                var pt = points[i];
                calc.Calculate(eruption, pt, wind[0], stats, eruptionValues);
                //  (pt+i)->cum_mass += (pt+i)->mass;
                pt.SetAccumulateMass(pt.GetAccumulateMass() + pt.GetMass());
                //}
            }
            CreateResultFile(eruption, points);
        }
        private FileInfo _configFileInfo;
        private FileInfo _pointFileInfo;
        private FileInfo _windFileInfo;
        //private FileInfo configFileInfo = getResourceFile("CerroNegro.conf");
        //@Option(name= "-p", usage= "Point File", required= false)
        //private File pointFile = getResourceFile("CerroNegro.pos");
        //@Option(name= "-w", usage= "Wind File", required= false)
        //private File windFile = getResourceFile("CerroNegro.wind");

        // receives other command line parameters than options
        //@Argument
        //private List<String> arguments = new ArrayList<String>();

        private static Config GetDefaultConfig()
        {
            var defaultConfig = new Config();
            return defaultConfig;
        }
        private static void CreateResultFile(Eruption eruption, Point[] points)
        {
            /* Here I am assuming that each eruptive senario has the same min and max particle size range
               So, the grainsize distribution contains the same number of bins. The amount in each bin accumulates
               between eruptive senarios.
            */
            try
            {
                var outFile = new FileInfo("tephra2_output.txt");
                if (outFile.Exists)
                {
                    outFile.Delete();
                }
                //PrintWriter output = new PrintWriter(new BufferedWriter(new FileWriter(outFile, true)));
                using (var output = outFile.CreateText())
                {
                    //phi_bins = (int)((erupt+j)->max_phi - (erupt+j)->min_phi);
                    var phiBins = (int)(eruption.MaxPhi - eruption.MinPhi);
                    //fprintf(stdout, "#Easting Northing Elev. Mass/Area  ");
                    Console.Write("#Easting Northing Elev. Mass/Area  ");
                    output.Write("#Easting Northing Elev. Mass/Area  ");
                    //for (bin = 0; bin < phi_bins; bin++) 
                    for (var bin = 0; bin < phiBins; bin++)
                    {
                        //  fprintf(stdout,"[%d->%d) ", 
                        //	(int)((erupt+j)->min_phi)+bin, 
                        //	(int)((erupt+j)->min_phi)+bin+1);
                        Console.Write("[" + ((int)(eruption.MinPhi) + bin) + "->" + ((int)(eruption.MinPhi) + bin + 1) + ") ");
                        output.Write("[" + ((int)(eruption.MinPhi) + bin) + "->" + ((int)(eruption.MinPhi) + bin + 1) + ") ");
                    }
                    //fprintf(stdout,"\n");
                    Console.WriteLine();
                    output.WriteLine();

                    //
                    ////		fprintf(stderr, "\nPART_STEPS=%d phi_bins=%d\n", PART_STEPS, phi_bins);
                    //for (i=0; i < num_pts; i++) {
                    foreach (var pt in points)
                    {
                        //  fprintf(stdout, "%.0f %.0f %.0f %.2g  ", 
                        //	(pt+i)->easting, 				
                        Console.Write("{0:0}", pt.GetEasting());
                        output.Write("{0:0}", pt.GetEasting());
                        Console.Write(" ");
                        output.Write(" ");
                        //	(pt+i)->northing,
                        Console.Write("{0:0}", pt.GetNorthing());
                        output.Write("{0:0}", pt.GetNorthing());
                        Console.Write(" ");
                        output.Write(" ");
                        //	(pt+i)->elevation, 
                        Console.Write("{0:0}", pt.GetElevation());
                        output.Write("{0:0}", pt.GetElevation());
                        Console.Write(" ");
                        output.Write(" ");
                        //	(pt+i)->cum_mass);
                        Console.Write("{0:G2}", pt.GetAccumulateMass());
                        output.Write("{0:G2}", pt.GetAccumulateMass());
                        Console.Write(" ");
                        output.Write(" ");
                        //  for (bin=0; bin < phi_bins; bin++) {
                        for (var bin = 0; bin < phiBins; bin++)
                        {
                            //  	val = ((pt+i)->phi[bin]/(pt+i)->cum_mass) * 100.0;
                            var val = (pt.GetPhi()[bin] / pt.GetAccumulateMass()) * 100.0;
                            //    fprintf(stdout, "%.2g ", val);
                            if (val == 0)
                            {
                                Console.Write("0.0");
                                output.Write("0.0");
                            }
                            else
                            {
                                Console.Write("{0:G2}", val);
                                output.Write("{0:G2}", val);
                            }
                            Console.Write(" ");
                            output.Write(" ");
                            //  }
                        }
                        //  fprintf(stdout, "\n");
                        Console.WriteLine();
                        output.WriteLine();
                    }
                }
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
                Logger.Error("Error generating output file", e);
            }
        }
        private static void PrintEruption(Eruption eruption)
        {
            Console.WriteLine("Eruption:");
            Console.WriteLine("\tVolcanoEasting: " + eruption.VolcanoEasting);
            Console.WriteLine("\tVolcanoNorthing: " + eruption.VolcanoNorthing);
            Console.WriteLine("\tTotalAshMass: " + eruption.TotalAshMass);
            Console.WriteLine("\tMinPhi: " + eruption.MinPhi);
            Console.WriteLine("\tMaxPhi: " + eruption.MaxPhi);
            Console.WriteLine("\tMeanPhi: " + eruption.MeanPhi);
            Console.WriteLine("\tSigmaPhi: " + eruption.SigmaPhi);
            Console.WriteLine("\tVentHeight: " + eruption.VentHeight);
            Console.WriteLine("\tMaxPlumeHeight: " + eruption.MaxPlumeHeight);
        }
        private static void PrintWind(Wind[][] wind)
        {
            Console.WriteLine("Wind:");
            Console.WriteLine("\tDays: " + wind.Length);
            for (var i = 0; i < wind.Length; i++)
            {
                Console.WriteLine("\t\tDay: " + i);
                Console.WriteLine("\t\tLevels: " + wind[i].Length);
            }
        }
        private static Wind[][] GetWind(Config config, FileSystemInfo windFile)
        {
            var configReader = new WindDataReader(windFile);
            var wind = new WindFactory();
            try
            {
                return wind.get_wind(config, configReader.Read());
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
                Logger.Error("Error Processing Wind File", e);
            }
            return null;
        }
        private static Point[] GetPoints(FileSystemInfo pointFile)
        {
            var reader = new PointReader(pointFile);
            try
            {
                return reader.Read();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
                Logger.Error("Error Processing Points File", e);
            }
            return null;
        }
        private static void PrintPoints(Point[] points)
        {
            Console.WriteLine("Points:");
            Console.WriteLine("\tCount:" + points.Length);
        }

        private Config getConfiguration()
        {
            return _configFileInfo == null ? DefaultConfig : getConfiguration(_configFileInfo);
        }

        private static Config getConfiguration(FileSystemInfo configFile)
        {
            var configReader = new ConfigReader(configFile);
            try
            {
                return configReader.Read();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                //e.printStackTrace();
                Logger.Error("Error Processing Config File", e);
            }
            return null;
        }

        private static void PrintConfig(Config config)
        {
            Console.WriteLine("Config:");
            Console.WriteLine("\tAirDensity:" + config.AirDensity);
            Console.WriteLine("\tAirViscosity:" + config.AirViscosity);
            Console.WriteLine("\tColumnIntegrationSteps:" + config.ColumnIntegrationSteps);
            Console.WriteLine("\tDiffusionCoefficient:" + config.DiffusionCoefficient);
            Console.WriteLine("\tEddyDiff:" + config.EddyDiff);
            Console.WriteLine("\tFallTimeThreshold:" + config.FallTimeThreshold);
            Console.WriteLine("\tGravity:" + config.Gravity);
            Console.WriteLine("\tPlume:");
            Console.WriteLine("\t\tPlumeModel :" + config.Plume.PlumeModel);
            Console.WriteLine("\t\tRatio      :" + config.Plume.Ratio);
            Console.WriteLine("\tEruption:");
            Console.WriteLine("\t\tPlumeHeight  : " + config.Eruption.PlumeHeight);
            Console.WriteLine("\t\tEruptionMass : " + config.Eruption.EruptionMass);
            Console.WriteLine("\t\tGrainSize:");
            Console.WriteLine("\t\t\tMax      : " + config.Eruption.GrainSize.Max);
            Console.WriteLine("\t\t\tMin      : " + config.Eruption.GrainSize.Min);
            Console.WriteLine("\t\t\tMedian   : " + config.Eruption.GrainSize.Median);
            Console.WriteLine("\t\t\tStandard : " + config.Eruption.GrainSize.Standard);
            Console.WriteLine("\tVent:");
            Console.WriteLine("\t\tEasting   : " + config.Vent.Easting);
            Console.WriteLine("\t\tNorthing  : " + config.Vent.Northing);
            Console.WriteLine("\t\tElevation : " + config.Vent.Elevation);
            Console.WriteLine("\tDensity:");
            Console.WriteLine("\t\tLithicDensity : " + config.Density.LithicDensity);
            Console.WriteLine("\t\tPumiceDensity : " + config.Density.PumiceDensity);
        }

        private void OutputArguments()
        {
            Console.WriteLine("Tephra was configured with...");
            Logger.InfoFormat("configFile    : {0}", _configFileInfo);
            Console.WriteLine("\tconfigFile    : " + _configFileInfo);
            if (_configFileInfo == null)
            {
                Console.WriteLine("\t\tUsing default configuration.");
            }
            Logger.InfoFormat("pointFile    : {0}", _pointFileInfo);
            Console.WriteLine("\tpointFile    : " + _pointFileInfo);
        }

    }
}
