using Core;
using Ball;
using UnityEngine;

namespace LevelTimer
{
    public class Timer : MonoBehaviour
    {
        public static float CurrentTime { get; private set; } = 0f;
        public static float CompletionTime { get; private set; }
        private static bool completionTimeIsSet = false;
        private static bool shouldIncrementTimer = false;

        void Update()
        {
            if (shouldIncrementTimer)
            {
                CurrentTime += Time.deltaTime;
            }
            else
            {
                if (!completionTimeIsSet)
                {
                    CompletionTime = CurrentTime;
                    completionTimeIsSet = true;
                }
                CurrentTime = 0f;
            }
        }

        public static void Start()
        {
            shouldIncrementTimer = true;
            completionTimeIsSet = false;
        }

        public static void Stop()
        {
            shouldIncrementTimer = false;
        }

        public static void Reset()
        {
            Stop();
            CurrentTime = 0f;
        }
    }
}
