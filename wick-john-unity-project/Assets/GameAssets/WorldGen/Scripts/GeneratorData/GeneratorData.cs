using System;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.GeneratorData
{
    public class GeneratorData : ScriptableObject
    {
        public bool autoUpdate = false;

        public event Action OnDataUpdate = () => { };
        private void OnValidate()
        {
            if(autoUpdate)
                OnDataUpdate?.Invoke();
        }
    }
}