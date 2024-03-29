using UnityEngine;
using System.Collections;

public static class GlobalData {

    public static bool started = false;
    public static GameObject worldObject = null;
	// public static Events[] Event;
	public static string[] NameItems;
	public static int MaxEnemies;
    public static string OS;
    public static MainCharacter[] agents = new MainCharacter[6];
    public static int[] positionCharacterCombat = new int[2];

    public static Biome currentBiome;
    public static int currentSpecialEvent = 2;
    public static int myAgent = 0;
    public static int[] order;
    public static int currentAgentTurn = 0;
    public static int activeAgents = 0;
    public static Color color0 = new Color(1f, 0.62f, 0.62f);
    public static Color color1 = new Color(0.62f, 1f, 0.62f);
    public static Color color2 = new Color(0.25f, 0.5f, 1f);
    public static Color color3 = new Color(0.62f, 1f, 1f);
    public static Color color4 = new Color(1f, 0.62f, 1f);
    public static Color color5 = new Color(1f, 1f, 0.62f);
    public static Color[] colorCharacters = {color0, color1, color2, color3, color4, color5};
    public static string[] biomeNames;
    private static int[] biomeCosts;

	public static EventCharacter[] RandomEnemies = new EventCharacter[25];
	public static Skill[] Skills = new Skill[24];

	public static Class[] Classes = new Class[8];

	public static float[] ExperienceEachLevel = new float[10];
	public static float LevelModifier = 1.25f;

    public static bool online = false;
    public static bool hosting = false;
    public static bool connected = false;
    public static float boardSeed = 42f;

    public static float bossFatigue = 0f;
    public static float crossfadeAnimation = 0.15f;

    public static int eventRon = 0;
    public static int eventDouchebards = 1;
    public static int eventExpert = 2;

    public static int eventFinalBoss = 42;

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
		GenerateSkills ();
		GenerateClasses ();
		GenerateEnemies ();
		GenerateExperienceLevels ();

        biomeCosts = new int[11];
        biomeCosts[(int)Biome.Sanctuary] = 1;
        biomeCosts[(int)Biome.Prairie] = 1;
        biomeCosts[(int)Biome.Forest] = 2;
        biomeCosts[(int)Biome.Swamp] = 3;
        biomeCosts[(int)Biome.Lake] = 42;
        biomeCosts[(int)Biome.Desert] = 4;
        biomeCosts[(int)Biome.Mountain] = 5;
        biomeCosts[(int)Biome.TheEvil] = 6;
        biomeCosts[(int)Biome.Harbour1] = 1;
        biomeCosts[(int)Biome.Harbour2] = 1;
        biomeCosts[(int)Biome.Harbour3] = 1;

        Language.Start();
        //Language.currentLanguage = Language.Spanish;

	}

    public static int getBiomeCost(Biome biome, int position, int agent) {
        
        if (biome == Biome.Sanctuary && position != GlobalData.agents[agent].sanctuary) {
            return 42;
        }
        
        return biomeCosts[(int)biome];
    }

	private static void GenerateSkills(){
		Skills [0] = new Skill (0, "Hack", 40, 0, 20, false);
		Skills [1] = new Skill (1, "Axe Throw", 60, 0, 30, true);
		Skills [2] = new Skill (2, "Axe Dunk", 45, 0, 10, true);
		Skills [3] = new Skill (3, "Pilosity", 20, 0, 10, true);
		Skills [4] = new Skill (4, "Laser Depilation", 40, 0, 10, true);
		Skills [5] = new Skill (5, "Fashion Victim", 50, 0, 0, true);
		Skills [6] = new Skill (6, "Pierce", 30, 0, 10, true);
		Skills [7] = new Skill (7, "Daymare", 30, 0, 0, true);
		Skills [8] = new Skill (8, "Deep Dream", 70, 0, 0, true);
		Skills [9] = new Skill (9, "Summon Hen", 10, 0, 0, false);
		Skills [10] = new Skill (10, "Roast Chicken", 30, 0, 10, false);
		Skills [11] = new Skill (11, "Kamikahen", 20, 0, 20, true);
		Skills [12] = new Skill (12, "Haunt", 30, 15, 20, true);
		Skills [13] = new Skill (13, "Death Kiss", 20, 15, 20, true);
		Skills [14] = new Skill (14, "Dark Pact", 10, 30, 0, false);
		 
		
	}

	private static void GenerateClasses(){
		Classes [0] = new Class (0, "Boar Ryder", Skills [0], Skills [1], Skills [2]);
        Classes [1] = new Class (1, "Pilumantic", Skills [3], Skills [4], Skills [5]);
        Classes [2] = new Class (2, "Dreamwalker", Skills [6], Skills [7], Skills [8]);
        Classes [3] = new Class (3, "Henmancer", Skills [9], Skills [10], Skills [11]);
        Classes [4] = new Class (4, "Disembodied", Skills [12], Skills [13], Skills [14]);
        Classes [5] = new Class (5, "Black Shield", Skills [0], Skills [1], Skills [2]);
		Classes [6] = new Class (6, "Animal", null, null, null);
        Classes [7] = new Class (7, "FinalBoss", null, null, null);
	}


        

	private static void GenerateEnemies (){
		Biome newBiome;
		newBiome = Biome.Forest;

		RandomEnemies [0] = new EventCharacter (7, "Patateitor", 50, 100, Classes [6], Biome.Sanctuary);
		RandomEnemies [1] = new EventCharacter (8, "Patateitor paria", 25, 100, Classes [6], Biome.Sanctuary);
		RandomEnemies [2] = new EventCharacter (9, "Patateitor alfa", 100, 100, Classes [6], Biome.Sanctuary);
        RandomEnemies[3] = new EventCharacter(6, "FinalBoss", 100, 100, Classes[7], Biome.TheEvil);

	}

	private static void GenerateExperienceLevels (){
		float auxExperience = 100;
        ExperienceEachLevel[0] = auxExperience;
		for (int i = 1; i<ExperienceEachLevel.Length; i++) {
			ExperienceEachLevel[i] = ExperienceEachLevel[i-1] + auxExperience * Mathf.Pow(1.10f, i);
            Debug.Log(ExperienceEachLevel[i]);
		}
	}



    public static void Battle()
    {
        Application.LoadLevelAdditive("Battle");
    }

    public static void Event()
    {
        Application.LoadLevelAdditive("Talk");
    }

    public static void World()
    {
        worldObject.SetActive(true);
    }

}
