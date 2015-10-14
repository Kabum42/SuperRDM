using UnityEngine;
using System.Collections;

public class MainCharacter : Character {

	private Item EquippedItem;
	private Item MissionItem;
	private Sprite Icon;
	private int CurrentMP;
	private int MaxMP;
	private int Experience;
	private int ID;
    public NetworkPlayer player;
    public bool IA = false;
    public int currentCell;
    public GameObject cellChampion;

	public MainCharacter()
	{
		Debug.Log("MainCharacter created");
	}

    public int getCurrentSteps()
    {
        return 6;
    }

	public void Interact()
	{

	}

	public void Death(){

	}
	
}
