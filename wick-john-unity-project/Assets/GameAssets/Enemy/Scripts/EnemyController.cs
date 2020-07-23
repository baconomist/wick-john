using System;
using System.Collections.Generic;
using GameAssets.Scripts;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Enemy.Scripts
{
    public static class EnemyAnimations
    {
        public static string CrouchIdle = "crouchIdleAnim";
        public static string CrouchToShoot = "crouchToShootAnim";
        public static string Shoot = "shootAnim";
    }

    // TODO: add IK for arm aiming? https://www.davepagurek.com/blog/inverse-kinematics/
    public class EnemyController : MonoBehaviour
    {
        public Transform target;
        public float aimSpeedMultiplier = 1.0f;
        public float targetAngleOffset;
        public bool engaged = false;
        
        [Range(0, 360)]
        public int minAngle = 0;
        [Range(0, 360)]
        public int maxAngle = 360;

        private SkeletonAnimation _skeletonAnimation;
        private Vector3 _size;
        private float _crouchUpDistance;
        private Vector3 _startPos;

        private float _targetSkeletonTorsoAngle;
        private float _currentSkeletonTorsoAngle;

        private bool _isFlipped;

        private void Start()
        {
            _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            _skeletonAnimation.UpdateWorld += OnSkeletonUpdate;
            _size = GetComponentInChildren<BoxCollider2D>().bounds.size;
            _startPos = transform.position;

            _crouchUpDistance = _size.y * 0.2f;

            if (target == null)
                target = GameManager.PlayerController.transform;

            EnemyManager.RegisterEnemy(this);
        }

        private void Update()
        {
            if (target.transform.position.x < transform.position.x != _isFlipped)
                OnFlip();
            _isFlipped = target.transform.position.x < transform.position.x;
            _skeletonAnimation.skeleton.ScaleX = _isFlipped ? -1 : 1;

            if (!engaged)
            {
                // if (_skeletonAnimation.state.GetCurrent(0).Animation.Name == EnemyAnimations.CrouchToShoot)
                // {
                //     float percentAnimationComplete =
                //         Mathf.InverseLerp(_skeletonAnimation.state.GetCurrent(0).AnimationStart,
                //             _skeletonAnimation.state.GetCurrent(0).AnimationEnd,
                //             _skeletonAnimation.state.GetCurrent(0).AnimationTime);
                //     transform.position = _startPos + new Vector3(0, _crouchUpDistance * percentAnimationComplete);
                // }

                if (_skeletonAnimation.state.GetCurrent(0).Animation.Name == EnemyAnimations.CrouchToShoot &&
                    _skeletonAnimation.state.GetCurrent(0).IsComplete)
                {
                    _skeletonAnimation.state.SetAnimation(0, EnemyAnimations.Shoot, false);
                    engaged = true;
                }
            }
            else
            {
                if (Mathf.Abs(_targetSkeletonTorsoAngle - _currentSkeletonTorsoAngle) > 0.1f)
                {
                    if (To360Angle(_currentSkeletonTorsoAngle) < To360Angle(_targetSkeletonTorsoAngle))
                    {
                        if (To360Angle(_targetSkeletonTorsoAngle) - To360Angle(_currentSkeletonTorsoAngle) > 180)
                            _currentSkeletonTorsoAngle -= Time.deltaTime * 100 * aimSpeedMultiplier;
                        else
                            _currentSkeletonTorsoAngle += Time.deltaTime * 100 * aimSpeedMultiplier;
                    }
                    else
                    {
                        if (To360Angle(_currentSkeletonTorsoAngle) - To360Angle(_targetSkeletonTorsoAngle) > 180)
                            _currentSkeletonTorsoAngle += Time.deltaTime * 100 * aimSpeedMultiplier;
                        else
                            _currentSkeletonTorsoAngle -= Time.deltaTime * 100 * aimSpeedMultiplier;
                    }
                }

                if (_currentSkeletonTorsoAngle < minAngle)
                    _currentSkeletonTorsoAngle = minAngle;
                if (_currentSkeletonTorsoAngle > maxAngle)
                    _currentSkeletonTorsoAngle = maxAngle;
            }
        }

        private void OnFlip()
        {
        }

        private float To360Angle(float angle)
        {
            if (angle < 0)
                return 360.0f * (Mathf.FloorToInt(Mathf.Abs(angle) / 360f) + 1) + angle;
            if (angle > 360)
                return angle % 360;
            return angle;
        }

        public void BeginEngage()
        {
            _skeletonAnimation.state.SetAnimation(0, EnemyAnimations.CrouchToShoot, false);
        }

        // Override skeleton here
        private void OnSkeletonUpdate(ISkeletonAnimation animated)
        {
            Vector3 targetDistance = target.transform.position - transform.position;
            float targetWorldAngle = Mathf.Atan2(targetDistance.y, targetDistance.x) * Mathf.Rad2Deg;

            _targetSkeletonTorsoAngle = targetWorldAngle;

            float appliedRotation;
            if (_isFlipped)
            {
                // Idk why this works, but when the skeleton is flipped this is needed :|
                appliedRotation = _currentSkeletonTorsoAngle + targetAngleOffset * 2;
            }
            else
            {
                appliedRotation = _currentSkeletonTorsoAngle + targetAngleOffset;
            }

            if (engaged)
            {
                _skeletonAnimation.skeleton.FindBone("Torso").Rotation = _skeletonAnimation.skeleton
                    .FindBone("Torso")
                    .WorldToLocalRotation(appliedRotation);
            }
        }
    }
}