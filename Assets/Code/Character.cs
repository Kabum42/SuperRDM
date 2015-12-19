using UnityEngine;
using System.Collections;

public class Character {

	protected int ID;
	protected string Name;
	protected float BaseHealth;
	protected float PreviousHealth;
	protected float CurrentHealth;
	protected float MaxHealth;
	protected float StunIPBar;
	protected float ProgressIPBar;
	protected float MaxIPBar;
	protected int CurrentLevel;
	protected Effect[] CurrentEffects = new Effect[6];
	protected bool Aerial;
	protected Class OwnClass;
	protected int LastSkillUsed;
    protected int lastEnemyAttacked;
	protected bool Bottom;

	public Character()
	{
		Debug.Log("Character created");
	}

	public void UseSkill(int SkillSelected, int Attacker, ref Character[] CharactersInBattle, int EnemyFocused)
	{
		OwnClass.UseSkill (SkillSelected, Attacker, ref CharactersInBattle, EnemyFocused);
		LastSkillUsed = SkillSelected;
	}

	public Skill getSkill(int position){
		return OwnClass.getSkill (position);
	}

	public bool CheckEnemies(int position){
		return OwnClass.CheckEnemiesSkill (position);
	}

	// Effects

	public void UpdateEffects(int CharacterTurn, ref Character[] CharactersInBattle, int CharacterSelected){
		for (int i = 0; i<CurrentEffects.Length; i++){
			if (CurrentEffects[i] != null){
				if (CurrentEffects[i].getDuration() != 0){
					CurrentEffects[i].Update(CharacterTurn, ref CharactersInBattle, CharacterSelected);
				}
			}
		}
	}

	public int getStackedNumberEffect(string Name){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position] != null){
				if (CurrentEffects[position].getName() == Name){
					return CurrentEffects[position].getStackedNumber(); 
				}
			}
		}

		return -1;
	}

	public void setStackedNumberEffect(string Name, int StackedNumber){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position] != null){
				if (CurrentEffects[position].getName() == Name){
					CurrentEffects[position].setStackedNumber(StackedNumber); 
				}
			}
		}
	}

	public void newEffect(string Name, int StackedNumber, int Duration, int DamageValue, int IDCreator){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position] == null){
				CurrentEffects[position] = new Effect(position, Name, StackedNumber, Duration, DamageValue, IDCreator);
				break;
			}
		}
	}

	public int getDurationEffect(string Name){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position] != null){
				if (CurrentEffects[position].getName() == Name){
					return CurrentEffects[position].getDuration();
				}
			}
		}
		return -1;
	}

	public void setDurationEffect(string Name, int Duration){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position] != null){
				if (CurrentEffects[position].getName() == Name){
					CurrentEffects[position].setDuration(Duration); 
				}
			}
		}
	}

	public bool IsNotStun(){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position] != null){
				if(CurrentEffects[position].Stun()){
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckRevive(){
		switch (OwnClass.getName ()) {
			case "Disembodied":
				if (getDurationEffect("Dark Pact Effect")>0){
					setCurrentHealth(25);
					setProgressIPBar(100);
					return false;
				}
				break;
		}
		return true;
	}

	// Getters and Setters

    public Skill getLastSkillUsed()
    {
        return OwnClass.getSkill(LastSkillUsed);
    }

    public void setLastEnemyAttacked(int position)
    {
        lastEnemyAttacked = position;
    }

    public int getLastEnemyAttacked()
    {
        return lastEnemyAttacked;
    }

	public int getID(){
		return this.ID;
	}

	public string getName(){
		return this.Name;
	}
	
	public void setProgressIPBar(float number){
		if (IsNotStun ()) {
			this.ProgressIPBar = number;
		}
	}

	public float getProgressIPBar(){
		return this.ProgressIPBar;
	}

	public float getMaxIPBar(){
		return this.MaxIPBar;
	}

	public void setCurrentHealth(float number){
		this.CurrentHealth = number;
	}

	public float getCurrentHealth(){
		return this.CurrentHealth;
	}

	public float getMaxHealth(){
		return this.MaxHealth;
	}

	public bool getAerial(){
		return this.Aerial;
	}

	public void setBottom(bool newBottom){
		this.Bottom = newBottom;
	}

	public bool getBottom(){
		return Bottom;
	}

	public float getPreviousHealth(){
		return PreviousHealth;
	}

	public void setPreviousHealth(float PreviousHealth){
		this.PreviousHealth = PreviousHealth;
	}

	public Class getOwnClass() {
		return OwnClass;
	}

	public Effect[] getCurrentEffects(){
		return CurrentEffects;
	}

	public void setCurrentLevel(int Level){
		this.CurrentLevel = Level;
		MaxHealth = BaseHealth * (Mathf.Pow (GlobalData.LevelModifier, (float)Level));
	}

	public int getCurrentLevel(){
		return CurrentLevel;
	}
}
