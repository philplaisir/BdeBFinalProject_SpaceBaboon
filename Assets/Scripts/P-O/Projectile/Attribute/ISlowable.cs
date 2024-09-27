namespace SpaceBaboon
{
    public interface ISlowable
    {
        public void StartSlow(float slowAmount, float slowTimer);
        public void EndSlow();
    }
}
