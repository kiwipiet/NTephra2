using System;
using System.Diagnostics;
using Common.Logging;

namespace NTephra2.Core
{
    /* The following parameters are the properties of a eruption
     * each eruption must have all of these parameters defined:
     *
     * erupt->total_ash_mass is the total amount of ash erupted by
     * the volcano over the course of the entire eruption or calculation period
     * erupt->max_part_size is the maximum particle diameter considered
     * in the calculation. This is input in phi units (so it will likely be
     * a negative number like -5 and appear to be less than min_part_size)
     * erupt->min_part_size is the minimum particle diameter condsidered in the
     * calculation. This input is in phi units.
     *
     * Note: erupt->max/min_part_size are used to set the limits of integration
     * on the calculation. Particles outside this range are not considered at all.
     *
     * erupt->part_mean_size is the mean particle diameter erupted in phi units
     * erupt->part_sigma_size is the standard deviation in particle diameter in phi units
     * erupt-> vent_height is the elevation of the vent m.a.s.l. in meters
     * erupt->max_column_height is the eruption column height m.a.s.l. 
     * (not used) erupt->column_beta is the shape factor governing 
     * the particle size distribution in the eruption column. 
     */
    public class EruptionValues
    {
        private static readonly ILog Logger = LogManager.GetLogger<EruptionValues>();

        // TABLE **T
        private Table[,] _table;

        private readonly Config _config;
        private const double LithicDiameterThreshold = 7.0;
        private const double PumiceDiameterThreshold = -1.0;

        public EruptionValues(Eruption erupt, Wind[] wind, Config config)
        {
            Guard.NotNull(erupt, "erupt");
            Guard.NotNull(wind, "wind");
            Guard.NotNull(config, "config");

            _config = config;
            SetEruptionValues(erupt, wind, config);
        }

