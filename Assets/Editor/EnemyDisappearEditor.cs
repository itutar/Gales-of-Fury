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

        // Referans� al
        EnemyDisappear script = (EnemyDisappear)target;

        // disappearType enumunu g�ster
        SerializedProperty disappearTypeProp = serializedObject.FindProperty("disappearType");
        EditorGUILayout.PropertyField(disappearTypeProp);

        // disappearType enum de�eri kontrol�
        if ((DisappearType)disappearTypeProp.enumValueIndex == DisappearType.MoveToTopCornerAndDisappear)
        {
            SerializedProperty forceProp = serializedObject.FindProperty("moveToCornerForce");
            EditorGUILayout.PropertyField(forceProp, new GUIContent("Top Corner Force"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
