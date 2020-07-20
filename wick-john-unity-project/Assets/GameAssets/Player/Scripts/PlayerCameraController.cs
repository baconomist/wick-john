using System;
using UnityEngine;

namespace GameAssets.Player.Scripts
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Range(0, 1)]
        public float startFollowScreenPercentage = 0.5f;
        
        private Camera _camera;

        private bool _followPlayer = false;
        private float _minY;
        private Vector3 _cameraPlayerOffset;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _minY = transform.position.y;

            PlayerController.PlayerUpdate += PlayerUpdate;
            PlayerController.PlayerFixedUpdate += PlayerFixedUpdate;
        }

        private void PlayerUpdate(PlayerController playerController)
        {
            if (!_followPlayer && playerController.transform.position.x > _camera.ViewportToWorldPoint(new Vector3(startFollowScreenPercentage, 0)).x)
            {
                _cameraPlayerOffset = _camera.transform.position - playerController.transform.position;
                _followPlayer = true;
            }
        }

        private void PlayerFixedUpdate(PlayerController playerController)
        {
            if (_followPlayer)
            {
                float x = playerController.transform.position.x + _cameraPlayerOffset.x;
                float y = playerController.transform.position.y + _cameraPlayerOffset.y;
                if (y < _minY)
                    y = _minY;
                _camera.transform.position = new Vector3(x, y, _camera.transform.position.z); 
            }
        }
    }
}