using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using KK_Plugins;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;
using UniRx;
using UnityEngine;

[BepInPlugin(GUID, "KKBP_Exporter", Version)]
public class PmxExpoter : BaseUnityPlugin
{
	private PmxBuilder builder;
	private KKBPExporterConfig exportConfig = new KKBPExporterConfig();
	public ConfigEntry<bool> visibilityPref;

    public const string GUID = "kkbp.kkbpexporter";
    public const string Version = "4.36";
	public static PmxExpoter Instance;

    private void Awake()
    {
		//Default to true the first time the plugin is installed. Every other time will read from the config file located in /Koikatsu/bepinex/config/kkbp.kkbpexporter.cfg
        visibilityPref = Config.Bind("Visibility", "Show the KKBP exporter by default", false,
            "Show the KKBP exporter by default when entering the character maker");
    }

    private void RegisterCustomControls(RegisterCustomControlsEvent callback)
    {

        var toggle = callback.AddSidebarControl(new SidebarToggle("Show KKBP Exporter", visibilityPref.Value, Instance));
        var observer = Observer.Create<bool>(b => { toggleEnable(b); });
        toggle.ValueChanged.Subscribe(observer);
        PmxExporterGUI.OnEnabledChanged = v => toggle.Value = v;
		MakerAPI.MakerExiting += (sender, args) => PmxExporterGUI.OnEnabledChanged = null;
    }

	private void toggleEnable(bool b)
	{
		//Keep track of the GUI visibility in the config file
        visibilityPref.BoxedValue = b;

		//then toggle the visibility
        if (b) PmxExporterGUI.OnEnable(); else PmxExporterGUI.OnDisable();
    }

    private void OnEarlyMakerFinishedLoading(object sender, RegisterCustomControlsEvent e)
    {
        RegisterCustomControls(e);
    }

    private void Start()
    {
        MakerAPI.MakerBaseLoaded += OnEarlyMakerFinishedLoading;
        MakerAPI.MakerExiting += OnMakerExiting;
	}

    private void OnMakerExiting(object sender, EventArgs e)
	{
		return;
	}

	private void OnGUI()
	{
		if (MakerAPI.InsideAndLoaded)
		{
			if (PmxExporterGUI.optionsEnabled)
			{
                float num = 80f;
                float num2 = 90f;
                float num3 = 50f;
                float num4 = 80f;
                float num5 = 70f;
                float num6 = 170f;
				float horizontal_offset = 60f * 4f;
                //GUI.Box(new Rect((float)Screen.width / 2f - num2, 0f, num2 * 2f, 60f), "");
                //openOptions = GUI.Toggle(new Rect((float)Screen.width / 2f - num3, 35f, num3 * 2f, 30f), openOptions, "Show Options");
                GUI.Box(new Rect((float)Screen.width / 2f - num6 + horizontal_offset, num5, num6 * 2f, 120f), "");
				exportConfig.exportAllOutfits = GUI.Toggle(new Rect((float)Screen.width / 2f - num4 * 2f + horizontal_offset, num5 + 5f, num4 * 2f, 30f), exportConfig.exportAllOutfits, "Export All Outfits");
				exportConfig.exportWithEnabledShapekeys= GUI.Toggle(new Rect((float)Screen.width / 2f + horizontal_offset, num5 + 5f, num4 * 2f, 30f), exportConfig.exportWithEnabledShapekeys, "Freeze Shapekeys");
				exportConfig.exportAllVariations = GUI.Toggle(new Rect((float)Screen.width / 2f - num4 * 2f + horizontal_offset, num5 + 35f, num4 * 2f, 30f), exportConfig.exportAllVariations, "Export Variations");
				exportConfig.exportWithoutPhysics = GUI.Toggle(new Rect((float)Screen.width / 2f + horizontal_offset, num5 + 35f, num4 * 2f, 30f), exportConfig.exportWithoutPhysics, "Export Without Physics");
				exportConfig.exportWithPushups = GUI.Toggle(new Rect((float)Screen.width / 2f - num4 * 2f + horizontal_offset, num5 + 65f, num4 * 2f, 30f), exportConfig.exportWithPushups, "Enable Pushups");
				exportConfig.exportHitBoxes = GUI.Toggle(new Rect((float)Screen.width / 2f + horizontal_offset, num5 + 65f, num4 * 2f, 30f), exportConfig.exportHitBoxes, "Export Hit Meshes");
				exportConfig.exportCurrentPose = GUI.Toggle(new Rect((float)Screen.width / 2f - num4 + horizontal_offset, num5 + 95f, num4 * 2f, 30f), exportConfig.exportCurrentPose, "Freeze Current Pose");

				if (GUI.Button(new Rect((float)Screen.width / 2f - num + horizontal_offset, 0, num * 2f, 60f), "Export Model for KKBP") && builder == null)
				{
					builder = new PmxBuilder
					{
						exportAll = exportConfig.exportAllVariations,
						exportHitBoxes = exportConfig.exportHitBoxes,
						exportWithEnabledShapekeys = exportConfig.exportWithEnabledShapekeys,
						exportCurrentPose = exportConfig.exportCurrentPose
					};
					StartCoroutine(StartBuild());
				}
			}
		}
	}

