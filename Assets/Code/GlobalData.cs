using UnityEngine;
using System.Collections;

public static class GlobalData {

    public static bool started = false;
	// public static Events[] Event;
	public static string[] NameItems;
	public static int MaxEnemies;
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
        biomeNames[(int) Biome.Sanctuary] = "Sanctuary";
		biomeNames[(int) Biome.Prairie] = "Prairie";
		biomeNames[(int) Biome.Forest] = "Forest";
		biomeNames[(int) Biome.Swamp] = "Swamp";
		biomeNames[(int) Biome.Lake] = "Lake";
		biomeNames[(int) Biome.Desert] = "Desert";
		biomeNames[(int) Biome.Mountain] = "Mountain";
		biomeNames[(int) Biome.TheEvil] = "TheEvil";
		biomeNames[(int) Biome.Harbour1] = "Dock_1";
		biomeNames[(int) Biome.Harbour2] = "Dock_2";
		biomeNames[(int) Biome.Harbour3] = "Dock_3";

	}
	

}
