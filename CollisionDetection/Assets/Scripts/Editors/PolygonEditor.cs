using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Polygon))]
public class PolygonEditor : Editor
{
    SerializedProperty edges, vertices;
    SerializedProperty thickness, color;
    SerializedProperty showLine, isPhysical;

    bool editMode = false;

    private void OnEnable()
    {
        edges = serializedObject.FindProperty("edges");
        vertices = serializedObject.FindProperty("vertices");
        thickness = serializedObject.FindProperty("thickness");
        color = serializedObject.FindProperty("color");
        showLine = serializedObject.FindProperty("showLines");
        isPhysical = serializedObject.FindProperty("isPhysical");
    }

    public override void OnInspectorGUI()
    {
        var polygon = target as Polygon;

        EditorGUI.BeginChangeCheck();
        serializedObject.Update();
        EditorGUILayout.PropertyField(thickness);
        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(showLine);
        EditorGUILayout.PropertyField(isPhysical);
        EditorGUILayout.PropertyField(edges);
        EditorGUILayout.PropertyField(vertices);
        if (GUILayout.Button("Edit Vertices"))
        {
            editMode = !editMode;
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(polygon, "Changed Line Properties");
            if (vertices.arraySize < 3)
            {
                vertices.arraySize = 3;
                Debug.Log("too small vertices array");
            }
            serializedObject.ApplyModifiedProperties();   
            polygon.GenerateEdges();
        }



        polygon.OnEditorUpdate();
    }

    private void OnSceneGUI()
    {
        var polygon = target as Polygon;
        if (editMode)
        {
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                Handles.color = Color.white;
                EditorGUI.BeginChangeCheck();
                Vector3 newStart = Handles.PositionHandle(polygon.transform.TransformPoint(polygon.vertices[i]), Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(polygon, "Change Look At Target Position");
                    polygon.vertices[i] = polygon.transform.InverseTransformPoint(newStart);
                    polygon.OnEditorUpdate();
                    polygon.GenerateEdges();
                }
            }
        }
        

        

    }
}
