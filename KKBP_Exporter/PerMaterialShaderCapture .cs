using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace KKBP_Exporter;
public class PerMaterialShaderCapture : MonoBehaviour
{
    public Camera targetCamera;


    public IEnumerator CaptureRendererPerMaterial(
        SkinnedMeshRenderer smr,
        Material mat,
        RenderTexture renderTexture,
        int index)
    {
        if (targetCamera == null)
        {
            Console.WriteLine("[PerMaterialShaderCapture] targetCamera is null.");
            yield break;
        }
        //if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

        yield return StartCoroutine(CaptureCoroutine(smr, mat, renderTexture ,index));
    }

    IEnumerator CaptureCoroutine(SkinnedMeshRenderer smr, Material mat, RenderTexture renderTexture, int index)
    {
        var cmd = new CommandBuffer();

        cmd.SetRenderTarget(renderTexture);
        cmd.ClearRenderTarget(true, true, Color.clear);
        try
        {
            cmd.DrawRenderer(smr, mat, index, -1);
            targetCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, cmd);
        }
        catch (System.Exception ex)
        {
            cmd.Release();
            cmd = null;

            Mesh baked = new Mesh();
            smr.BakeMesh(baked);

            cmd = new CommandBuffer();
            cmd.SetRenderTarget(renderTexture);
            cmd.ClearRenderTarget(true, true, Color.clear);

            Matrix4x4 m = smr.transform.localToWorldMatrix;
            cmd.DrawMesh(baked, m, mat, index, -1);

            targetCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, cmd);
        }

        yield return new WaitForEndOfFrame();

        //Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        //var prev = RenderTexture.active;
        //RenderTexture.active = rt;
        //tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        //tex.Apply();
        //RenderTexture.active = prev;

        //File.WriteAllBytes(path, tex.EncodeToPNG());

        if (cmd != null)
        {
            try { targetCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, cmd); } catch { }
            cmd.Release();
        }
        //RenderTexture.ReleaseTemporary(rt);
        //Destroy(tex);
    }
}
