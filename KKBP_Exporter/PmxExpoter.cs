using ADV.Commands.Object;
using BepInEx;
using BepInEx.Configuration;
using KK_Plugins;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;
using System;
using System.Collections;
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
		//Default to false the first time the plugin is installed. Every other time will read from the config file located in /Koikatsu/bepinex/config/kkbp.kkbpexporter.cfg
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
                float box_y = 60f;
                float box_x = (float)Screen.width / 2f + 60f;
                float box_width = 300f;
                float box_height = 90f;
                float button_x = box_x + box_width / 4f;
                float button_width = box_width / 2f;
                float button_height = box_height / 2f;

                float col_a = box_x + 5f;
                float col_b = box_x + box_width / 2 - 5f;
                float row_a = box_y + 5f;
                float row_b = box_y + box_height * 1 / 3 + 5f;
                float row_c = box_y + box_height * 2 / 3 + 5f;
                float toggle_width = box_width * 5 / 11;
                float toggle_height = box_height / 3f;

                GUI.Box(new Rect(box_x, box_y, box_width, box_height), "");
                exportConfig.exportAllOutfits =				GUI.Toggle(new Rect(col_a, row_a, toggle_width, toggle_height), exportConfig.exportAllOutfits, "Export All Outfits");
                exportConfig.exportAllVariations =			GUI.Toggle(new Rect(col_a, row_b, toggle_width, toggle_height), exportConfig.exportAllVariations, "Export Variations");
                exportConfig.exportHitBoxes =				GUI.Toggle(new Rect(col_a, row_c, toggle_width, toggle_height - 5f), exportConfig.exportHitBoxes, "Export Hit Meshes");

                exportConfig.exportCurrentPose =			GUI.Toggle(new Rect(col_b, row_a, toggle_width, toggle_height), exportConfig.exportCurrentPose, "Freeze Current Pose");
                exportConfig.exportWithEnabledShapekeys =	GUI.Toggle(new Rect(col_b, row_b, toggle_width, toggle_height), exportConfig.exportWithEnabledShapekeys, "Freeze Shapekeys");
                exportConfig.exportWithoutPhysics =			GUI.Toggle(new Rect(col_b, row_c, toggle_width, toggle_height - 5f), exportConfig.exportWithoutPhysics, "Disable Physics");

                //exportConfig.exportWithPushups =			GUI.Toggle(new Rect(box_x, num5 + 65f, num4 * 2f, 30f), exportConfig.exportWithPushups, "Enable Pushups");
                // exportConfig.exportLightDarkTexture =	GUI.Toggle(new Rect(box_x, num5 + 95f, num4 * 2f, 30f), exportConfig.exportLightDarkTexture, "Export Light Dark Texture");

                if (GUI.Button(new Rect(button_x, 0, button_width, button_height), "Export Model for KKBP") && builder == null)
                {
                    builder = new PmxBuilder
					{
						exportAll = exportConfig.exportAllVariations,
						exportHitBoxes = exportConfig.exportHitBoxes,
						exportWithEnabledShapekeys = exportConfig.exportWithEnabledShapekeys,
						exportCurrentPose = exportConfig.exportCurrentPose,
						exportLightDarkTexture = true //exportConfig.exportLightDarkTexture
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
		yield return new WaitForSeconds(.4f);
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
		}
		if (exportConfig.exportCurrentPose)
		{
			chaControl.animBody.speed = 1f;
		}
		builder.ExportAllDataLists();
		builder.CleanUp();
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
