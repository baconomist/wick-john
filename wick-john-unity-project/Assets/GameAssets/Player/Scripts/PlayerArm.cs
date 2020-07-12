using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameAssets.Scripts
{
    public class PlayerArm
    {
        public string rootBone;
        public string subBone;

        private Transform _spineGameObject;
        private Skeleton _skeleton;
        private string _gunSlot;
        private Action<Action> _registerSkeletonOverride;

        private PlayerArm _offsetSkeletonArm = null;

        public PlayerArm(Transform spineGameObject, Skeleton skeleton, string rootBone, string subBone, string gunSlot,
            Action<Action> registerSkeletonOverride)
        {
            _spineGameObject = spineGameObject;
            _skeleton = skeleton;
            this.rootBone = rootBone;
            this.subBone = subBone;
            _gunSlot = gunSlot;
            _registerSkeletonOverride = registerSkeletonOverride;
        }

        public void AimAt(Vector3 worldPos)
        {
            Vector3 rootBonePos = _skeleton.FindBone(rootBone).GetWorldPosition(_spineGameObject);

            Vector3 aimOffset = worldPos - rootBonePos;
            float aimRotation = Mathf.Atan2(aimOffset.y, aimOffset.x) * Mathf.Rad2Deg;

            // Notify to override bone rotation where appropriate
            _registerSkeletonOverride(delegate
            {
                float animRotation = _skeleton.FindBone(rootBone).Rotation;
                float targetRotation = _skeleton.FindBone(rootBone).WorldToLocalRotation(aimRotation);

                if (_skeleton.ScaleX < -0.999f)
                {
                    _skeleton.FindBone(rootBone).Rotation = targetRotation + 180 - _skeleton.FindBone(subBone).Rotation;
                }
                else
                {
                    _skeleton.FindBone(rootBone).Rotation = targetRotation - _skeleton.FindBone(subBone).Rotation;
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