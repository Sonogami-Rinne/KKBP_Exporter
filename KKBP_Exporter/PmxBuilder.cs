using Accessory_States;
using ChaCustom;
using IllusionUtility.SetUtility;
using KKAPI.Maker;
using KKAPI.Utilities;
using PmxLib;
using StrayTech;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;


internal class PmxBuilder
{
    private class GagData
    {
        public string MainTex;

        public int EyeObjNo;

        public GagData(string mainTex, int eyeObjNo)
        {
            MainTex = mainTex;
            EyeObjNo = eyeObjNo;
        }
    }

    public string msg = "";

    public HashSet<string> ignoreList = new HashSet<string>
    {
        "Bonelyfans", "c_m_shadowcast", "Standard", "cf_m_body", "cf_m_face_00", "cf_m_tooth", "cf_m_canine", "cf_m_mayuge_00", "cf_m_noseline_00", "cf_m_eyeline_00_up",
        "cf_m_eyeline_kage", "cf_m_eyeline_down", "cf_m_sirome_00", "cf_m_hitomi_00", "cf_m_tang", "cf_m_namida_00", "cf_m_gageye_00", "cf_m_gageye_01", "cf_m_gageye_02", "o_hit_armL_M",
        "o_hit_armR_M", "o_hit_footL_M", "o_hit_footR_M", "o_hit_handL_M", "o_hit_handR_M", "o_hit_hara_M", "o_hit_haraB_M", "o_hit_haraUnder_M", "o_hit_kneeBL_M", "o_hit_kneeBR_M",
        "o_hit_kneeL_M", "o_hit_kneeR_M", "o_hit_kokan_M", "o_hit_legBL_M", "o_hit_legBR_M", "o_hit_legL_M", "o_hit_legR_M", "o_hit_mune_M", "o_hit_muneB_M", "o_hit_siriL_M",
        "o_hit_siriR_M", "cf_O_face_atari_M", "Highlight_cm_O_face_rend", "cm_m_body", "Highlight_o_body_a_rend", "Highlight_cf_O_face_rend", "o_shadowcaster", "o_body_a", "cf_O_face", "cf_O_tooth",
        "cf_O_canine", "cf_O_mayuge", "cf_O_noseline", "cf_O_eyeline", "cf_O_eyeline_low", "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "cf_Ohitomi_L", "cf_Ohitomi_R",
        "cf_Ohitomi_L02", "cf_Ohitomi_R02", "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02", "o_tang", "o_hit_armL", "o_hit_armR", "o_hit_footL", "o_hit_footR",
        "o_hit_handL", "o_hit_handR", "o_hit_hara", "o_hit_haraB", "o_hit_haraUnder", "o_hit_kneeBL", "o_hit_kneeBR", "o_hit_kneeL", "o_hit_kneeR", "o_hit_kokan",
        "o_hit_legBL", "o_hit_legBR", "o_hit_legL", "o_hit_legR", "o_hit_mune", "o_hit_muneB", "o_hit_siriL", "o_hit_siriR", "cf_O_face_atari"
    };

    public static readonly Dictionary<string, string> typeMap = new Dictionary<string, string>
    {
        { "_MT_CT", "_MainTex_ColorTexture" },
        { "_MT", "_MainTex" },
        { "_AM", "_AlphaMask" },
        { "_CM", "_ColorMask" },
        { "_DM", "_DetailMask" },
        { "_LM", "_LineMask" },
        { "_NM", "_NormalMask" },
        { "_NMP", "_NormalMap" },
        { "_NMPD", "_NormalMapDetail" },
        { "_ot1", "_overtex1" },
        { "_ot2", "_overtex2" },
        { "_ot3", "_overtex3" },
        { "_lqdm", "_liquidmask" },
        { "_HGLS", "_HairGloss" },
        { "_T2", "_Texture2" },
        { "_T3", "_Texture3" },
        { "_T4", "_Texture4" },
        { "_T5", "_Texture5" },
        { "_T6", "_Texture6" },
        { "_T7", "_Texture7" },
        { "_PM1", "_PatternMask1" },
        { "_PM2", "_PatternMask2" },
        { "_PM3", "_PatternMask3" },
        { "_AR", "_AnotherRamp" },
        { "_GLSR", "_GlassRamp" },
        { "_EXPR", "_expression" }
    };

    public string EyeMatName = "cf_m_hitomi_00";

    public HashSet<string> whitelistOffsetBones = new HashSet<string>
    {
        "cf_d_sk_top", "cf_d_sk_00_00", "cf_j_sk_00_00", "cf_j_sk_00_01", "cf_j_sk_00_02", "cf_j_sk_00_03", "cf_j_sk_00_04", "cf_j_sk_00_05", "cf_d_sk_01_00", "cf_j_sk_01_00",
        "cf_j_sk_01_01", "cf_j_sk_01_02", "cf_j_sk_01_03", "cf_j_sk_01_04", "cf_j_sk_01_05", "cf_d_sk_02_00", "cf_j_sk_02_00", "cf_j_sk_02_01", "cf_j_sk_02_02", "cf_j_sk_02_03",
        "cf_j_sk_02_04", "cf_j_sk_02_05", "cf_d_sk_03_00", "cf_j_sk_03_00", "cf_j_sk_03_01", "cf_j_sk_03_02", "cf_j_sk_03_03", "cf_j_sk_03_04", "cf_j_sk_03_05", "cf_d_sk_04_00",
        "cf_j_sk_04_00", "cf_j_sk_04_01", "cf_j_sk_04_02", "cf_j_sk_04_03", "cf_j_sk_04_04", "cf_j_sk_04_05", "cf_d_sk_05_00", "cf_j_sk_05_00", "cf_j_sk_05_01", "cf_j_sk_05_02",
        "cf_j_sk_05_03", "cf_j_sk_05_04", "cf_j_sk_05_05", "cf_d_sk_06_00", "cf_j_sk_06_00", "cf_j_sk_06_01", "cf_j_sk_06_02", "cf_j_sk_06_03", "cf_j_sk_06_04", "cf_j_sk_06_05",
        "cf_d_sk_07_00", "cf_j_sk_07_00", "cf_j_sk_07_01", "cf_j_sk_07_02", "cf_j_sk_07_03", "cf_j_sk_07_04", "cf_j_sk_07_05"
    };

    public bool exportAll;

    public bool exportHitBoxes;

    public bool exportWithEnabledShapekeys;

    public bool exportCurrentPose;

    public bool exportLightDarkTexture = false;

    public static int minCoord;

    public static int maxCoord;

    public static int nowCoordinate = -1;

    public string baseSavePath;

    private string savePath;

    public static Dictionary<string, int> currentMaterialList = new Dictionary<string, int>();

    public static HashSet<string> currentBonesList = new HashSet<string>();

    public Dictionary<Renderer, List<string>> currentRendererMaterialMapping = new Dictionary<Renderer, List<string>>();

    public static Pmx pmxFile;

    private List<SMRData> characterSMRData = new List<SMRData>();

    private List<MaterialDataComplete> materialDataComplete = new List<MaterialDataComplete>();

    private List<TextureData> textureData = new List<TextureData>();

    private List<MaterialData> materialData = new List<MaterialData>();

    private List<ClothesData> clothesData = new List<ClothesData>();

    private List<AccessoryData> accessoryData = new List<AccessoryData>();

    private List<ReferenceInfoData> referenceInfoData = new List<ReferenceInfoData>();

    private List<ChaFileData> chaFileCustomFaceData = new List<ChaFileData>();

    private List<ChaFileData> chaFileCustomBodyData = new List<ChaFileData>();

    private List<ChaFileData> chaFileCustomHairData = new List<ChaFileData>();

    private List<CharacterInfoData> characterInfoData = new List<CharacterInfoData>();

    private List<ChaFileCoordinateData> chaFileCoordinateData = new List<ChaFileCoordinateData>();

    private List<DynamicBoneData> dynamicBonesData = new List<DynamicBoneData>();

    private List<DynamicBoneColliderData> dynamicBoneCollidersData = new List<DynamicBoneColliderData>();

    private List<AccessoryStateData> accessoryStateData = new List<AccessoryStateData>();

    private List<ListInfoData> listInfoData = new List<ListInfoData>();

    private List<BoneOffsetData> boneOffsetData = new List<BoneOffsetData>();

    private static Dictionary<int, int> instanceIDs = new Dictionary<int, int>();

    private static HashSet<string> offsetBoneCandidates = new HashSet<string>();

    private int vertexCount;

    private readonly int scale = 1;

    private int[] vertics_num;

    private string[] vertics_name;

    private Dictionary<string, int> currentBoneKeysList = new Dictionary<string, int>();

    private List<BoneInfo> editBoneInfo = new List<BoneInfo>();

    private List<object> recoverInfos = new List<object>();

    private Dictionary<string, BoneInfo> finalBoneInfo = new Dictionary<string, BoneInfo>();

    private List<BoneInfo> finalBoneInfoCache = new List<BoneInfo>();

    private List<Renderer> meshRenders = new List<Renderer>();

    private List<string> lightDarkMaterials = new List<string>();

    private Dictionary<string, List<string>> smrMaterialsCache = new Dictionary<string, List<string>>();

    private Mesh square;

    public string BuildStart()
    {
        try
        {
            msg += "\n";
        }
        catch (Exception ex)
        {
            msg = msg + ex?.ToString() + "\n";
        }
        return msg;
    }

