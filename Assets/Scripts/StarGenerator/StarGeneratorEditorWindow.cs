using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

internal class StarGeneratorEditorWindow : EditorWindow
{
    [MenuItem("Window/Custom/Star Generator")]
    public static void ShowWindow()
    {
        GetWindow<StarGeneratorEditorWindow>("Star Generator");
    }

    [SerializeField] private GameObject _starPrefabs;
    private List<SpawnZone> _spawnZones = new List<SpawnZone>();
    private List<GameObject> _spawnObjects = new List<GameObject>();

    private void OnGUI()
    {
        _starPrefabs = (GameObject)EditorGUILayout.ObjectField("Prefab", _starPrefabs, typeof(GameObject), false);
      
        if (GUILayout.Button("Add Spawn Zone"))
        {
            AddSpawnZone();
        }

        GUILayout.Space(10);

        for (int i = 0; i < _spawnZones.Count; i++)
        {
            _spawnZones[i].CountOfPrefab = EditorGUILayout.IntField("Count of Prefabs", _spawnZones[i].CountOfPrefab);
            EditorGUILayout.BeginHorizontal();  
            GUILayout.Label($"Zone {i + 1}", GUILayout.Width(50));
            _spawnZones[i].Position = EditorGUILayout.Vector3Field("Position", _spawnZones[i].Position);   
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveSpawnZone(i);
            }
            EditorGUILayout.EndHorizontal();    
            _spawnZones[i].Radius = EditorGUILayout.FloatField("Radius", _spawnZones[i].Radius);
            if (GUILayout.Button("Spawn Objects"))
            {
                SpawnObjectsInZone(_spawnZones[i]);
            }
        }
    }

    private void AddSpawnZone()
    {
        _spawnZones.Add(new SpawnZone());
    }

    private void RemoveSpawnZone(int index)
    {
        _spawnZones.RemoveAt(index);
    }

    private void SpawnObjectsInZone(SpawnZone spawnZone)
    {
        GameObject starParent = new GameObject("Stars");
        starParent.transform.position = Vector3.zero;
        for (int i = 0; i < spawnZone.CountOfPrefab; i++)
        { 
            Vector3 randomPointInCircle = spawnZone.Position + Random.insideUnitSphere * spawnZone.Radius;
            GameObject spawnObject = Instantiate(_starPrefabs, randomPointInCircle, Quaternion.identity);
            spawnObject.transform.SetParent(starParent.transform);
            _spawnObjects.Add(spawnObject);    
        }
    }
}

