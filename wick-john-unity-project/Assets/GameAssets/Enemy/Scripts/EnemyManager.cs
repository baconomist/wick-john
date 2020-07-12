using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAssets.Enemy.Scripts
{
    public class EnemyManager : MonoBehaviour
    {
        private static EnemyManager _instance;
        
        private List<Enemy> _enemies;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _enemies = new List<Enemy>();
        }

        public static List<Enemy> GetEnemies()
        {
            return _instance._enemies;
        }
    }
}