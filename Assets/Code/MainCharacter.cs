using UnityEngine;
using System.Collections;

public class MainCharacter : Character {

	private Item EquippedItem;
	private Item MissionItem;
	private Sprite Icon;
	private int CurrentMP;
	private int MaxMP;
	private int Experience;
	private int CurrentLevel;
    private float CurrentFatigue;
    public NetworkPlayer player;
    public bool IA = false;
    public int currentCell;
    public int sanctuary;
    public GameObject cellChampion;

	public MainCharacter(int ID, string Name, float MaxHealth, float MaxIP, Class OwnClass)
	{
		this.ID = ID; 
		this.Name = Name;
		this.MaxHealth = MaxHealth;
		this.MaxIPBar = MaxIP;
		this.OwnClass = OwnClass;
		this.Experience = 0;
		this.CurrentLevel = 1;

        MaxMP = 6;
        CurrentMP = MaxMP;

		switch (OwnClass.getName ()) {
			case "Boar Ryder":
				this.Aerial = false;
				this.CurrentEffects[0] = new Effect(0, "Anger Management", 0, -1, 0);
				break;
		}

		Debug.Log(this.Name + " created");
	}

	public void ApplyEnemyIA(int Attacker, ref Character[] CharactersInBattle){
		switch (OwnClass.getName ()) {
			case "Boar Ryder":
				// Check if all enemies are aerial
				bool AirEnemy = true;
				for (int i = 0; i < CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (!CharactersInBattle[i].getAerial()){
							AirEnemy = false;
							break;
						}
					}
				}

				// If all enemies are aerial > Use Axe Throw to the lowest hp enemy.
				if (AirEnemy){
					UseSkill (1, Attacker, ref CharactersInBattle, CheckLowestHPEnemy(Attacker, CharactersInBattle));
				}
				// Else use Hack if Anger < 3 or Wild Roar if Anger >= 2
				else {
					if (getStackedNumberEffect("Anger Management")<3){
						UseSkill (0, Attacker, ref CharactersInBattle, -1);
					}
					else {
						UseSkill (2, Attacker, ref CharactersInBattle, -1);
					}
				}
				break;
		}
	}

	private int CheckLowestHPEnemy(int Attacker, Character[] CharactersInBattle){
		int LowestHPEnemy = -1;
		for (int i = 1; i < CharactersInBattle.Length; i++){
			if (CharactersInBattle[i] != null){
				if (LowestHPEnemy == -1 && CharactersInBattle[i].getBottom() != CharactersInBattle[Attacker].getBottom ()){
					LowestHPEnemy = i;
				}
				else {
					if (CharactersInBattle[i].getBottom() != CharactersInBattle[Attacker].getBottom ()){
						if (CharactersInBattle[i].getCurrentHealth() < CharactersInBattle[LowestHPEnemy].getCurrentHealth()){
							LowestHPEnemy = i;
						}
					}
				}
			}
		}
		return LowestHPEnemy;
	}

	public void setExperience(int Experience){
		this.Experience += Experience;
		CheckLevel();
	}

	private void CheckLevel(){
		if (Experience < 100) {
			CurrentLevel = 1;
		} else if (Experience < 300) {
			CurrentLevel = 2;
		} else if (Experience < 700) {
			CurrentLevel = 3;
		} else if (Experience < 1500) {
			CurrentLevel = 4;
		} else if (Experience < 3100) {
			CurrentLevel = 5;
		} else if (Experience < 6300) {
			CurrentLevel = 6;
		} else if (Experience < 12700) {
			CurrentLevel = 7;
		} else if (Experience < 25500) {
			CurrentLevel = 8;
		} else if (Experience < 51000) {
			CurrentLevel = 9;
		} else if (Experience < 102200) {
			CurrentLevel = 10;
		}
		Debug.Log ("Level: " + this.CurrentLevel + "\n" + "Experience: " + this.Experience);
	}

    public int getCurrentSteps()
    {
        return CurrentMP;
    }

    public void setCurrentSteps(int n)
    {
        CurrentMP = n;
    }

    public int getMaxSteps()
    {
        return MaxMP;
    }

	public void Interact()
	{

	}

    public int getCurrentLevel()
    {
        return CurrentLevel;
    }

    public float getCurrentFatigue()
    {
        return CurrentFatigue;
    }
	
	
}
