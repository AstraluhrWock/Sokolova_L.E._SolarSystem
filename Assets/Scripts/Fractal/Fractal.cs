using UnityEngine;

public class Fractal : MonoBehaviour
{
    private struct FractalPart
    {
        public Vector3 Direction;
        public Quaternion Rotation;
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
        public float SpinAngle;
    }
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField, Range(1, 8)] private int _depth = 4;
    [SerializeField, Range(1, 360)] private int _rotationSpeed = 80;

    private const float _positionOffset = 1.5f;
    private const float _scaleBias = 0.5f;
    private const int _childCount = 5;
    private FractalPart[][] _parts;
    private Matrix4x4[][] _matrices;
    private ComputeBuffer[] _matricesBuffers;

    private static readonly int _matricesID = Shader.PropertyToID("_Matrices");
    private static MaterialPropertyBlock _propertyBlock;
    private static readonly Vector3[] _directions = new Vector3[]
        {
            Vector3.up,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };
    private static readonly Quaternion[] _rotations =
        { 
            Quaternion.identity,
            Quaternion.Euler(0.0f, 0.0f, 90.0f),
            Quaternion.Euler(0.0f, 0.0f, -90.0f),
            Quaternion.Euler(90.0f, 0.0f, 0.0f),
            Quaternion.Euler(-90.0f, 0.0f, 0.0f)
        };

    private void OnEnable()
    {
        _parts = new FractalPart[_depth][];
        _matrices = new Matrix4x4[_depth][];
        _matricesBuffers = new ComputeBuffer[_depth];
        var stride = 16 * 4;
        for (int i = 0, length = 1; i < _parts.Length; i++, length *= _childCount)
        {
            _parts[i] = new FractalPart[length];
            _matrices[i] = new Matrix4x4[length];
            _matricesBuffers[i] = new ComputeBuffer(length, stride);
        }
        _parts[0][0] = CreatePart(0);
        for(int j=1; j < _parts.Length; j++)
        {
            var levelParts = _parts[j];
            for (int k = 0; k < levelParts.Length; k += _childCount)
            {
                for (int c = 0; c < _childCount; c++)
                {
                    levelParts[k + c] = CreatePart(c);
                }
            }
        }
        _propertyBlock ??= new MaterialPropertyBlock();
    }
    private void OnDisable()
    {
       for (int i=0; i < _matricesBuffers.Length; i++)
        {
            _matricesBuffers[i].Release();
        }
        _parts = null;
        _matrices = null;
        _matricesBuffers = null;
    }

    private void OnValidate()
    {
        if (_parts is null || !enabled)
        {
            return;
        }
        OnDisable();
        OnEnable();
    }

    private FractalPart CreatePart(int childIndex) => new FractalPart
    {
        Direction = _directions[childIndex],
        Rotation = _rotations[childIndex]
    };

    private void Update()
    {
        var spinAngelDelta = _rotationSpeed * Time.deltaTime;
        var rootPart = _parts[0][0];
        rootPart.SpinAngle += spinAngelDelta;
        var deltaRotation = Quaternion.Euler(0.0f, rootPart.SpinAngle, 0.0f);
        rootPart.WorldRotation = rootPart.Rotation * deltaRotation;
        _parts[0][0] = rootPart;
        _matrices[0][0] = Matrix4x4.TRS(rootPart.WorldPosition, rootPart.WorldRotation, Vector3.one);
        var scale = 1.0f;
        for (int i = 1; i < _parts.Length; i++)
        {
            scale *= _scaleBias;
            var parentParts = _parts[i - 1];
            var levelParts = _parts[i];
            var levelMatrices = _matrices[i];
            for (int j = 0; j < levelParts.Length; j++)
            {
                var parent = parentParts[j / _childCount];
                var part = levelParts[j];
                part.SpinAngle += spinAngelDelta;
                deltaRotation = Quaternion.Euler(0.0f, part.SpinAngle, 0.0f);
                part.WorldRotation = parent.WorldRotation * part.Rotation * deltaRotation;
                part.WorldPosition = parent.WorldPosition + parent.WorldRotation *
                    (_positionOffset * scale * part.Direction);
                levelParts[j] = part;
                levelMatrices[j] = Matrix4x4.TRS(part.WorldPosition, part.WorldRotation,
                    scale * Vector3.one);
            }
        }
        var bounds = new Bounds(rootPart.WorldPosition, 3f * Vector3.one);
        for (int i = 0; i < _matricesBuffers.Length; i++)
        {
            var buffer = _matricesBuffers[i];
            buffer.SetData(_matrices[i]);
            _propertyBlock.SetBuffer(_matricesID, buffer);
            _material.SetBuffer(_matricesID, buffer);
            Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, buffer.count, _propertyBlock);
        }
    }
}
