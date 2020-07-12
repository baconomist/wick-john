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
        private Vector3 _cameraPlayerOffset;

        private void Start()
        {
            _camera = GetComponent<Camera>();

            Player.PlayerUpdate += PlayerUpdate;
            Player.PlayerFixedUpdate += PlayerFixedUpdate;
        }

        private void PlayerUpdate(Player player)
        {
            if (!_followPlayer && player.transform.position.x > _camera.ViewportToWorldPoint(new Vector3(startFollowScreenPercentage, 0)).x)
            {
                _cameraPlayerOffset = _camera.transform.position - player.transform.position;
                _followPlayer = true;
            }
        }

        private void PlayerFixedUpdate(Player player)
        {
            if (_followPlayer)
            {
                float x = player.transform.position.x + _cameraPlayerOffset.x;
                float y = player.transform.position.y + _cameraPlayerOffset.y;
                _camera.transform.position = new Vector3(x, y, _camera.transform.position.z); 
            }
        }
    }
}