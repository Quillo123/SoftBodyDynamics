using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoftBodyObject))]
public class SoftBodyObjectEditor : Editor
{
    //Vector3
    SerializedProperty
        vertices;

    //int
    SerializedProperty
        fillFactor;

    //float
    SerializedProperty
        mass;

    //bool
    SerializedProperty
        isStatic,
        roundForces;

    //properties
    SerializedProperty
        massPointProperties,
        springProperties,
        massPoints,
        springs;

    bool editMode = false;
    bool showPointGrid = false;


    private void OnEnable()
    {
        vertices = serializedObject.FindProperty("polygon");

        fillFactor = serializedObject.FindProperty("fillFactor");

        mass = serializedObject.FindProperty("mass");

        isStatic = serializedObject.FindProperty("isStatic");
        roundForces = serializedObject.FindProperty("roundForces");

        massPointProperties = serializedObject.FindProperty("massPointProperties");
        springProperties = serializedObject.FindProperty("springProperties");

        massPoints = serializedObject.FindProperty("massPoints");
        springs = serializedObject.FindProperty("springs");
    }

    public override void OnInspectorGUI()
    {
        var sb = target as SoftBodyObject;

        EditorGUI.BeginChangeCheck();
        serializedObject.Update();

        GUILayout.Space(10);
        GUILayoutOption[] layoutOptions = { GUILayout.Width(50), GUILayout.Height(40) };
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit", layoutOptions))
        {
            editMode = !editMode;
            SceneView.RepaintAll();
        }
        if (sb.SB_Mesh.initialized)
        {
            string text = "Show Grid";
            if (showPointGrid)
            {
                text = "Hide Grid";
            }
            layoutOptions[0] = GUILayout.Width(75);
            if (GUILayout.Button(text, layoutOptions))
            {
                showPointGrid = !showPointGrid;
                SceneView.RepaintAll();
            }
            layoutOptions[0] = GUILayout.Width(115);
            if (GUILayout.Button("Regenerate Mesh", layoutOptions))
            {
                sb.GenerateMesh();
            }
        }
        else
        {
            layoutOptions[0] = GUILayout.Width(115);
            if (GUILayout.Button("Generate Mesh", layoutOptions))
            {
                sb.GenerateMesh();
            }
        }

        if(GUILayout.Button("Freeze Point", layoutOptions))
        {
            sb.massPoints[0].properties.isStatic = true;
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(mass);

        EditorGUILayout.PropertyField(fillFactor);
        //EditorGUILayout.Slider(sb.fillFactor, 3, 15);

        EditorGUILayout.PropertyField(isStatic);
        EditorGUILayout.PropertyField(roundForces);
        GUILayout.Space(10);


        EditorGUILayout.PropertyField(massPointProperties);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(springProperties);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(massPoints);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(springs);
        GUILayout.Space(10);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sb, "Changed SoftBodyObject Properties");
            if(fillFactor.intValue != sb.fillFactor && sb.SB_Mesh.initialized)
            {
                if(fillFactor.intValue % 2 == 0)
                {
                    fillFactor.intValue++;
                }
                serializedObject.ApplyModifiedProperties();
                sb.GenerateMesh();
            }
            else
            {
                serializedObject.ApplyModifiedProperties();
            }
            
        }


        EditorGUI.BeginChangeCheck();
        serializedObject.Update();

