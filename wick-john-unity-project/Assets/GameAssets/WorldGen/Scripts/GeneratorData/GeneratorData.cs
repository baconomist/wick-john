using System;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.GeneratorData
{
    public class GeneratorData : ScriptableObject
    {
        // public event Action OnDataUpdate = () => { };

        private void OnValidate()
        {
            // OnDataUpdate?.Invoke();
        }
    }
}