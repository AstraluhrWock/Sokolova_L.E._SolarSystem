using UnityEngine;

internal class SpawnZone
{
    private Vector3 _position;
    private float _radius;
    private int _countOfPrefab;

    public Vector3 Position 
    {
        get => _position;
        set => _position = value;
    }

    public float Radius 
    {
        get => _radius;
        set => _radius = value;
    }

    public int CountOfPrefab
    {
        get => _countOfPrefab;
        set => _countOfPrefab = value;
    }
}