        EditorGUILayout.PropertyField(vertices);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sb, "Changed SoftBodyObject Properties");
            if (sb.SB_Mesh.initialized)
            {
                sb.GenerateMesh();
            }
        }

        sb.OnEditorUpdate();
    }

    void OnSceneGUI()
    {
        var sb = target as SoftBodyObject;

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < sb.vertices.Length; i++)
        {
            Handles.color = Color.green;

            float handleSize = HandleUtility.GetHandleSize(sb.transform.TransformPoint(sb.vertices[i])) * .05f;
            var mousePos = Helpers.GetMousePositionInEditor();


            Vector3 max;
            Vector3 min;

            

            if (editMode)
            {
                if (i != sb.vertices.Length - 1)
                {
                    Handles.DrawLine(sb.transform.TransformPoint(sb.vertices[i]), sb.transform.TransformPoint(sb.vertices[i + 1]));
                }
                else
                {
                    Handles.DrawLine(sb.transform.TransformPoint(sb.vertices[i]), sb.transform.TransformPoint(sb.vertices[0]));
                }

                sb.vertices[i] = sb.transform.InverseTransformPoint(Handles.FreeMoveHandle(
                sb.transform.TransformPoint(sb.vertices[i]),
                Quaternion.identity,
                handleSize,
                new Vector3(.1f, .1f, .1f),
                Handles.DotHandleCap));

                /**
                if (i != sb.vertices.Length - 1)
                {
                    max = new Vector3(
                        Mathf.Max(sb.transform.TransformPoint(sb.vertices[i]).x, sb.transform.TransformPoint(sb.vertices[i + 1]).x) + .5f, 
                        Mathf.Max(sb.transform.TransformPoint(sb.vertices[i]).y, sb.transform.TransformPoint(sb.vertices[i + 1]).y) + .5f, 0);
                    min = new Vector3(
                        Mathf.Min(sb.transform.TransformPoint(sb.vertices[i]).x, sb.transform.TransformPoint(sb.vertices[i + 1]).x) - .5f,
                        Mathf.Min(sb.transform.TransformPoint(sb.vertices[i]).y, sb.transform.TransformPoint(sb.vertices[i + 1]).y) - .5f, 0);
                }
                else
                {
                    max = new Vector3(
                        Mathf.Max(sb.transform.TransformPoint(sb.vertices[i]).x, sb.transform.TransformPoint(sb.vertices[0]).x) + .5f,
                        Mathf.Max(sb.transform.TransformPoint(sb.vertices[i]).y, sb.transform.TransformPoint(sb.vertices[0]).y) + .5f, 0);
                    min = new Vector3(                                                                                          
                        Mathf.Min(sb.transform.TransformPoint(sb.vertices[i]).x, sb.transform.TransformPoint(sb.vertices[0]).x) - .5f,
                        Mathf.Min(sb.transform.TransformPoint(sb.vertices[i]).y, sb.transform.TransformPoint(sb.vertices[0]).y) - .5f, 0);
                }

                if (mousePos.x <= max.x && mousePos.x >= min.x && mousePos.y <= max.y && mousePos.y >= min.y)
                {
                    GUI.SetNextControlName("Handle" + i + ',' + i + 1);
                    Vector3 handlePos, newPos;
                    if (i != sb.vertices.Length - 1)
                    {
                        handlePos = Helpers.ClosestPointOnLine(sb.vertices[i], sb.vertices[i + 1], mousePos);
                        newPos = sb.transform.InverseTransformPoint(Handles.FreeMoveHandle(
                        sb.transform.TransformPoint(handlePos),
                        Quaternion.identity,
                        handleSize * .75f,
                        new Vector3(.1f, .1f, .1f),
                        Handles.DotHandleCap));
                    }
                    else
                    {
                        handlePos = Helpers.ClosestPointOnLine(sb.vertices[i], sb.vertices[0], mousePos);
                        newPos = sb.transform.InverseTransformPoint(Handles.FreeMoveHandle(
                        sb.transform.TransformPoint(handlePos),
                        Quaternion.identity,
                        handleSize * .75f,
                        new Vector3(.1f, .1f, .1f),
                        Handles.DotHandleCap));
                    }
                    if(GUIUtility.hotControl != 0)
                    {
                        if(mousePos == handlePos)
                        {
                            sb.AddVertex(i, newPos);
                        }
                    }
                }
                **/
            }


            

        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(sb, "Changed SoftBodyObject's Polygon");
            serializedObject.ApplyModifiedProperties();
            sb.GenerateMesh();
            sb.OnEditorUpdate();
        }

        if (showPointGrid)
        {
            if(sb.springs == null || sb.springs.Length == 0)
            {
                Debug.Log("drawing Grid");
                Handles.color = Color.white;
                var v = sb.GetMeshVertices();
                var t = sb.GetMeshTriangles();
                for (int i = 0; i < t.Length - 1; i += 3)
                {
                    Handles.DrawLine(sb.transform.TransformPoint(v[t[i]]), sb.transform.TransformPoint(v[t[i + 1]]));
                    Handles.DrawLine(sb.transform.TransformPoint(v[t[i + 1]]), sb.transform.TransformPoint(v[t[i + 1]]));
                    Handles.DrawLine(sb.transform.TransformPoint(v[t[i + 2]]), sb.transform.TransformPoint(v[t[i]]));
                }
            }
            else
            {
                for(int i = 0; i < sb.springs.Length; i++)
                {
                    Handles.DrawLine(sb.transform.TransformPoint(sb.springs[i].A.position), sb.transform.TransformPoint(sb.springs[i].B.position));
                }
            }
        }
    }

    
}
