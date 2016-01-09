namespace NTephra2.Core
{
    public class Plume
    {
        /*
         * PLUME_MODEL
         */
        public PlumeModel PlumeModel { get; set; } = PlumeModel.UniformDistribution;
        /*
         * PLUME_RATIO
         */
        public double Ratio { get; set; } = 0.1;

        public Plume()
        {
        }
        public Plume(PlumeModel plumeModel, double ratio)
        {
            PlumeModel = plumeModel;
            Ratio = ratio;
        }
    }
}
