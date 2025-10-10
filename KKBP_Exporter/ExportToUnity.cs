using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameObjectInfo
{
    private static int idx = 0;
    public int id;
    public string name;
    public int layer;
    public int parent;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public GameObjectInfo(GameObject game)
    {
        this.id = idx++;
        if (game.name != "BodyTop")
        {
            this.parent = game.transform.parent.gameObject.GetInstanceID();
        }
        else
        {
            this.parent = game.GetInstanceID();
        }
        this.name = game.name;
        this.layer = game.layer;
        this.position = game.transform.localPosition;
        this.rotation = game.transform.localRotation;
        this.scale = game.transform.localScale;
    }
    public void WriteToText(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{this.name},{this.layer},{this.parent}");
        stringBuilder.AppendLine($"{this.position.x},{this.position.y},{this.position.z}");
        stringBuilder.AppendLine($"{this.rotation.w},{this.rotation.x},{this.rotation.y},{this.rotation.z}");
        stringBuilder.AppendLine($"{this.scale.x},{this.scale.y},{this.scale.z}");
    }
    public static void Reset()
    {
        idx = 0;
    }
}

public class MeshInfo
{
    private static int idx = 0;
    public int id;
    public string name;
    public Vector3[] vertices;
    public Vector2[] uv;
    public List<int[]> triangles = new();
    public List<Matrix4x4> bindPoses = new();
    public CBlendShapeInfo[] blendShapeinfos;
    public List<BoneWeight> boneWeights = new();

    public MeshInfo(Mesh mesh)
    {
        this.id = idx++;
        this.name = mesh.name;
        this.vertices = mesh.vertices;
        this.uv = mesh.uv;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            this.triangles.Add(mesh.GetTriangles(i));
        }
        mesh.GetBindposes(this.bindPoses);
        this.blendShapeinfos = new CBlendShapeInfo[mesh.blendShapeCount];
        for(int i = 0; i < mesh.blendShapeCount; i++)
        {
            CBlendShapeInfo cBlendShapeInfo = new CBlendShapeInfo();
            cBlendShapeInfo.name = mesh.GetBlendShapeName(i);
            cBlendShapeInfo.deltaPositions = new Vector3[mesh.vertices.Length];
            cBlendShapeInfo.deltaNormals = new Vector3[mesh.normals.Length];
            cBlendShapeInfo.deltaTangents = new Vector3[mesh.tangents.Length];

            mesh.GetBlendShapeFrameVertices(i, 0, cBlendShapeInfo.deltaPositions, cBlendShapeInfo.deltaNormals, cBlendShapeInfo.deltaTangents);
            this.blendShapeinfos[i] = cBlendShapeInfo;
        }
        mesh.GetBoneWeights(this.boneWeights);

    }
    public void WriteToText(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{this.name},{this.vertices.Length},{this.uv.Length},{this.triangles.Count},{this.bindPoses.Count},{this.blendShapeinfos.Length},{this.boneWeights.Count}");
        foreach(var i in this.vertices)
        {
            stringBuilder.AppendLine($"{i.x},{i.y},{i.z}");
        }
        foreach(var i in this.uv)
        {
            stringBuilder.AppendLine($"{i.x},{i.y}");
        }
        foreach(var i in this.triangles)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var j in i)
            {
                sb.Append(j + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            stringBuilder.AppendLine(sb.ToString());
        }
        foreach(var i in this.bindPoses)
        {
            stringBuilder.AppendLine($"{i.m00},{i.m01},{i.m02},{i.m03},{i.m10},{i.m11},{i.m12},{i.m13},{i.m20},{i.m21},{i.m22},{i.m23},{i.m30},{i.m31},{i.m32},{i.m33}");
        }
        foreach(var i in this.blendShapeinfos)
        {
            stringBuilder.AppendLine(i.name);
            foreach(var j in i.deltaPositions)
            {
                stringBuilder.AppendLine($"{j.x},{j.y},{j.z}");
            }
            foreach(var j in i.deltaNormals)
            {
                stringBuilder.AppendLine($"{j.x},{j.y},{j.z}");
            }
            foreach(var j in i.deltaTangents)
            {
                stringBuilder.AppendLine($"{j.x},{j.y},{j.z}");
            }
        }
        foreach(var i in this.boneWeights)
        {
            stringBuilder.AppendLine($"{i.boneIndex0},{i.boneIndex1},{i.boneIndex2},{i.boneIndex3},{i.weight0},{i.weight1},{i.weight2},{i.weight3}");
        }
    }
    public static void Reset()
    {
        idx = 0;
    }
}
public class CBlendShapeInfo
{
    public string name;
    public Vector3[] deltaPositions;
    public Vector3[] deltaNormals;
    public Vector3[] deltaTangents;
}

