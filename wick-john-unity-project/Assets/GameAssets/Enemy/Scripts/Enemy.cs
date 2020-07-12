using System;
using System.Collections.Generic;
using GameAssets.Scripts;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Enemy.Scripts
{
    // TODO: add IK for arm aiming? https://www.davepagurek.com/blog/inverse-kinematics/
    [RequireComponent(typeof(SkeletonAnimation))]
    public class Enemy : MonoBehaviour
    {
        public Transform target;

        public float targetAngleOffset;
        
        private SkeletonAnimation _skeletonAnimation;

        private void Start()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeletonAnimation.UpdateWorld += OnSkeletonUpdate;
        }

        private void Update()
        {
            _skeletonAnimation.skeleton.ScaleX = target.transform.position.x < transform.position.x ? -1 : 1;
        }

        // Override skeleton here
        private void OnSkeletonUpdate(ISkeletonAnimation animated)
        {
            Vector3 targetDistance = target.transform.position - transform.position;
            float targetWorldAngle = Mathf.Atan2(targetDistance.y, targetDistance.x) * Mathf.Rad2Deg;

            float boneAngle = _skeletonAnimation.skeleton.FindBone("Torso").WorldToLocalRotation(targetWorldAngle) +
                        targetAngleOffset;
            
            // If skeleton flipped
            if(_skeletonAnimation.skeleton.ScaleX < 0)
            {
                boneAngle += 180;
            }

            _skeletonAnimation.skeleton.FindBone("Torso").Rotation = boneAngle;
        }
    }
}