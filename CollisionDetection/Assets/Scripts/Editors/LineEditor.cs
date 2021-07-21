using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LineComponent))]
public class LineEditor : Editor
{ 
    SerializedProperty line, line2;
    SerializedProperty thickness, color;
    SerializedProperty showLine;

    Vector3 lastPos;

    private void OnEnable()
    {
        line = serializedObject.FindProperty("line");
        thickness = serializedObject.FindProperty("thickness");
        color = serializedObject.FindProperty("color");
        showLine = serializedObject.FindProperty("showLine");

        line2 = serializedObject.FindProperty("line2");
    }

    public override void OnInspectorGUI() {
        var lineComp = target as LineComponent;

        EditorGUI.BeginChangeCheck();
        serializedObject.Update();
        EditorGUILayout.PropertyField(thickness);
        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(showLine);
        EditorGUILayout.PropertyField(line);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(lineComp, "Changed Line Properties");
            serializedObject.ApplyModifiedProperties();
        }




        if (lineComp.thickness == 0)
            lineComp.thickness = 1;
        lineComp.OnEditorUpdate();
    }

    private void OnSceneGUI()
    {
        var lineComp = target as LineComponent;
        var line = Line.Clone(lineComp.line);

        if (!lineComp.isPoly)
        {

            Handles.color = Color.white;
            EditorGUI.BeginChangeCheck();
            Vector3 newStart = Handles.PositionHandle(lineComp.transform.TransformPoint(line.start), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(lineComp, "Change Look At Target Position");
                lineComp.SetStart(lineComp.transform.InverseTransformPoint(newStart));
                lineComp.CenterPositionOnLine();
                lineComp.OnEditorUpdate();
            }

            Handles.color = Color.white;

            EditorGUI.BeginChangeCheck();
            Vector3 newEnd = Handles.PositionHandle(lineComp.transform.TransformPoint(line.end), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(lineComp, "Change Look At Target Position");
                lineComp.SetEnd(lineComp.transform.InverseTransformPoint(newEnd));
                lineComp.CenterPositionOnLine();
                lineComp.OnEditorUpdate();
            }

            lastPos = lineComp.transform.position;
        }

    }
}
