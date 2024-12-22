# KKBP Exporter
KKBP_Exporter V4.30 decompiled. Original plugin by MediaMoots.  

This plugin allows you to export a Koikatsu card as a .pmx model. The exported data can be imported to Blender with the [KKBP Importer](https://github.com/FlailingFog/KK-Blender-Porter-Pack). You can also import the pmx file to Blender with MMD tools and use the png and json files the exporter generates to manually set the character up yourself.

![](https://raw.githubusercontent.com/FlailingFog/flailingfog.github.io/master/assets/images/exportpanel.png)


## Building the KKBP exporter

1. Download Visual Studio 2022 Community edition
2. Install .net desktop development workload for C#
3. Install .net 4.6 pack
4. Open the KKBP_Exporter.sln
5. Select Build Configuration at the top. Use NET35 to build for Koikatsu. Use NET46 to build for Koikatsu Sunshine
 ![image](https://github.com/user-attachments/assets/8eb2726a-8df4-466e-90dc-6f0da47c2409)
6. Right click on the KKBP folder in the solution explorer and choose Build
![image](https://github.com/user-attachments/assets/2e4c6213-e82b-4637-ae5e-c7d4c62b2ec0)
7. Built .dll file will be in /KKBP_Exporter/bin/NET35/ or /KKBP_Exporter/bin/NET46/
8. Place this file in /your_game_folder/bepinex/plugins/ and run the game.
9. Load the character maker and enable KKBP in the sidebar.  
![](https://raw.githubusercontent.com/FlailingFog/flailingfog.github.io/master/assets/images/exportpanel2.png)

## More Koikatsu exporters
* [KKPMX](https://github.com/CazzoPMX/KKPMX)
* [Koikatsu Pmx Exporter (Reverse Engineered & updated)](https://github.com/Snittern/KoikatsuPmxExporterReverseEngineered)
* [Grey's mesh exporter for Koikatsu](https://github.com/FlailingFog/KK-Blender-Porter-Pack/tree/9fcef4127ba56b4e8e8718fb546945fc00eaaad9/GME)
