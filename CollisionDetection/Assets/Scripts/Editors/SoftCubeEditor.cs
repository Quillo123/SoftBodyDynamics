using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoftCube))]
public class SoftCubeEditor : Editor
{
    SerializedProperty nsize;
    SerializedProperty fstiffness, frestLength, fdampFactor;
    SerializedProperty fmassPointRadius, fmass;
    SerializedProperty bisFixed;
    SerializedProperty mpSprite;

    private void OnEnable()
    {
        nsize = serializedObject.FindProperty("size");
        fstiffness = serializedObject.FindProperty("stiffness");
        frestLength = serializedObject.FindProperty("restLength");
        fdampFactor = serializedObject.FindProperty("dampingFactor");
        fmassPointRadius = serializedObject.FindProperty("massPointRadius");
        fmass = serializedObject.FindProperty("mass");
        bisFixed = serializedObject.FindProperty("isFixed");
        mpSprite = serializedObject.FindProperty("massPointSprite");
    }

    public override void OnInspectorGUI()
    {
        var softCube = target as SoftCube;

        EditorGUI.BeginChangeCheck();
        serializedObject.Update();
        EditorGUILayout.PropertyField(mpSprite);
        EditorGUILayout.PropertyField(nsize);
        EditorGUILayout.PropertyField(bisFixed);
        if (GUILayout.Button("Generate Cube"))
        {
            softCube.GenerateCube(DestroyImmediate, true);
        }
        EditorGUILayout.Space();

        EditorGUILayout.PrefixLabel("*Spring Properties*");
        EditorGUILayout.PropertyField(fstiffness);
        EditorGUILayout.PropertyField(frestLength);
        EditorGUILayout.PropertyField(fdampFactor);
        EditorGUILayout.Space();

        EditorGUILayout.PrefixLabel("*MassPoint Properties*");
        EditorGUILayout.PropertyField(fmassPointRadius);
        EditorGUILayout.PropertyField(fmass);
        EditorGUILayout.Space();


        

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(softCube, "ChangedSoftCubeProperties");
            serializedObject.ApplyModifiedProperties();
        }

        softCube.OnEditorUpdate();
    }
}
