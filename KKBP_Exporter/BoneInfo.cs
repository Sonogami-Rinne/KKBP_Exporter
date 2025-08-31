using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
internal class BoneInfo
{
    public string boneName = "";
    public List<float> transform;
    public List<float> position;
    public List<float> rotation;
    public List<float> scale;
    public List<float> worldTransform;
    public List<float> worldPosition;
    public List<float> worldRotation;
    public List<float> worldScale;
    [NonSerialized]
    public Vector3 _scale;
    [NonSerialized]
    public Transform targetTransform;

    private static Matrix4x4 unityToBlender = new Matrix4x4();

    public static GameObject converter;

    private BoneInfo()
    {

    }

    public BoneInfo(string boneName, Transform transform)
    {
        Transform converter;
        if (BoneInfo.converter == null)
        {
            BoneInfo.converter = new GameObject("UnityToBlenderConverter");
            BoneInfo.unityToBlender = new Matrix4x4();
            BoneInfo.unityToBlender.SetColumn(0, new Vector4(-1, 0, 0, 0));
            BoneInfo.unityToBlender.SetColumn(1, new Vector4(0, 0, 1, 0));
            BoneInfo.unityToBlender.SetColumn(2, new Vector4(0, -1, 0, 0));
            BoneInfo.unityToBlender.SetColumn(3, new Vector4(0, 0, 0, 1));
            converter = BoneInfo.converter.transform;
            converter.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));
            converter.localScale = Vector3.zero;
        }
        else
        {
            converter = BoneInfo.converter.transform;
        }
        this.targetTransform = transform;
        this.boneName = boneName;
        Vector3 position;
        Quaternion rotation;
        Vector3 scale;
        //Local
        position = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        rotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
        scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
        matrix = unityToBlender * matrix * unityToBlender.inverse;
        this.transform = new List<float>()
        {
            matrix.m00, matrix.m01, matrix.m02, matrix.m03,
            matrix.m10, matrix.m11, matrix.m12, matrix.m13,
            matrix.m20, matrix.m21, matrix.m22, matrix.m23,
            matrix.m30, matrix.m31, matrix.m32, matrix.m33
        };
        //For some reasons I can't access Matrix4x4.Rotate() and Matrix4x4.rotation......
        GameObject temp = new GameObject("Temp");
        Transform tempT = temp.transform;
        tempT.SetParent(converter, false);
        tempT.rotation = rotation;
        Quaternion rotBlender = tempT.localRotation;

        this._scale = scale;

        this.position = new List<float> { -position.x, -position.z, position.y };
        this.rotation = new List<float> { rotBlender.w, rotBlender.x, rotBlender.y, rotBlender.z };
        this.scale = new List<float> { scale.x, scale.z, scale.y };

        //World
        position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        scale = new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        matrix = Matrix4x4.TRS(position, rotation, scale);
        matrix = unityToBlender * matrix * unityToBlender.inverse;
        this.worldTransform = new List<float>()
        {
            matrix.m00, matrix.m01, matrix.m02, matrix.m03,
            matrix.m10, matrix.m11, matrix.m12, matrix.m13,
            matrix.m20, matrix.m21, matrix.m22, matrix.m23,
            matrix.m30, matrix.m31, matrix.m32, matrix.m33
        };
        //For some reason I can't access Matrix4x4.Rotate() and Matrix4x4.rotation......
        tempT.rotation = rotation;
        rotBlender = tempT.localRotation;
        GameObject.DestroyImmediate(temp);

        this.worldPosition = new List<float> { -position.x, -position.z, position.y };
        this.worldRotation = new List<float> { rotBlender.w, rotBlender.x, rotBlender.y, rotBlender.z };
        this.worldScale = new List<float> { scale.x, scale.z, scale.y };

    }
    
    public BoneInfo CreateNew(string newName)
    {
        BoneInfo info = new BoneInfo();

        info.boneName = newName;
        info.transform = new List<float>(transform);
        info.position = new List<float>(position);
        info.rotation = new List<float>(rotation);
        info.scale = new List<float>(scale);
        info.worldTransform = new List<float>(worldTransform);
        info.worldPosition = new List<float>(worldPosition);
        info.worldRotation = new List<float>(worldRotation);
        info.worldScale = new List<float>(worldScale);
        info._scale = new Vector3(_scale.x, _scale.y, _scale.z);
        info.targetTransform = targetTransform;
        return info;
    }
}