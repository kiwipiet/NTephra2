namespace NTephra2.Core
{
    /* 
     * These values are calculated ahead of time and used in the double integration [PART_STEPS][COL_STEPS]
     * */
    public class Table
    {
        private double _particleHt;
        private double _ashDiam;
        private double _partDensity;
        private double _fallTime;
        private double _plumeDiffusionFineParticle;
        private double _plumeDiffusionCoarseParticle;
        private double _totalFallTime;
        private double _windSumX;
        private double _windSumY;
        private double _demon1;

        public double GetParticleHt()
        {
            return _particleHt;
        }
        public void SetParticleHt(double particleHt)
        {
            _particleHt = particleHt;
        }
        public double GetAshDiam()
        {
            return _ashDiam;
        }
        public void SetAshDiam(double ashDiam)
        {
            _ashDiam = ashDiam;
        }
        public double GetPartDensity()
        {
            return _partDensity;
        }
        public void SetPartDensity(double partDensity)
        {
            _partDensity = partDensity;
        }
        public double GetFallTime()
        {
            return _fallTime;
        }
        public void SetFallTime(double fallTime)
        {
            _fallTime = fallTime;
        }
        public double GetPlumeDiffusionFineParticle()
        {
            return _plumeDiffusionFineParticle;
        }
        public void SetPlumeDiffusionFineParticle(double plumeDiffusionFineParticle)
        {
            _plumeDiffusionFineParticle = plumeDiffusionFineParticle;
        }
        public double GetPlumeDiffusionCoarseParticle()
        {
            return _plumeDiffusionCoarseParticle;
        }
        public void SetPlumeDiffusionCoarseParticle(double plumeDiffusionCoarseParticle)
        {
            _plumeDiffusionCoarseParticle = plumeDiffusionCoarseParticle;
        }
        public double GetTotalFallTime()
        {
            return _totalFallTime;
        }
        public void SetTotalFallTime(double totalFallTime)
        {
            _totalFallTime = totalFallTime;
        }
        public double GetWindSumX()
        {
            return _windSumX;
        }
        public void SetWindSumX(double windSumX)
        {
            _windSumX = windSumX;
        }
        public double GetWindSumY()
        {
            return _windSumY;
        }
        public void SetWindSumY(double windSumY)
        {
            _windSumY = windSumY;
        }
        public double GetDemon1()
        {
            return _demon1;
        }
        public void SetDemon1(double demon1)
        {
            _demon1 = demon1;
        }
    }
}