public class StaticMeshRendererInfo
{
    private static int idx = 0;
    public int id;
    public string name;
    public int mesh;
    public int[] materials;
    public int gameobject;

    public StaticMeshRendererInfo(MeshRenderer meshRenderer, int mesh, int gameobject, Dictionary<int, CMaterialInfo> dic) 
    {
        this.id = idx++;
        this.mesh = mesh;
        this.name = meshRenderer.name;
        this.materials = new int[meshRenderer.sharedMaterials.Length];
        for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
        {
            this.materials[i] = dic[meshRenderer.sharedMaterials[i].GetInstanceID()].id;
        }
        this.gameobject = gameobject;
    }
    public void WriteToText(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{this.name},{this.mesh},{this.gameobject},{this.materials.Length}");
        var sb = new StringBuilder();
        foreach (var i in this.materials)
        {
            sb.Append(i + " ");
        }
        sb.Remove(sb.Length - 1, 1);
        stringBuilder.AppendLine(sb.ToString());
    }

    public static void Reset()
    {
        idx = 0;
    }
}
public class SkinnedMeshRendererInfo
{
    private static int idx = 0;
    public int id;
    public string name;
    public int mesh;
    public int[] materials;
    public int[] bones;
    public int gameobject;

    public SkinnedMeshRendererInfo(SkinnedMeshRenderer meshRenderer, int mesh, int gameobject, Dictionary<int, GameObjectInfo> dic, Dictionary<int, CMaterialInfo> dic2)
    {
        this.id= idx++;
        this.mesh = mesh;
        this.name = meshRenderer.name;
        this.gameobject = gameobject;
        this.materials = new int[meshRenderer.sharedMaterials.Length];
        for(int i = 0;i < meshRenderer.sharedMaterials.Length; i++)
        {
            this.materials[i] = dic2[meshRenderer.sharedMaterials[i].GetInstanceID()].id;
        }
        this.bones = new int[meshRenderer.bones.Length];
        for(int i = 0; i < meshRenderer.bones.Length; i++)
        {
            this.bones[i] = dic[meshRenderer.bones[i].gameObject.GetInstanceID()].id;
        }
    }
    public void WriteToText(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{this.name},{this.mesh},{this.gameobject},{this.materials.Length},{this.bones.Length}");
        StringBuilder sb = new StringBuilder();
        foreach (var i in this.materials)
        {
            sb.Append(i + " ");
        }
        sb.Remove(sb.Length - 1, 1);
        stringBuilder.AppendLine(sb.ToString());

        sb = new StringBuilder();
        foreach (var i in this.bones) 
        { 
            sb.Append(i + " ");
        }
        sb.Remove(sb.Length - 1, 1);
        stringBuilder.AppendLine(sb.ToString());
    }
    public static void Reset()
    {
        idx = 0;
    }
    
}
public class CMaterialInfo
{
    private static int idx = 0;
    public int id;
    public string name;
    public List<string> properties;
    public List<string> textures;
    public List<Color> colors;
    public List<float> floats;
    public List<Vector2> offsets;
    public List<Vector2> scales; 
    public CMaterialInfo(Material material)
    {
        this.name = material.name;
        this.id = idx++;        
    }
    public void WriteToText(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine(this.name);
    }
    public static void Reset() 
    {
        idx = 0;
    }

}