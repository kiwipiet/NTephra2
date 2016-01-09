namespace NTephra2.Core
{
    /*
     * The following parameters are SPECIFIC for COTOPAXI
     * 
     * Note: UTM coordinates are used (add 10,000,000 m in 
     *      northern hemisphere
     * */
    public class Vent
    {
        /*
         * VENT_EASTING
         */
        public double Easting { get; set; } = 645110;
        /*
         * VENT_NORTHING
         */
        public double Northing { get; set; } = 2158088;
        /*
         * VENT_ELEVATION
         */
        public double Elevation { get; set; } = 3850;
    }
}
