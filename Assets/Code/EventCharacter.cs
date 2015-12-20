using UnityEngine;
using System.Collections;

public class EventCharacter : Character {

	// private Type OwnType;
	private Biome OwnBiome; 	
	public EventCharacter(int ID, string Name, float BaseHealth, float MaxIP, Class OwnClass, Biome OwnBiome)
	{
		this.ID = ID; 
		this.Name = Name;
		this.BaseHealth = BaseHealth;
		this.MaxHealth = BaseHealth;
		this.MaxIPBar = MaxIP;
		this.OwnClass = OwnClass;
		this.OwnBiome = OwnBiome;
		this.CurrentLevel = 1;

		switch (OwnClass.getName()) {
			case "Wolf":
				this.Aerial = false;
				break;
		}

	}

	public Biome getBiome(){
		return this.OwnBiome;
	}

	public void SimpleIA(int Attacker, ref Character[] CharactersInBattle){
		switch (OwnClass.getName ()) {
			case "Chicken":
				CharactersInBattle[Attacker].setProgressIPBar(1);
				break;

			default:
				CharactersInBattle [Attacker].UseSkill (0, Attacker, ref CharactersInBattle, 0);
				break;
		}

	}
	
}
