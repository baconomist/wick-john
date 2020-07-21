using System;
using System.Collections.Generic;
using GameAssets.Enemy.Scripts;
using GameAssets.Scripts;
using GameAssets.World.Scripts;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Player.Scripts
{
    public static class PlayerAnimations
    {
        public const string Run = "runAnim";
        public const string ShootSingle = "shootAnim";
        public const string JumpStart = "jumpStartAnim";
        public const string JumpMiddle = "jumpMiddleAnim";
        public const string JumpEnd = "jumpEndAnim";
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
        public float windowJumpDistance = 5f;

        private Rigidbody2D _rigidbody;

        public Transform target;
        public Transform target2;

        private SkeletonAnimation _skeletonAnimation;
        private PlayerArm _leftArm;
        private PlayerArm _rightArm;
        private Queue<Action> _skeletonOverrideQueue = new Queue<Action>();
        public static event Action<PlayerController> PlayerUpdate;
        public static event Action<PlayerController> PlayerFixedUpdate;

        private bool _isRunning = false;
        private bool _isJumping = false;

        private Vector3 _lastJumpToPos;
        private float _lastJumpDt;
        private float _timeSinceJump;

        private SkeletonData _skeletonData;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeletonAnimation.UpdateWorld += OnSkeletonUpdate;
            _skeletonAnimation.AnimationState.Event += OnSkeletonEvent;
            _skeletonAnimation.AnimationState.Complete += OnSkeletonAnimationComplete;

            _leftArm = new PlayerArm(transform, _skeletonAnimation.skeleton, "armLT", "armLB", "gunL",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });

            _rightArm = new PlayerArm(transform, _skeletonAnimation.skeleton, "armRT", "armRB", "gunR",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });
            
            _skeletonData = _skeletonAnimation.skeletonDataAsset.GetSkeletonData(true);
        }

        private void Update()
        {
            // _leftArm.SetGunEnabled(true);
            // _leftArm.AimAt(target.position);
            // _rightArm.AimAt(target2.position);

            Run();
            // ShootStrayEnemies();

            Jump();

            PlayerUpdate?.Invoke(this);
        }

        private void FixedUpdate()
        {
            PlayerFixedUpdate?.Invoke(this);
        }

        private void Run()
        {
            _isRunning = !_isJumping;

            if (_isRunning)
            {
                if (_skeletonAnimation.state.GetCurrent(0)?.Animation.Name != PlayerAnimations.Run)
                    _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.Run, true);
            }
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

        private void Jump()
        {
            if (_isJumping && _timeSinceJump >= _lastJumpDt * 1.8f)
            {
                _skeletonAnimation.state.TimeScale = 1;
                _isJumping = false;
            }

            if (_isJumping)
            {
                _timeSinceJump += Time.deltaTime;
            }
            else
            {
                int i = 0;
                foreach (Window window in WindowManager.Windows)
                {
                    if (!_isJumping && window.transform.position.x > transform.position.x &&
                        window.transform.position.x - transform.position.x < windowJumpDistance)
                    {
                        if (window.isLeftWindow)
                            JumpToPos(window.transform.position);
                        else if (i + 1 < WindowManager.Windows.Count)
                        {
                            Vector3 leftWindowPos = window.transform.position;
                            Vector3 rightWindowPos = WindowManager.Windows[i + 1].transform.position;
                            JumpToPos(new Vector3((rightWindowPos.x + leftWindowPos.x) / 2f,
                                (rightWindowPos.y + leftWindowPos.y) / 2f));
                        }
                    }

                    i++;
                }
            }
        }

        private void JumpToPos(Vector3 position)
        {
            float dX = _lastJumpToPos.x - _rigidbody.position.x;
            float dY = _lastJumpToPos.y - _rigidbody.position.y;

            // Time to complete jump
            float dT;
            float velX = _rigidbody.velocity.x;
            if (velX <= 0.001f)
            {
                velX = 10;
                dT = 0.5f;
            }
            else
                dT = dX / velX;

            // Gravity's acceleration
            float a = Physics.gravity.y * _rigidbody.gravityScale;
            float vIY = dY / dT - a / 2f * dT;

            _rigidbody.velocity = new Vector2(velX, vIY);
            
            float jumpStartDuration = _skeletonData.FindAnimation(PlayerAnimations.JumpStart).Duration;
            float jumpMiddleDuration = _skeletonData.FindAnimation(PlayerAnimations.JumpMiddle).Duration;
            float jumpEndDuration = _skeletonData.FindAnimation(PlayerAnimations.JumpEnd).Duration;

            _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.JumpMiddle, false);
            _skeletonAnimation.state.TimeScale = (jumpStartDuration + jumpMiddleDuration + jumpEndDuration) / (dT * 2f);

            _lastJumpDt = dT;
            _timeSinceJump = 0;

            _lastJumpToPos = position;
            _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.JumpStart, false);
            _isJumping = true;
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
            if (_isRunning && e.Data.Name == PlayerAnimationEvents.FootStep && _rigidbody.velocity.x < maxVelocity)
                _rigidbody.velocity += Vector2.right * acceleration;
        }

        private void OnSkeletonAnimationComplete(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == PlayerAnimations.JumpStart)
            {
                _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.JumpMiddle, false);
            }
            else if (trackEntry.Animation.Name == PlayerAnimations.JumpMiddle)
            {
                _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.JumpEnd, false);
            }
        }
    }
}