using System;
using GameAssets.Player.Scripts;
using GameAssets.Scripts;
using UnityEngine;

namespace GameAssets.World.Scripts
{
    public class Window : MonoBehaviour
    {
        public float worldWidth;
        public GameObject windowParticles;

        private void Update()
        {
            if (Mathf.Abs(transform.position.x - GameManager.PlayerController.transform.position.x) < worldWidth)
            {
                GameObject wp = Instantiate(windowParticles);
                wp.transform.position = transform.position;
                Destroy(gameObject);
            }
        }
    }
}