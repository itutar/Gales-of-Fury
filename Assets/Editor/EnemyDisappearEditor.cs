using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyDisappear))]
public class EnemyDisappearEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Referansý al
        EnemyDisappear script = (EnemyDisappear)target;

        // disappearType enumunu göster
        SerializedProperty disappearTypeProp = serializedObject.FindProperty("disappearType");
        EditorGUILayout.PropertyField(disappearTypeProp);

        // disappearType enum deðeri kontrolü
        if ((DisappearType)disappearTypeProp.enumValueIndex == DisappearType.MoveToTopCornerAndDisappear)
        {
            SerializedProperty forceProp = serializedObject.FindProperty("moveToCornerForce");
            EditorGUILayout.PropertyField(forceProp, new GUIContent("Top Corner Force"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
