using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Star : MonoBehaviour
{
    [SerializeField] public ColorPoint center;
    [SerializeField, NonReorderable] public ColorPoint[] points;
    [SerializeField] public int frequency = 1;

    public Vector3[] _vertices;
    public Color[] _colors;
    public Mesh _mesh;
    public int[] _triangles;

    private void OnEnable()
    {
        UpdateMesh();
    }
    public void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Star Mesh";
        if (frequency < 1)
        {
            frequency = 1;
        }
        points ??= Array.Empty<ColorPoint>();
        var numberOfPoints = frequency * points.Length;
        if (_vertices == null || _vertices.Length != numberOfPoints + 1)
        {
            _vertices = new Vector3[numberOfPoints + 1];
            _colors = new Color[numberOfPoints + 1];
            _triangles = new int[numberOfPoints * 3];
            _mesh.Clear();
        }
        if (numberOfPoints >= 3)
        {
            _vertices[0] = center.Position;
            _colors[0] = center.Color;
            var angle = -360f / numberOfPoints;
            for (int repetitions = 0, v = 1, t = 1; repetitions < frequency; repetitions++)
            {
                for (var p = 0; p < points.Length; p++, v++, t += 3)
                {
                    _vertices[v] = Quaternion.Euler(0f, 0f, angle * (v - 1)) * points[p].Position;
                    _colors[v] = points[p].Color;
                    _triangles[t] = v;
                    _triangles[t + 1] = v + 1;
                }
            }
            _triangles[_triangles.Length - 1] = 1;
        }
        _mesh.vertices = _vertices;
        _mesh.colors = _colors;
        _mesh.triangles = _triangles;
    }

    private void Reset()
    {
        UpdateMesh();
    }
}

