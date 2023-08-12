using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal class ShowGameObjectWindowEditor : EditorWindow
{
    [MenuItem("Window/Custom/Scene Objects")]
    public static void ShowWindow()
    {
        GetWindow<ShowGameObjectWindowEditor>("Window scene");
    }
    [SerializeField] private float _distanceBetweenPlanets = 2.0f;
    private List<DataPlanets> _planets;
    private DataPlanets _dataPlanets;

    private void OnEnable()
    {
        _planets = new List<DataPlanets>();
        _dataPlanets = new DataPlanets();
    }
    private void OnGUI()
    {   
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("TrackingObject");
        EditorGUILayout.LabelField("Planets", EditorStyles.boldLabel);
        for (int i=0; i < gameObjects.Length; i++)
        { 
            _dataPlanets.GameObject = gameObjects[i];
            _dataPlanets.Position = gameObjects[i].transform.position;
            _planets.Add(_dataPlanets);
            _planets[i].GameObject = (GameObject)EditorGUILayout.ObjectField(gameObjects[i].name, _planets[i].GameObject, typeof(GameObject), false);   
            _planets[i].Position = EditorGUILayout.Vector3Field("Position", _dataPlanets.Position);
            if (GUILayout.Button("Change Data", GUILayout.Width(100)))
            {
                ChangeData(i);
            }
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Prefab", GUILayout.Width(80)))
        {
            AddPrefab();
            Debug.Log("Add");
        }
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {     
            RemovePrefab(0);
        }   
        EditorGUILayout.EndHorizontal();
    }

    private void AddPrefab()
    {
        _planets.Add(new DataPlanets());
    }

    private void RemovePrefab(int index)
    {
        _planets.RemoveAt(index);
    }

    private void ChangeData(int index)
    {
        if (index >= 0 && index < _planets.Count)
        {
            DataPlanets dataPlanets = _planets[index];
            GameObject prefab = dataPlanets.GameObject;
            Vector3 newPosition = dataPlanets.Position;
            _planets[index].Position = newPosition;
            prefab.transform.position = newPosition;          
        }
    }
}
