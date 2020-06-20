using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Scripts
{
    public class SkeletonArm
    {
        private Transform _spineGameObject;
        private Skeleton _skeleton;
        private string _rootBone;
        private string _gunSlot;
        private Action<Action> _registerSkeletonOverride;
    
        public SkeletonArm(Transform spineGameObject, Skeleton skeleton, string rootBone, string gunSlot,
            Action<Action> registerSkeletonOverride)
        {
            _spineGameObject = spineGameObject;
            _skeleton = skeleton;
            _rootBone = rootBone;
            _gunSlot = gunSlot;
            _registerSkeletonOverride = registerSkeletonOverride;
        }

        public void AimAt(Vector3 worldPos)
        {
            Vector3 rootBonePos = _skeleton.FindBone(_rootBone).GetWorldPosition(_spineGameObject);

            Vector3 aimOffset = worldPos - rootBonePos;
            float aimRotation = Mathf.Atan2(aimOffset.y, aimOffset.x) * Mathf.Rad2Deg;

            // Notify to override bone rotation where appropriate
            _registerSkeletonOverride(delegate
            {
                float rootAnimationRotation = _skeleton.FindBone(_rootBone).Rotation;
                float rootTargetRotation = _skeleton.FindBone(_rootBone).WorldToLocalRotation(aimRotation);

                _skeleton.FindBone(_rootBone).Rotation = rootTargetRotation;
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