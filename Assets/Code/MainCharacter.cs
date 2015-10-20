using UnityEngine;
using System.Collections;

public class MainCharacter : Character {

	private Item EquippedItem;
	private Item MissionItem;
	private Sprite Icon;
	private int CurrentMP;
	private int MaxMP;
	private int Experience;
    public NetworkPlayer player;
    public bool IA = false;
    public int currentCell;
    public GameObject cellChampion;

	public MainCharacter(int ID, string Name, float MaxHealth, float MaxIP, Class OwnClass)
	{
		this.ID = ID; 
		this.Name = Name;
		this.MaxHealth = MaxHealth;
		this.MaxIPBar = MaxIP;
		this.OwnClass = OwnClass;

		switch (OwnClass.getName ()) {
			case "Boar Ryder":
				this.Aerial = false;
				break;
		}

		Debug.Log(this.Name + " created");
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
