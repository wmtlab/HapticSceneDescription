namespace HapticSceneDescription
{
    public interface IHapticDriver
    {
        void Start();
        void OnDestroy();
        void PlayValue(float intensity, int perceptionIndex);
        void PlayValue(float intensity);
        void Stop(int perceptionIndex);
        void Stop();
    }
}