
namespace GGJ2014.Graphics.SpriteAnimations
{
    public class Delay
    {
        public float CurrentTime { get; private set; }
        public float Duration { get; private set; }

        public Delay(float delayInMilliseconds)
        {
            this.Duration = delayInMilliseconds;
        }

        public void Update(float millisecondsElapsed)
        {
            this.CurrentTime += millisecondsElapsed;
        }

        public bool Over()
        {
            return CurrentTime > Duration;
        }
    }
}
