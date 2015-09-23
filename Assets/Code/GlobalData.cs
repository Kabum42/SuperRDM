using UnityEngine;
using System.Collections;

public static class GlobalData {

    public static bool started = false;
    public static string OS;
    public static MainCharacter[] agents = new MainCharacter[6];

	// Use this for initialization
	public static void Start () {

        started = true;
        agents[0] = new MainCharacter();

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

	}
	

}
