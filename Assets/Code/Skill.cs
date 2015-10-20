using UnityEngine;
using System.Collections;

public class Skill {

	private int ID;
	private string Name;
	private float CostIP;
	private float CostHealth;
	private int TargetID;
	private int[] AgentID = new int[100];
	private float Damage;
	private bool NeedEnemy;

	public Skill(int ID, string Name, float CostIP, float CostHealth, float Damage, bool NeedEnemy)
	{
		this.ID = ID;
		this.Name = Name;
		this.CostIP = CostIP;
		this.CostHealth = CostHealth;
		this.Damage = Damage;
		this.NeedEnemy = NeedEnemy;
		Debug.Log("Skill " + this.Name + " created");
	}

	public void Activate(int Attacker, ref Character[] CharactersInBattle, int EnemyFocused)
	{
		Debug.Log ("Attacker: " + CharactersInBattle [Attacker].getName ());
		switch (this.Name) {
			case "Hack":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (!CharactersInBattle[i].getAerial() && (CharactersInBattle[Attacker].getBottom() != CharactersInBattle[i].getBottom ())){
							Debug.Log ("Defender: " + CharactersInBattle [i].getName ());
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth()- Damage);
							CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
							break;
						}
					}
				}
				break;

			case "Axe Throw":
				CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- Damage);
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				break;

			case "Wild Roar":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[Attacker].getBottom() != CharactersInBattle[i].getBottom ()){
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth()- Damage);
						}
					}
				}
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);	
				break;
		}
	}

	public bool getNeedEnemy(){
		return NeedEnemy;
	}

	public string getName(){
		return this.Name;
	}
}

