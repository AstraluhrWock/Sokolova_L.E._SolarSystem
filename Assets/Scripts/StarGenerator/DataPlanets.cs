using UnityEngine;

internal class DataPlanets
{
    private GameObject _gameObject;
    private Vector3 _position;

    public GameObject GameObject
    {
        get => _gameObject;
        set => _gameObject = value;
    }

    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }
}
