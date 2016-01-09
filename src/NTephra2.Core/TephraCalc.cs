using System;
using Common.Logging;

namespace NTephra2.Core
{
    public class TephraCalc
    {
        private static readonly ILog Logger = LogManager.GetLogger<TephraCalc>();

        private Config _config;

        public TephraCalc(Config config)
        {
            _config = config;
        }

        /*
         * ---------------------------------------------------------------------
         * FUNCTION:tephra_calc.c
         * 
         * Purpose: This function calculates and returns the expected accumulation
         * of volcanic ash (kg/m2) at a specific geogrphic location (x,y) due to an
         * eruption with specific input parameters. These points may be random or on
         * a UTM grid (m)
         * 
         * This implementation accounts for variation in wind velocity with height.
         * The model is discretized w.r.t. height and particle size.
         * 
         * This function is called for each point (x,y,) If more than one eruption
         * is involved, for example in a probabilistic analysis, the function is
         * called for each set of eruption parameters.
         * 
         * INPUTS: ERUPTION *erupt: pointer to array of eruption parameters POINT
         * *pt: pointer to an array of location specific parameters, WIND *level:
         * pointer to a day of wind data : height asl in m; wind speed in ms-1 wind
         * direction in degrees N
         * 
         * OUTPUTs: the value of the mass accumulated at the input location
         * (northing, easting) in kg/m2
         * 
         * a distribution of particle sizes the exact number of binss (i.e. sizes)
         * and phi size used per bin is an integer and is determined by
         * (erupt->max_phi - erupt->min_phi) each bin accumulates phi sizes up to
         * its integer size ex. bin[0] holds grainsizes [min_phi to min_phi+1)
         * *************************************************************************
         */
        //void tephra_calc(ERUPTION *erupt, POINT *pt, WIND *level, STATS *stats) { /* tephra_calc starts ... */
        // TODO : I have added EruptionValues, could replace Eruption with EruptionValues only?
        public void Calculate(Eruption erupt, Point pt, Wind[] level, Stats stats, EruptionValues eruptionValues)
        {
            //logger.info("IN tephra_calc ...");

            /* Initialize mass to zero */
            pt.SetMass(0.0);
            //wind_sum_x = 0.0;
            //wind_sum_y = 0.0;

            /* Transform the volcano location coordinate to 0,0 */
            var newXspace = pt.GetNorthing() - erupt.VolcanoNorthing;
            var newYspace = pt.GetEasting() - erupt.VolcanoEasting;

            /* Interpolate to find the wind speed and direction below the height of the vent.
             * Find one average wind speed and wind direction between vent and grid elevation point. 
             * The first values in the wind array give the wind speed and direction at the
             * vent height.
             * */
            var layer = erupt.VentHeight - pt.GetElevation();

            var windspeed = (level[0].GetWindSpeed() * pt.GetElevation()) / erupt.VentHeight;
            var cosWind = Math.Cos(level[0].GetWindDir()) * windspeed;
            var sinWind = Math.Sin(level[0].GetWindDir()) * windspeed;

            var bin = -1;
            double min = 10e6, max = 0.0;

            //logger.info("Beginning integration loops ...");
            //for (i = 0; i < PART_STEPS; i++) { /* PART_STEPS_LOOP */
            for (var i = 0; i < eruptionValues.PartSteps; i++)
            { /* PART_STEPS_LOOP */
              //fall_time_adj = 0.0;
                var fallTimeAdj = 0.0;
                /* Accumulate the particle sizes into bins of whole numbered phi sizes */
                //if (!(i % 10)) { 
                if ((i % 10) == 0)
                {
                    bin++;
                    //fprintf(log_file, "PART_STEP=%d phi[%d] = %g\n", i, bin, pt->phi[bin]);
                    //logger.debug("PART_STEP=" + i + " phi[" + bin + "] = " + pt.getPhi()[bin]);
                }

                /* 
                 * Modify the total fall time of each particle size (i) 
                 * by the time that it takes particle size (i) to descend from vent height to the grid cell. 
                 * This is only necessary when the elevation of the grid cell < elevation of the vent.
                 * */
                if (layer > 0)
                {
                    //fall_time_adj = PartFallTime(erupt->vent_height, layer, T[i][0].ashdiam, T[i][0].part_density);
                    fallTimeAdj = eruptionValues.PartFallTime(erupt.VentHeight, layer, eruptionValues.GetT()[i, 0].GetAshDiam(), eruptionValues.GetT()[i, 0].GetPartDensity());
                    //fprintf(log_file, "%d %g %g\n",  i, layer, fall_time_adj) ;
                    //logger.debug("\tS=" + i + " layer=" + layer + " fall_time_adj=" + fall_time_adj);
                }

                //for (j = 0; j < COL_STEPS; j++) { /* COL_STEPS_LOOP */
                for (var j = 0; j < _config.ColumnIntegrationSteps; j++)
                { /* COL_STEPS_LOOP */
                  //total_fall_time = T[i][j].total_fall_time + fall_time_adj;
                    var totalFallTime = eruptionValues.GetT()[i, j].GetTotalFallTime() + fallTimeAdj;
                    //logger.debug("\tS=" + i + ","+ j + " total_fall_time=" + total_fall_time);
                    // fprintf(stderr, "%g %g %g ", T[i][j].total_fall_time, total_fall_time, fall_time_adj);
                    //logger.debug("\tPART_STEP=" + i + " layer=" + layer + " fall_time_adj=" + fall_time_adj);

                    /* Sum the adjustments (windspeed and wind_direction) 
                     * for each particle size  falling from each level.
                     */
                    /* removed 2 lines, 12-2-2010
                    wind_sum_x = cos_wind * fall_time_adj * windspeed;
                    wind_sum_y = sin_wind * fall_time_adj * windspeed;
                     */
                    /* change 2 lines, 12-2-2010 */
                    var windSumX = cosWind * fallTimeAdj;
                    var windSumY = sinWind * fallTimeAdj;

                    //logger.debug("\tPART_STEP=" + i + " wind_sum_x=" + wind_sum_x + " wind_sum_y=" + wind_sum_y);
                    /* Now add the summed adjustments to the already summed
                     * windspeeds and directions 
                     * and 
                     * Account for the wind:
                     * Find the average windspeed in the x and y directions
                     * over the total fall time.
                     * */
                    //average_windspeed_x = (T[i][j].wind_sum_x + wind_sum_x)/total_fall_time;
                    var averageWindspeedX = (eruptionValues.GetT()[i, j].GetWindSumX() + windSumX) / totalFallTime;

                    //average_windspeed_y = (T[i][j].wind_sum_y + wind_sum_y)/total_fall_time;
                    var averageWindspeedY = (eruptionValues.GetT()[i, j].GetWindSumY() + windSumY) / totalFallTime;

                    /* If zero, make windspeed a very small value (cannot divide by zero in next step) */
                    //if (!average_windspeed_x) average_windspeed_x = .001;
                    if (averageWindspeedX == 0) averageWindspeedX = .001;
                    //if (!average_windspeed_y) average_windspeed_y = .001;
                    if (averageWindspeedY == 0) averageWindspeedY = .001;

                    double averageWindDirection;
                    /* Find the average wind direction (direction of the velocity vector) */
                    if (averageWindspeedX < 0)
                    {
                        //average_wind_direction = atan(average_windspeed_y/average_windspeed_x ) + pi;
                        averageWindDirection = Math.Atan(averageWindspeedY / averageWindspeedX) + Config.Pi;
                    }
                    else {
                        //average_wind_direction = atan(average_windspeed_y/average_windspeed_x);
                        averageWindDirection = Math.Atan(averageWindspeedY / averageWindspeedX);
                    }
                    //logger.debug("\tPART_STEP=" + i + " average_wind_direction=" + average_wind_direction);
                    /* Find the average windspeed ( magnitude of the velocity vector) */
                    //average_windspeed = sqrt(average_windspeed_x*average_windspeed_x + average_windspeed_y*average_windspeed_y);
                    var averageWindspeed = Math.Sqrt(averageWindspeedX * averageWindspeedX + averageWindspeedY * averageWindspeedY);
                    //logger.debug("\tPART_STEP=" + i + " average_windspeed=" + average_windspeed);

                    if (totalFallTime > max) max = totalFallTime;
                    if (totalFallTime < min) min = totalFallTime;

                    /* calculate the value of sigma (dispersion) based on total_fall_time  
                     * to acct for the change in the shape of the column with ht - increasing radius 
                     */
                    //ht_above_vent = T[i][j].particle_ht - erupt->vent_height;
                    //double ht_above_vent = eruptionValues.getT()[i][j].getParticleHt() - erupt.getVentHeight();

                    double sigma;
                    /* falltime for fine particles */
                    //if (total_fall_time >= FALL_TIME_THRESHOLD) {
                    if (totalFallTime >= _config.FallTimeThreshold)
                    {
                        //sigma = EDDY_CONST_x_8_div_5 * pow((total_fall_time + T[i][j].plume_diffusion_fine_particle), 2.5);
                        sigma = (8.0 * _config.EddyDiff / 5.0) * Math.Pow((totalFallTime + eruptionValues.GetT()[i, j].GetPlumeDiffusionFineParticle()), 2.5);
                        //fprintf(stderr,"f");
                    }
                    else {
                        /* falltime for coarse particles */
                        //sigma = 4.0 * DIFFUSION_COEFFICIENT * (total_fall_time + T[i][j].plume_diffusion_coarse_particle);
                        sigma = 4.0 * _config.DiffusionCoefficient * (totalFallTime + eruptionValues.GetT()[i, j].GetPlumeDiffusionCoarseParticle());
                        //fprintf(stderr, "c");
                    }

                    var demon2 = Config.Pi * sigma;

                    /* Modify fall time by the variation of wind velocity with height */
                    //				demon3 = 
                    //				strat_average( average_wind_direction, 
                    //	      	             average_windspeed,             
                    //					             new_xspace, new_yspace, 
                    //					             total_fall_time,
                    //					             sigma); 
                    var demon3 = strat_average(averageWindDirection,
                            averageWindspeed,
                            newXspace, newYspace,
                            totalFallTime,
                            sigma);
                    /*
                                if (!demon2 || isnan(demon2) || isinf(demon2) || isnan(demon3) || isinf(demon3)) {
                                    fprintf(stderr, 
                            "[%d][%d] layer= %.1f totalfalltime=%g [falltimeadj=%g] demon1=%g demon2=%g demon3=%g sigma=%g\n",
                            i,j, layer,total_fall_time, fall_time_adj, T[i][j].demon1, demon2, demon3, sigma);
                            exit(-1);
                                }
                     */
                    //ash_fall = (T[i][j].demon1 / demon2) * demon3;
                    var ashFall = (eruptionValues.GetT()[i, j].GetDemon1() / demon2) * demon3;
                    //pt->mass += ash_fall;
                    pt.SetMass(pt.GetMass() + ashFall);
                    //pt->phi[bin] += ash_fall;
                    pt.GetPhi()[bin] += ashFall;
                    //fprintf(stderr, "\n");

                    //logger.debug("\tS=" + i + ", " + j + " layer=" + layer + "d1=" + eruptionValues.getT()[i][j].getDemon1() + " d2=" + demon2 + " d3=" + demon3 + " sigma=" + sigma + " ash_fall=" + ash_fall);
                }
                //logger.debug("S=" + i + " mass=" + pt.getMass() + " phi[" + bin + "]=" + pt.getPhi()[bin]);
            }
            //	#ifdef _PRINT
            //	  fprintf(log_file, "PART_STEP=%d phi[%d] = %g\n", i, bin, pt->phi[bin]);
            //	  fprintf(log_file, "OUT\n");
            //	#endif
            //stats->min_falltime = min;
            stats.SetMinFallTime(min);
            //stats->max_falltime = max;
            stats.SetMaxFallTime(max);
        }

