using UnityEngine;
using System.Collections;

public class MainCharacter : Character {

	private Item EquippedItem;
	private Item MissionItem;
	private Sprite Icon;
	private int CurrentMP;
	private int MaxMP;
	private float Experience;
	private float CurrentFatigue = 0f;
    public NetworkPlayer player;
    public bool IA = false;
    public int currentCell;
    public int sanctuary;
    public GameObject cellChampion;

	public MainCharacter(int ID, string Name, float BaseHealth, float MaxIP, Class OwnClass)
	{
		this.ID = ID; 
		this.Name = Name;
		this.BaseHealth = BaseHealth;
		this.MaxHealth = BaseHealth;
		this.MaxIPBar = MaxIP;
		this.OwnClass = OwnClass;
		this.Experience = 0;
		this.CurrentLevel = 1;

        MaxMP = 2;
        CurrentMP = MaxMP;

		switch (OwnClass.getName ()) {
			case "Boar Ryder":
				this.Aerial = false;
				this.CurrentEffects[0] = new Effect(0, "Anger Management", 0, 0, 0, this.ID);
				break;

			case "Pilumantic":
				this.Aerial = true;
				break;

			default:
				this.Aerial = false;
				break;
		}

		Debug.Log(this.Name + " created");
	}

	public void ApplyEnemyIA(int Attacker, ref Character[] CharactersInBattle){
		int SkillSelected = 0;
		int FocusEnemy = 0;
		switch (OwnClass.getName ()) {
			case "Boar Ryder":
				// Check if all enemies are aerial
				bool AirEnemy = true;
				for (int i = 0; i < CharactersInBattle.Length; i++) {
					if (CharactersInBattle [i] != null) {
						if (CharactersInBattle [i].getBottom () != CharactersInBattle [Attacker].getBottom ()) {
							if (!CharactersInBattle [i].getAerial ()) {
								AirEnemy = false;
								break;
							}
						}
					}
				}

					// If all enemies are aerial > Use Axe Throw to the lowest hp enemy.
				if (AirEnemy) {
					SkillSelected = 1;
					FocusEnemy =  CheckLowestHPEnemy (Attacker, CharactersInBattle);
				}
					// Else use Hack if Anger < 3 or Wild Roar if Anger >= 2
					else {
					if (getStackedNumberEffect ("Anger Management") < 3) {
						SkillSelected = 0;
						FocusEnemy = -1;
					} else {
						SkillSelected = 2;
						FocusEnemy =  CheckLowestHPEnemy (Attacker, CharactersInBattle);
					}
				}
				break;

			case "Pilumantic":

				// Focus Enemy
				int FocusHealth = 0;

				for (int i = 1; i < CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getBottom () != CharactersInBattle[Attacker].getBottom ()){
							if(CharactersInBattle[i].getStackedNumberEffect("Pilosity Stacks") >= CharactersInBattle[FocusEnemy].getStackedNumberEffect("Pilosity Stacks")){
								FocusEnemy = i;
							}
						}
					}
				}

				FocusHealth = CheckLowestHPEnemy(Attacker, CharactersInBattle);

				if (FocusHealth != FocusEnemy){
					if(CharactersInBattle[FocusHealth].getStackedNumberEffect("Pilosity Stacks") == CharactersInBattle[FocusEnemy].getStackedNumberEffect("Pilosity Stacks")){
						FocusEnemy = FocusHealth;
					}
				}