    public IEnumerator BuildStart_OG()
    {
        try
        {
            ResetPmxBuilder();
            CreateModelInfo();
            CreateInstanceIDs();
            SetSavePath();
            Directory.CreateDirectory(savePath);
            if (exportLightDarkTexture)
            {
                Directory.CreateDirectory(savePath + "/pre_light");
                Directory.CreateDirectory(savePath + "/pre_dark");
            }
            if (!exportWithEnabledShapekeys)
            {
                ClearMorphs();
            }
            SetSkinnedMeshList();
            PrepareModel();

            if (nowCoordinate < maxCoord)
            {
                CreateBoneList();
            }
            CreateMeshList();

            if (nowCoordinate == maxCoord)
            {
                CreateMorph();
                ExportGagEyes();
            }
            AddAccessory();
            if (exportLightDarkTexture)
            {
                ExportLightTexture();
            }
            ExportSpecialTextures();
            if (nowCoordinate < maxCoord)
            {
                GetCreateClothesMaterials();
                CreateClothesData();
                CreateAccessoryData();
                CreateChaFileCoordinateData();
                CreateReferenceInfoData();
                try
                {
                    CreateDynamicBonesData();
                    CreateDynamicBoneCollidersData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                CreateAccessoryStateData();
                CreateListInfoData();
                SaveBodyTextures();
                SaveHeadTextures();
            }
            if (nowCoordinate == maxCoord)
            {
                GetCreateBodyMaterials();
            }
            CreatePmxHeader();
            Save();
            msg += "\n";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            msg = msg + ex?.ToString() + "\n";
        }
        yield return null;
    }

    public void PrepareModel()
    {
        Transform transform = GameObject.Find("BodyTop").transform;
        Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>(includeInactive: true);

        finalBoneInfoCache.Clear();
        foreach (Transform component in componentsInChildren)
        {
            finalBoneInfoCache.Add(new BoneInfo(component.name, component));
        }
        foreach (Transform component in componentsInChildren)
        {
            component.SetLocalScale(1, 1, 1);
            component.hasChanged = true;
        }
        smrMaterialsCache.Clear();
    }

    public void CleanUp()
    {
        try
        {
            GameObject.Destroy(BoneInfo.converter);
            finalBoneInfo.Clear();
            editBoneInfo.Clear();
            lightDarkMaterials.Clear();

            if (exportLightDarkTexture)
            {
                GameObject light = Light.FindObjectsOfType<Light>()[0].gameObject;
                Camera camera = Camera.main;

                light.transform.rotation = (UnityEngine.Quaternion)recoverInfos[0];
                camera.orthographic = (bool)recoverInfos[1];
                camera.aspect = (float)recoverInfos[2];
                camera.orthographicSize = (float)recoverInfos[3];
                camera.transform.position = (UnityEngine.Vector3)recoverInfos[4];
                camera.transform.rotation = (UnityEngine.Quaternion)recoverInfos[5];
                camera.clearFlags = (CameraClearFlags)recoverInfos[6];
                camera.allowMSAA = (bool)recoverInfos[7];

                recoverInfos.Clear();
                Mesh.Destroy(square);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error when cleaning up, reason:" + ex.Message);
        }
        finally
        {
            MakerAPI.GetCharacterControl().ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)(nowCoordinate - 1));
        }
    }
    public void ExportLightTexture()
    {
        // This method get texture by transform meshes into a surface according to their UV, then use a camera to capture.As Graphic.Blit() can't work with some shaders, so we use this complex one.
        // However, it modify vertices' position, if the shader need it to render,we will get wrong data.
        // If that occur, we should walk through all the triangle faces, collect data and then combine them into a entire one.
        // However, it seems to be impossible to happen, because this means the color will change if we simply adjust character's height.

        GameObject.Find("BodyTop").transform.Translate(new UnityEngine.Vector3(0, 10, 0));

        List<GameObject> hiddenObjects = new List<GameObject>();
        foreach (var i in GameObject.FindObjectsOfType<GameObject>())
        {
            if (i.activeSelf && Math.Abs(i.transform.position.y) <= 3 && i.GetComponent<Renderer>() != null)
            {
                hiddenObjects.Add(i);
                i.SetActive(false);
            }
        }
        // Some wired things will happen if you move main camera far away from 0,0 and export.
        // For sure, I think nobody will do this except me.
        string[] ignoredSMRs = { "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02", "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "Highlight_o_body_a_rend", "Highlight_cf_O_face_rend", "o_Mask" };
        // Use the main camera and light directly instead of create new one to refuse some wierd render bugs
        GameObject light = Light.FindObjectsOfType<Light>()[0].gameObject;// The scene has only one light.But I'm failed to get the light by its name
        Camera camera = Camera.main;
        UnityEngine.Quaternion lightRotation = new UnityEngine.Quaternion(0, 1, 0, 0);
        UnityEngine.Quaternion darkRotation = new UnityEngine.Quaternion(0, 0, 0, -1);

        if (recoverInfos.Count == 0)
        {
            recoverInfos.Add(light.transform.rotation);
            recoverInfos.Add(camera.orthographic);
            recoverInfos.Add(camera.aspect);
            recoverInfos.Add(camera.orthographicSize);
            recoverInfos.Add(camera.transform.position);
            recoverInfos.Add(camera.transform.rotation);
            recoverInfos.Add(camera.clearFlags);
            recoverInfos.Add(camera.allowMSAA);

            camera.orthographic = true;
            camera.aspect = 1f;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.allowMSAA = false;

            square = new Mesh();
            square.vertices = new UnityEngine.Vector3[4]
            {
                new(1.001f, -0.001f, 0f),
                new(1.001f, 1.001f, 0f),
                new(-0.001f, 1.001f, 0f),
                new(-0.001f, -0.001f, 0f)
            };
            square.uv = new UnityEngine.Vector2[4]
            {
                new(-0.001f, -0.001f),
                new(-0.001f, 1.001f),
                new(1.001f, 1.001f),
                new(1.001f, -0.001f)
            };
            square.triangles = new int[6]
            {
                1, 3, 0,
                2, 3, 1
            };
            square.RecalculateBounds();
            square.RecalculateNormals();
            square.RecalculateTangents();
        }

        for (int i = 0; i < meshRenders.Count; i++)
        {
            var smr = meshRenders[i];
            string smrName = smr.name;

            bool flag = false;
            smrName = ((ignoreList.Contains(smrName, StringComparer.Ordinal) && smr.sharedMaterials.Count() > 0 && ignoreList.Contains(PmxBuilder.CleanUpMaterialName(smr.sharedMaterial.name), StringComparer.Ordinal)) ? smrName : (smrName + " " + PmxBuilder.GetAltInstanceID(smr)));
            for (int j = 0; j < ignoredSMRs.Length; j++)
            {
                if (smrName.Contains(ignoredSMRs[j]))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                continue;
            }
            Console.WriteLine("Exporting textures for " + smr.name);
            Mesh mesh = new Mesh();
            LightProbeUsage probeUsage;
            Transform probeAnchor;
            ShadowCastingMode probeCastingMode;
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            bool receiveShadow;
            bool isFaceBody = false;
            
            if (smr.GetType().Name == "SkinnedMeshRenderer")
            {
                var _tmp = (SkinnedMeshRenderer)smr;
                _tmp.BakeMesh(mesh);
                mesh.colors = _tmp.sharedMesh.colors;
                probeUsage = _tmp.lightProbeUsage;
                probeAnchor = _tmp.probeAnchor;
                probeCastingMode = _tmp.shadowCastingMode;
                receiveShadow = _tmp.receiveShadows;
                _tmp.GetPropertyBlock(materialPropertyBlock);

                isFaceBody = smr.name == "o_body_a" || smr.name == "cf_O_face";
            }
            else
            {
                MeshFilter meshFilter = smr.gameObject.GetComponent<MeshFilter>();
                var _tmp = (MeshRenderer)smr;
                if (meshFilter.mesh != null)
                {
                    mesh = UnityEngine.Object.Instantiate(meshFilter.mesh);
                }
                else
                {
                    mesh = UnityEngine.Object.Instantiate(meshFilter.sharedMesh);
                }
                probeUsage = _tmp.lightProbeUsage;
                probeAnchor = _tmp.probeAnchor;
                probeCastingMode = _tmp.shadowCastingMode;
                receiveShadow = _tmp.receiveShadows;
                _tmp.GetPropertyBlock(materialPropertyBlock);

            }
            int subMeshCount = mesh.subMeshCount;
            int layer = smr.gameObject.layer;// Do not forget to drawmesh in this layer

            //Processing mesh is unnecessary if we do not use it.But I am too lazy.And this pluin run fast enough.
            UnityEngine.Vector2[] uvs = mesh.uv;
            if (uvs.Length == 0)
            {
                Console.WriteLine("This mesh do not have uv.Ignored");
                continue;
            }
            int horizontalBlockCount = 1;
            int verticalBlockCount = 1;
            int xOffset;
            int yOffset;
            
            UnityEngine.Vector3[] verts = new UnityEngine.Vector3[uvs.Length];
            // Transform mesh into a surface according to its uv
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;
            for (int j = 0; j < uvs.Length; j++)
            {
                verts[j] = new UnityEngine.Vector3(-uvs[j].x, uvs[j].y, 0f);
                minX = Mathf.Min(minX, uvs[j].x);
                maxX = Mathf.Max(maxX, uvs[j].x);
                minY = Mathf.Min(minY, uvs[j].y);
                maxY = Mathf.Max(maxY, uvs[j].y);
            }

            xOffset = ((int)Math.Floor(minX));
            yOffset = ((int)Math.Floor(minY));
            horizontalBlockCount = (int)Math.Ceiling(maxX) - xOffset;
            verticalBlockCount = (int)Math.Ceiling(maxY) - yOffset;

            // Correct triangle if its normal direction is not positive z
            var triangles = mesh.triangles;
            for (int j = 0; j < triangles.Length; j += 3)
            {
                var v0 = verts[triangles[j]];
                var v1 = verts[triangles[j + 1]];
                var v2 = verts[triangles[j + 2]];

                if (UnityEngine.Vector3.Cross(v1 - v0, v2 - v0).z < 0)
                {
                    var tmp = triangles[j + 1];
                    triangles[j + 1] = triangles[j + 2];
                    triangles[j + 2] = tmp;
                }
            }

            uvIslandSolver(triangles, verts);

            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            var positionFront = new UnityEngine.Vector3(-xOffset - horizontalBlockCount / 2.0f, yOffset + verticalBlockCount / 2.0f, 10f);
            var positionLookAt = new UnityEngine.Vector3(positionFront.x, positionFront.y, 0f);
            

            for (int j = 0; j < Math.Min(smr.sharedMaterials.Length, subMeshCount); j++)
            {
                if (meshRenders[i].sharedMaterials[j] == null) continue;
                Material material = new Material(meshRenders[i].sharedMaterials[j]);

                string matName = smrMaterialsCache[GetGameObjectPath(smr.gameObject)][j];

                lightDarkMaterials.Add(matName);
                try
                {
                    int baseLength = isFaceBody ? 4096 : material.mainTexture != null ? Math.Max(material.mainTexture.width, material.mainTexture.height) : 2048;

                    material.SetShaderPassEnabled("OUTLINE", false);
                    material.SetShaderPassEnabled("SHADOWCASTER", false);

                    if (material.HasProperty("_AlphaMask"))
                    {
                        material.SetTexture("_AlphaMask", null);
                    }
                    //if (material.HasProperty("_alpha_a"))
                    //{
                    //    alphaMaskAStage.Add((int)material.GetFloat("_alpha_a"));
                    //}
                    //else
                    //{
                    //    alphaMaskAStage.Add(-1);
                    //}
                    //if (material.HasProperty("_alpha_b"))
                    //{
                    //    alphaMaskBStage.Add((int)material.GetFloat("_alpha_b"));
                    //}
                    //else
                    //{
                    //    alphaMaskBStage.Add(-1);
                    //}

                    if (material.HasProperty("_SpecularPower"))
                    {
                        material.SetFloat("_SpecularPower", 0f);
                    }
                    if (material.HasProperty("_SpecularPowerNail"))
                    {
                        material.SetFloat("_SpecularPowerNail", 0f);
                    }
                    if (material.HasProperty("_NormalMap"))
                    {
                        material.SetTexture("_NormalMap", null);
                    }
                    if (material.HasProperty("_NormalMap_ST"))
                    {
                        material.SetTexture("_NormalMap_ST", null);
                    }
                    if (material.HasProperty("_NormalMapDetail"))
                    {
                        material.SetTexture("_NormalMapDetail", null);
                    }
                    if (material.HasProperty("_NormalMapDetail_ST"))
                    {
                        material.SetTexture("_NormalMapDetail_ST", null);
                    }
                    if (material.HasProperty("_NormalMask"))
                    {
                        material.SetTexture("_NormalMask", null);
                    }
                    if (material.HasProperty("_NormalMask_ST"))
                    {
                        material.SetTexture("_NormalMask_ST", null);
                    }
                    // Render eye hightlight or not.
                    //if (material.HasProperty("_isHighLight"))
                    //{
                    //    material.SetFloat("_isHighLight", 0f);
                    //}
                    if (material.HasProperty("_nip_specular"))
                    {
                        material.SetFloat("_nip_specular", 0f);
                    }

                    Color32[] lightColor;
                    Color32[] darkColor;
                    Color32[] lightOverlay = new Color32[0];
                    Color32[] darkOverlay = new Color32[0];

                    bool hasOverTex = material.HasProperty("_overtex1") && material.GetTexture("_overtex1") != null || material.HasProperty("_overtex2") && material.GetTexture("_overtex2") != null || material.HasProperty("_overtex3") && material.GetTexture("_overtex3") != null;
                    bool hasSpeclarHeight = material.HasProperty("_SpeclarHeight");
                    if (hasOverTex || hasSpeclarHeight)
                    {
                        camera.orthographicSize = verticalBlockCount / 2.0f;
                        camera.aspect = (float)horizontalBlockCount / verticalBlockCount;
                        camera.transform.position = positionFront;
                        camera.transform.LookAt(positionLookAt);
                        int texturewidth = baseLength * horizontalBlockCount;
                        int textureheight = baseLength * verticalBlockCount;

                        lightOverlay = render(lightRotation, j, mesh, texturewidth, textureheight);
                        darkOverlay = render(darkRotation, j, mesh, texturewidth, textureheight);

                        lightOverlay = shrink(lightOverlay);
                        darkOverlay = shrink(darkOverlay);

                        if (material.HasProperty("_overtex1"))
                        {
                            material.SetTexture("_overtex1", null);
                        }
                        if (material.HasProperty("_overtex2"))
                        {
                            material.SetTexture("_overtex2", null);
                        }
                        if (material.HasProperty("_overtex3"))
                        {
                            material.SetTexture("_overtex3", null);
                        }
                    }
                    camera.orthographicSize = 0.5f;
                    camera.aspect = 1f;
                    camera.transform.position = new UnityEngine.Vector3(0.5f, 0.5f, 1f);
                    camera.transform.LookAt(new UnityEngine.Vector3(0.5f, 0.5f, 0f));

                    lightColor = render(lightRotation, 0, square, baseLength, baseLength);
                    darkColor = render(darkRotation, 0, square, baseLength, baseLength);

                    if (hasOverTex || hasSpeclarHeight)
                    {
                        blend(lightColor, lightOverlay);
                        blend(darkColor, darkOverlay);
                    }

                    saveTexture(lightColor, baseLength, baseLength, savePath + "/pre_light/" + matName + "_light.png");
                    saveTexture(darkColor, baseLength, baseLength, savePath + "/pre_dark/" + matName + "_dark.png");

                    Color32[] render(UnityEngine.Quaternion rotation, int subMeshIndex, Mesh mesh, int texturewidth, int textureheight)
                    {
                        light.transform.rotation = rotation;
                        GL.Flush();

                        RenderTexture renderTexture = new RenderTexture(texturewidth, textureheight, 24, RenderTextureFormat.ARGB32);
                        renderTexture.antiAliasing = 1;
                        renderTexture.filterMode = FilterMode.Point;
                        renderTexture.Create();
                        camera.targetTexture = renderTexture;
                        camera.backgroundColor = Color.clear;
                        Graphics.DrawMesh(mesh, Matrix4x4.identity, material, layer, camera, subMeshIndex, materialPropertyBlock, probeCastingMode, receiveShadow, probeAnchor, probeUsage != LightProbeUsage.Off);
                        camera.Render();

                        RenderTexture.active = renderTexture;
                        Texture2D _ = new Texture2D(texturewidth, textureheight, TextureFormat.ARGB32, false);
                        _.ReadPixels(new Rect(0, 0, texturewidth, textureheight), 0, 0);
                        _.Apply();
                        RenderTexture.active = null;
                        var data = _.GetPixels32();
                        Texture.Destroy(_);
                        camera.targetTexture = null;
                        renderTexture.Release();
                        return data;
                    }

                    void saveTexture(Color32[] output, int texturewidth, int textureheight, string savePath)
                    {
                        Texture2D result = new Texture2D(texturewidth, textureheight, TextureFormat.ARGB32, false);
                        if (System.IO.File.Exists(savePath))// If two skinnedmeshrenderers share a same material.
                        {
                            byte[] existingBytes = System.IO.File.ReadAllBytes(savePath);
                            Texture2D existingTexture = new Texture2D(texturewidth, textureheight, TextureFormat.ARGB32, false);
                            existingTexture.LoadImage(existingBytes);

                            Color32[] existingPixels = existingTexture.GetPixels32();

                            for (int i = 0; i < output.Length; i++)
                            {
                                if (output[i].a > 250)
                                {
                                    existingPixels[i] = output[i];
                                }
                            }

                            result.SetPixels32(existingPixels);
                        }
                        else
                        {
                            result.SetPixels32(output);
                        }
                        result.filterMode = FilterMode.Point;
                        result.wrapMode = TextureWrapMode.Clamp;
                        result.Apply();
                        byte[] bytes = result.EncodeToPNG();
                        System.IO.File.WriteAllBytes(savePath, bytes);
                    }

                    void blend(Color32[] color, Color32[] blend)
                    {
                        for(int i = 0; i < color.Length; i++)
                        {
                            if (blend[i].a == 255)
                            {
                                color[i] = blend[i];
                            }
                        }
                    }
                    
                    Color32[] shrink(Color32[] overlay)
                    {
                        if (horizontalBlockCount == 1 && verticalBlockCount == 1)
                        {
                            return overlay;
                        }

                        Color32[] result = new Color32[baseLength * baseLength];

                        int _horizonLength0 = horizontalBlockCount * baseLength;
                        int _horizonLength1 = _horizonLength0 - baseLength;

                        int _verticalLength = _horizonLength0 * baseLength;

                        int cur = 0;
                        int curresult = 0;
                        int pointer = 0;
                        bool flag;
                        for(int cx = 0; cx < baseLength; cx++)
                        {
                            for(int cy = 0; cy < baseLength; cy++)
                            {
                                pointer = cur;
                                flag = false;
                                for(int i = 0; i < verticalBlockCount; i++)
                                {
                                    for(int j = 0; j < horizontalBlockCount; j++)
                                    {
                                        if (overlay[cur].a > result[curresult].a)
                                        {
                                            result[curresult] = overlay[pointer];

                                            if (result[curresult].a == 255)
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }                                        
                                        pointer += baseLength;
                                    }
                                    if (flag) break;
                                    pointer += _verticalLength;
                                }
                                ++cur;
                                ++curresult;
                            }
                            cur += _horizonLength1;
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (material != null)
                    {
                        Material.Destroy(material);
                    }
                }
            }
            if (mesh != null)
            {
                Mesh.Destroy(mesh);
            }
        }
        GameObject.Find("BodyTop").transform.Translate(new UnityEngine.Vector3(0, -10, 0));

        foreach (var i in hiddenObjects)
        {
            i.SetActive(true);
        }

        void uvIslandSolver(int[] triangles, UnityEngine.Vector3[] vertices)
        {
            int[] parent = new int[vertices.Length];
            float[] minX = new float[vertices.Length];
            float[] minY = new float[vertices.Length];
            float[] maxX = new float[vertices.Length];
            float[] maxY = new float[vertices.Length];

            int findAndUpdate(int index)
            {
                Stack<int> stack = new Stack<int>();
                while (parent[index] != index)
                {
                    stack.Push(index);
                    index = parent[index];
                }
                while (stack.Count > 0)
                {
                    int cur = stack.Pop();
                    parent[cur] = index;
                }
                return index;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                parent[i] = i;
                var vertex = vertices[i];
                minX[i] = vertex.x;
                maxX[i] = vertex.x;
                minY[i] = vertex.y;
                maxY[i] = vertex.y;
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var parentIndex = findAndUpdate(parent[triangles[i]]);
                var _parentIndex1 = findAndUpdate(parent[triangles[i + 1]]);
                var _parentIndex2 = findAndUpdate(parent[triangles[i + 2]]);

                parent[_parentIndex1] = parentIndex;
                parent[_parentIndex2] = parentIndex;

                minX[parentIndex] = Math.Min(minX[parentIndex], minX[_parentIndex1]);
                minX[parentIndex] = Math.Min(minX[parentIndex], minX[_parentIndex2]);

                minY[parentIndex] = Math.Min(minY[parentIndex], minY[_parentIndex1]);
                minY[parentIndex] = Math.Min(minY[parentIndex], minY[_parentIndex2]);

                maxX[parentIndex] = Math.Max(maxX[parentIndex], maxX[_parentIndex1]);
                maxX[parentIndex] = Math.Max(maxX[parentIndex], maxX[_parentIndex2]);

                maxY[parentIndex] = Math.Max(maxY[parentIndex], maxY[_parentIndex1]);
                maxY[parentIndex] = Math.Max(maxY[parentIndex], maxY[_parentIndex2]);
            }
            List<List<int>> islands = new List<List<int>>();
            Dictionary<int, float> offsets = new Dictionary<int, float>();

            for (int i = 0; i < vertices.Length; i++)
            {
                int parentIndex = findAndUpdate(parent[i]);
                if (offsets.TryGetValue(parentIndex, out float offset))
                {
                    var _v = vertices[i];
                    vertices[i] = new UnityEngine.Vector3(_v.x, _v.y, offset);
                    continue;
                }
                offset = 0f;
                bool flag = false;

                for (int layer = 0; layer < islands.Count; layer++)
                {
                    flag = true;
                    var layerInfo = islands[layer];
                    for (int j = 0; j < islands[layer].Count; j++)
                    {
                        int _parentIndex = layerInfo[j];
                        if (!(minX[_parentIndex] >= maxX[parentIndex] || maxX[_parentIndex] <= minX[parentIndex] || minY[_parentIndex] >= maxY[parentIndex] || maxY[_parentIndex] <= minY[parentIndex]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        offsets.Add(parentIndex, offset);
                        layerInfo.Add(parentIndex);
                        var _v = vertices[i];
                        vertices[i] = new UnityEngine.Vector3(_v.x, _v.y, offset);
                        break;
                    }
                    offset -= 0.0001f;
                }
                if (!flag)
                {
                    offsets.Add(parentIndex, offset);
                    islands.Add(new List<int>() { parentIndex });
                }
            }
        }
    }

    public void ChangeAnimations()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        CustomBase makerBase = MakerAPI.GetMakerBase();
        CvsDrawCtrl cvsDrawCtrl = UnityEngine.Object.FindObjectOfType<CvsDrawCtrl>();
        characterControl.ChangeEyesBlinkFlag(blink: false);
        characterControl.ChangeLookEyesTarget(1, null, 0f);
        characterControl.ChangeEyesPtn(21);
        characterControl.ChangeEyesPtn(23);
        characterControl.ChangeEyesPtn(27);
        characterControl.ChangeEyesPtn(0);
        if (exportHitBoxes)
        {
            characterControl.LoadHitObject();
        }
        else
        {
            characterControl.ReleaseHitObject();
        }
        if (!exportCurrentPose)
        {
            characterControl.ChangeEyesPtn(21);
            characterControl.ChangeEyesPtn(23);
            characterControl.ChangeEyesPtn(32);
            characterControl.ChangeEyesPtn(0);
#if NET35
            int index = makerBase.lstPose.FindIndex((Predicate<ExcelData.Param>)(list => list.list[4] == "tpose"));
            cvsDrawCtrl.ChangeAnimationForce(index, 0.0f);
#elif NET46
				cvsDrawCtrl.ChangeAnimationForce(makerBase.lstPose.Length - 1, 0f);
#endif
        }
        else
        {
            characterControl.animBody.speed = 0f;
        }
    }

    public void CreateBaseSavePath()
    {
        //Use the card's name in the save path, but remove any invalid characters to prevent save issues
        string characterName = Singleton<CustomBase>.Instance.chaCtrl.chaFile.parameter.fullname.Replace(" ", string.Empty);
        characterName = string.Join("_", characterName.Split(Path.GetInvalidFileNameChars()));
        string exportFolder = "Export_PMX";
        baseSavePath = Path.Combine(Application.dataPath, $"../{exportFolder}/{DateTime.Now:yyyyMMddHHmmss}_{characterName}/");
        Directory.CreateDirectory(baseSavePath);
    }

    private void ResetPmxBuilder()
    {
        currentMaterialList.Clear();
        currentRendererMaterialMapping.Clear();
        List<PmxBone> boneList = new List<PmxBone>();
        Dictionary<string, Pmx.BackupBoneData> boneBackupData = new Dictionary<string, Pmx.BackupBoneData>();
        if (pmxFile != null)
        {
            boneList = pmxFile.BoneList;
            boneBackupData = pmxFile.BoneBackupData;
        }
        pmxFile = new Pmx
        {
            BoneList = boneList,
            BoneBackupData = boneBackupData
        };
        vertexCount = 0;
        vertics_num = new int[0];
        vertics_name = new string[0];
    }

    private void SetSavePath()
    {
        if (nowCoordinate == maxCoord)
        {
            savePath = baseSavePath;
        }
        else
        {
            savePath = baseSavePath + "Outfit " + nowCoordinate.ToString("00") + "/";
        }
    }

    public void CreateModelInfo()
    {
        PmxModelInfo pmxModelInfo = new PmxModelInfo
        {
            ModelName = "koikatu",
            ModelNameE = "",
            Comment = "exported koikatu"
        };
        pmxModelInfo.Comment = "";
        pmxFile.ModelInfo = pmxModelInfo;
    }

    public void CreateInstanceIDs()
    {
        instanceIDs.Clear();
        Transform[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<Transform>(includeInactive: true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (!(componentsInChildren[i].gameObject == null))
            {
                instanceIDs.Add(componentsInChildren[i].gameObject.GetInstanceID(), instanceIDs.Count);
            }
        }
        Component[] componentsInChildren2 = GameObject.Find("BodyTop").transform.GetComponentsInChildren<Component>(includeInactive: true);
        for (int j = 0; j < componentsInChildren2.Length; j++)
        {
            if (!(componentsInChildren2[j] == null))
            {
                instanceIDs.Add(componentsInChildren2[j].GetInstanceID(), instanceIDs.Count);
            }
        }
    }

    public void CollectIgnoreList()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        Renderer[] componentsInChildren = characterControl.objHead.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer in componentsInChildren)
        {
            if (!ignoreList.Contains(renderer.name, StringComparer.Ordinal))
            {
                ignoreList.Add(renderer.name);
            }
            Material[] materials = renderer.materials;
            for (int j = 0; j < materials.Length; j++)
            {
                string text = CleanUpMaterialName(materials[j].name);
                if (!ignoreList.Contains(text, StringComparer.Ordinal))
                {
                    ignoreList.Add(text);
                }
            }
        }
        if (characterControl.rendEye[0] != null && characterControl.rendEye[0].material != null)
        {
            EyeMatName = CleanUpMaterialName(characterControl.rendEye[0].material.name);
        }
    }

    private void SetSkinnedMeshList()
    {
        int num = GameObject.Find("BodyTop").transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true).Length;
        vertics_num = new int[num];
        vertics_name = new string[num];
        msg += "\n";
    }

    public void CreateMeshList()
    {
        meshRenders.Clear();
        string[] source = new string[0];
        string[] source2 = new string[7] { "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02", "o_tang" };
        string[] source3 = new string[8] { "o_mnpa", "o_mnpb", "n_tang", "n_tang_silhouette", "o_dankon", "o_gomu", "o_dan_f", "cf_O_canine" };
        string[] source4 = new string[23]
        {
            "o_hit_armL", "o_hit_armR", "o_hit_footL", "o_hit_footR", "o_hit_handL", "o_hit_handR", "o_hit_hara", "o_hit_haraB", "o_hit_haraUnder", "o_hit_kneeBL",
            "o_hit_kneeBR", "o_hit_kneeL", "o_hit_kneeR", "o_hit_kokan", "o_hit_legBL", "o_hit_legBR", "o_hit_legL", "o_hit_legR", "o_hit_mune", "o_hit_muneB",
            "o_hit_siriL", "o_hit_siriR", "cf_O_face_atari"
        };
        SkinnedMeshRenderer[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (((!componentsInChildren[i].enabled || !componentsInChildren[i].isVisible || source.Contains(componentsInChildren[i].name, StringComparer.Ordinal)) && (!componentsInChildren[i].enabled || !source2.Contains(componentsInChildren[i].name, StringComparer.Ordinal)) && (!exportAll || !componentsInChildren[i].enabled || source3.Contains(componentsInChildren[i].name, StringComparer.Ordinal)) && (!exportHitBoxes || !componentsInChildren[i].enabled || !source4.Contains(componentsInChildren[i].name, StringComparer.Ordinal) || nowCoordinate != maxCoord)) || (nowCoordinate < maxCoord && ignoreList.Contains(componentsInChildren[i].name, StringComparer.Ordinal) && componentsInChildren[i].sharedMaterials.Count() > 0 && ignoreList.Contains(CleanUpMaterialName(componentsInChildren[i].sharedMaterial.name), StringComparer.Ordinal)) || (nowCoordinate == maxCoord && (!ignoreList.Contains(componentsInChildren[i].name, StringComparer.Ordinal) || (ignoreList.Contains(componentsInChildren[i].name, StringComparer.Ordinal) && componentsInChildren[i].sharedMaterials.Count() > 0 && !ignoreList.Contains(CleanUpMaterialName(componentsInChildren[i].sharedMaterial.name), StringComparer.Ordinal)))))
            {
                continue;
            }
            meshRenders.Add(componentsInChildren[i]);
            Console.WriteLine("Exporting: " + componentsInChildren[i].name);
            if (componentsInChildren[i].sharedMaterials.Count() == 0)
            {
                Material material = new Material(Shader.Find("Diffuse"));
                material.name = componentsInChildren[i].name + "_M";
                componentsInChildren[i].material = material;
            }
            SMRData sMRData = new SMRData(this, componentsInChildren[i]);
            //smrMaterialsCache.TryAdd(sMRData.SMRPath, sMRData.SMRMaterialNames);
            smrMaterialsCache[sMRData.SMRPath] = sMRData.SMRMaterialNames;
            AddToSMRDataList(sMRData);
            MaterialDataComplete matData = new MaterialDataComplete(this, componentsInChildren[i]);
            AddToMaterialDataCompleteList(matData);
            if (currentRendererMaterialMapping.ContainsKey(componentsInChildren[i]))
            {
                Console.WriteLine("Issue - Renderer already added to Material name cache: " + sMRData.SMRName);
            }
            else
            {
                currentRendererMaterialMapping.Add(componentsInChildren[i], sMRData.SMRMaterialNames);
            }
            vertics_num[i] = componentsInChildren[i].sharedMesh.vertices.Length;
            vertics_name[i] = componentsInChildren[i].sharedMaterial.name;
            _ = componentsInChildren[i].gameObject;
            BoneWeight[] boneWeights = componentsInChildren[i].sharedMesh.boneWeights;
            Transform transform = componentsInChildren[i].gameObject.transform;
            int bone = sbi(GetAltBoneName(transform), transform.GetInstanceID().ToString());
            Mesh mesh = new Mesh();
            componentsInChildren[i].BakeMesh(mesh);
            Mesh mesh2 = mesh;
            UnityEngine.Vector2[] uv = mesh2.uv;
            List<UnityEngine.Vector2[]> list = new List<UnityEngine.Vector2[]> { mesh2.uv2, mesh2.uv3, mesh2.uv4 };
            _ = mesh2.colors;
            UnityEngine.Vector3[] normals = mesh2.normals;
            UnityEngine.Vector3[] vertices = mesh2.vertices;
            for (int j = 0; j < mesh2.subMeshCount; j++)
            {
                int[] triangles = mesh2.GetTriangles(j);
                AddFaceList(triangles, vertexCount);
                if (j < componentsInChildren[i].sharedMaterials.Count())
                {
                    CreateMaterial(componentsInChildren[i].sharedMaterials[j], sMRData.SMRMaterialNames[j], triangles.Length);
                }
                else if (componentsInChildren[i].sharedMaterial != null)
                {
                    CreateMaterial(componentsInChildren[i].sharedMaterial, sMRData.SMRMaterialNames[0], triangles.Length);
                }
            }
            if (string.CompareOrdinal(componentsInChildren[i].name, "cf_O_eyeline") == 0)
            {
                int[] triangles2 = mesh2.GetTriangles(0);
                AddFaceList(triangles2, vertexCount);
                for (int k = 1; k < 2; k++)
                {
                    CreateMaterial(componentsInChildren[i].sharedMaterials[k], sMRData.SMRMaterialNames[k], triangles2.Length);
                }
            }
            vertexCount += mesh2.vertexCount;
            for (int l = 0; l < mesh2.vertexCount; l++)
            {
                PmxVertex pmxVertex = new PmxVertex
                {
                    UV = new PmxLib.Vector2(uv[l].x, (float)((double)(0f - uv[l].y) + 1.0))
                };
                for (int m = 0; m < list.Count; m++)
                {
                    if (list[m].Length != 0)
                    {
                        pmxVertex.UVA[m] = new PmxLib.Vector4(list[m][l].x, (float)((double)(0f - list[m][l].y) + 1.0), 0f, 0f);
                    }
                }
                if (boneWeights.Length != 0)
                {
                    pmxVertex.Weight = ConvertBoneWeight(boneWeights[l], componentsInChildren[i].bones);
                }
                else
                {
                    pmxVertex.Weight = new PmxVertex.BoneWeight[4];
                    pmxVertex.Weight[0].Bone = bone;
                    pmxVertex.Weight[0].Value = 1f;
                }
                UnityEngine.Vector3 vector = componentsInChildren[i].transform.TransformDirection(normals[l]);
                pmxVertex.Normal = new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z);
                //UnityEngine.Vector3 vector2 = componentsInChildren[i].transform.TransformPointUnscaled(vertices[l]);
                UnityEngine.Vector3 vector2 = componentsInChildren[i].transform.TransformPoint(vertices[l]);
                pmxVertex.Position = new PmxLib.Vector3((0f - vector2.x) * (float)scale, vector2.y * (float)scale, (0f - vector2.z) * (float)scale);
                pmxVertex.Deform = PmxVertex.DeformType.BDEF4;
                pmxFile.VertexList.Add(pmxVertex);
            }
            Mesh.Destroy(mesh2);
        }
    }

    private PmxVertex.BoneWeight[] ConvertBoneWeight(BoneWeight unityWeight, Transform[] bones)
    {
        PmxVertex.BoneWeight[] array = new PmxVertex.BoneWeight[4];
        try
        {
            if (unityWeight.boneIndex0 >= 0 && unityWeight.boneIndex0 < bones.Length)
            {
                array[0].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex0]), bones[unityWeight.boneIndex0].GetInstanceID().ToString());
            }
            array[0].Value = unityWeight.weight0;
            if (unityWeight.boneIndex1 >= 0 && unityWeight.boneIndex0 < bones.Length)
            {
                array[1].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex1]), bones[unityWeight.boneIndex1].GetInstanceID().ToString());
            }
            array[1].Value = unityWeight.weight1;
            if (unityWeight.boneIndex2 >= 0 && unityWeight.boneIndex0 < bones.Length)
            {
                array[2].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex2]), bones[unityWeight.boneIndex2].GetInstanceID().ToString());
            }
            array[2].Value = unityWeight.weight2;
            if (unityWeight.boneIndex3 >= 0 && unityWeight.boneIndex0 < bones.Length)
            {
                array[3].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex3]), bones[unityWeight.boneIndex3].GetInstanceID().ToString());
            }
            array[3].Value = unityWeight.weight3;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return array;
    }

    private void ClearMorphs()
    {
        ChaControl instance = Singleton<ChaControl>.Instance;
        FBSTargetInfo[] fBSTarget = instance.eyesCtrl.FBSTarget;
        for (int i = 0; i < fBSTarget.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = fBSTarget[i].GetSkinnedMeshRenderer();
            int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
            for (int j = 0; j < blendShapeCount; j++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(j, 0f);
            }
        }
        fBSTarget = instance.mouthCtrl.FBSTarget;
        for (int i = 0; i < fBSTarget.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer2 = fBSTarget[i].GetSkinnedMeshRenderer();
            int blendShapeCount2 = skinnedMeshRenderer2.sharedMesh.blendShapeCount;
            for (int k = 0; k < blendShapeCount2; k++)
            {
                skinnedMeshRenderer2.SetBlendShapeWeight(k, 0f);
            }
        }
        fBSTarget = instance.eyebrowCtrl.FBSTarget;
        for (int i = 0; i < fBSTarget.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer3 = fBSTarget[i].GetSkinnedMeshRenderer();
            int blendShapeCount3 = skinnedMeshRenderer3.sharedMesh.blendShapeCount;
            for (int l = 0; l < blendShapeCount3; l++)
            {
                skinnedMeshRenderer3.SetBlendShapeWeight(l, 0f);
            }
        }
    }

    private void CreateMorph()
    {
        ChaControl instance = Singleton<ChaControl>.Instance;
        FBSTargetInfo[] fBSTarget = instance.eyesCtrl.FBSTarget;
        for (int i = 0; i < fBSTarget.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = fBSTarget[i].GetSkinnedMeshRenderer();
            string name = skinnedMeshRenderer.sharedMaterial.name;
            int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
            UnityEngine.Vector3[] array = new UnityEngine.Vector3[skinnedMeshRenderer.sharedMesh.vertices.Length];
            UnityEngine.Vector3[] deltaNormals = new UnityEngine.Vector3[skinnedMeshRenderer.sharedMesh.normals.Length];
            UnityEngine.Vector3[] deltaTangents = new UnityEngine.Vector3[skinnedMeshRenderer.sharedMesh.tangents.Length];
            int num = 0;
            for (int j = 0; j < vertics_num.Length && (vertics_num[j] != array.Length || vertics_name[j] != name); j++)
            {
                num += vertics_num[j];
            }
            if (num >= pmxFile.VertexList.Count)
            {
                continue;
            }
            for (int k = 0; k < blendShapeCount; k++)
            {
                skinnedMeshRenderer.sharedMesh.GetBlendShapeFrameVertices(k, 0, array, deltaNormals, deltaTangents);
                PmxMorph pmxMorph = new PmxMorph
                {
                    Name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(k),
                    NameE = "",
                    Panel = 1,
                    Kind = PmxMorph.OffsetKind.Vertex
                };
                for (int l = 0; l < array.Length; l++)
                {
                    PmxVertexMorph pmxVertexMorph = new PmxVertexMorph(num + l, new PmxLib.Vector3(0f - array[l].x, array[l].y, 0f - array[l].z));
                    pmxVertexMorph.Offset *= (float)scale;
                    pmxMorph.OffsetList.Add(pmxVertexMorph);
                }
                bool flag = true;
                for (int m = 0; m < pmxFile.MorphList.Count; m++)
                {
                    if (pmxFile.MorphList[m].Name.Equals(pmxMorph.Name))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    pmxFile.MorphList.Add(pmxMorph);
                }
            }
        }
        fBSTarget = instance.mouthCtrl.FBSTarget;
        for (int i = 0; i < fBSTarget.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer2 = fBSTarget[i].GetSkinnedMeshRenderer();
            string name2 = skinnedMeshRenderer2.sharedMaterial.name;
            int blendShapeCount2 = skinnedMeshRenderer2.sharedMesh.blendShapeCount;
            UnityEngine.Vector3[] array2 = new UnityEngine.Vector3[skinnedMeshRenderer2.sharedMesh.vertices.Length];
            UnityEngine.Vector3[] deltaNormals2 = new UnityEngine.Vector3[skinnedMeshRenderer2.sharedMesh.normals.Length];
            UnityEngine.Vector3[] deltaTangents2 = new UnityEngine.Vector3[skinnedMeshRenderer2.sharedMesh.tangents.Length];
            int num2 = 0;
            for (int n = 0; n < vertics_num.Length && (vertics_num[n] != array2.Length || vertics_name[n] != name2); n++)
            {
                num2 += vertics_num[n];
            }
            if (num2 >= pmxFile.VertexList.Count)
            {
                continue;
            }
            for (int num3 = 0; num3 < blendShapeCount2; num3++)
            {
                skinnedMeshRenderer2.sharedMesh.GetBlendShapeFrameVertices(num3, 0, array2, deltaNormals2, deltaTangents2);
                PmxMorph pmxMorph2 = new PmxMorph
                {
                    Name = skinnedMeshRenderer2.sharedMesh.GetBlendShapeName(num3),
                    NameE = "",
                    Panel = 1,
                    Kind = PmxMorph.OffsetKind.Vertex
                };
                for (int num4 = 0; num4 < array2.Length; num4++)
                {
                    PmxVertexMorph pmxVertexMorph2 = new PmxVertexMorph(num2 + num4, new PmxLib.Vector3(0f - array2[num4].x, array2[num4].y, 0f - array2[num4].z));
                    pmxVertexMorph2.Offset *= (float)scale;
                    pmxMorph2.OffsetList.Add(pmxVertexMorph2);
                }
                bool flag2 = true;
                for (int num5 = 0; num5 < pmxFile.MorphList.Count; num5++)
                {
                    if (pmxFile.MorphList[num5].Name.Equals(pmxMorph2.Name))
                    {
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    pmxFile.MorphList.Add(pmxMorph2);
                }
            }
        }
        fBSTarget = instance.eyebrowCtrl.FBSTarget;
        for (int i = 0; i < fBSTarget.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer3 = fBSTarget[i].GetSkinnedMeshRenderer();
            string name3 = skinnedMeshRenderer3.sharedMaterial.name;
            int blendShapeCount3 = skinnedMeshRenderer3.sharedMesh.blendShapeCount;
            UnityEngine.Vector3[] array3 = new UnityEngine.Vector3[skinnedMeshRenderer3.sharedMesh.vertices.Length];
            UnityEngine.Vector3[] deltaNormals3 = new UnityEngine.Vector3[skinnedMeshRenderer3.sharedMesh.normals.Length];
            UnityEngine.Vector3[] deltaTangents3 = new UnityEngine.Vector3[skinnedMeshRenderer3.sharedMesh.tangents.Length];
            int num6 = 0;
            for (int num7 = 0; num7 < vertics_num.Length && (vertics_num[num7] != array3.Length || vertics_name[num7] != name3); num7++)
            {
                num6 += vertics_num[num7];
            }
            if (num6 >= pmxFile.VertexList.Count)
            {
                continue;
            }
            for (int num8 = 0; num8 < blendShapeCount3; num8++)
            {
                skinnedMeshRenderer3.sharedMesh.GetBlendShapeFrameVertices(num8, 0, array3, deltaNormals3, deltaTangents3);
                PmxMorph pmxMorph3 = new PmxMorph
                {
                    Name = skinnedMeshRenderer3.sharedMesh.GetBlendShapeName(num8),
                    NameE = "",
                    Panel = 1,
                    Kind = PmxMorph.OffsetKind.Vertex
                };
                for (int num9 = 0; num9 < array3.Length; num9++)
                {
                    PmxVertexMorph pmxVertexMorph3 = new PmxVertexMorph(num6 + num9, new PmxLib.Vector3(0f - array3[num9].x, array3[num9].y, 0f - array3[num9].z));
                    pmxVertexMorph3.Offset *= (float)scale;
                    pmxMorph3.OffsetList.Add(pmxVertexMorph3);
                }
                bool flag3 = true;
                for (int num10 = 0; num10 < pmxFile.MorphList.Count; num10++)
                {
                    if (pmxFile.MorphList[num10].Name.Equals(pmxMorph3.Name))
                    {
                        flag3 = false;
                    }
                }
                if (flag3)
                {
                    pmxFile.MorphList.Add(pmxMorph3);
                }
            }
        }
    }

    public void CreateMaterial(Material material, string matName, int count)
    {
        PmxMaterial pmxMaterial = new PmxMaterial
        {
            Name = matName,
            NameE = matName,
            Flags = (PmxMaterial.MaterialFlags.DrawBoth | PmxMaterial.MaterialFlags.Shadow | PmxMaterial.MaterialFlags.SelfShadowMap | PmxMaterial.MaterialFlags.SelfShadow)
        };
        if (material.mainTexture != null)
        {
            string text = matName + "_MT_CT.png";
            WriteToTexture2D(material, "_MT_CT", savePath + text, material.mainTexture);
        }
        SaveTexture(material, matName, "_MT");
        SaveTexture(material, matName, "_AM");
        SaveTexture(material, matName, "_CM");
        SaveTexture(material, matName, "_DM");
        SaveTexture(material, matName, "_LM");
        SaveTexture(material, matName, "_NM");
        SaveTexture(material, matName, "_NMP");
        SaveTexture(material, matName, "_NMPD");
        SaveTexture(material, matName, "_ot1");
        SaveTexture(material, matName, "_ot2");
        SaveTexture(material, matName, "_ot3");
        SaveTexture(material, matName, "_lqdm");
        SaveTexture(material, matName, "_HGLS");
        SaveTexture(material, matName, "_T2");
        SaveTexture(material, matName, "_T3");
        SaveTexture(material, matName, "_AR");
        SaveTexture(material, matName, "_GLSR");
        MaterialData matData = new MaterialData(material, matName);
        AddToMaterialDataList(matData);
        if (material.HasProperty("_Color"))
        {
            pmxMaterial.Diffuse = material.GetColor("_Color");
        }
        if (material.HasProperty("_AmbColor"))
        {
            pmxMaterial.Ambient = material.GetColor("_AmbColor");
        }
        if (material.HasProperty("_Opacity"))
        {
            pmxMaterial.Diffuse.a = material.GetFloat("_Opacity");
        }
        if (material.HasProperty("_SpecularColor"))
        {
            pmxMaterial.Specular = material.GetColor("_SpecularColor");
        }
        if (!material.HasProperty("_Shininess") && material.HasProperty("_OutlineColor"))
        {
            pmxMaterial.EdgeSize = material.GetFloat("_OutlineWidth");
            pmxMaterial.EdgeColor = material.GetColor("_OutlineColor");
        }
        pmxMaterial.FaceCount = count;
        pmxFile.MaterialList.Add(pmxMaterial);
    }

    public void SaveTexture(Material material, string matName, string type)
    {
        string text = typeMap[type];
        if (material.HasProperty(text) && material.GetTexture(text) != null)
        {
            string text2 = matName + type + ((type == "_MT") ? "_CT" : "") + ".png";
            Texture texture = material.GetTexture(text);
            WriteToTexture2D(material, text, savePath + text2, texture);
        }
    }

    public void WriteToTexture2D(Material mat, string prop, string path, Texture tex)
    {
        TextureData texData = new TextureData(mat, prop, path.Split('/').Last());
        AddToTextureDataList(texData);
        TextureWriter.SaveTex(tex, path);
        if (TextureWriter.ConvertNormalMap(ref tex, prop))
        {
            TextureWriter.SaveTex(tex, path.Substring(0, path.Length - 4) + "_CNV.png");
        }
    }

    private void AddFaceList(int[] faceList, int count)
    {
        for (int i = 0; i < faceList.Length; i++)
        {
            faceList[i] += count;
            pmxFile.FaceList.Add(faceList[i]);
        }
    }

    private void AddAccessory()
    {
        MeshFilter[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<MeshFilter>(includeInactive: true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            MeshRenderer meshRenderer = componentsInChildren[i].gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            if (!meshRenderer.enabled || (nowCoordinate < maxCoord && ignoreList.Contains(meshRenderer.name, StringComparer.Ordinal) && meshRenderer.sharedMaterials.Count() > 0 && ignoreList.Contains(CleanUpMaterialName(meshRenderer.sharedMaterial.name), StringComparer.Ordinal)) || (nowCoordinate == maxCoord && !ignoreList.Contains(meshRenderer.name, StringComparer.Ordinal) && meshRenderer.sharedMaterials.Count() > 0 && !ignoreList.Contains(CleanUpMaterialName(meshRenderer.sharedMaterial.name), StringComparer.Ordinal)))
            {
                continue;
            }
            meshRenders.Add(meshRenderer);
            Console.WriteLine("Exporting Acc: " + meshRenderer.name);
            SMRData sMRData = new SMRData(this, meshRenderer);
            //smrMaterialsCache.Add(sMRData.SMRPath, sMRData.SMRMaterialNames);
            smrMaterialsCache[sMRData.SMRPath] = sMRData.SMRMaterialNames;
            AddToSMRDataList(sMRData);
            MaterialDataComplete matData = new MaterialDataComplete(this, meshRenderer);
            AddToMaterialDataCompleteList(matData);
            if (currentRendererMaterialMapping.ContainsKey(meshRenderer))
            {
                Console.WriteLine("Issue - Renderer already added to Material name cache: " + sMRData.SMRName);
            }
            else
            {
                currentRendererMaterialMapping.Add(meshRenderer, sMRData.SMRMaterialNames);
            }
            GameObject gameObject = componentsInChildren[i].gameObject;
            Mesh sharedMesh = componentsInChildren[i].sharedMesh;
            _ = sharedMesh.boneWeights;
            Transform transform = componentsInChildren[i].gameObject.transform;
            int bone = sbi(GetAltBoneName(transform), transform.GetInstanceID().ToString());
            UnityEngine.Vector2[] uv = sharedMesh.uv;
            List<UnityEngine.Vector2[]> list = new List<UnityEngine.Vector2[]> { sharedMesh.uv2, sharedMesh.uv3, sharedMesh.uv4 };
            UnityEngine.Vector3[] normals = sharedMesh.normals;
            UnityEngine.Vector3[] vertices = sharedMesh.vertices;
            for (int j = 0; j < sharedMesh.subMeshCount; j++)
            {
                int[] triangles = sharedMesh.GetTriangles(j);
                AddFaceList(triangles, vertexCount);
                CreateMaterial(meshRenderer.sharedMaterials[j], sMRData.SMRMaterialNames[j], triangles.Length);
            }
            vertexCount += sharedMesh.vertexCount;
            bool uv_error_flag = false;
            for (int k = 0; k < sharedMesh.vertexCount; k++)
            {
                PmxVertex pmxVertex = new PmxVertex();
                try
                {
                    pmxVertex.UV = new PmxLib.Vector2(uv[k].x, (float)((double)(0f - uv[k].y) + 1.0));
                    pmxVertex.Weight = new PmxVertex.BoneWeight[4];
                }
                catch
                {
                    if (uv_error_flag == false)
                    {
                        Console.WriteLine("Issue - Object did not have a UV map: " + meshRenderer.name);
                    }
                    uv_error_flag = true;
                    pmxVertex.UV = new PmxLib.Vector2();
                    pmxVertex.Weight = new PmxVertex.BoneWeight[4];
                }
                pmxVertex.Weight[0].Bone = bone;
                pmxVertex.Weight[0].Value = 1f;
                for (int l = 0; l < list.Count; l++)
                {
                    if (list[l].Length != 0)
                    {
                        pmxVertex.UVA[l] = new PmxLib.Vector4(list[l][k].x, (float)((double)(0f - list[l][k].y) + 1.0), 0f, 0f);
                    }
                }
                UnityEngine.Vector3 vector = gameObject.transform.TransformDirection(normals[k]);
                pmxVertex.Normal = new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z);
                UnityEngine.Vector3 vector2 = gameObject.transform.TransformPoint(vertices[k]);
                pmxVertex.Position = new PmxLib.Vector3((0f - vector2.x) * (float)scale, vector2.y * (float)scale, (0f - vector2.z) * (float)scale);
                pmxVertex.Deform = PmxVertex.DeformType.BDEF4;
                pmxFile.VertexList.Add(pmxVertex);
            }
        }
    }

    public void CreateBoneList()
    {
        offsetBoneCandidates.Clear();
        currentBonesList.Clear();
        Transform transform = GameObject.Find("BodyTop").transform;
        Dictionary<Transform, int> dictionary = new Dictionary<Transform, int>();
        Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(GetAltBoneName(componentsInChildren[i]));
            stringBuilder.Append(" ");
            stringBuilder.Append(componentsInChildren[i].GetInstanceID().ToString());
            string text = stringBuilder.ToString();
            if (pmxFile.BoneBackupData.TryGetValue(text, out var value))
            {
                if (!whitelistOffsetBones.Contains(GetAltBoneName(componentsInChildren[i], ignoreID: true), StringComparer.Ordinal))
                {
                    continue;
                }
                PmxBone pmxBone = value.PmxBone;
                if (pmxBone != null)
                {
                    UnityEngine.Vector3 vector = componentsInChildren[i].transform.position * scale;
                    BoneOffsetData bnOffsetData = new BoneOffsetData(offset: new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z) - pmxBone.Position, boneName: GetAltBoneName(componentsInChildren[i]));
                    AddToBoneOffsetDataList(bnOffsetData);
                    offsetBoneCandidates.Add(text);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.Append(GetAltBoneName(componentsInChildren[i]));
                    stringBuilder2.Append(" ");
                    stringBuilder2.Append(componentsInChildren[i].GetInstanceID().ToString());
                    text = stringBuilder2.ToString();
                }
            }
            int value2;
            if (pmxFile.BoneList.Count > 0)
            {
                StringBuilder stringBuilder3 = new StringBuilder();
                stringBuilder3.Append(GetAltBoneName(componentsInChildren[i].parent));
                stringBuilder3.Append(" ");
                stringBuilder3.Append(componentsInChildren[i].parent.GetInstanceID().ToString());
                string key = stringBuilder3.ToString();
                currentBoneKeysList.TryGetValue(key, out value2);
            }
            else
            {
                dictionary.TryGetValue(componentsInChildren[i].parent, out value2);
            }
            PmxBone pmxBone2 = new PmxBone
            {
                Name = GetAltBoneName(componentsInChildren[i]),
                NameE = componentsInChildren[i].GetInstanceID().ToString(),
                Parent = value2
            };
            UnityEngine.Vector3 vector2 = componentsInChildren[i].transform.position * scale;
            pmxBone2.Position = new PmxLib.Vector3(0f - vector2.x, vector2.y, 0f - vector2.z);
            dictionary.Add(componentsInChildren[i], i);
            pmxFile.BoneList.Add(pmxBone2);
            pmxFile.BoneBackupData.Add(text, new Pmx.BackupBoneData(pmxBone2.Name, componentsInChildren[i].GetInstanceID(), pmxBone2));
            currentBoneKeysList.Add(text, currentBoneKeysList.Count);
            currentBonesList.Add(pmxBone2.Name);
            editBoneInfo.Add(new BoneInfo(pmxBone2.Name, componentsInChildren[i]));
        }
        foreach (BoneInfo boneInfo in finalBoneInfoCache)
        {
            string name = GetAltBoneName(boneInfo.targetTransform);

            if (!finalBoneInfo.ContainsKey(name))
            {
                finalBoneInfo.Add(name, boneInfo.CreateNew(name));
            }
            //boneInfo.rename(GetAltBoneName(boneInfo.targetTransform)); // Update the bone name to match the exported data
        }
        finalBoneInfoCache.Clear();
    }

    private int sbi(string name, string id)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(name);
        stringBuilder.Append(" ");
        stringBuilder.Append(id);
        string text = stringBuilder.ToString();
        if (!currentBoneKeysList.TryGetValue(text, out var value))
        {
            Console.WriteLine("SBI Failed for: " + text);
        }
        return value;
    }

    public void SaveBodyTextures()
    {
        List<string> list = new List<string> { "_AM", "_MT" };
        try
        {
            ChaControl characterControl = MakerAPI.GetCharacterControl();
            foreach (string item in list)
            {
                string text = typeMap[item];
                Material matDraw = characterControl.customTexCtrlBody.matDraw;
                Texture texture = matDraw.GetTexture(text);
                if (texture == null)
                {
                    Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    texture2D.SetPixel(0, 0, new Color(1f, 1f, 1f, 1f));
                    texture2D.Apply();
                    texture = texture2D;
                }
                if (texture != null)
                {
                    WriteToTexture2D(matDraw, text, baseSavePath + "cf_m_body" + item + "_" + nowCoordinate.ToString("00") + ".png", texture);
                }
            }
        }
        catch (Exception value)
        {
            Console.WriteLine(value);
        }
    }

    public void SaveHeadTextures()
    {
        List<string> list = new List<string> { "_MT" };
        List<string> list2 = new List<string> { "cf_m_face_tex", "cf_m_hitomi_00_cf_Ohitomi_L02", "cf_m_hitomi_00_cf_Ohitomi_R02" };
        try
        {
            ChaControl characterControl = MakerAPI.GetCharacterControl();
            List<Material> list3 = new List<Material>
            {
                characterControl.customMatFace,
                characterControl.rendEye[0].material,
                characterControl.rendEye[1].material
            };
            Console.WriteLine(characterControl.rendEye[0].name);
            Console.WriteLine(characterControl.rendEye[1].name);
            for (int i = 0; i < list3.Count; i++)
            {
                if (list3[i] == null)
                {
                    continue;
                }
                foreach (string item in list)
                {
                    string text = typeMap[item];
                    Texture texture = list3[i].GetTexture(text);
                    if (texture == null)
                    {
                        Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                        texture2D.SetPixel(0, 0, new Color(1f, 1f, 1f, 1f));
                        texture2D.Apply();
                        texture = texture2D;
                    }
                    if (texture != null)
                    {
                        WriteToTexture2D(list3[i], text, baseSavePath + list2[i] + item + "_" + nowCoordinate.ToString("00") + ".png", texture);
                    }
                }
            }
        }
        catch (Exception value)
        {
            Console.WriteLine(value);
        }
    }

    public void ExportSpecialTextures()
    {
        try
        {
            ChaControl characterControl = MakerAPI.GetCharacterControl();
            if (nowCoordinate == maxCoord)
            {
                Material matCreate = ((CustomTextureCreate)characterControl.customTexCtrlBody).matCreate;
                string text = CleanUpMaterialName(matCreate.name);
                text = text.Substring(0, text.Length - 7);
                foreach (string item in new List<string> { "_CM", "_T3", "_T4", "_T6" })
                {
                    string text2 = typeMap[item];
                    if (matCreate.HasProperty(text2))
                    {
                        Texture texture = matCreate.GetTexture(text2);
                        if (texture != null)
                        {
                            WriteToTexture2D(matCreate, text2, savePath + text + item + ".png", texture);
                        }
                    }
                }
                Texture texMain = ((CustomTextureCreate)characterControl.customTexCtrlBody).texMain;
                if (texMain != null)
                {
                    WriteToTexture2D(((CustomTextureCreate)characterControl.customTexCtrlBody).matCreate, "_MainTex", savePath + text + "_MT.png", texMain);
                }
                Material matCreate2 = ((CustomTextureCreate)characterControl.customTexCtrlFace).matCreate;
                string text3 = CleanUpMaterialName(matCreate2.name);
                text3 = text3.Substring(0, text3.Length - 7) + "_00";
                foreach (string item2 in new List<string> { "_CM", "_T3", "_T4", "_T5", "_T6", "_T7" })
                {
                    string text4 = typeMap[item2];
                    if (matCreate2.HasProperty(text4))
                    {
                        Texture texture2 = matCreate2.GetTexture(text4);
                        if (texture2 != null)
                        {
                            WriteToTexture2D(matCreate2, text4, savePath + text3 + item2 + ".png", texture2);
                        }
                    }
                }
                Texture texMain2 = ((CustomTextureCreate)characterControl.customTexCtrlFace).texMain;
                if (texMain2 != null)
                {
                    WriteToTexture2D(((CustomTextureCreate)characterControl.customTexCtrlFace).matCreate, "_MainTex", savePath + text3 + "_MT.png", texMain2);
                }
            }
            if (nowCoordinate < maxCoord)
            {
                ExportClothingTextures(characterControl.ctCreateClothes, characterControl.cusClothesCmp, debug: false);
                ExportClothingTextures(characterControl.ctCreateClothesSub, characterControl.cusClothesSubCmp, debug: false);
            }
        }
        catch (Exception value)
        {
            Console.WriteLine(value);
        }
    }

    public void ExportClothingTextures(CustomTextureCreate[,] ctCloths, ChaClothesComponent[] cusCloths, bool debug)
    {
        for (int i = 0; i < ctCloths.GetLength(0); i++)
        {
            for (int j = 0; j < ctCloths.GetLength(1); j++)
            {
                CustomTextureCreate customTextureCreate = ctCloths[i, j];
                if (customTextureCreate == null)
                {
                    continue;
                }
                List<string> list = new List<string>();
                ChaClothesComponent chaClothesComponent = cusCloths[i];
                List<Renderer[]> list2;
                try
                {
                    list2 = GetClothRenderers(chaClothesComponent);
                }
                catch (Exception)
                {
                    list2 = new List<Renderer[]> { chaClothesComponent.rendNormal01, chaClothesComponent.rendNormal02 };
                }
                try
                {
                    Renderer[] array = list2[j];
                    foreach (Renderer renderer in array)
                    {
                        if (renderer != null)
                        {
                            string name = renderer.material.name;
                            name = CleanUpMaterialName(name);
                            name = name + " " + GetAltInstanceID(renderer.transform.parent.gameObject);
                            list.Add(name);
                        }
                    }
                }
                catch (Exception)
                {
                }
                if (chaClothesComponent.rendAccessory != null)
                {
                    string name2 = chaClothesComponent.rendAccessory.sharedMaterial.name;
                    name2 = CleanUpMaterialName(name2);
                    name2 = name2 + " " + GetAltInstanceID(chaClothesComponent.rendAccessory.transform.parent.gameObject);
                    list.Add(name2);
                }
                if (list.Count == 0)
                {
                    string item = "NotFound_" + customTextureCreate.texMain.name.Substring(0, customTextureCreate.texMain.name.Length - 2);
                    list.Add(item);
                }
                foreach (string item2 in list)
                {
                    Material matCreate = customTextureCreate.matCreate;
                    foreach (string item3 in new List<string> { "_CM", "_PM1", "_PM2", "_PM3" })
                    {
                        string text = typeMap[item3];
                        if (matCreate.HasProperty(text))
                        {
                            Texture texture = matCreate.GetTexture(text);
                            if (texture != null && !debug)
                            {
                                WriteToTexture2D(matCreate, text, savePath + item2 + item3 + ".png", texture);
                            }
                        }
                    }
                    Texture texMain = customTextureCreate.texMain;
                    if (texMain != null && !debug)
                    {
                        WriteToTexture2D(customTextureCreate.matCreate, "_MainTex", savePath + item2 + "_MT.png", texMain);
                    }
                }
            }
        }
    }

    public void ExportGagEyes()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        string assetBundleName = "chara/mt_eye_00.unity3d";
        List<GagData> list = new List<GagData>
        {
            new GagData("cf_t_gageye_00", 1),
            new GagData("cf_t_gageye_01", 2),
            new GagData("cf_t_gageye_02", 1),
            new GagData("cf_t_gageye_03", 2),
            new GagData("cf_t_gageye_04", 1),
            new GagData("cf_t_gageye_05", 1),
            new GagData("cf_t_gageye_06", 1),
            new GagData("cf_t_gageye_07", 3),
            new GagData("cf_t_gageye_08", 3),
            new GagData("cf_t_gageye_09", 3),
            new GagData("cf_t_expression_00", 0),
            new GagData("cf_t_expression_01", 0)
        };
        for (int i = 0; i < list.Count; i++)
        {
            Texture texture = CommonLib.LoadAsset<Texture2D>(assetBundleName, list[i].MainTex, clone: false, string.Empty);
            int num = list[i].EyeObjNo - 1;
            Material material = ((num >= 0) ? characterControl.matGag[list[i].EyeObjNo - 1] : characterControl.eyeLookMatCtrl[0]._material);
            string text = ((num >= 0) ? "_MT" : "_EXPR");
            string prop = typeMap[text];
            string text2 = CleanUpMaterialName(material.name) + "_" + list[i].MainTex;
            if (texture != null)
            {
                WriteToTexture2D(material, prop, baseSavePath + text2 + text + ((num >= 0) ? "_CT" : "") + ".png", texture);
            }
        }
    }

    public void GetCreateBodyMaterials()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        MaterialData matData = new MaterialData(((CustomTextureCreate)characterControl.customTexCtrlBody).matCreate, "cf_m_body_create");
        AddToMaterialDataList(matData);
        // The body material name changes with the gender, so change the material name or some colors won't be available in the Complete json
        AddCreateBodyMaterialsToComplete(((CustomTextureCreate)characterControl.customTexCtrlBody).matCreate, characterControl.sex == 0 ? "cm_m_body" : "cf_m_body");

        MaterialData matData2 = new MaterialData(((CustomTextureCreate)characterControl.customTexCtrlFace).matCreate, "cf_m_face_create");
        AddToMaterialDataList(matData2);
        AddCreateBodyMaterialsToComplete(((CustomTextureCreate)characterControl.customTexCtrlFace).matCreate, "cf_m_face_00");

        MaterialData matData3 = new MaterialData(characterControl.ctCreateEyeW.matCreate, "cf_m_eyewhite_create");
        AddToMaterialDataList(matData3);
        AddCreateBodyMaterialsToComplete(characterControl.ctCreateEyeW.matCreate, "cf_m_sirome_00");

        MaterialData matData4 = new MaterialData(characterControl.ctCreateEyeL.matCreate, "cf_m_eye_create_L");
        AddToMaterialDataList(matData4);
        AddCreateBodyMaterialsToComplete(characterControl.ctCreateEyeL.matCreate, "cf_Ohitomi_L02");

        MaterialData matData5 = new MaterialData(characterControl.ctCreateEyeR.matCreate, "cf_m_eye_create_R");
        AddToMaterialDataList(matData5);
        AddCreateBodyMaterialsToComplete(characterControl.ctCreateEyeR.matCreate, "cf_Ohitomi_R02");
    }

    public void AddCreateBodyMaterialsToComplete(Material mat, String name)
    {
        //Loop through the existing MaterialDataComplete material list to find what material to append the create information to
        MaterialInfo matDataCreate = new MaterialInfo(mat, name);
        foreach (MaterialDataComplete data in materialDataComplete)
        {
            foreach (MaterialInfo infoData in data.MatInfo)
            {
                if (infoData.MaterialName.Contains(matDataCreate.MaterialName))
                {
                    infoData.ShaderPropNames.AddRange(matDataCreate.ShaderPropNames);
                    infoData.ShaderPropColorValues.AddRange(matDataCreate.ShaderPropColorValues);
                }
            }
        }
    }

    public void GetCreateClothesMaterials()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        GetClothesCreateMaterials(characterControl.ctCreateClothes, characterControl.cusClothesCmp);
        GetClothesCreateMaterials(characterControl.ctCreateClothesSub, characterControl.cusClothesSubCmp, isSub: true);
    }

    public void GetClothesCreateMaterials(CustomTextureCreate[,] ctCloths, ChaClothesComponent[] cusCloths, bool isSub = false)
    {
        for (int i = 0; i < ctCloths.GetLength(0); i++)
        {
            for (int j = 0; j < ctCloths.GetLength(1); j++)
            {
                CustomTextureCreate customTextureCreate = ctCloths[i, j];
                if (customTextureCreate == null)
                {
                    continue;
                }
                List<string> list = new List<string>();
                Texture texMain = customTextureCreate.texMain;
                ChaClothesComponent chaClothesComponent = cusCloths[i];
                List<Renderer[]> list2;
                try
                {
                    list2 = GetClothRenderers(chaClothesComponent);
                }
                catch (Exception)
                {
                    list2 = new List<Renderer[]> { chaClothesComponent.rendNormal01, chaClothesComponent.rendNormal02 };
                }
                try
                {
                    Renderer[] array = list2[j];
                    foreach (Renderer renderer in array)
                    {
                        if (renderer != null)
                        {
                            currentRendererMaterialMapping.TryGetValue(renderer, out var value);
                            list.AddRange(value);
                        }
                    }
                }
                catch (Exception)
                {
                }
                if (chaClothesComponent.rendAccessory != null)
                {
                    currentRendererMaterialMapping.TryGetValue(chaClothesComponent.rendAccessory, out var value2);
                    list.AddRange(value2);
                }
                if (list.Count == 0)
                {
                    string item = "NotFound_" + texMain.name.Substring(0, texMain.name.Length - 2);
                    list.Add(item);
                }
                for (int l = 0; l < list.Count; l++)
                {
                    MaterialData matData = new MaterialData(customTextureCreate.matCreate, "create_" + list[l]);
                    AddToMaterialDataList(matData);

                    //Loop through the existing MaterialDataComplete material list to find what material to append the create information to
                    MaterialInfo matDataCreate = new MaterialInfo(customTextureCreate.matCreate, list[l]);
                    foreach (MaterialDataComplete data in materialDataComplete)
                    {
                        foreach (MaterialInfo infoData in data.MatInfo)
                        {
                            if (infoData.MaterialName.Contains(matDataCreate.MaterialName))
                            {
                                infoData.ShaderPropNames.AddRange(matDataCreate.ShaderPropNames);
                                infoData.ShaderPropColorValues.AddRange(matDataCreate.ShaderPropColorValues);
                            }
                        }
                    }
                }
            }
        }
    }

    private List<Renderer[]> GetClothRenderers(ChaClothesComponent clothesComponent)
    {
        List<Renderer[]> list = new List<Renderer[]> { clothesComponent.rendNormal01, clothesComponent.rendNormal02 };
        typeof(ChaClothesComponent).TryGetVariable<ChaClothesComponent, Renderer[]>("rendNormal03", clothesComponent, out var variable);
        if (variable != null)
        {
            list.Add(variable);
        }
        return list;
    }

    public void CreateClothesData()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        for (int i = 0; i < characterControl.cusClothesCmp.Length; i++)
        {
            ClothesData clothData = new ClothesData("CusClothesCmp", characterControl.fileStatus.coordinateType, characterControl.cusClothesCmp[i]);
            AddToClothesDataList(clothData);

            //force the indoor shoes enum in materialDataComplete to be 999
            try
            {
                if ((i % 7 == 0) && (i != 0))
                {
                    String indoor_shoe_smr_name = clothData.RendNormal01[0];
                    //Loop through the existing MaterialDataComplete material list to find what smr object to change the enum of
                    foreach (MaterialDataComplete data in materialDataComplete)
                    {
                        if (data.SMRName == indoor_shoe_smr_name)
                        {
                            data.EnumIndex = 999;
                        }
                    }
                }
            }
            //whatever
            catch
            {
                Console.WriteLine("Issue with detecting indoor shoes");
            }
            ;
        }
        for (int i = 0; i < characterControl.cusClothesSubCmp.Length; i++)
        {
            ClothesData clothData2 = new ClothesData("CusClothesSubCmp", characterControl.fileStatus.coordinateType, characterControl.cusClothesSubCmp[i]);
            AddToClothesDataList(clothData2);
        }
    }

    public void CreateAccessoryData()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        for (int i = 0; i < characterControl.cusAcsCmp.Length; i++)
        {
            if (!(characterControl.cusAcsCmp[i] == null))
            {
                AccessoryData accData = new AccessoryData(characterControl.fileStatus.coordinateType, characterControl.cusAcsCmp[i]);
                AddToAccessoryDataList(accData);
            }
        }
    }

    public void CreateChaFileCoordinateData()
    {
        ChaFileCoordinateData coordData = new ChaFileCoordinateData(MakerAPI.GetCharacterControl());
        AddToChaFileCoordinateDataList(coordData);
    }

    public void CreateReferenceInfoData()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        foreach (int value in Enum.GetValues(typeof(ChaReference.RefObjKey)))
        {
            ReferenceInfoData refInfoData = new ReferenceInfoData(value, characterControl.GetReferenceInfo((ChaReference.RefObjKey)value));
            AddToReferenceInfoDataList(refInfoData);
        }
    }

    public void CreateDynamicBonesData()
    {
        DynamicBone[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBone>(includeInactive: true);
        DynamicBone_Ver01[] componentsInChildren2 = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBone_Ver01>(includeInactive: true);
        DynamicBone_Ver02[] componentsInChildren3 = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBone_Ver02>(includeInactive: true);
        DynamicBone[] array = componentsInChildren;
        foreach (DynamicBone dynamicBone in array)
        {
            if (dynamicBone != null)
            {
                DynamicBoneData dynamicBoneData = new DynamicBoneData(dynamicBone);
                AddToDynamicBonesDataList(dynamicBoneData);
            }
        }
        DynamicBone_Ver01[] array2 = componentsInChildren2;
        foreach (DynamicBone_Ver01 dynamicBone_Ver in array2)
        {
            if (dynamicBone_Ver != null)
            {
                DynamicBoneData dynamicBoneData2 = new DynamicBoneData(dynamicBone_Ver);
                AddToDynamicBonesDataList(dynamicBoneData2);
            }
        }
        DynamicBone_Ver02[] array3 = componentsInChildren3;
        foreach (DynamicBone_Ver02 dynamicBone_Ver2 in array3)
        {
            if (dynamicBone_Ver2 != null)
            {
                DynamicBoneData dynamicBoneData3 = new DynamicBoneData(dynamicBone_Ver2);
                AddToDynamicBonesDataList(dynamicBoneData3);
            }
        }
    }

    public void CreateDynamicBoneCollidersData()
    {
        DynamicBoneCollider[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBoneCollider>(includeInactive: true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            DynamicBoneColliderData dynamicBoneColliderData = new DynamicBoneColliderData(componentsInChildren[i]);
            AddToDynamicBoneCollidersDataList(dynamicBoneColliderData);
        }
    }

    public void CreateAccessoryStateData()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        try
        {
            CharaEvent componentInChildren = characterControl.GetComponentInChildren<CharaEvent>();
            if (!(componentInChildren != null))
            {
                return;
            }
            foreach (KeyValuePair<int, Slotdata> item in componentInChildren.Coordinate[nowCoordinate].Slotinfo)
            {
                AccessoryStateData accStateData = new AccessoryStateData(item.Key, item.Value);
                AddToAccessoryStateDataList(accStateData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void CreateCharacterInfoData()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        EyeLookMaterialControll eyeMatControllerl = characterControl.eyeLookMatCtrl[0].eyeLR == EYE_LR.EYE_L ? characterControl.eyeLookMatCtrl[0] : characterControl.eyeLookMatCtrl[1];
        EyeLookMaterialControll eyeMatControllerr = characterControl.eyeLookMatCtrl[0].eyeLR == EYE_LR.EYE_R ? characterControl.eyeLookMatCtrl[0] : characterControl.eyeLookMatCtrl[1];
        ChaFileFace fileFace = characterControl.fileFace;
        List<string> list = (from field in fileFace.GetType().GetFields(bindingAttr)
                             select field.Name).ToList();
        List<object> list2 = (from field in fileFace.GetType().GetFields(bindingAttr)
                              select field.GetValue(fileFace)).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            string value = ((list2[i] != null) ? list2[i].ToString() : "");
            ChaFileData item = new ChaFileData(list[i].ToString(), value);
            chaFileCustomFaceData.Add(item);
        }
        ChaFileBody fileBody = characterControl.fileBody;
        List<string> list3 = (from field in fileBody.GetType().GetFields(bindingAttr)
                              select field.Name).ToList();
        List<object> list4 = (from field in fileBody.GetType().GetFields(bindingAttr)
                              select field.GetValue(fileBody)).ToList();
        for (int j = 0; j < list3.Count; j++)
        {
            string value2 = ((list4[j] != null) ? list4[j].ToString() : "");
            ChaFileData item2 = new ChaFileData(list3[j].ToString(), value2);
            chaFileCustomBodyData.Add(item2);
        }
        ChaFileHair fileHair = characterControl.fileHair;
        List<string> list5 = (from field in fileHair.GetType().GetFields(bindingAttr)
                              select field.Name).ToList();
        List<object> list6 = (from field in fileHair.GetType().GetFields(bindingAttr)
                              select field.GetValue(fileHair)).ToList();
        for (int k = 0; k < list5.Count; k++)
        {
            string value3 = ((list6[k] != null) ? list6[k].ToString() : "");
            ChaFileData item3 = new ChaFileData(list5[k].ToString(), value3);
            chaFileCustomHairData.Add(item3);
        }

        List<BlendShapeinfo> blendShapeinfos = new();

        int orPtn = characterControl.GetEyesPtn();
        bool orSha = characterControl.GetEyesShaking();
        bool orBli = characterControl.GetEyesShaking();
        float orOpr = characterControl.eyesCtrl.openRate;
        float orOpm = characterControl.GetEyesOpenMax();
        var ptnNames = characterControl.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_eyeset).Values.ToList();

        characterControl.ChangeEyesOpenMax(1);
        characterControl.ChangeEyesShaking(false);
        characterControl.ChangeEyesBlinkFlag(false);
        characterControl.ChangeEyesPtn(1, false);


        for (int j = 0; j < ptnNames.Count; j++)
        {
            BlendShapeinfo bld = new BlendShapeinfo(ptnNames[j].Name);
            blendShapeinfos.Add(bld);
            characterControl.ChangeEyesPtn(j, false);
            characterControl.fbsCtrl.LateUpdate();
            characterControl.eyesCtrl.CalculateBlendShape();

            foreach (var _ in characterControl.eyesCtrl.FBSTarget)
            {
                var smr = _.GetSkinnedMeshRenderer();
                for (int k = 0; k < smr.sharedMesh.blendShapeCount; k++)
                {
                    float weight = smr.GetBlendShapeWeight(k);
                    if (weight > 0)
                    {
                        bld.Add(smr.sharedMesh.GetBlendShapeName(k), weight);
                    }
                }
            }
        }

        characterControl.ChangeEyesPtn(orPtn);
        characterControl.ChangeEyesShaking(orSha);
        characterControl.ChangeEyesBlinkFlag(orBli);
        characterControl.eyesCtrl.SetOpenRateForce(orOpr);
        characterControl.ChangeEyesOpenMax(orOpm);
        characterControl.fbsCtrl.LateUpdate();
        characterControl.eyesCtrl.CalculateBlendShape();

        CharacterInfoData item4 = new CharacterInfoData
        {
            Personality = characterControl.fileParam.personality,
            VoiceRate = characterControl.fileParam.voiceRate,
            PupilWidth = characterControl.fileFace.pupilWidth,
            PupilHeight = characterControl.fileFace.pupilHeight,
            PupilX = characterControl.fileFace.pupilX,
            PupilY = characterControl.fileFace.pupilY,
            HlUpY = characterControl.fileFace.hlUpY,
            HlDownY = characterControl.fileFace.hlDownY,
            ShapeInfoFace = characterControl.chaFile.custom.face.shapeValueFace.ToList(),
            ShapeInfoBody = characterControl.chaFile.custom.body.shapeValueBody.ToList(),
        };

        UnityEngine.Vector2 _vecl = eyeMatControllerl._material.GetTextureOffset(eyeMatControllerl.texStates[0].texID);
        UnityEngine.Vector2 _vecr = eyeMatControllerr._material.GetTextureOffset(eyeMatControllerr.texStates[0].texID);
        item4.pupilOffset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl._material.GetTextureOffset(eyeMatControllerl.texStates[1].texID);
        _vecr = eyeMatControllerr._material.GetTextureOffset(eyeMatControllerr.texStates[1].texID);
        item4.highlightUpOffset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl._material.GetTextureOffset(eyeMatControllerl.texStates[2].texID);
        _vecr = eyeMatControllerr._material.GetTextureOffset(eyeMatControllerr.texStates[2].texID);
        item4.highlightDownOffset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl._material.GetTextureScale(eyeMatControllerl.texStates[0].texID);
        _vecr = eyeMatControllerr._material.GetTextureScale(eyeMatControllerr.texStates[0].texID);
        item4.pupilScale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl._material.GetTextureScale(eyeMatControllerl.texStates[1].texID);
        _vecr = eyeMatControllerr._material.GetTextureScale(eyeMatControllerr.texStates[1].texID);
        item4.highlightUpScale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl._material.GetTextureScale(eyeMatControllerl.texStates[2].texID);
        _vecr = eyeMatControllerr._material.GetTextureScale(eyeMatControllerr.texStates[2].texID);
        item4.highlightDownScale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        item4.eyeRotation = new List<float>() { eyeMatControllerl._material.GetFloat("_rotation"), eyeMatControllerr._material.GetFloat("_rotation") };

        Color _color = eyeMatControllerl._material.GetColor("_overcolor1");
        item4.highlightUpColor = new List<float>() { _color.r, _color.g, _color.b, _color.a };

        _color = eyeMatControllerl._material.GetColor("_overcolor2");
        item4.highlightDownColor = new List<float>() { _color.r, _color.g, _color.b, _color.a };

        characterInfoData.Add(item4);

        ExportDataListToJson(chaFileCustomFaceData, "KK_ChaFileCustomFace.json");
        ExportDataListToJson(chaFileCustomBodyData, "KK_ChaFileCustomBody.json");
        ExportDataListToJson(chaFileCustomHairData, "KK_ChaFileCustomHair.json");
        ExportDataListToJson(characterInfoData, "KK_CharacterInfoData.json");
        ExportDataListToJson(blendShapeinfos, "KK_BlendShapeInfo.json");
    }

    public void CreateListInfoData()
    {
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        ListInfoBase[] infoClothes = characterControl.infoClothes;
        for (int i = 0; i < infoClothes.Length; i++)
        {
            ListInfoData lstData = new ListInfoData(infoClothes[i], "InfoClothes");
            AddToListInfoDataList(lstData);
        }
        infoClothes = characterControl.infoParts;
        for (int i = 0; i < infoClothes.Length; i++)
        {
            ListInfoData lstData2 = new ListInfoData(infoClothes[i], "InfoParts");
            AddToListInfoDataList(lstData2);
        }
        infoClothes = characterControl.infoAccessory;
        for (int i = 0; i < infoClothes.Length; i++)
        {
            ListInfoData lstData3 = new ListInfoData(infoClothes[i], "InfoAccessory");
            AddToListInfoDataList(lstData3);
        }
        infoClothes = characterControl.infoHair;
        for (int i = 0; i < infoClothes.Length; i++)
        {
            ListInfoData lstData4 = new ListInfoData(infoClothes[i], "InfoHair");
            AddToListInfoDataList(lstData4);
        }
    }

    public void AddToSMRDataList(SMRData smrData)
    {
        if (characterSMRData.Find((SMRData data) => string.CompareOrdinal(smrData.SMRName, data.SMRName) == 0 && string.CompareOrdinal(smrData.SMRPath, data.SMRPath) == 0) == null)
        {
            characterSMRData.Add(smrData);
        }
    }
    public void AddToMaterialDataCompleteList(MaterialDataComplete smrData)
    {
        if (materialDataComplete.Find((MaterialDataComplete data) => string.CompareOrdinal(smrData.SMRName, data.SMRName) == 0 && string.CompareOrdinal(smrData.SMRPath, data.SMRPath) == 0) == null)
        {
            materialDataComplete.Add(smrData);
        }
    }

    public void AddToTextureDataList(TextureData texData)
    {
        if (texData.textureName != null && textureData.Find((TextureData data) => string.CompareOrdinal(texData.textureName, data.textureName) == 0) == null)
        {
            textureData.Add(texData);
        }
    }

    public void AddToMaterialDataList(MaterialData matData)
    {
        if ((materialData.Find((MaterialData data) => string.CompareOrdinal(matData.MaterialName, data.MaterialName) == 0) == null || string.CompareOrdinal(matData.MaterialName, EyeMatName) == 0) && string.CompareOrdinal(matData.MaterialName, "Bonelyfans") != 0)
        {
            materialData.Add(matData);
        }
    }

    public void AddToClothesDataList(ClothesData clothData)
    {
        clothesData.Add(clothData);
    }

    public void AddToAccessoryDataList(AccessoryData accData)
    {
        accessoryData.Add(accData);
    }

    public void AddToChaFileCoordinateDataList(ChaFileCoordinateData coordData)
    {
        chaFileCoordinateData.Add(coordData);
    }

    public void AddToReferenceInfoDataList(ReferenceInfoData refInfoData)
    {
        referenceInfoData.Add(refInfoData);
    }

    public void AddToDynamicBonesDataList(DynamicBoneData dynamicBoneData)
    {
        dynamicBonesData.Add(dynamicBoneData);
    }

    public void AddToDynamicBoneCollidersDataList(DynamicBoneColliderData dynamicBoneColliderData)
    {
        dynamicBoneCollidersData.Add(dynamicBoneColliderData);
    }

    public void AddToAccessoryStateDataList(AccessoryStateData accStateData)
    {
        accessoryStateData.Add(accStateData);
    }

    public void AddToBoneOffsetDataList(BoneOffsetData bnOffsetData)
    {
        boneOffsetData.Add(bnOffsetData);
    }

    public void AddToListInfoDataList(ListInfoData lstData)
    {
        listInfoData.Add(lstData);
    }

    public void ExportChaFileCoordinateDataListToJson(List<ChaFileCoordinateData> dataList, string fileName)
    {
        string text = "";
        foreach (ChaFileCoordinateData data in dataList)
        {
            string text2 = JsonUtility.ToJson(data);
            text2 = text2.Replace("}", ",");
            text2 = text2 + "\"Clothes\":" + JsonUtility.ToJson(data.Clothes).Replace("}", ",");
            text2 += "\"Parts\":[";
            foreach (ChaFileClothes_PartsInfo part in data.Clothes.Parts)
            {
                text2 = text2 + JsonUtility.ToJson(part) + ",";
            }
            text2 = text2.TrimEnd(',');
            text2 += "]},";
            text2 = text2 + "\"Accessory\":" + JsonUtility.ToJson(data.Accessory).Replace("}", "");
            text2 += "\"Parts\":[";
            foreach (ChaFileAccessory_PartsInfo part2 in data.Accessory.Parts)
            {
                text2 = text2 + JsonUtility.ToJson(part2) + ",";
            }
            text2 = text2.TrimEnd(',');
            text2 += "]},";
            text2 = text2 + "\"Makeup\":" + JsonUtility.ToJson(data.Makeup);
            text2 += "}";
            text = text + text2 + ",\n";
        }
        if (!text.IsNullOrEmpty())
        {
            text = text.Substring(0, text.Length - 2);
        }
        text = "[" + text + "]";
        File.WriteAllText(baseSavePath + fileName, text);
    }

    public void ExportDataListToJson<T>(List<T> dataList, string fileName)
    {
        string text = "";
        if (dataList.Count() > 0)
        {
            foreach (T data in dataList)
            {
                string text2 = JsonUtility.ToJson(data);
                text = text + text2 + ",\n";
            }
            if (!text.IsNullOrEmpty())
            {
                text = text.Substring(0, text.Length - 2);
            }
            text = "[" + text + "]";
        }
        File.WriteAllText(baseSavePath + fileName, text);
    }

    public void ExportListStringToJSON(List<string> list, string fileName)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[");
        for(int i = 0; i < list.Count - 1; i++)
        {
            sb.AppendLine('\"' + list[i] + "\",");
        }
        if (list.Count > 0)
        {
            sb.AppendLine('\"' + list[list.Count - 1] + "\"");
        }
        sb.Append("]");
        File.WriteAllText(baseSavePath + fileName, sb.ToString());
    }


    public void ExportMatDataCompListToJson(List<MaterialDataComplete> dataList, string fileName)
    {
        string text = "";
        if (dataList.Count() > 0)
        {
            foreach (MaterialDataComplete data in dataList)
            {
                string text2 = JsonUtility.ToJson(data);
                string text3 = "";

                //The JsonUtility does not seem to work when you have a class instance as a class variable,
                //so each MaterialInfo variable in each MaterialDataComplete instance has to be converted separately then appended to the json line
                foreach (MaterialInfo infoData in data.MatInfo)
                {
                    text3 = text3 + JsonUtility.ToJson(infoData);
                }
                if (!text3.IsNullOrEmpty())
                {
                    text3 = text3.Substring(1, text3.Length - 1);
                    text3 = "\"MaterialInformation\":[{" + text3 + "]}";
                    text3 = text3.Replace("}{", "},{"); //add comma between material entries
                }
                if (!text2.IsNullOrEmpty())
                {
                    text2 = text2.Substring(0, text2.Length - 1) + ",";
                }
                text = text + text2 + text3 + ",\n";
            }
            if (!text.IsNullOrEmpty())
            {
                text = text.Substring(0, text.Length - 2);
            }
            text = "[" + text + "]";
        }
        File.WriteAllText(baseSavePath + fileName, text);
    }

    public void ExportDataToJson<T>(T data, string fileName)
    {
        string contents = JsonUtility.ToJson(data);
        File.WriteAllText(baseSavePath + fileName, contents);
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string text = "/" + GetAltBoneName(obj, ignoreID: true);
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            text = "/" + GetAltBoneName(obj, ignoreID: true) + text;
        }
        return text;
    }

    public static int GetAltInstanceID(UnityEngine.Object _object)
    {
        if (instanceIDs.TryGetValue(_object.GetInstanceID(), out var value) && value >= 0)
        {
            int.TryParse(value.ToString() + nowCoordinate, out value);
            return value;
        }
        Console.WriteLine("No ID Found");
        return -1;
    }

    public static string GetAltBoneName(UnityEngine.Object _object, bool ignoreID = false)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(_object.name);
        stringBuilder.Append(" ");
        stringBuilder.Append(_object.GetInstanceID().ToString());
        string text = stringBuilder.ToString();
        bool flag = nowCoordinate == minCoord;
        bool flag2 = false;
        bool flag3 = pmxFile.BoneBackupData.ContainsKey(text);
        if (flag3)
        {
            flag2 = offsetBoneCandidates.Contains(text, StringComparer.Ordinal);
        }
        if (flag && currentBonesList.Contains(_object.name))
        {
            flag = false;
        }
        if (flag || ignoreID || (flag3 && !flag2))
        {
            return _object.name;
        }
        StringBuilder stringBuilder2 = new StringBuilder();
        stringBuilder2.Append(_object.name);
        stringBuilder2.Append(" ");
        stringBuilder2.Append(GetAltInstanceID(_object).ToString());
        return stringBuilder2.ToString();
    }

    public static string GetAltMaterialName(PmxBuilder pmxBuilder, string materialName)
    {
        if (currentMaterialList.TryGetValue(materialName, out var value))
        {
            if (!pmxBuilder.ignoreList.Contains(materialName))
            {
                currentMaterialList[materialName] = value + 1;
                materialName = materialName + " " + value.ToString("00");
            }
        }
        else
        {
            currentMaterialList.Add(materialName, value);
        }
        return materialName;
    }

    public void CreatePmxHeader()
    {
        PmxElementFormat pmxElementFormat = new PmxElementFormat(1f)
        {
            VertexSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.VertexList.Count),
            UVACount = 3
        };
        int val = int.MinValue;
        for (int i = 0; i < pmxFile.BoneList.Count; i++)
        {
            val = Math.Max(val, Math.Abs(pmxFile.BoneList[i].IK.LinkList.Count));
        }
        int count = Math.Max(val, pmxFile.BoneList.Count);
        pmxElementFormat.BoneSize = PmxElementFormat.GetSignedBufSize(count);
        if (pmxElementFormat.BoneSize < 2)
        {
            pmxElementFormat.BoneSize = 2;
        }
        pmxElementFormat.MorphSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MorphList.Count);
        pmxElementFormat.MaterialSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MaterialList.Count);
        pmxElementFormat.BodySize = PmxElementFormat.GetUnsignedBufSize(pmxFile.BodyList.Count);
        PmxHeader pmxHeader = new PmxHeader(2.1f);
        pmxHeader.FromElementFormat(pmxElementFormat);
        pmxFile.Header = pmxHeader;
    }

    public void Save()
    {
        pmxFile.ToFile(savePath + "model.pmx");
    }

    public void ExportAllDataLists()
    {
        ExportDataListToJson(characterSMRData, "KK_SMRData.json");
        ExportMatDataCompListToJson(materialDataComplete, "KK_MaterialDataComplete.json");
        ExportDataListToJson(materialData, "KK_MaterialData.json");
        ExportDataListToJson(textureData, "KK_TextureData.json");
        ExportDataListToJson(clothesData, "KK_ClothesData.json");
        ExportDataListToJson(accessoryData, "KK_AccessoryData.json");
        ExportDataListToJson(referenceInfoData, "KK_ReferenceInfoData.json");
        ExportDataListToJson(dynamicBonesData, "KK_DynamicBoneData.json");
        ExportDataListToJson(dynamicBoneCollidersData, "KK_DynamicBoneColliderData.json");
        ExportDataListToJson(accessoryStateData, "KK_AccessoryStateData.json");
        ExportDataListToJson(boneOffsetData, "KK_BoneOffsetData.json");
        ExportDataListToJson(listInfoData, "KK_ListInfoData.json");
        ExportChaFileCoordinateDataListToJson(chaFileCoordinateData, "KK_ChaFileCoordinateData.json");
        ExportDataListToJson(editBoneInfo, "KK_EditBoneInfo.json");
        ExportDataListToJson(finalBoneInfo.Values.ToList(), "KK_FinalBoneInfo.json");
        ExportListStringToJSON(lightDarkMaterials, "KK_LightDarkMaterials.json");
    }

    public void OpenFolderInExplorer(string filename)
    {
        if (filename == null)
        {
            throw new ArgumentNullException("filename");
        }
        try
        {
            filename = Path.GetFullPath(filename);
            Process.Start("explorer.exe", filename ?? "");
        }
        catch (Exception value)
        {
            Console.WriteLine(value);
        }
    }

    public static string CleanUpMaterialName(string str)
    {
        return str.Replace("(Instance)", "").Trim();
    }

    public static string AnimationCurveToJSON(AnimationCurve curve)
    {
        StringBuilder stringBuilder = new StringBuilder();
        BeginObject(stringBuilder);
        BeginArray(stringBuilder, "keys");
        Keyframe[] keys = curve.GetKeys();
        for (int i = 0; i < keys.Length; i++)
        {
            Keyframe keyframe = keys[i];
            BeginObject(stringBuilder);
            WriteFloat(stringBuilder, "time", keyframe.time);
            Next(stringBuilder);
            WriteFloat(stringBuilder, "value", keyframe.value);
            Next(stringBuilder);
            WriteFloat(stringBuilder, "intangent", keyframe.inTangent);
            Next(stringBuilder);
            WriteFloat(stringBuilder, "outtangent", keyframe.outTangent);
            EndObject(stringBuilder);
            if (i < keys.Length - 1)
            {
                Next(stringBuilder);
            }
        }
        EndArray(stringBuilder);
        EndObject(stringBuilder);
        return stringBuilder.ToString();
    }

    public static void BeginObject(StringBuilder sb)
    {
        sb.Append("{ ");
    }

    public static void EndObject(StringBuilder sb)
    {
        sb.Append(" }");
    }

    public static void BeginArray(StringBuilder sb, string keyname)
    {
        sb.AppendFormat("\"{0}\" : [", keyname);
    }

    public static void EndArray(StringBuilder sb)
    {
        sb.Append(" ]");
    }

    public static void WriteString(StringBuilder sb, string key, string value)
    {
        sb.AppendFormat("\"{0}\" : \"{1}\"", key, value);
    }

    public static void WriteFloat(StringBuilder sb, string key, float val)
    {
        sb.AppendFormat("\"{0}\" : {1}", key, val);
    }

    public static void WriteInt(StringBuilder sb, string key, int val)
    {
        sb.AppendFormat("\"{0}\" : {1}", key, val);
    }

    public static void WriteVector3(StringBuilder sb, string key, UnityEngine.Vector3 val)
    {
        sb.AppendFormat("\"{0}\" : {1}", key, JsonUtility.ToJson(val));
    }

    public static void Next(StringBuilder sb)
    {
        sb.Append(", ");
    }
}
