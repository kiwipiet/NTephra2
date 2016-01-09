namespace NTephra2.Core
{
    /*
     * density model for the pyroclasts
     */
    public class Density
    {
        /*
         * LITHIC_DENSITY
         */
        public double LithicDensity { get; set; } = 2600.0;
        /*
         * PUMICE_DENSITY
         */
        public double PumiceDensity { get; set; } = 1000.0;

        public Density()
        {
        }

        public Density(double lithicDensity, double pumiceDensity)
        {
            LithicDensity = lithicDensity;
            PumiceDensity = pumiceDensity;
        }
    }
}
