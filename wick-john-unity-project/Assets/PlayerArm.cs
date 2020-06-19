using System;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public class PlayerArm : MonoBehaviour
    {
        public Transform testTarget;
        public Vector3 armEndPos;

        public void AimAt(Vector3 pos)
        {
            Vector3 armOffset = transform.position - armEndPos;
            float angleOffset = Mathf.Atan2(armOffset.y, armOffset.x) * Mathf.Rad2Deg;
            Debug.Log(angleOffset);
            // if (angleOffset < 0)
            //     angleOffset = 360 + angleOffset;

            Vector3 aimOffset = pos - transform.position;
            Quaternion aimRotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                Mathf.Atan2(aimOffset.y, aimOffset.x) * Mathf.Rad2Deg + angleOffset);

            transform.rotation = aimRotation;
            Debug.DrawLine(transform.position, transform.position + transform.right * 10f, Color.red, 1);
        }
    }
}