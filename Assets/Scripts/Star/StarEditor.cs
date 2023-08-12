using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Star)), CanEditMultipleObjects]
public class StarEditor : Editor
{
    private SerializedProperty _center;
    private SerializedProperty _points;
    private SerializedProperty _frequency;
    private Vector3 _pointSnap = new Vector3(0.1f, 0.1f, 0.1f);

    private GUIContent _moveUpButton = new GUIContent("\u2191", "Move up");
    private GUIContent _moveDownButton = new GUIContent("\u2193", "Move down");
    private GUIContent _addButton = new GUIContent("+", "Add");
    private GUIContent _removeButton = new GUIContent("-", "Remove");

    private void OnEnable()
    {
        _center = serializedObject.FindProperty("center");
        _points = serializedObject.FindProperty("points");
        _frequency = serializedObject.FindProperty("frequency");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_center); 
        ShowGUIOfColorPoints();          
        EditorGUILayout.PropertyField(_frequency);
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (!(target is Star star))
        {
            return;
        }
        var starTransform = star.transform;
        var angle = -360f / (star.frequency * star.points.Length);

        for (var i = 0; i < star.points.Length; i++)
        {
            var rotation = Quaternion.Euler(0f, 0f, angle * i);
            var oldPoint = starTransform.TransformPoint(rotation * star.points[i].Position);
            var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, 0.02f, _pointSnap, Handles.DotHandleCap);
            if (oldPoint == newPoint)
            {
                continue;
            }
            star.points[i].Position = Quaternion.Inverse(rotation) * starTransform.InverseTransformPoint(newPoint);
            star.UpdateMesh();
        }
    }

    private void ShowGUIOfColorPoints()
    {
        for (int i = 0; i < _points.arraySize; i++)
        {   
            EditorGUILayout.PropertyField(_points.GetArrayElementAtIndex(i));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(_moveUpButton)) 
            {
                _points.MoveArrayElement(i, i - 1);
            }
            if (GUILayout.Button(_moveDownButton)) 
            {
                _points.MoveArrayElement(i, i + 1);
            }
            if (GUILayout.Button(_addButton)) 
            {
                _points.InsertArrayElementAtIndex(i);
            }
            if (GUILayout.Button(_removeButton))
            {
                _points.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}


