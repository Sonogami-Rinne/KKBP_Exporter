using KKAPI.Maker;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
internal class MaterialDataComplete
{
    public string SMRPath;
    public int CoordinateType = -1;
    public int EnumIndex = -1;
    public string SMRName;

    public List<MaterialInfo> MatInfo = new List<MaterialInfo>();

    public MaterialDataComplete(PmxBuilder pmxBuilder, SkinnedMeshRenderer smr)
    {
        if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
        {
            CoordinateType = PmxBuilder.nowCoordinate;
        }
        SMRName = smr.name;
        SMRName = ((pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal) && smr.sharedMaterials.Count() > 0 && pmxBuilder.ignoreList.Contains(PmxBuilder.CleanUpMaterialName(smr.sharedMaterial.name), StringComparer.Ordinal)) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
        SMRPath = PmxBuilder.GetGameObjectPath(smr.gameObject);
        for (int i = 0; i < smr.materials.Count(); i++)
        {
            if ((bool)smr.materials[i])
            {
                string name = smr.materials[i].name;
                name = PmxBuilder.CleanUpMaterialName(name);
                name = ((!pmxBuilder.ignoreList.Contains(name, StringComparer.Ordinal) || !pmxBuilder.ignoreList.Contains(smr.name, StringComparer.Ordinal)) ? (name + " " + PmxBuilder.GetAltInstanceID(smr.transform.parent.gameObject)) : ((!name.Contains(pmxBuilder.EyeMatName)) ? name : (name + "_" + smr.name)));
                //name = PmxBuilder.GetAltMaterialName(pmxBuilder, name);
                MatInfo.Add(new MaterialInfo(smr.materials[i], name));
                Console.WriteLine("Adding to json SMR: " + name);
            }
        }

        //Get the enum value for this game object
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        foreach (int value in Enum.GetValues(typeof(ChaReference.RefObjKey)))
        {
            GameObject gameObject = characterControl.GetReferenceInfo((ChaReference.RefObjKey)value);
            string GameObjectPath = "";
            if ((bool)gameObject)
            {
                GameObjectPath = PmxBuilder.GetGameObjectPath(gameObject);
            }
            if (SMRPath.Contains(GameObjectPath) && GameObjectPath != "")
            {
                EnumIndex = value;
            }
        }
    }

    public MaterialDataComplete(PmxBuilder pmxBuilder, MeshRenderer smr)
    {
        if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
        {
            CoordinateType = PmxBuilder.nowCoordinate;
        }
        SMRName = smr.name;
        SMRName = (pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
        SMRPath = PmxBuilder.GetGameObjectPath(smr.gameObject);
        for (int i = 0; i < smr.materials.Count(); i++)
        {
            string name = smr.materials[i].name;
            name = PmxBuilder.CleanUpMaterialName(name);
            name = ((pmxBuilder.ignoreList.Contains(name, StringComparer.Ordinal) && pmxBuilder.ignoreList.Contains(smr.name, StringComparer.Ordinal)) ? ((!name.Contains(pmxBuilder.EyeMatName)) ? name : (name + "_" + smr.name)) : (name + " " + PmxBuilder.GetAltInstanceID(smr.transform.parent.gameObject)));
            //name = PmxBuilder.GetAltMaterialName(pmxBuilder, name);
            MatInfo.Add(new MaterialInfo(smr.materials[i], name));
            Console.WriteLine("Adding to json MR: " + name);
        }

        //Get the enum value for this game object
        ChaControl characterControl = MakerAPI.GetCharacterControl();
        foreach (int value in Enum.GetValues(typeof(ChaReference.RefObjKey)))
        {
            GameObject gameObject = characterControl.GetReferenceInfo((ChaReference.RefObjKey)value);
            string GameObjectPath = "";
            if ((bool)gameObject)
            {
                GameObjectPath = PmxBuilder.GetGameObjectPath(gameObject);
            }
            if (GameObjectPath == SMRPath)
            {
                EnumIndex = value;
            }
        }
    }
}