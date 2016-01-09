namespace NTephra2.Core
{
    public class WindData
    {
        private double _windHeight; /* height a.s.l. in km */
        private double _windSpeed;   /* the average windspeed in m/s */
        private double _windDir;     /* average wind direction in +/- degrees from north */
        public double GetWindHeight()
        {
            return _windHeight;
        }
        public void SetWindHeight(double windHeight)
        {
            _windHeight = windHeight;
        }
        public double GetWindSpeed()
        {
            return _windSpeed;
        }
        public void SetWindSpeed(double windSpeed)
        {
            _windSpeed = windSpeed;
        }
        public double GetWindDir()
        {
            return _windDir;
        }
        public void SetWindDir(double windDir)
        {
            _windDir = windDir;
        }
    }
}
