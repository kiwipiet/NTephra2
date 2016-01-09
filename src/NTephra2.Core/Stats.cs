namespace NTephra2.Core
{
    public class Stats
    {
        private double _minFallTime;
        private double _maxFallTime;
        public double GetMinFallTime()
        {
            return _minFallTime;
        }
        public void SetMinFallTime(double minFallTime)
        {
            _minFallTime = minFallTime;
        }
        public double GetMaxFallTime()
        {
            return _maxFallTime;
        }
        public void SetMaxFallTime(double maxFallTime)
        {
            _maxFallTime = maxFallTime;
        }
    }
}