    private IEnumerator StartBuild()
	{
		DateTime startDateTime = DateTime.Now;
		ChaControl chaControl = MakerAPI.GetCharacterControl();
		Pushup.PushupController pushupController = MakerAPI.GetCharacterControl().gameObject.GetComponent<Pushup.PushupController>();
		PmxBuilder.pmxFile = null;
		builder.CreateBaseSavePath();
		builder.ChangeAnimations();
		builder.CollectIgnoreList();
		builder.CreateCharacterInfoData();
		builder.ExportDataToJson(exportConfig, "KK_KKBPExporterConfig.json");
		yield return null;
		int num = ((!exportConfig.exportAllOutfits) ? chaControl.fileStatus.coordinateType : 0);
		int maxCoord = (exportConfig.exportAllOutfits ? chaControl.chaFile.coordinate.Length : (num + 1));
		PmxBuilder.minCoord = num;
		PmxBuilder.maxCoord = maxCoord;
		bool braPushupBackup = false;
		bool topPushupBackup = false;
		for (int i = num; i < maxCoord + 1; i++)
		{
			yield return null;
			if (i < maxCoord)
			{
				chaControl.ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)i);
			}
			chaControl.SetClothesState(7, 1);
			yield return new WaitForSeconds(0.1f);
			chaControl.SetClothesState(7, 0);
			if (!exportConfig.exportWithPushups && pushupController != null)
			{
				braPushupBackup = pushupController.CurrentBraData.EnablePushup;
				topPushupBackup = pushupController.CurrentTopData.EnablePushup;
				pushupController.CurrentBraData.EnablePushup = false;
				pushupController.CurrentTopData.EnablePushup = false;
				pushupController.RecalculateBody();
				yield return new WaitForSeconds(2f);
			}
			if (!exportConfig.exportWithoutPhysics)
			{
				yield return new WaitForSeconds(2f);
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return null;
				chaControl.resetDynamicBoneAll = true;
				yield return null;
			}
			PmxBuilder.nowCoordinate = i;
			yield return StartCoroutine(builder.BuildStart_OG());
			if (!exportConfig.exportWithPushups && pushupController != null)
			{
				pushupController.CurrentBraData.EnablePushup = braPushupBackup;
				pushupController.CurrentTopData.EnablePushup = topPushupBackup;
				pushupController.RecalculateBody();
			}
		}
		if (exportConfig.exportCurrentPose)
		{
			chaControl.animBody.speed = 1f;
		}
		builder.ExportAllDataLists();
		builder.OpenFolderInExplorer(builder.baseSavePath);
		builder = null;
		Console.WriteLine("KKBP Exporter finished in: " + (DateTime.Now - startDateTime).TotalSeconds + " seconds");
	}
}


internal sealed class PmxExporterGUI : MonoBehaviour
{
	private static PmxExporterGUI _instance;

	public static Action<bool> OnEnabledChanged;

	public static bool optionsEnabled = false;

    public static void OnEnable()
    {
		optionsEnabled = true;
        OnEnabledChanged?.Invoke(true);
    }

    public static void OnDisable()
    {
        optionsEnabled = false;
        OnEnabledChanged?.Invoke(false);
    }
}