        /* 
         * Function strat_average accounts for the variation in wind velocity
         * with height by using the average velocity value
         * 
         * exp[ -5{ (x'-ut)^2 + y'^2} / {8*pi*C(t+td)} ]
         * 
         * over the path of the particle as it falls from
         * its column release height to the ground.
         * 
         * u = wind velocity (m/s), varies with height
         * t = particle fall time
         * td = particle diffusion time
         * 
         * The Suzuki equation has been formulated s.t. the wind is in the x direction
         * We therefore need to transform the coordinates (xspace and yspace) to xprime
         * and yprime, with xprime increasing in the downwind direction:
         * x' = x cos a + y sin a
         * y' = y cos a - x sin a
         * */
        double strat_average(
                double averageWindDirection,
                double averageWindspeed,
                double xspace,
                double yspace,
                double totalFallTime,
                double sigma)
        {

            var temp0 = Math.Cos(averageWindDirection);
            var temp1 = Math.Sin(averageWindDirection);

            var xprime = xspace * temp0 + yspace * temp1;
            var yprime = yspace * temp0 - xspace * temp1;

            var temp2 = xprime - averageWindspeed * totalFallTime;
            var demon1 = temp2 * temp2 + yprime * yprime;
            /* where sigma is calculated for the total fall time */
            var demon3 = Math.Exp(-demon1 / sigma);
            return demon3;

        }
    }
}
