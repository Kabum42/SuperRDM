using UnityEngine;
using System.Collections;

public static class GlobalData {

    public static bool started = false;
    public static string OS;
    public static MainCharacter[] agents = new MainCharacter[6];
    public static int[] order;
    public static int currentAgentTurn = 0;
    public static int activeAgents = 0;
    public static Color color0 = new Color(1f, 0.62f, 0.62f);
    public static Color color1 = new Color(0.62f, 1f, 0.62f);
    public static Color color2 = new Color(0.62f, 0.62f, 1f);
    public static Color color3 = new Color(0.62f, 1f, 1f);
    public static Color color4 = new Color(1f, 0.62f, 1f);
    public static Color color5 = new Color(1f, 1f, 0.62f);
    public static Color[] colorCharacters = {color0, color1, color2, color3, color4, color5};
    public static string[] biomeNames;

    public static bool online = false;
    public static bool connected = false;
    public static float boardSeed = 42f;

    // Use this for initialization
    public static void Start () {

        started = true;

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
        {
            OS = "Windows";
        }
        else if (Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
        {
            OS = "Mac";
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer)
        {
            OS = "Linux";
        }

        biomeNames = new string[11];
        biomeNames[Biome.Sanctuary] = "Sanctuary";
        biomeNames[Biome.Prairie] = "Prairie";
        biomeNames[Biome.Forest] = "Forest";
        biomeNames[Biome.Swamp] = "Swamp";
        biomeNames[Biome.Lake] = "Lake";
        biomeNames[Biome.Desert] = "Desert";
        biomeNames[Biome.Mountain] = "Mountain";
        biomeNames[Biome.TheEvil] = "TheEvil";
        biomeNames[Biome.Dock_1] = "Dock_1";
        biomeNames[Biome.Dock_2] = "Dock_2";
        biomeNames[Biome.Dock_3] = "Dock_3";

	}
	

}
