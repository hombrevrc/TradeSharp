namespace TradeSharp.Contract.Entity
{
    public struct VolumeStep
    {
        public int minVolume;

        public int volumeStep;

        public VolumeStep(int min, int step)
        {
            minVolume = min;
            volumeStep = step;
        }

        public override string ToString()
        {
            return $"{minVolume} by {volumeStep}";
        }
    }
}
