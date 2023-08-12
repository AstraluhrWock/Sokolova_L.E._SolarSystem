using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
internal class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _planetsPrefab = new List<GameObject>();
    [SerializeField] private GameObject _sunPrefab;
    [SerializeField] private float _distanceBetweenPlanets = 2.0f;

    private void OnEnable()
    {
        GameObject sun = Instantiate(_sunPrefab, transform);
        sun.transform.position = Vector3.zero;

        for (int i = 0; i < _planetsPrefab.Count; i++)
        {
            float angle = i * (360.0f / 5);
            Vector3 position = Quaternion.Euler(0, angle, 0) * Vector3.forward * _distanceBetweenPlanets;
            GameObject planet = Instantiate(_planetsPrefab[i], transform);
            planet.transform.position = position;
        }
    }


   
}

