using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
internal class MorphInfo
{
    public string name = "";
    public float value = 0.0f;

    public MorphInfo(string name, float value)
    {
        this.name = name;
        this.value = value;
    }
}
