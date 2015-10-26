using UnityEngine;
using System.Collections;

public class Character {

	protected int ID;
	protected string Name;
	protected float PreviousHealth;
	protected float CurrentHealth;
	protected float MaxHealth;
	protected float ProgressIPBar;
	protected float MaxIPBar;
	protected Effect[] CurrentEffects = new Effect[6];
	protected bool Aerial;
	protected Class OwnClass;
	protected int LastSkillUsed;
	protected bool Bottom;

	public Character()
	{
		Debug.Log("Character created");
	}

	public void EndBattleTurn(){
	}

	public void StartBattleTurn(){
	}

	public void EndBattle(){
	}

	public void UseSkill(int SkillSelected, int Attacker, ref Character[] CharactersInBattle, int EnemyFocused)
	{
		OwnClass.UseSkill (SkillSelected, Attacker, ref CharactersInBattle, EnemyFocused);
		LastSkillUsed = SkillSelected;
	}

	public string getSkillName(int position){
		return OwnClass.getSkillName (position);
	}

	public bool CheckEnemies(int position){
		return OwnClass.CheckEnemiesSkill (position);
	}

	public int getStackedNumberEffect(string Name){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position].getName() == Name){
				return CurrentEffects[position].getStackedNumber(); 
			}
		}

		return 0;
	}

	public void setStackedNumberEffect(string Name, int StackedNumber){
		int position;
		for (position = 0; position< CurrentEffects.Length; position++){
			if (CurrentEffects[position].getName() == Name){
				CurrentEffects[position].setStackedNumber(StackedNumber); 
				break;
			}
		}
	}

	public string getLastSkillUsed(){
		return OwnClass.getSkillName (LastSkillUsed);
	}

	// Getters and Setters

	public int getID(){
		return this.ID;
	}

	public string getName(){
		return this.Name;
	}

	public void setProgressIPBar(float number){
		this.ProgressIPBar = number;
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

	
}
