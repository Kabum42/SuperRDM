using UnityEngine;
using System.Collections;

public class Effect {
	
	private int ID;
	private string Name;
	private int IDCreator;
	private int StackedNumber;
	private int Duration;
	private int DamageValue;
	
	public Effect(int ID, string Name, int StackedNumber, int Duration, int DamageValue, int IDCreator)
	{
		this.ID = ID;
		this.Name = Name;
		this.StackedNumber = StackedNumber;
		this.Duration = Duration;
		this.DamageValue = DamageValue;
		this.IDCreator = IDCreator;
	}
	
	public void Update(int CharacterTurn, ref Character[] CharactersInBattle, int CharacterSelected){
		switch (this.Name) {
			case "Pierce Stacks":
				if (CharactersInBattle[CharacterTurn].getID() == IDCreator){
					this.Duration -= 1;
					if (this.Duration == 0){
						this.StackedNumber = 1;
					}
				}
				break;

			case "DeepDream Effect":
				if (CharactersInBattle[CharacterTurn].getID() == IDCreator){
					float auxLifeIncreased = (CharactersInBattle[CharacterSelected].getMaxHealth() - CharactersInBattle[CharacterSelected].getCurrentHealth())/2;
					CharactersInBattle[CharacterSelected].setCurrentHealth(CharactersInBattle[CharacterSelected].getCurrentHealth()+auxLifeIncreased);
					this.Duration -= 1;
				}
				break;
		}
	}

	public bool Stun(){
		switch (this.Name) {
			case "DeepDream Effect":
				if (this.Duration != 0){
					return true;
				}
				else {
					return false;
				}
				break;

			default:
				return false;
				break;
		}
	}

	// Getters and Setters

	public string getName(){
		return this.Name;
	}

	public void setStackedNumber(int StackedNumber){
		this.StackedNumber = StackedNumber;
	}

	public int getStackedNumber(){
		return StackedNumber;
	}

	public int getDuration(){
		return this.Duration;
	}

	public void setDuration(int Duration){
		this.Duration = Duration;
	}

	public void setIDCreator (int ID){
		this.IDCreator = ID;
	}

	public int getIDCreator(){
		return this.IDCreator;
	}
}



