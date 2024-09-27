namespace SpaceBaboon
{
    public interface IGlidable
    {
        public void StartGlide(float newLinearDrag, float glideTime);
        public void StopGlide();
    }
}
