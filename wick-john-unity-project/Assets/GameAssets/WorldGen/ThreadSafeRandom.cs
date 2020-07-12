using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameAssets.WorldGen
{
    // Thread-safe random generator
    public static class ThreadSafeRandom
    {
        private static Dictionary<int, System.Random> _randoms = new Dictionary<int, System.Random>();

        public static UnityEngine.Color Color(UnityEngine.Color from, UnityEngine.Color to, bool randomHue = false)
        {
            System.Random random = GetRandom();

            float h1, s1, v1;
            UnityEngine.Color.RGBToHSV(from, out h1, out s1, out v1);
            float h2, s2, v2;
            UnityEngine.Color.RGBToHSV(to, out h2, out s2, out v2);

            UnityEngine.Color randomColor = UnityEngine.Color.HSVToRGB(Float() * (h2 - h1) + h1,
                Float() * (s2 - s1) + s1, Float() * (v2 - v1) + v1);
            
            if (!randomHue)
            {
                float rh2, rs2, rv2;
                UnityEngine.Color.RGBToHSV(randomColor, out rh2, out rs2, out rv2);

                // Don't change the hue, only pick between hues
                return UnityEngine.Color.HSVToRGB((Float() > 0.5f ? h1 : h2), rs2, rv2);
            }

            return randomColor;
        }

        public static float Float()
        {
            return (float) GetRandom().NextDouble();
        }

        public static float Range(float min, float max)
        {
            return Float() * (max - min) + min;
        }

        public static int Range(int min, int max)
        {
            return (int) (Float() * (max - min) + min);
        }

        private static System.Random GetRandom()
        {
            if(!_randoms.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                _randoms[Thread.CurrentThread.ManagedThreadId] = new System.Random();
            return _randoms[Thread.CurrentThread.ManagedThreadId];
        }
    }
}