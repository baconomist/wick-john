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
        public bool isLeftWindow = false;

        private void Start()
        {
            WindowManager.RegisterWindow(this);
        }

        private void Update()
        {
            if (Mathf.Abs(transform.position.x - GameManager.PlayerController.transform.position.x) < worldWidth)
            {
                GameObject wp = Instantiate(windowParticles);
                wp.transform.position = transform.position;

                WindowManager.RemoveWindow(this);
                Destroy(gameObject);
            }
        }
    }
}