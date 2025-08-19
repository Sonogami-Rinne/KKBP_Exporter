using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
internal class BoneRotation
{
    public string boneName = "";
    public float rotation = 0.0f;

    public BoneRotation(string boneName, float rotation)
    {
        this.boneName = boneName;
        this.rotation = rotation;
    }
}