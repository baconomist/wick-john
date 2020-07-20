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
        private float _floorY;

        private Vector3 _jumpToPos;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeletonAnimation.UpdateWorld += OnSkeletonUpdate;
            _skeletonAnimation.AnimationState.Event += OnSkeletonEvent;
            _skeletonAnimation.AnimationState.End += OnSkeletonAnimationEnd;

            _leftArm = new PlayerArm(transform, _skeletonAnimation.skeleton, "armLT", "armLB", "gunL",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });

            _rightArm = new PlayerArm(transform, _skeletonAnimation.skeleton, "armRT", "armRB", "gunR",
                delegate(Action action) { _skeletonOverrideQueue.Enqueue(action); });

            _floorY = transform.position.y;
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
            if (_isJumping && transform.position.y < _floorY &&
                _skeletonAnimation.state.GetCurrent(0).Animation.Name == PlayerAnimations.JumpEnd)
            {
                _isJumping = false;
                _skeletonAnimation.state.TimeScale = 1;   
            }

            int i = 0;
            foreach (Window window in WindowManager.Windows)
            {
                if (!_isJumping && window.transform.position.x > transform.position.x &&
                    window.transform.position.x - transform.position.x < windowJumpDistance)
                {
                    if (window.isLeftWindow)
                        JumpThroughLeftWindow(window.transform.position);
                    else if (i + 1 < WindowManager.Windows.Count)
                        JumpThroughRightWindow(window.transform.position,
                            WindowManager.Windows[i + 1].transform.position);
                }

                i++;
            }
        }

        private void JumpThroughLeftWindow(Vector3 position)
        {
            _jumpToPos = position;
            _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.JumpStart, false);
            _isJumping = true;
        }

        private void JumpThroughRightWindow(Vector3 rightWindowPos, Vector3 leftWindowPos)
        {
            // Average the trajectory cus I'm lazy ;)
            _jumpToPos = new Vector3((rightWindowPos.x + leftWindowPos.x) / 2f,
                (rightWindowPos.y + leftWindowPos.y) / 2f);
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

        private void OnSkeletonAnimationEnd(TrackEntry trackEntry)
        {
            if (_isJumping && trackEntry.Animation.Name == PlayerAnimations.Run)
            {
                float dX = _jumpToPos.x - _rigidbody.position.x;
                float dY = _jumpToPos.y - _rigidbody.position.y;

                // Time to complete jump
                float dT;
                if (_rigidbody.velocity.x <= 0.001f)
                    dT = 0.5f;
                else
                    dT = dX / _rigidbody.velocity.x;

                // Gravity's acceleration
                float a = Physics.gravity.y * _rigidbody.gravityScale;
                float vIY = dY / dT - a / 2f * dT;

                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, vIY);
                _skeletonAnimation.state.SetAnimation(0, PlayerAnimations.JumpMiddle, false);
                _skeletonAnimation.state.TimeScale = _skeletonAnimation.state.GetCurrent(0).AnimationEnd / dT;
            }
            else if (trackEntry.Animation.Name == PlayerAnimations.JumpStart)
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