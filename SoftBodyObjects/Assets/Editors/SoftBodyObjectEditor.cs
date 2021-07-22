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
        fillFactor,
        repulsionCurveFactor;

    //float
    SerializedProperty
        mass,
        stiffness,
        restLength,
        dampingFactor,
        maxStrength,
        repulsionStrength,
        radius;

    //bool
    SerializedProperty 
        isStatic, 
        roundForces, 
        squareForce;

    private void OnEnable()
    {
        vertices = serializedObject.FindProperty("vertices");

        fillFactor = serializedObject.FindProperty("fillFactor");

        mass = serializedObject.FindProperty("mass");
        stiffness = serializedObject.FindProperty("stiffness");
        restLength = serializedObject.FindProperty("restLength");
        dampingFactor = serializedObject.FindProperty("dampingFactor");
        maxStrength = serializedObject.FindProperty("maxStrength");
        repulsionStrength = serializedObject.FindProperty("repulsionStrength");
        repulsionCurveFactor = serializedObject.FindProperty("repulsionCurveFactor");
        radius = serializedObject.FindProperty("radius");

        isStatic = serializedObject.FindProperty("isStatic");
        roundForces = serializedObject.FindProperty("roundForces");
        squareForce = serializedObject.FindProperty("squareForce");
    }
}
