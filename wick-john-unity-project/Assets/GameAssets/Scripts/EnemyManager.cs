using System.Collections.Generic;
using GameAssets.Enemy.Scripts;
using UnityEngine;

namespace GameAssets.Scripts
{
    public class EnemyManager : MonoBehaviour
    {
        public static List<Enemy.Scripts.EnemyController> Enemies = new List<EnemyController>();

        private void Start()
        {
            Enemies.Clear();
        }
        
        public static void RegisterEnemy(Enemy.Scripts.EnemyController enemyController)
        {
            Enemies.Add(enemyController);
        }

        public static void RemoveEnemy(Enemy.Scripts.EnemyController enemyController)
        {
            Enemies.Remove(enemyController);
        }
    }
}