using System;
using Player;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace.Editor
{
    // https://medium.com/@ProGM/show-a-draggable-point-into-the-scene-linked-to-a-vector3-field-using-the-handle-api-in-unity-bffc1a98271d
    [CustomEditor(typeof(PlayerArm))]
    public class DraggablePointDrawer : UnityEditor.Editor
    {
        readonly GUIStyle style = new GUIStyle();

        private float _lastCalculatedArmEndAngle = 0;
        private float _lastZRot = 0;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Take Aim"))
                (target as PlayerArm).AimAt((target as PlayerArm).testTarget.transform.position);

            if (GUILayout.Button("Reset Arm Offset"))
            {
                SerializedProperty property = serializedObject.FindProperty(nameof(PlayerArm.armEndPos));
                property.vector3Value = Vector3.zero;
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }

        private void OnEnable()
        {
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
        }

        private void OnSceneGUI()
        {
            // Make the points a more or less "constant" size regardless of camera zoom
            Vector3 size =
                Camera.current.ScreenToWorldPoint(new Vector3(Camera.current.pixelWidth, Camera.current.pixelHeight,
                    0));
            float discRadius = (size.x - Camera.current.transform.position.x) / 100f;

            // Using serializedObject rather than target allows for history tracking and redo/undo inside Unity
            SerializedProperty armEndPosProperty = serializedObject.FindProperty(nameof(PlayerArm.armEndPos));

            // Calculate world offset pos
            Vector3 armStartWorldPos = (target as PlayerArm).transform.position;
            Vector3 armEndWorldPos = (target as PlayerArm).transform.position + armEndPosProperty.vector3Value;

            // Draw arm start point
            Handles.color = Color.red;
            Handles.DrawSolidDisc(armStartWorldPos, Camera.current.transform.forward,
                discRadius);
            Handles.Label(armStartWorldPos, "Arm Start");

            // Update offset point based on object rotation
            if (Mathf.Abs((target as PlayerArm).transform.rotation.eulerAngles.z - _lastZRot) > 0.001f)
            {
                float currentArmAngle = (target as PlayerArm).transform.rotation.eulerAngles.z;
                
                if (!float.IsNaN(_lastCalculatedArmEndAngle))
                {
                    Vector3 moveBy = new Vector3(
                        Mathf.Cos((currentArmAngle - _lastCalculatedArmEndAngle) *
                                  Mathf.Deg2Rad) * armEndPosProperty.vector3Value.magnitude,
                        Mathf.Sin((currentArmAngle - _lastCalculatedArmEndAngle) *
                                  Mathf.Deg2Rad) * armEndPosProperty.vector3Value.magnitude,
                        0);

                    armEndWorldPos = armStartWorldPos + moveBy;
                }
            }
            else
            {
                // Draw dot to show end point
                Handles.color = Color.blue;
                Handles.DrawSolidDisc(armEndWorldPos, Camera.current.transform.forward, discRadius);
                Handles.Label(armEndWorldPos, "Arm End");
                armEndWorldPos = Handles.PositionHandle(armEndWorldPos, Quaternion.identity);

                _lastCalculatedArmEndAngle =
                    Mathf.Atan2(armEndPosProperty.vector3Value.y, armEndPosProperty.vector3Value.x) * Mathf.Rad2Deg;
            }

            armEndPosProperty.vector3Value = armEndWorldPos - armStartWorldPos;

            // Apply properties (saves change history as well)
            serializedObject.ApplyModifiedProperties();

            _lastZRot = (target as PlayerArm).transform.rotation.eulerAngles.z;
        }
    }
}