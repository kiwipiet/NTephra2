namespace NTephra2.Core
{
    public class Eruption
    {
        /* eruption parameters */

        /* volcano location in UTM north (meters) */
        public double VolcanoNorthing { get; set; }
        /* volcano location in UTM east (meters) */
        public double VolcanoEasting { get; set; }
        /* is the total amount of ash erupted (kg) */
        public double TotalAshMass { get; set; }
        /* the maximum particle diameter considered (phi) */
        public double MinPhi { get; set; }
        /* is the minimum particle diameter considered (phi) */
        public double MaxPhi { get; set; }

        /*
         * Note: erupt->max/min_part_size are used to set the limits of integration
         * on the calculation. Particles outside this range are not considered at all 
         * */

        /*
         * Note: phi units are such that max will appear to be less than min, this is
         * accounted for in the conversion to cm, which is internal 
         * */

        /*the mean particle diameter erupted in phi units */
        public double MeanPhi { get; set; }  
        /*standard deviation in particle diameter in phi units */
        public double SigmaPhi { get; set; } 
        /* elevation of the vent amsl in meters */
        public double VentHeight { get; set; }  
        /*eruption column height amsl in meters */
        public double MaxPlumeHeight { get; set; }  

        /* parameter governing the particle size distribution in column */
        // TODO : Note that this is always 0!!!
        public double ColumnBeta { get; } = 0;
        /* 
         * Note: A large value  of beta (1) places most of the particles
         * high in the eruption column, a low value of beta (0.01) spreads the particle density
         * lower in the column. Particle release models based on "corner" models etc strongly
         * suggest that a larger value for beta should be used. 
         * */

        public Eruption(Config config)
        {
            //		(erupt+i)->volcano_easting = VENT_EASTING;
            VolcanoEasting = config.Vent.Easting;
            //		(erupt+i)->volcano_northing = VENT_NORTHING;
            VolcanoNorthing = config.Vent.Northing;
            //		(erupt+i)->total_ash_mass = ERUPTION_MASS;
            TotalAshMass = config.Eruption.EruptionMass;
            //		(erupt+i)->min_phi = MAX_GRAINSIZE;
            MinPhi = config.Eruption.GrainSize.Max;
            //		(erupt+i)->max_phi= MIN_GRAINSIZE;
            MaxPhi = config.Eruption.GrainSize.Min;
            //		(erupt+i)->mean_phi = MEDIAN_GRAINSIZE;
            MeanPhi = config.Eruption.GrainSize.Median;
            //		(erupt+i)->sigma_phi= STD_GRAINSIZE;
            SigmaPhi = config.Eruption.GrainSize.Standard;
            //		(erupt+i)->vent_height = VENT_ELEVATION;
            VentHeight = config.Vent.Elevation;
            //		(erupt+i)->max_plume_height = PLUME_HEIGHT;
            MaxPlumeHeight = config.Eruption.PlumeHeight;
        }
    }
}
