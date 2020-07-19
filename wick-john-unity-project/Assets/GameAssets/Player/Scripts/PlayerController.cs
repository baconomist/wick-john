using System;
using System.Collections.Generic;
using GameAssets.Enemy.Scripts;
using GameAssets.Scripts;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Player.Scripts
{
    public static class PlayerAnimations
    {
        public const string Run = "runAnim";
        public const string ShootSingle = "shootAnim";
    }

    public static class PlayerAnimationEvents
    {
        public const string FootStep = "footStepEvent";
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SkeletonAnimation))]
    public class PlayerController : MonoBehaviour
    {
        public float ambientShootRadius = 5f;
        public float maxVelocity = 50f;
        public float acceleration = 5f;

        private Rigidbody2D _rigidbody;
        
        public Transform target;
        public Transform target2;

        private SkeletonAnimation _skeletonAnimation;
        private PlayerArm _leftArm;
        private PlayerArm _rightArm;
        private Queue<Action> _skeletonOverrideQueue = new Queue<Action>();
        public static event Action<PlayerController> PlayerUpdate;
        public static event Action<PlayerController> PlayerFixedUpdate;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeletonAnimation.UpdateWorld += OnSkeletonUpdate;
            _skeletonAnimation.AnimationState.Event += OnSkeletonEvent;

            _leftArm = new PlayerArm(transform, _skeletonAnimation.skeleton, "armLT", "armLB", "gunL",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });

            _rightArm = new PlayerArm(transform, _skeletonAnimation.skeleton, "armRT", "armRB", "gunR",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });
        }

        private void Update()
        {
            // _leftArm.SetGunEnabled(true);
            // _leftArm.AimAt(target.position);
            // _rightArm.AimAt(target2.position);

            Run();
            // ShootStrayEnemies();

            PlayerUpdate?.Invoke(this);
        }

        private void FixedUpdate()
        {
            PlayerFixedUpdate?.Invoke(this);
        }

        private void Run()
        {
            if (_skeletonAnimation.state.GetCurrent(0)?.Animation.Name != PlayerAnimations.Run)
                _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.Run, true);
        }

        private void ShootStrayEnemies()
        {
            if (_skeletonAnimation.state.GetCurrent(1)?.Animation.Name != PlayerAnimations.ShootSingle)
            {
                _skeletonAnimation.state.SetAnimation(1, PlayerAnimations.ShootSingle, false);
            }

            foreach (Enemy.Scripts.Enemy enemy in EnemyManager.GetEnemies())
            {
                if ((enemy.transform.position - transform.position).sqrMagnitude <
                    ambientShootRadius * ambientShootRadius)
                {
                    
                }
            }
        }

        // Override skeleton here
        private void OnSkeletonUpdate(ISkeletonAnimation animated)
        {
            for (int i = 0; i < _skeletonOverrideQueue.Count; i++)
            {
                _skeletonOverrideQueue.Dequeue()();
            }
        }

        private void OnSkeletonEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == PlayerAnimationEvents.FootStep && _rigidbody.velocity.x < maxVelocity)
                _rigidbody.velocity += Vector2.right * acceleration;
        }
    }
}