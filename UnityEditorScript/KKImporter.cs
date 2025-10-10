using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class KKImporter : EditorWindow
{
    [MenuItem("Tools/KK Importer")]
    public static void ShowWindow()
    {
        GetWindow<KKImporter>("KK Importer");
    }

    private string kkPath;

    void OnGUI()
    {
        GUILayout.Label("KK 文件导入器", EditorStyles.boldLabel);

        kkPath = EditorGUILayout.TextField("文件路径:", kkPath);

        if (GUILayout.Button("选择 .kk 文件"))
        {
            kkPath = EditorUtility.OpenFilePanel("选择 KK 文件", "", "kk");
        }

        if (GUILayout.Button("导入并重建场景"))
        {
            if (File.Exists(kkPath))
            {
                ImportKK(kkPath);
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "文件不存在", "确定");
            }
        }
    }

    private void ImportKK(string path)
    {
        string[] lines = File.ReadAllLines(path);
        int index = 0;

        int boneCount = int.Parse(lines[index++]);
        var bones = new List<GameObjectInfo>();
        for (int i = 0; i < boneCount; i++)
        {
            string[] head = lines[index++].Split(',');
            string[] pos = lines[index++].Split(',');
            string[] rot = lines[index++].Split(',');
            string[] scale = lines[index++].Split(',');

            GameObjectInfo g = new GameObjectInfo();
            g.name = head[0];
            g.layer = int.Parse(head[1]);
            g.parent = int.Parse(head[2]);
            g.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            g.rotation = new Quaternion(float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]), float.Parse(rot[0]));
            g.scale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));
            bones.Add(g);
        }


        Dictionary<int, GameObject> goMap = new();
        for (int i = 0; i < bones.Count; i++)
        {
            GameObject go = new GameObject(bones[i].name);
            go.layer = bones[i].layer;
            go.transform.localPosition = bones[i].position;
            go.transform.localRotation = bones[i].rotation;
            go.transform.localScale = bones[i].scale;
            goMap[i] = go;
        }

        for (int i = 0; i < bones.Count; i++)
        {
            var info = bones[i];
            if (info.parent != info.id && goMap.ContainsKey(info.parent))
            {
                goMap[i].transform.SetParent(goMap[info.parent].transform, false);
            }
        }


        int meshCount = int.Parse(lines[index++]);
        var meshes = new List<Mesh>();
        for (int i = 0; i < meshCount; i++)
        {
            string[] header = lines[index++].Split(',');
            string meshName = header[0];
            int vertCount = int.Parse(header[1]);
            int uvCount = int.Parse(header[2]);
            int triCount = int.Parse(header[3]);
            int bindCount = int.Parse(header[4]);
            int blendCount = int.Parse(header[5]);
            int weightCount = int.Parse(header[6]);

            Mesh m = new Mesh();
            m.name = meshName;

            Vector3[] vertices = new Vector3[vertCount];
            for (int v = 0; v < vertCount; v++)
            {
                string[] pos = lines[index++].Split(',');
                vertices[v] = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            }

            Vector2[] uvs = new Vector2[uvCount];
            for (int v = 0; v < uvCount; v++)
            {
                string[] uv = lines[index++].Split(',');
                uvs[v] = new Vector2(float.Parse(uv[0]), float.Parse(uv[1]));
            }

            m.vertices = vertices;
            m.uv = uvs;

            for (int t = 0; t < triCount; t++)
            {
                string[] triLine = lines[index++].Split(',');
                int[] tris = System.Array.ConvertAll(triLine, int.Parse);
                m.SetTriangles(tris, t);
            }

            Matrix4x4[] bindposes = new Matrix4x4[bindCount];
            for (int b = 0; b < bindCount; b++)
            {
                string[] line = lines[index++].Split(',');
                float[] d = System.Array.ConvertAll(line, float.Parse);
                bindposes[b] = new Matrix4x4(
                    new Vector4(d[0], d[4], d[8], d[12]),
                    new Vector4(d[1], d[5], d[9], d[13]),
                    new Vector4(d[2], d[6], d[10], d[14]),
                    new Vector4(d[3], d[7], d[11], d[15])
                );
            }
            m.bindposes = bindposes;

            for (int b = 0; b < blendCount; b++)
            {
                string blendName = lines[index++];
                Vector3[] deltaPositions = new Vector3[vertCount];
                Vector3[] deltaNormals = new Vector3[vertCount];
                Vector3[] deltaTangents = new Vector3[vertCount];

                for (int v = 0; v < vertCount; v++)
                {
                    string[] line = lines[index++].Split(',');
                    deltaPositions[v] = new Vector3(float.Parse(line[0]), float.Parse(line[1]), float.Parse(line[2]));
                }
                for (int v = 0; v < vertCount; v++)
                {
                    string[] line = lines[index++].Split(',');
                    deltaNormals[v] = new Vector3(float.Parse(line[0]), float.Parse(line[1]), float.Parse(line[2]));
                }
                for (int v = 0; v < vertCount; v++)
                {
                    string[] line = lines[index++].Split(',');
                    deltaTangents[v] = new Vector3(float.Parse(line[0]), float.Parse(line[1]), float.Parse(line[2]));
                }

                m.AddBlendShapeFrame(blendName, 100f, deltaPositions, deltaNormals, deltaTangents);
            }

            BoneWeight[] boneWeights = new BoneWeight[weightCount];
            for (int w = 0; w < weightCount; w++)
            {
                string[] line = lines[index++].Split(',');
                boneWeights[w].boneIndex0 = int.Parse(line[0]);
                boneWeights[w].boneIndex1 = int.Parse(line[1]);
                boneWeights[w].boneIndex2 = int.Parse(line[2]);
                boneWeights[w].boneIndex3 = int.Parse(line[3]);
                boneWeights[w].weight0 = float.Parse(line[4]);
                boneWeights[w].weight1 = float.Parse(line[5]);
                boneWeights[w].weight2 = float.Parse(line[6]);
                boneWeights[w].weight3 = float.Parse(line[7]);
            }
            m.boneWeights = boneWeights;

            meshes.Add(m);
        }


        int matCount = int.Parse(lines[index++]);
        var mats = new List<Material>();
        for (int i = 0; i < matCount; i++)
        {
            string matName = lines[index++];
            Material m = new Material(Shader.Find("Standard"));
            m.name = matName;
            mats.Add(m);
        }


        int staticCount = int.Parse(lines[index++]);
        for (int i = 0; i < staticCount; i++)
        {
            string[] head = lines[index++].Split(',');
            string name = head[0];
            int meshIdx = int.Parse(head[1]);
            int goIdx = int.Parse(head[2]);
            int matCount2 = int.Parse(head[3]);

            // var matList = new List<int>();
            string[] matline = lines[index++].Split(' ');
            int[] matList = System.Array.ConvertAll(matline, int.Parse);
            // matList.Add()
            // for (int j = 0; j < matCount2; j++)
            // {
            //     matList.Add(int.Parse(lines[index++]));
            // }

            GameObject go = goMap[goIdx];
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = meshes[meshIdx];
            Material[] mArr = new Material[matList.Length];
            for (int j = 0; j < matList.Length; j++) mArr[j] = mats[matList[j]];
            mr.sharedMaterials = mArr;
        }


        int skinCount = int.Parse(lines[index++]);
        for (int i = 0; i < skinCount; i++)
        {
            string[] head = lines[index++].Split(',');
            string name = head[0];
            int meshIdx = int.Parse(head[1]);
            int goIdx = int.Parse(head[2]);
            int matCount2 = int.Parse(head[3]);
            int boneCount2 = int.Parse(head[4]);

            string[] matline = lines[index++].Split(' ');
            int[] matList = System.Array.ConvertAll(matline, int.Parse);
            // var matList = new List<int>();
            // for (int j = 0; j < matCount2; j++)
            //     matList.Add(int.Parse(lines[index++]));

            // var boneList = new List<int>();
            // for (int j = 0; j < boneCount2; j++)
            //     boneList.Add(int.Parse(lines[index++]));
            string[] boneline = lines[index++].Split(' ');
            int[] boneList = System.Array.ConvertAll(boneline, int.Parse);

            GameObject go = goMap[goIdx];
            var smr = go.AddComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = meshes[meshIdx];
            Material[] mArr = new Material[matList.Length];
            for (int j = 0; j < matList.Length; j++) mArr[j] = mats[matList[j]];
            smr.sharedMaterials = mArr;

            Transform[] boneRefs = new Transform[boneList.Length];
            for (int j = 0; j < boneList.Length; j++)
                boneRefs[j] = goMap[boneList[j]].transform;
            smr.bones = boneRefs;
        }

        EditorUtility.DisplayDialog("完成", "导入完成！", "确定");
    }

    private class GameObjectInfo
    {
        public int id;
        public string name;
        public int layer;
        public int parent;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}
