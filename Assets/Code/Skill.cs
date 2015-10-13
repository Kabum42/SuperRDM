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

	public Skill(int ID, string Name, float CostIP, float CostHealth, float Damage)
	{
		this.ID = ID;
		this.Name = Name;
		this.CostIP = CostIP;
		this.CostHealth = CostHealth;
		this.Damage = Damage;
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
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth()-this.Damage);
							CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
							break;
						}
					}
				}
				break;
		}
	}
}

