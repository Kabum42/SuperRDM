using UnityEngine;
using System.Collections;

public class MainCharacter : Character {
	
	// private Skill[] Skills = new Skill[3];
	// private Item EquippedItem;
	// private Class OwnClass;
	private int ID;
    public NetworkPlayer player;

	public MainCharacter()
	{
		Debug.Log("MainCharacter created");
	}
	
}
