using System;
using System.Collections.Generic;

[Serializable]
internal class CharacterInfoData
{
	public int Personality;

	public float VoiceRate;

	public float PupilWidth;

	public float PupilHeight;

	public float PupilX;

	public float PupilY;

	public float HlUpY;

	public float HlDownY;

	public List<float> ShapeInfoFace = new List<float>();

	public List<float> ShapeInfoBody = new List<float>();

	public float eyeOpenMax;

	public float eyebrowOpenMax;

	public float mouthOpenMax;

	public List<float> pupilOffset;

	public List<float> pupilScale;

	public List<float> highlightUpOffset;

	public List<float> highlightUpScale;

    public List<float> highlightDownOffset;

    public List<float> highlightDownScale;

	public List<float> eyeRotation;

	public List<float> highlightUpColor;

	public List<float> highlightDownColor;
}
