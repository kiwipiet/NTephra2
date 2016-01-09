using Common.Logging;

namespace NTephra2.Core
{
    public class WindFactory
    {
        private static readonly ILog Logger = LogManager.GetLogger<WindFactory>();

        /**************************************************************
        FUNCTION:  get_wind
        DESCRIPTION:  This function reads wind data into the
        WIND array. Each node stores all of the wind data. 

         * @throws IOException 
        ***************************************************************/
        //	int get_wind(FILE *in) {
        public Wind[][] get_wind(Config config, WindData[] windData)
        {
            //	  int i=0, j=0, ret;
            //	  char line[MAX_LINE];
            //	  double wind_height, wind_dir, windspeed, dir0, ht0, sp0;
            //	  double level;
            //	  
            //	#ifdef _PRINT
            //	  fprintf(log_file,"ENTER[get_wind].\n");
            //	#endif
            //	  
            //	  WIND_INTERVAL = (PLUME_HEIGHT - VENT_ELEVATION)/COL_STEPS;
            var windInterval = GetWindInterval(config);
            //	  
            //	  W = (WIND**)GC_MALLOC(WIND_DAYS * sizeof(WIND *));
            var w = GetWindArray(config);
            //	  if (W == NULL) {
            //	    fprintf(stderr, "Cannot malloc memory for wind columns:[%s]\n", strerror(errno));
            //	    return -1;
            //	  } else {
            //	    for (i=0; i < WIND_DAYS; i++) {
            //	      W[i] = (WIND *)GC_MALLOC((COL_STEPS+1) * sizeof(WIND));
            //	      if (W[i] == NULL) {
            //		fprintf(stderr, "Cannot malloc memory for wind rows %d:[%s]\n", i, strerror(errno));
            //		return -1;
            //	    }
            //	  }
            for (var i = 0; i < config.WindDays; i++)
            {
                /* start at the vent */
                //	    level = VENT_ELEVATION;
                var level = config.Vent.Elevation;
                //	    
                /* Do for each column step */
                /* j = 0 is for the interval between the vent and the ground.
                 * Here we set the wind speed and direction to be at the level of the vent;
                 * The values used in the calculations change for each location and are 
                 * set in the tephra_calc routine when the point elevation is known. 
                 * The last interval ends at the top of the column. 
                 */
                //	  for (j=0; j <= COL_STEPS; j++) { 
                for (var j = 0; j <= config.ColumnIntegrationSteps; j++)
                {
                    //W[i][j] = new Wind();
                    /* my own */
                    var windInstance = w[i][j];
                    windInstance.SetDay(i + 1);
                    //	    W[i][j].wind_height = 0.0;
                    //	    ht0 = 0.0;
                    var ht0 = 0.0;
                    //	    dir0 = 0.0;
                    var dir0 = 0.0;
                    //	    sp0 = 0.0;
                    var sp0 = 0.0;

                    /* Find wind elevation just greater than current level */
                    /* Start scanning the wind file for the best match.
                     * Each new level starts scanning the file from the beginning.
                     */
                    //	    while (NULL != fgets(line, MAX_LINE, in)) {
                    foreach (var data in windData)
                    {
                        //		    if (line[0] == '#' || strlen(line) < WIND_COLUMNS) continue;
                        //		    else {
                        //		      while (ret = sscanf(line,
                        //				      "%lf %lf %lf",
                        //				      &wind_height,
                        //				      &windspeed,
                        //				      &wind_dir), ret != 3) { 
                        //		    
                        //		        if (ret == EOF && errno == EINTR) continue;
                        //		        
                        //		        fprintf(stderr, 
                        //		        "[line=%d,ret=%d] Did not read in 3 parameters:[%s]\n", 
                        //		        i+1,ret, strerror(errno));
                        //		        
                        //		        return -1;
                        //		      }
                        //		    }
                        //		    
                        /* This is the case where we find the first height that is equal to
                         * or greater that the level that we are assigning.
                         */
                        //		    if (wind_height >= level) {
                        if (data.GetWindHeight() >= level)
                        {
                            //		      if(wind_height == level) {
                            if (data.GetWindHeight() == level)
                            {
                                //		        W[i][j].wind_dir = wind_dir;
                                windInstance.SetWindDir(data.GetWindDir());
                                //		        W[i][j].windspeed = windspeed;
                                windInstance.SetWindSpeed(data.GetWindSpeed());
                                //		        
                                //		      } else { /* interpolate */
                            }
                            else { /* interpolate */
                                   //		        W[i][j].wind_dir = 
                                   //		        ((wind_dir - dir0) * (level - ht0) / (wind_height - ht0)) + dir0;
                                windInstance.SetWindDir(((data.GetWindDir() - dir0) * (level - ht0) / (data.GetWindHeight() - ht0)) + dir0);
                                //		      
                                //		        W[i][j].windspeed = 
                                //		        ((windspeed - sp0) * (level - ht0) / (wind_height - ht0)) + sp0;
                                windInstance.SetWindSpeed(((data.GetWindSpeed() - sp0) * (level - ht0) / (data.GetWindHeight() - ht0)) + sp0);
                                //		      }
                            }
                            //		      W[i][j].wind_height = level;
                            windInstance.SetWindHeight(level);
                            //		      fprintf(log_file, 
                            //		      "%f %f %f\n", 
                            //		      W[i][j].wind_height, W[i][j].windspeed, W[i][j].wind_dir);
                            //		      W[i][j].wind_dir *= DEG2RAD; /* change to radians */
                            windInstance.SetWindDir(windInstance.GetWindDir() * Config.Deg2Rad);
                            //		      break; /* ready to rescan the file for a match for the next level */
                            break;
                            //		    }
                        }
                        /* This is the case where the scanned height is less than the level
                         * we are assigning.
                         */
                        //		    else {
                        else {
                            //		      /* Maintain the scanned values for possible interpolation 
                            //		       * at the next level.
                            //		       */
                            //		      ht0 = wind_height;
                            ht0 = data.GetWindHeight();
                            //		      dir0 = wind_dir;
                            dir0 = data.GetWindDir();
                            //		      sp0 = windspeed;
                            sp0 = data.GetWindSpeed();
                            //		    }
                        }
                        //		  }
                    }
                    /* If we finish scanning the file and all heights are below the level we are
                     * currently assigning, then just use the direction and speed
                     * at the upper-most height.
                     */
                    //		  if (!W[i][j].wind_height) {
                    if (windInstance.GetWindHeight() == 0.0)
                    {
                        //		    W[i][j].wind_height = level;
                        windInstance.SetWindHeight(level);
                        //		    W[i][j].windspeed = sp0;
                        windInstance.SetWindSpeed(sp0);
                        //		    W[i][j].wind_dir = dir0;
                        windInstance.SetWindDir(dir0);
                        //		  }
                    }
                    //		  /* Go to the next column height */
                    //		  rewind(in); 
                    //		  level += WIND_INTERVAL; 
                    level += windInterval;

                    //logger.info("Wind Instance: Day: {}\t Height: {}\tSpeed: {}\tDir: {}", new Object[] { windInstance.getDay(), windInstance.getWindHeight(), windInstance.getWindSpeed(), windInstance.getWindDir() });
                }
            }
            //
            //	  
            //	#ifdef _PRINT
            //	  fprintf(log_file, "\tRead %d wind days with %d wind levels per day.\n", i, j);
            Logger.InfoFormat("Read {0} wind days with {1} wind levels per day.", config.WindDays, config.ColumnIntegrationSteps);
            //	  fprintf(log_file, "EXIT[get_wind].\n");
            //	#endif	  	  
            //	  return 0;
            return w;
        }
        public static double GetWindInterval(Config config)
        {
            return (config.Eruption.PlumeHeight - config.Vent.Elevation) / config.ColumnIntegrationSteps;
        }
        private Wind[][] GetWindArray(Config config)
        {
            var winds = new Wind[config.WindDays][];
            for (var i = 0; i < config.WindDays; i++)
            {
                winds[i] = new Wind[config.ColumnIntegrationSteps + 1];
                for (var j = 0; j <= config.ColumnIntegrationSteps; j++)
                {
                    winds[i][j] = new Wind();
                }
            }
            return winds;
        }
    }
}
