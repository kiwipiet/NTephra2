namespace NTephra2.Core
{
    public class Config
    {
        //#define pi 3.141592654
        public static double Pi = 3.141592654;
        //#define DEG2RAD 0.017453293
        public static double Deg2Rad = 0.017453293;
        //#define MAX_LINE 200
        public static int MaxLine = 200;

        // PLUME_HEIGHT 
        // ERUPTION_MASS 
        // MAX_GRAINSIZE 
        // MIN_GRAINSIZE 
        // MEDIAN_GRAINSIZE 
        // STD_GRAINSIZE 
        public EruptionConfig Eruption { get; } = new EruptionConfig();

        // VENT_EASTING 
        // VENT_NORTHING 
        // VENT_ELEVATION 
        public Vent Vent { get; } = new Vent();
        /* 
         * Eddy diff for small particles in m2/s (400 cm2/s)
         */
        // EDDY_CONST
        public double EddyDiff { get; set; } = 0.04;
        /* density of air */
        /* air density in kg/m3 */
        // AIR_DENSITY 1.293
        public double AirDensity { get; } = 1.293;
        /* dynamic viscosity of air */
        // AIR_VISCOSITY 0.000018325
        public double AirViscosity { get; } = 0.000018325;
        // GRAVITY 9.81
        public double Gravity { get; } = 9.81;

        /*
         * diffusion coeff for large particles (m2/s)
         */
        // DIFFUSION_COEFFICIENT
        public double DiffusionCoefficient { get; set; } = 200.0;
        /*
         * threshold for change in diffusion (seconds fall time)
         */
        // FALL_TIME_THRESHOLD
        public double FallTimeThreshold { get; set; } = 180.0;
        /*
         * density model for the pyroclasts
         */
        // LITHIC_DENSITY
        // PUMICE_DENSITY
        public Density Density { get; } = new Density();
        /*
         * define column integration steps
         */
        // COL_STEPS
        public int ColumnIntegrationSteps { get; set; } = 100;
        // PLUME_MODEL
        // PLUME_RATIO
        public Plume Plume { get; } = new Plume();

        // WIND_DAYS;
        public int WindDays { get; set; } = 1;

        // WIND_COLUMNS; TODO : Does not look like this is used!!!
        public int WindColumns { get; set; } = 3;
    }
}
