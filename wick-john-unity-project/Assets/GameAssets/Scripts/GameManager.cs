using System;
using GameAssets.Player.Scripts;
using UnityEngine;

namespace GameAssets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private static PlayerController _playerController;

        public static PlayerController PlayerController
        {
            get
            {
                if (_playerController == null)
                    _playerController = FindObjectOfType<PlayerController>();
                return _playerController;
            }
        }

        private void Start()
        {
            new GameObject("Window Manager").AddComponent<WindowManager>();
            new GameObject("Enemy Manager").AddComponent<EnemyManager>();
        }
    }
}