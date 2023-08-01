using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

partial class CameraRenderer
{
    private ScriptableRenderContext _renderContext;
    private Camera _camera;
    private CommandBuffer _commandBuffer;
    private CullingResults _cullingResults;

    private static readonly List<ShaderTagId> _drawingShaderTagIds = new List<ShaderTagId> { new ShaderTagId("SRPDefaultUnlit") };

    private const string _bufferName = "Camera Wah";

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _camera = camera;
        _renderContext = context;
        UIGO();
        if (!Cull(out var parameters))
        {
            return;
        }
        Settings(parameters);
        DrawVisible();
        DrawUnsupportedShaders();
        DrawGizmos();
        Sumbit();
    }

    private void Sumbit()
    {
        _commandBuffer.EndSample(_bufferName);
        ExecuteCommandBuffer();
        _renderContext.Submit();
    }

    private void DrawVisible()
    {
        var drawingSettings = CreateDrawingSettings(_drawingShaderTagIds, SortingCriteria.CommonOpaque, out var sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        _renderContext.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        _renderContext.DrawSkybox(_camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        _renderContext.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
    }

    private void Settings(ScriptableCullingParameters parameters)
    {
        _commandBuffer = new CommandBuffer { name = _camera.name };
        _cullingResults = _renderContext.Cull(ref parameters);
        _renderContext.SetupCameraProperties(_camera);
        _commandBuffer.ClearRenderTarget(true, true, Color.clear);
        _commandBuffer.BeginSample(_bufferName);
        _commandBuffer.SetGlobalColor("_GlobalCal", Color.blue);
        ExecuteCommandBuffer();
    }

    private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, SortingCriteria sortingCriteria, out SortingSettings sortingSettings)
    {
        sortingSettings = new SortingSettings(_camera)
        {
            criteria = sortingCriteria,
        };

        var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
        for (var i = 1; i < shaderTags.Count; i++)
        {
            drawingSettings.SetShaderPassName(i, shaderTags[i]);
        }
        return drawingSettings;
    }

    private bool Cull(out ScriptableCullingParameters parameters)
    {
        return _camera.TryGetCullingParameters(out parameters);
    }

    private void ExecuteCommandBuffer()
    {
        _renderContext.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }

 
}

