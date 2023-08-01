using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine;

partial class CameraRenderer
{
    partial void DrawGizmos();
    partial void DrawGizmos()
    {
        if (!Handles.ShouldRenderGizmos())
        {
            return;

            _renderContext.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _renderContext.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }
    partial void UIGO();

    partial void UIGO()
    {
        if (_camera.cameraType == CameraType.SceneView)
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
    }
}
