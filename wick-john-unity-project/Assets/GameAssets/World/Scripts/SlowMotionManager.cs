using System;
using UnityEngine;

namespace GameAssets.World.Scripts
{
    public class SlowMotionManager : MonoBehaviour
    {
        public float slowMotionTimeScale = 0.25f;
        public float mixTime = 0.5f;
        public float slowMotionDuration = 2.5f;
        
        
        public static bool slowMotionRunning = false;
        private static float _mixTimer;
        private static float _timeSinceSlowMotionStart;
        
        private void Update()
        {
            if (slowMotionRunning)
            {
                Time.timeScale = Mathf.Lerp(slowMotionTimeScale, 1, mixTime - _mixTimer);
                
                // For smooth physics
                Time.fixedDeltaTime = Mathf.Lerp(slowMotionTimeScale, 1, mixTime - _mixTimer) * 0.02f;
            }
            else
            {
                Time.timeScale = Mathf.Lerp(slowMotionTimeScale, 1, _mixTimer);
                
                // For smooth physics
                Time.fixedDeltaTime = Mathf.Lerp(slowMotionTimeScale, 1, _mixTimer) * 0.02f;
            }

            if (slowMotionRunning && _timeSinceSlowMotionStart >= slowMotionDuration)
            {
                EndSlowMotion();
            }

            _mixTimer += Time.unscaledDeltaTime;
            _timeSinceSlowMotionStart += Time.unscaledDeltaTime;
        }

        public static void StartSlowMotion()
        {
            slowMotionRunning = true;
            _mixTimer = 0;
            _timeSinceSlowMotionStart = 0;
        }
        
        public static void EndSlowMotion()
        {
            slowMotionRunning = false;
            _mixTimer = 0;
        }
    }
}