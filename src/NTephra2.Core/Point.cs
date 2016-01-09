namespace NTephra2.Core
{
    /* 
     * geographic location of a point where ash mass in estimated 
     * */
    public class Point
    {
        /*
         * utm coordinate in meters
         */
        private double _easting;
        /*
         * utm coordinate in meters
         */
        private double _northing;
        /*
         * elevation at the point in masl
         * */
        private double _elevation;
        /*
         * mass of material accumulated at the point (gm) 
         * */
        private double _mass;
        /* 
         * the grainsize distribution at this location 
         * */
        private double[] _phi = new double[20];
        /* 
         * used to accumulate mass from multiple eruptions 
         * */
        private double _accumulateMass;
        /* 
         * pointer to an array of mass accumulations, one for each eruption 
         * */
        // TODO : Does not look like it is used at the moment.
        //private double mass_pt;


        public Point()
        {
            for (var i = 0; i < 20; i++)
            {
                _phi[i] = 0.0;
            }
        }
        public double GetEasting()
        {
            return _easting;
        }
        public void SetEasting(double easting)
        {
            _easting = easting;
        }
        public double GetNorthing()
        {
            return _northing;
        }
        public void SetNorthing(double northing)
        {
            _northing = northing;
        }
        public double GetElevation()
        {
            return _elevation;
        }
        public void SetElevation(double elevation)
        {
            _elevation = elevation;
        }
        public double GetMass()
        {
            return _mass;
        }
        public void SetMass(double mass)
        {
            _mass = mass;
        }
        public double[] GetPhi()
        {
            return _phi;
        }
        public void SetPhi(double[] phi)
        {
            _phi = phi;
        }
        public double GetAccumulateMass()
        {
            return _accumulateMass;
        }
        public void SetAccumulateMass(double accumulateMass)
        {
            _accumulateMass = accumulateMass;
        }
        //	public double getMass_pt() {
        //		return mass_pt;
        //	}
        //	public void setMass_pt(double mass_pt) {
        //		this.mass_pt = mass_pt;
        //	}
    }
}
