using System;
using System.Collections.Generic;
using GameAssets.Scripts;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Player.Scripts
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class Player : MonoBehaviour
    {
        public Transform target;
        public Transform target2;

        private SkeletonAnimation _skeletonAnimation;
        private SkeletonArm _leftArm;
        private SkeletonArm _rightArm;
        private Queue<Action> _skeletonOverrideQueue = new Queue<Action>();

        private void Start()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeletonAnimation.UpdateWorld += OnSkeletonUpdate;

            _leftArm = new SkeletonArm(transform, _skeletonAnimation.skeleton, "armLT", "gunL",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });
            
            _rightArm = new SkeletonArm(transform, _skeletonAnimation.skeleton, "armRT", "gunR",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });
        }

        private void Update()
        {
            _leftArm.SetGunEnabled(true);
            _leftArm.AimAt(target.position);
            _rightArm.AimAt(target2.position);
        }

        // Override skeleton here
        private void OnSkeletonUpdate(ISkeletonAnimation animated)
        {
            for (int i = 0; i < _skeletonOverrideQueue.Count; i++)
            {
                _skeletonOverrideQueue.Dequeue()();
            }

            // _skeletonAnimation.skeleton.FindBone("armRT").Rotation = rotation;
            // _skeletonAnimation.skeleton.SetAttachment("gunL", "Gun");
        }
    }
}