				for (int i = 1; i < CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if(CharactersInBattle[i].getStackedNumberEffect("Pilosity Stacks") > 0 && i != FocusEnemy){
							SkillSelected = 2;
						}
					}
				}

				if (SkillSelected != 2){
					if (CharactersInBattle[FocusEnemy].getStackedNumberEffect("Pilosity Stacks") != 5){
						SkillSelected = 0;
					}
					else {
						SkillSelected = 1;
					}
				}

				break;

			case "Dreamwalker":
				
				// Number of Enemies and check Daymare 
				bool Daymare = false;
				int Enemies = 0;
                FocusEnemy = CheckLowestHPEnemy(Attacker, CharactersInBattle);
				for (int i = 0; i < CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getBottom () != CharactersInBattle[Attacker].getBottom ()){
							Enemies += 1;
							if (CharactersInBattle[i].getDurationEffect("Pierce Stacks") > 0){
								Daymare = true;
								FocusEnemy = i;
							}
						}
					}
				}

				if (Enemies > 1){
					if (Daymare){
						SkillSelected = 0;
					}
					else {
						SkillSelected = 1;
					}
				}
				else {
					if (CharactersInBattle[FocusEnemy].getDurationEffect("DeepDream Effect") > 0){
						if (Daymare){
							SkillSelected = 0;
						}
						else {
							SkillSelected = 1;
						}
					}
					else {
						SkillSelected = 2;
					}
				}
				break;

			case "Henmancer":
				
				// Check number of chickens in battle
				int NumberChickens = 0;
				for (int i = 0; i < CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getName() == "Chicken"){
							NumberChickens += 1;
						}
					}
				}

				if (CurrentHealth > 25){
					if (NumberChickens > 2){
						SkillSelected = 2;
						FocusEnemy = CheckLowestHPEnemy(Attacker, CharactersInBattle);
					}
					else {
						SkillSelected = 0;
					}
				}
				else {
					if (NumberChickens > 0){
						SkillSelected = 1;
					}
					else {
						SkillSelected = 0;
					}
				}
				break;

			case "Disembodied":
				FocusEnemy = CheckLowestHPEnemy(Attacker, CharactersInBattle);
                if (CharactersInBattle[FocusEnemy].getCurrentHealth() <= (20 / CharactersInBattle[FocusEnemy].getBaseHealth()) * CharactersInBattle[FocusEnemy].getMaxHealth())
                {
					SkillSelected = 1;
				}
				else {
					if (getDurationEffect("Dark Pact Effect") > 0){
						if (CharactersInBattle[FocusEnemy].getDurationEffect("Haunt Effect") > 0){
							SkillSelected = 1;
						}
						else {
							SkillSelected = 0;
						}
					}
					else {
                        if (getCurrentHealth() < (40 / BaseHealth) * MaxHealth)
                        {
							if (CharactersInBattle[FocusEnemy].getDurationEffect("Haunt Effect") > 0){
								SkillSelected = 1;
							}
							else {
								SkillSelected = 0;
							}
						}
						else {
							SkillSelected = 2;
						}
					}
				}
				break;
		}
		UseSkill (SkillSelected, Attacker, ref CharactersInBattle, FocusEnemy);
	}
		
	private int CheckLowestHPEnemy(int Attacker, Character[] CharactersInBattle){
		int LowestHPEnemy = -1;
		for (int i = 0; i < CharactersInBattle.Length; i++){
			if (CharactersInBattle[i] != null){
				if (LowestHPEnemy == -1 && CharactersInBattle[i].getBottom() != CharactersInBattle[Attacker].getBottom ()){
					LowestHPEnemy = i;
				}
				else {
					if (CharactersInBattle[i].getBottom() != CharactersInBattle[Attacker].getBottom ()){
						if (CharactersInBattle[i].getName () != "Chicken"){
							if (CharactersInBattle[i].getCurrentHealth() < CharactersInBattle[LowestHPEnemy].getCurrentHealth()){
								LowestHPEnemy = i;
							}
						}
					}
				}
			}
		}
		return LowestHPEnemy;
	}

	public void setExperience(float Experience){
		this.Experience += Experience;
		CheckLevel();
	}


	private void CheckLevel(){
		for (int i = 0; i<GlobalData.ExperienceEachLevel.Length; i++) {
			if (Experience < GlobalData.ExperienceEachLevel[i]){
				setCurrentLevel(i+1);
				break;
			}
		}
		Debug.Log ("Level: " + this.CurrentLevel + "\n" + "Experience: " + this.Experience);

	}

	public void addEffect(string Effect, ref Character[] CharactersInBattle, int CharacterAffected, int IDCreator){
		switch (Effect) {
			case "Pilosity Stacks":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						CharactersInBattle[i].newEffect(Effect, 0, 0, 0, IDCreator);
					}
				}
				break;

			case "Pierce Stacks":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getBottom() != this.getBottom()){
							CharactersInBattle[i].newEffect(Effect, 1, 0, 0, IDCreator);
						}
					}
				}
				break;

			case "DeepDream Effect":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getBottom() != this.getBottom()){
							CharactersInBattle[i].newEffect(Effect, 0, 0, 0, IDCreator);
						}
					}
				}
				break;

			case "Haunt Effect":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getBottom() != this.getBottom()){
							CharactersInBattle[i].newEffect(Effect, 0, 0, 5, IDCreator);
						}
					}
				}
				break;

			case "Dark Pact Effect":
				CharactersInBattle[CharacterAffected].newEffect(Effect, 0, 0, -10, IDCreator);
				break;
		}
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
		MaxMP = 2 + (int) Mathf.Floor(getCurrentLevel() / 2);
        return MaxMP;
    }

	public void Interact()
	{

	}

    public int getCurrentLevel()
    {
        return this.CurrentLevel;
    }

    public float getCurrentFatigue()
    {
        return CurrentFatigue;
    }


    public void setCurrentFatigue(float newFatigue)
    {
        this.CurrentFatigue = Mathf.Clamp(newFatigue, 0f, 1f);
    }


	
	
}
