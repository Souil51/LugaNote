using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transition))]
[CanEditMultipleObjects]
public class TransitionEditor : Editor
{
    SerializedProperty Duration;
    SerializedProperty InitialPosition;

    SerializedProperty LeftPart;
    SerializedProperty RightPart;

    SerializedProperty LeftPartYPosition_Open_1;
    SerializedProperty RightPartYPosition_Open_1;

    SerializedProperty LeftPartYPosition_Close;
    SerializedProperty RightPartYPosition_Close;

    SerializedProperty LeftPartYPosition_Open_2;
    SerializedProperty RightPartYPosition_Open_2;

    void OnEnable()
    {
        Duration = serializedObject.FindProperty("Duration");
        InitialPosition = serializedObject.FindProperty("InitialPosition");
        LeftPart = serializedObject.FindProperty("LeftPart");
        RightPart = serializedObject.FindProperty("RightPart");
        LeftPartYPosition_Open_1 = serializedObject.FindProperty("LeftPartYPosition_Open_1");
        RightPartYPosition_Open_1 = serializedObject.FindProperty("RightPartYPosition_Open_1");
        LeftPartYPosition_Close = serializedObject.FindProperty("LeftPartYPosition_Close");
        RightPartYPosition_Close = serializedObject.FindProperty("RightPartYPosition_Close");
        LeftPartYPosition_Open_2 = serializedObject.FindProperty("LeftPartYPosition_Open_2");
        RightPartYPosition_Open_2 = serializedObject.FindProperty("RightPartYPosition_Open_2");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(Duration);
        EditorGUILayout.PropertyField(InitialPosition);
        EditorGUILayout.PropertyField(LeftPart);
        EditorGUILayout.PropertyField(RightPart);
        EditorGUILayout.PropertyField(LeftPartYPosition_Open_1);
        EditorGUILayout.PropertyField(RightPartYPosition_Open_1);
        EditorGUILayout.PropertyField(LeftPartYPosition_Close);
        EditorGUILayout.PropertyField(RightPartYPosition_Close);
        EditorGUILayout.PropertyField(LeftPartYPosition_Open_2);
        EditorGUILayout.PropertyField(RightPartYPosition_Open_2);
        serializedObject.ApplyModifiedProperties();

        if(target is Transition t)
        {
            if (t.CurrentPosition == TransitionPosition.Close)
                t.SetPositionClose();
            else if (t.CurrentPosition == TransitionPosition.Open_1)
                t.SetPositionOpen_1();
            else
                t.SetPositionOpen_2();
        }
    }
}
