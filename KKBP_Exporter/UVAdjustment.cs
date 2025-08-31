using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
internal class UVAdjustment
{
    public string SMRName;
    public string SMRPath;
    public int instanceID;
    public int xOffset;
    public int yOffset;
    public int xScale;
    public int yScale;
    public UVAdjustment(string SMRName, string SMRPath, int instanceID, int xOffset, int yOffset, int xScale, int yScale)
    {
        this.SMRName = SMRName;
        this.SMRPath = SMRPath;
        this.instanceID = instanceID;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.xScale = xScale;
        this.yScale = yScale;
    }
}
