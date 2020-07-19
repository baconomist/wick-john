using UnityEngine;

namespace GameAssets.WorldGen.Scripts.GeneratorData
{
    [CreateAssetMenu(fileName = "PlayableBuildingData", menuName = "WorldGen/PlayableBuildingData", order = 0)]
    public class PlayableBuildingData : GeneratorData
    {
        public GameObject enemyPrefab;
        public GameObject windowParticlesPrefab;
        public Color windowColor = Color.red;
        
        public float playerFadeDistanceMultiplier = 1.0f;

        public int renderIndex = 5;

        public float minWallThickness = 1;
        public float maxWallThickness = 1;

        // Basically the size of the roof in length
        public float minSize = 10;
        public float maxSize = 10;
        
        public float minHeight = 2;
        public float maxHeight = 5;
        
        public int minFloors = 1;
        public int maxFloors = 2;
    }
}