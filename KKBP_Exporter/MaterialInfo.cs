using System;
using System.Collections.Generic;
using System.Linq;
using PmxLib;
using UnityEngine;

[Serializable]
internal class MaterialInfo
{
	public string MaterialName;
	public string ShaderName;

	public List<string> ShaderPropNames = new List<string>();
	public List<Color> ShaderPropColorValues = new List<Color>();
    public bool isHair = false;

    public MaterialInfo(Material material, string _materialName)
	{
		MaterialName = _materialName;
		ShaderName = material.shader.name;
		MaterialShader materialShader2 = MaterialShaders.materialShaders.Find((MaterialShader materialShader) => string.CompareOrdinal(materialShader.shaderName, ShaderName) == 0);
		if (materialShader2 == null)
		{
			return;
		}
		foreach (MaterialShaderItem property in materialShader2.properties)
		{
			// Add the color data
            string text = "_" + property.name;
			if (property.type == "Color")
			{
				ShaderPropNames.Add(text + " " + property.type + " " + ShaderPropColorValues.Count());
				ShaderPropColorValues.Add(material.GetColor(text));
            }

			// check the names of each texture. if there is a texture with "_HGLS" in the name, mark it as a hair shader.
            if (property.type == "Texture")
            {
                Dictionary<string, string> dictionary = PmxBuilder.typeMap.ToDictionary((KeyValuePair<string, string> x) => x.Value, (KeyValuePair<string, string> x) => x.Key);
                if (dictionary.ContainsKey(text))
                {
                    string text2 = dictionary[text];
					if (text2 == "_HGLS") {
						isHair = true;
                    }
                }
            }

        }
    }
}
