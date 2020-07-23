using System;
using UnityEngine;

namespace GameAssets.WorldGen.Scripts.Generators
{
    public abstract class Generator : MonoBehaviour
    {
        public bool autoUpdate = false;

        public static event Action<Generator> OnValidateEvent;

        private void OnValidate()
        {
            OnValidateEvent?.Invoke(this);
        }

        public abstract void GenerateOn(GameObject parentGameObject);
        public abstract void GeneratePreview();
    }
}