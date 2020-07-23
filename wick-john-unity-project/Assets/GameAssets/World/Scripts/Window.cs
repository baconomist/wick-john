using System;
using GameAssets.Player.Scripts;
using GameAssets.Scripts;
using UnityEngine;

namespace GameAssets.World.Scripts
{
    public class Window : MonoBehaviour
    {
        public float windowWidth;
        public GameObject windowParticles;
        public bool isLeftWindow = false;
        private bool _destroyQueued = false;

        private void Start()
        {
            WindowManager.RegisterWindow(this);
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        }

        private void Update()
        {
            if(_destroyQueued)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnPlayerCollided();
        }

        private void OnPlayerCollided()
        {
            GameObject wp = Instantiate(windowParticles);
            wp.transform.position = transform.position;

            WindowManager.RemoveWindow(this);
            _destroyQueued = true;
        }
    }
}