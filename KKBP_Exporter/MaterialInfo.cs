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
			string text = "_" + property.name;
			if (property.type == "Color")
			{
				ShaderPropNames.Add(text + " " + property.type + " " + ShaderPropColorValues.Count());
				ShaderPropColorValues.Add(material.GetColor(text));
			}
		}
	}
}