        // void set_eruption_values(ERUPTION *erupt, WIND *wind) { /* set_eruption_values */
        private void SetEruptionValues(Eruption erupt, Wind[] wind, Config config)
        {
            //PART_STEPS = (erupt->max_phi - erupt->min_phi) * 10;
            PartSteps = (int)Math.Abs((erupt.MaxPhi - erupt.MinPhi) * 10);

            //threshold = erupt->vent_height + (PLUME_RATIO * (erupt->max_plume_height - erupt->vent_height));
            Threshold = erupt.VentHeight + (config.Plume.Ratio * (erupt.MaxPlumeHeight - erupt.VentHeight));
            //SQRT_TWO_PI = sqrt(2.0 * pi); /* new line, 12-2-2010 */
            SqrtTwoPi = Math.Sqrt(2.0 * Config.Pi);

            //BETA_x_SQRT_TWO_PI = erupt->column_beta * SQRT_TWO_PI;
            BetaSqrtTwoPi = erupt.ColumnBeta * SqrtTwoPi;

            //TWO_BETA_SQRD = 2.0 * erupt->column_beta * erupt->column_beta;
            TwoBetaSqrd = 2.0 * erupt.ColumnBeta * erupt.ColumnBeta;

            //PDF_GRAINSIZE_DEMON1 = 1.0 / (2.506628 * erupt->sigma_phi); /* changed 12-02-2010 */
            PdfGrainSizeDemon1 = 1.0 / (2.506628 * erupt.SigmaPhi);

            //TWO_x_PART_SIGMA_SIZE = 2.0 * erupt->sigma_phi * erupt->sigma_phi;
            TwoPartSigmaSize = 2.0 * erupt.SigmaPhi * erupt.SigmaPhi;

            /*define the limits of integration */
            //ht_section_width = erupt->max_plume_height - erupt->vent_height; 
            var htSectionWidth = erupt.MaxPlumeHeight - erupt.VentHeight;
            //part_section_width = erupt->max_phi - erupt->min_phi;
            var partSectionWidth = erupt.MaxPhi - erupt.MinPhi;
            //ht_step_width = ht_section_width / (double)COL_STEPS;
            var htStepWidth = htSectionWidth / config.ColumnIntegrationSteps;
            //part_step_width = part_section_width / (double)PART_STEPS;
            var partStepWidth = partSectionWidth / PartSteps;

            /* steps for nomalization of probabilities */
            var totalPCol = GetTotalPCol(erupt, config, htSectionWidth, htStepWidth);

            var totalPPart = GetTotalPPart(erupt, partStepWidth);

            /* Normalization constant */
            var totalP = GetTotalP(totalPCol, totalPPart);

            /* End of normalization steps */

            /* Dynamically allocated table for storing integration data.
             * Used in the double integration steps below for each point considered.
             * */
            _table = CreateTableArray();

            double pmin = 10e6, pmax = 0.0;

            Logger.InfoFormat("\tpmax={0}", pmax);
            Logger.InfoFormat("\tpmin={0}", pmin);

            /* Start with the maximum particle size */
            //y = (erupt)->min_phi
            var y = erupt.MinPhi;
            for (var i = 0; i < PartSteps; i++)
            { /* PART_STEPS_LOOP */
                var partStepTable = _table[i, 0];
                //T[i][0].part_density  =  ParticleDensity(y);    
                partStepTable.SetPartDensity(ParticleDensity(y));
                //T[i][0].ashdiam = phi2m(y);
                partStepTable.SetAshDiam(Phi2M(y));

                /* the expected fraction of particles of this size based on given mean and std deviation */
                var partProb = pdf_grainsize(erupt.MeanPhi, y, partStepWidth);
                var cumFallTime = 0.0;
                var windX = 0.0;
                var windY = 0.0;

                /* Start at the height of the vent */
                var particleHt = erupt.VentHeight;
                for (var j = 0; j < config.ColumnIntegrationSteps; j++)
                { /* COL_STEPS_LOOP */
                    var table = _table[i, j];
                    /* define the small slice dz */
                    particleHt += htStepWidth;

                    /* 
                     * Calculate the time it takes a particle to fall from its release point
                     * in the column to the next column release point.
                     * */
                    //T[i][j].fall_time = PartFallTime(particle_ht, ht_step_width, T[i][0].ashdiam, T[i][0].part_density); 
                    table.SetFallTime(PartFallTime(particleHt, htStepWidth, partStepTable.GetAshDiam(), partStepTable.GetPartDensity()));

                    /* Particle diffusion time (seconds) */
                    //ht_above_vent = particle_ht - erupt->vent_height;
                    var htAboveVent = particleHt - erupt.VentHeight;

                    var temp = 0.2 * htAboveVent * htAboveVent;
                    //T[i][j].plume_diffusion_fine_particle = pow(temp, 0.4); /* 0.4 = 2.0/5.0 */
                    table.SetPlumeDiffusionFineParticle(Math.Pow(temp, 0.4));

                    //T[i][j].plume_diffusion_coarse_particle = 0.0032 * (ht_above_vent *  ht_above_vent) / DIFFUSION_COEFFICIENT; 
                    table.SetPlumeDiffusionCoarseParticle(0.0032 * (htAboveVent * htAboveVent) / config.DiffusionCoefficient);

                    /* 
                     * Sum the windspeed and wind_direction for each particle size 
                     * falling from each level. In the wind array, the first wind level
                     * gives wind speed and direction at the vent height. 
                     * Start with the next wind level, 
                     * so that we are using the wind speed and direction 
                     * starting from one step above the vent. 
                     * */

                    var windNextLevel = wind[j + 1];
                    //wind_x += T[i][j].fall_time * wind[j+1].windspeed * cos(wind[j+1].wind_dir);
                    windX += table.GetFallTime() * windNextLevel.GetWindSpeed() * Math.Cos(windNextLevel.GetWindDir());
                    //wind_y += T[i][j].fall_time * wind[j+1].windspeed * sin(wind[j+1].wind_dir);
                    windY += table.GetFallTime() * windNextLevel.GetWindSpeed() * Math.Sin(windNextLevel.GetWindDir());

                    //T[i][j].wind_sum_x = wind_x;
                    table.SetWindSumX(windX);
                    //T[i][j].wind_sum_y = wind_y;
                    table.SetWindSumY(windY);

                    /* 
                     * Accumulate the time it takes each particle size to descend
                     * from its release point down
                     * to its final resting place.This part of the code just
                     * calculates the fall_time from the release point to the
                     * height of the vent.
                     * The time it takes a particle to fall from the vent height
                     * to a grid cell will be calculated later.
                     * */
                    //cum_fall_time += T[i][j].fall_time;
                    cumFallTime += table.GetFallTime();
                    //T[i][j].total_fall_time = cum_fall_time;
                    table.SetTotalFallTime(cumFallTime);
                    //if (T[i][j].total_fall_time > pmax) pmax = T[i][j].total_fall_time;
                    if (cumFallTime > pmax)
                    {
                        pmax = table.GetTotalFallTime();
                    }
                    else
                    //if (T[i][j].total_fall_time < pmin) pmin = T[i][j].total_fall_time;
                    if (cumFallTime < pmin)
                    {
                        pmin = table.GetTotalFallTime();
                    }

                    /* the probability that a given grainsize will be released from a given height */
                    //col_prob = (*pdf)(particle_ht, j, erupt->column_beta, erupt->max_plume_height);
                    var colProb = config.Plume.PlumeModel == PlumeModel.UniformDistribution 
                        ? plume_pdf0(particleHt, j) 
                        : PlumePdf1(particleHt, j, erupt.ColumnBeta, erupt.MaxPlumeHeight);

                    /* Normalization is now done here */
                    //T[i][j].demon1 = (erupt->total_ash_mass * col_prob  * part_prob)/total_P;
                    table.SetDemon1((erupt.TotalAshMass * colProb * partProb) / totalP);

                    //T[i][j].particle_ht = particle_ht;
                    table.SetParticleHt(particleHt);
                    Logger.InfoFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", new[] {
                              i,
                              j,
                              table.GetParticleHt(),
                              table.GetAshDiam(),
                              table.GetPartDensity(),
                              table.GetFallTime(),
                              table.GetPlumeDiffusionFineParticle(),
                              table.GetPlumeDiffusionCoarseParticle(),
                              table.GetTotalFallTime(),
                              table.GetWindSumX(),
                              table.GetWindSumY(),
                              table.GetDemon1()
                          });
                    /*      	
                      fprintf(log_file,
                      "%g\t%g\t%g\t%g\t%g\t%g\t%g\t%g\t%g\t%g\n",
                      T[i][j].particle_ht, 
                      T[i][j].ashdiam, 
                      T[i][j].part_density, 
                      T[i][j].fall_time,
                      T[i][j].plume_diffusion_fine_particle,
                      T[i][j].plume_diffusion_coarse_particle,
                      T[i][j].total_fall_time,
                      T[i][j].wind_sum_x,
                      T[i][j].wind_sum_y,
                      T[i][j].demon1);
                     */
                } /* END COL_STEPS_LOOP */

