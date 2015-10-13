using UnityEngine;
using System.Collections;

public class Class {

	private Skill[] Skills = new Skill[3];
	private int ID;
	private string Name;

	public Class(int ID, string Name, Skill FirstSkill, Skill SecondSkill, Skill ThirdSkill)
	{
		this.ID = ID;
		this.Name = Name;
		this.Skills[0] = FirstSkill;
		this.Skills[1] = SecondSkill;
		this.Skills[2] = ThirdSkill;
		Debug.Log("Class " + this.Name + " created");
	}
	
	public void UseSkill(int SkillSelected, int Attacker, ref Character[] CharactersInBattle, int EnemyFocused)
	{
		if (Skills [SkillSelected] != null) {
			Skills [SkillSelected].Activate (Attacker, ref CharactersInBattle, EnemyFocused);
		} 
		else {
			SimpleAttack(Attacker, ref CharactersInBattle, EnemyFocused);
		}
	}

	public void SimpleAttack(int Attacker, ref Character[] CharactersInBattle, int EnemyFocused){
		CharactersInBattle [EnemyFocused].setCurrentHealth (CharactersInBattle [EnemyFocused].getCurrentHealth () - 10);
		CharactersInBattle[Attacker].setProgressIPBar (CharactersInBattle[Attacker].getProgressIPBar () - 20);
	}

	public string getName(){
		return this.Name;
	}
}





