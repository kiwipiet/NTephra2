namespace NTephra2.Core
{
    public class GrainSize
    {
        /* MAX_GRAINSIZE
         * max is in phi units (largest diameter particles 
         *   are the largest negative number). Normally set to -7.0
         *   for basalt and -10.0 for silicic eruptions
         *   This is the largest particle tracked by code calculations
         * */
        public double Max { get; set; }
        /* MIN_GRAINSIZE
         * min is in phi units (smallest diameter particles
         *   have largest positive number (normally set to 7.0 for 
         *   basalt and 10.0 for silicic eruptions) 
         *   This is the smallest particle (diameter) tracked by code
         *   calculations.
         * */
        public double Min { get; set; }
        /* MEDIAN_GRAINSIZE
         * median is in phi units. Median grainsize for the 
         *   total grainsize distribution estimated for the entire mass
         *   erupted. Examples: Cerro Negro 1992, Nicaragua - basaltic
         *   subplinian (0 phi); Etna 1998 - subplinian (1 phi);
         *   Soufriere Hills Volcano, Montserrat -
         *   vulcanian/dome collapse (3.5 phi); 
         *   Mount St Helens 1980, USA (4.5 phi)
         * */
        public double Median { get; set; }
        /* STD_GRAINSIZE
         * standard is the standard deviation in grainsize in
         *   phi units, estimated for the entire mass erupted 
         *   Examples: Cerro Negro 1992, Nicaragua - basaltic
         *   subplinian (1.0 phi); Etna 1998 - subplinian (1.5 phi);
         *   Soufriere Hills Volcano, Montserrat -
         *   vulcanian/dome collapse (2 phi); 
         *   Mount St Helens 1980, USA (3 phi)
         * */
        public double Standard { get; set; }
    }
}
