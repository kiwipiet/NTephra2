namespace NTephra2.Core
{
    public class Wind
    {
        private int _day;
        private int _hour;
        private double _windHeight; /* height a.s.l. in km */
        private double _windSpeed;   /* the average windspeed in m/s */
        private double _windDir;     /* average wind direction in +/- degrees from north */
        public int GetDay()
        {
            return _day;
        }
        public void SetDay(int day)
        {
            _day = day;
        }
        public int GetHour()
        {
            return _hour;
        }
        public void SetHour(int hour)
        {
            _hour = hour;
        }
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
