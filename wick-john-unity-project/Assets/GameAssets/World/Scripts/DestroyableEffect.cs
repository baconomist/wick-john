using System;
using UnityEngine;

namespace GameAssets.World.Scripts
{
    public class DestroyableEffect : MonoBehaviour
    {
        public float lifeTime = 2.5f;
        private float _timeSinceStart;

        private void Update()
        {
            if (_timeSinceStart >= lifeTime)
            {
                Destroy(gameObject);
            }

            _timeSinceStart += Time.deltaTime;
        }
    }
}