                Logger.Info(" ");
                /*     fprintf(log_file, "\n"); */
                y += partStepWidth; /* moved from beg of loop 12-02-2010 */

            } /* END PART_STEPS_LOOP */

            /*  	fprintf(log_file, "OUT\n"); */
            //  fprintf(stderr, "MIN particle fall time = %.1f\n", pmin);
            Logger.Info("MIN particle fall time = " + pmin);
            //  fprintf(stderr, "MAX particle fall time = %.1f\n", pmax);
            Logger.Info("MAX particle fall time = " + pmax);
        }

        public double GetTotalP(double totalPCol, double totalPPart)
        {
            var totalP = (totalPCol * totalPPart);
            return totalP;
        }

        public double GetTotalPPart(Eruption erupt, double partStepWidth)
        {
            var cumProbPart = 0.0;
            var y = erupt.MinPhi;
            for (var i = 0; i < PartSteps; i++)
            {
                var prob = pdf_grainsize(erupt.MeanPhi, y, partStepWidth);
                cumProbPart += prob;
                //fprintf(log_file, " grain_size=%g, prob=%g, cum_prob=%g\n", y, prob, cum_prob_part);
                //logger.debug(" grain_size={}, prob={}, cum_prob_part={}", new Object[] {y, prob, cum_prob_part});
                y += partStepWidth;
            }
            return cumProbPart;
        }

        public double GetTotalPCol(Eruption erupt, Config config, double htSectionWidth, double htStepWidth)
        {
            var cumProbCol = 0.0;
            //x = erupt->vent_height;
            var x = erupt.VentHeight;

            //for (i=0; i < COL_STEPS; i++) {
            for (var i = 0; i < config.ColumnIntegrationSteps; i++)
            {
                x += htStepWidth;
                double prob;
                //prob = (*pdf)(x, (double)i, ht_section_width, erupt->max_plume_height);
                if (config.Plume.PlumeModel == PlumeModel.UniformDistribution)
                {
                    prob = plume_pdf0(x, i);
                }
                else {
                    // TODO : I can't see how this can be used anywhere as beta is never set!
                    // TwoBetaSqrd & BetaSqrtTwoPi is not valid as beta is always 0.
                    Debug.Assert(Math.Abs(TwoBetaSqrd) > 0.0000001 || Math.Abs(BetaSqrtTwoPi) > 0.0000001);
                    prob = PlumePdf1(x, i, htSectionWidth, erupt.MaxPlumeHeight);
                }
                cumProbCol += prob;
            }
            return cumProbCol;
        }

        public Table[,] CreateTableArray()
        {
            var tables = new Table[PartSteps, _config.ColumnIntegrationSteps];
            for (var i = 0; i < PartSteps; i++)
            {
                for (var j = 0; j < _config.ColumnIntegrationSteps; j++)
                {
                    tables[i, j] = new Table();
                }
            }
            return tables;
        }

        /* 
         * this function calculates the expected fraction of particles
         * in a given grainsize class (part_size_slice) assuming a normal 
         * distribution in phi units about the mean, dmean,
         * with standard deviation sigma.
         * The probability that 
         */
        double pdf_grainsize(double partMeanSize, double partSizeSlice, double partStepWidth)
        {
            /* PDF_GRAINSIZE_DEMON1 = 1.0 / 2.506628 * erupt->part_sigma_size */
            var demon3 = partSizeSlice - partMeanSize;
            //logger.debug(" demon3={}, part_size_slice={}, part_mean_size={}", new Object[] {demon3, part_size_slice, part_mean_size});
            //temp = -demon3 * demon3 / TWO_x_PART_SIGMA_SIZE; /* 2.0 * erupt->part_sigma_size * erupt->part_sigma_size */
            var temp = -demon3 * demon3 / TwoPartSigmaSize;
            //logger.debug(" temp={}, TwoPartSigmaSize={}", temp, getTwoPartSigmaSize());
            var demon2 = Math.Exp(temp);
            //func_rho = PDF_GRAINSIZE_DEMON1 * demon2 * part_step_width; 
            var funcRho = PdfGrainSizeDemon1* demon2 * partStepWidth;
            //logger.debug(" func_rho={}, PdfGrainSizeDemon1={}, demon2={}, part_step_width={}", new Object[] {func_rho, getPdfGrainSizeDemon1(), demon2, part_step_width});
            if (funcRho < 0.0)
            {
                //fprintf(log_file, "error in ash size distribution - method pdf_grainsize");
                Logger.Error("error in ash size distribution - method pdf_grainsize");
            }
            return funcRho;
        }

        int _numSlicesLeft;
        double _plumeSlice;
        /* 
           inputs:
           x: height of a particle within the plume, relative to vent height
           slice: integration step (index)
           ht_section_width: the width of an integration step
           none: not used

           output: the probability that a given grainsize will be released from a given height
        */
        double plume_pdf0(double x, int slice)
        {

            /* if (!slice) fprintf(stderr, "ENTER plume_pdf0 ....\n"); */
            var probability = 0.0;
            if (x > Threshold)
            {
                if (_numSlicesLeft == 0)
                {
                    //num_slices_left = COL_STEPS - slice;
                    _numSlicesLeft = _config.ColumnIntegrationSteps - slice;
                    //plume_slice = 1.0 / (double)num_slices_left;
                    _plumeSlice = 1.0 / _numSlicesLeft;
                    /* fprintf(stderr, "slices left = %d\n ", num_slices_left); */
                }

                probability = _plumeSlice;
            }

            /* fprintf(stderr, "x=%g threshold=%g plume_slice=%g prob=%g\n", x, threshold, plume_slice, probability); 
            if (probability < 0.0) This only gets printed if an error occurs. 
              fprintf(stderr, "col_ht=%f prob=%f\n", x, probability); */
            //logger.debug("x={} threshold={} plume_slice={} probability={}", new Object[]{x, threshold, plume_slice, probability});
            if (probability < 0.0)
            {
                Logger.ErrorFormat("col_ht={0} probability={1}", x, probability);
                throw new Exception("Expected probability to be greater than or equal to 0.0");
            }
            return probability;
        }

        /* 
           inputs:
           x: height of a particle within the plume, relative to vent height
           slice: integration step
           beta: the column beta parameter
           none: not used

           output: the probability that a given grainsize will be released from a given height
        */

        private double PlumePdf1(double x, int slice, double plume, double total)
        {

            var betaLimit = plume;
            var colHt = betaLimit - betaLimit * x / total;
            if (colHt <= 0.0)
            {
                colHt = 1e-9;
            }
            var temp1 = Math.Log(colHt);
            temp1 *= temp1;
            var temp0 = -temp1 / TwoBetaSqrd; /* 2.0 * beta * beta */
            var demon1 = Math.Exp(temp0);
            //demon2 = col_ht * BETA_x_SQRT_TWO_PI; /* beta * sqrt(2.0 * PI) */
            var demon2 = colHt * BetaSqrtTwoPi; /* beta * sqrt(2.0 * PI) */

            var probability = demon1 / demon2;
            if (double.IsNaN(probability))
            {
                //fprintf(stderr, "ht=%g  demon1=%g demon2=%g temp0=%g\n", col_ht, demon1, demon2, temp0);
                Logger.ErrorFormat("ht={0}  demon1={1} demon2={2} temp0={3}", new[] { colHt, demon1, demon2, temp0 });
                //System.exit(1);
                throw new Exception();
            }

            if (probability < 0.0)
            {/* This only gets printed if an error occurs. */
             //fprintf(stderr, "col_ht=%f demon1=%f demon2=%f prob=%f\n", col_ht, demon1, demon2, probability);
                Logger.ErrorFormat("col_ht={0} demon1={1} demon2={2} prob={3}", new[] { colHt, demon1, demon2, probability });
                //System.exit(1);
                throw new Exception();
            }
            return probability;
        }

        /* 
         * function ParticleDensity calculates varying particle density based on their grain size diamete
         * using a linear correlation between pumice_threshold (PHI) and lithic_threshold (PHI)
         * */
        public double ParticleDensity(double phiSlice)
        {
            double meanDensity = 0;
            if (phiSlice >= LithicDiameterThreshold)
            {
                meanDensity = _config.Density.LithicDensity;
            }
            else if (phiSlice <= PumiceDiameterThreshold)
            {
                meanDensity = _config.Density.PumiceDensity;
            }
            else if (phiSlice < LithicDiameterThreshold && phiSlice > PumiceDiameterThreshold)
            {
                meanDensity =
                    _config.Density.LithicDensity -
                    //LITHIC_DENSITY_minus_PUMICE_DENSITY *
                    (_config.Density.LithicDensity - _config.Density.PumiceDensity) *
                    (phiSlice - LithicDiameterThreshold) /
                    //PUMICE_DIAMETER_THRESHOLD_minus_LITHIC_DIAMETER_THRESHOLD;
                    (PumiceDiameterThreshold - LithicDiameterThreshold);
            }
            return meanDensity;
        }
        /* 
         * function phi2m converts the ash diameter from
         * units of phi to m
         */

        private static double Phi2M(double xx)
        {
            var cms = 0.001 * Math.Pow(2, -xx);
            return cms;
        }

        /* 
         * function PartFallTime determines the time of particle fall within each falling step
         * falling steps are here:
         *     set = particle rising steps = ht_step_width
         *     returns the particle fall time within each falling step
         *     
         * This function follows the approach outlined in Bonadonna et al. (1998) 
         * Briefly, particle fall time is calculated based on terminal velocities in
         * layers that are 1000 m thick. The terminal velocity is a function of the
         * particle Reynolds number, which varies with grainsize, air properties.
         * 
         * The thickness of the first layer (closest to the ground) is equal to the vent 
         * height. The vent_height is in meters above sea level. The area the ash falls on
         * is considered to be at sea level. This leads to some assumptions (!) near the
         * volcano...
         * */
        public double PartFallTime(double particleHt, double layer, double ashdiam, double partDensity)
        {

            var hz = particleHt;  /* height of the particle above sea level */

            /*rho is the density of air (kg/m^3) at the elevation of the current particle*/
            var rho = _config.AirDensity * Math.Exp(-hz / 8200.0);

            /*
            (friction due to the air) :
                vtl is terminal velocity (m/s) in laminar regime RE<6 
                vti is terminal velocity (m/s) in intermediate regime 6<RE<500
                vtt is terminal velocity (m/s) in turbulent regime RE>500
            */
            var vtl = partDensity * _config.Gravity * ashdiam * ashdiam / (_config.AirViscosity * 18.0); /* 18.0 * AIR_VISCOSITY */
                                                                                                           /*
                                                                                                               vti = ashdiam * 
                                                                                                               pow(((4.0*GRAVITY*GRAVITY*erupt->part_mean_density *erupt->part_mean_density )/		(225.0*AIR_VISCOSITY*rho)),(1.0/3.0));
                                                                                                               vtt=sqrt(3.1*erupt->part_mean_density *GRAVITY*ashdiam/rho);
                                                                                                           */

            /*
                RE is calculated using vtl (RE is Reynolds Number)
            */
            var reynoldsNumber = ashdiam * rho * vtl / _config.AirViscosity;
            var particleTermVel = vtl;
            var temp0 = ashdiam * rho;

            /*
                c...if laminar RE>6 (intermediate regime), RE is calculated again considering vti
            */

            if (reynoldsNumber >= 6.0)
            {
                /*4.0 * GRAVITY * GRAVITY * part_density * part_density / AIR_VISCOSITY * 225.0 * rho */
                var temp1 = GetGravitySqrdx4() * partDensity * partDensity / (_config.AirViscosity * 225.0) * rho;
                var vti = ashdiam * Math.Pow(temp1, GetOneThird()); /* ONE_THIRD = 1.0/3.0 */
                reynoldsNumber = temp0 * vti / _config.AirViscosity;
                particleTermVel = vti;
                /*
                c...if intermediate RE>500 (turbulent regime), RE is calculated again considering vtt 
                */
                if (reynoldsNumber >= 500.0)
                {
                    var vtt = Math.Sqrt(3.1 * partDensity * _config.Gravity * ashdiam / rho);
                    reynoldsNumber = temp0 * vtt / _config.AirViscosity;
                    particleTermVel = vtt;
                }
            }
            /* Calculate the time it takes this particle to fall through this distance=layer */
            var particleFallTime = layer / particleTermVel;

            /* particle fall time is in sec   */

            //printf("i= %d, layer = %f, hz = %f, particle_term_vel = %f, diam=%f, reynolds = %f\n", i,layer_thickness, hz, particle_term_vel, a//shdiam, reynolds_number);
            return particleFallTime;
        }

        private double GetGravitySqrdx4()
        {
            // GRAV_SQRD_x_4 = 4.0 * GRAVITY * GRAVITY
            return 4.0 * _config.Gravity * _config.Gravity;
        }
        // ONE_THIRD
        private static double GetOneThird()
        {
            return 1.0 / 3.0;
        }

        public int PartSteps { get; set; }

        public double Threshold { get; set; }

        public double SqrtTwoPi { get; set; }

        public double BetaSqrtTwoPi { get; set; }

        public double TwoBetaSqrd { get; set; }

        // PDF_GRAINSIZE_DEMON1
        public double PdfGrainSizeDemon1 { get; set; }

        // TWO_x_PART_SIGMA_SIZE
        public double TwoPartSigmaSize { get; set; }

        public Table[,] GetT()
        {
            return _table;
        }
    }
}
