namespace NTephra2.Core
{
    /*
     * The Eruption parameters change for each eruption
     * */
    public class EruptionConfig
    {
        /* PLUME_HEIGHT
         * plumeHeight is given in meters above
         *   sealevel. For example, if the plume height is reported as
         *   10 km (10,000 m) above the vent and the vent is 5911 m
         *   above sealevel then PLUME_HEIGHT 15911
         *   examples: VEI 2 (1000-5000m above vent), 
         *   VEI 3 (3000-15000m above vent), VEI 4 (10000-25000 m
         *   above vent), VEI 5 (> 25000 m above vent)
         * */
        public double PlumeHeight { get; set; }
        /* ERUPTION_MASS
         * eruptionMass is in kilograms and refers to the total
         *   mass of tephra erupted. As examples: VEI 2: (1e9-1e10kg)
         *   VEI 3 (1e10-1e11 kg), VEI 4 (1e11-1e12 kg), VEI 5 (1e12-
         *   1e13 kg), VEI 6 (1e13-1e14 kg)
         * */
        public double EruptionMass { get; set; }
        /*
         * See GrainSize
         * */
        public GrainSize GrainSize { get; } = new GrainSize();
    }
}
