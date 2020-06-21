using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Scripts
{
    public class SkeletonArm
    {
        public string rootBone;
        public float angleDeltaFromAnimationPos = 0;
        
        private Transform _spineGameObject;
        private Skeleton _skeleton;
        private string _gunSlot;
        private Action<Action> _registerSkeletonOverride;

        private SkeletonArm _offsetSkeletonArm = null;

        public SkeletonArm(Transform spineGameObject, Skeleton skeleton, string rootBone, string gunSlot,
            Action<Action> registerSkeletonOverride)
        {
            _spineGameObject = spineGameObject;
            _skeleton = skeleton;
            this.rootBone = rootBone;
            _gunSlot = gunSlot;
            _registerSkeletonOverride = registerSkeletonOverride;
        }

        public void Update()
        {
            if (_offsetSkeletonArm != null)
            {
                _registerSkeletonOverride(delegate
                {
                    _skeleton.FindBone(rootBone).Rotation += _offsetSkeletonArm.angleDeltaFromAnimationPos;
                });
            }
        }

        public void KeepOffsetRotationFrom(SkeletonArm skeletonArm)
        {
            _offsetSkeletonArm = skeletonArm;
        }

        public void AimAt(Vector3 worldPos)
        {
            Vector3 rootBonePos = _skeleton.FindBone(rootBone).GetWorldPosition(_spineGameObject);

            Vector3 aimOffset = worldPos - rootBonePos;
            float aimRotation = Mathf.Atan2(aimOffset.y, aimOffset.x) * Mathf.Rad2Deg;

            // Notify to override bone rotation where appropriate
            _registerSkeletonOverride(delegate
            {
                float rootAnimationRotation = _skeleton.FindBone(rootBone).Rotation;
                float rootTargetRotation = _skeleton.FindBone(rootBone).WorldToLocalRotation(aimRotation);

                angleDeltaFromAnimationPos = rootTargetRotation - rootAnimationRotation;

                if (_skeleton.ScaleX < -0.999f)
                {
                    _skeleton.FindBone(rootBone).Rotation = rootTargetRotation + 180;
                }
                else
                {
                    _skeleton.FindBone(rootBone).Rotation = rootTargetRotation;
                }
            });
        }

        public void SetGunEnabled(bool enabled)
        {
            if(enabled)
                _skeleton.SetAttachment(_gunSlot, "Gun");
            else
                _skeleton.SetAttachment(_gunSlot, "");
        }
    }
}