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
		int StackedNumber;
		switch (this.Name) {

			// Boar Ryder abilities
			case "Hack":
				StackedNumber = CharactersInBattle[Attacker].getStackedNumberEffect("Anger Management");
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (!CharactersInBattle[i].getAerial() && (CharactersInBattle[Attacker].getBottom() != CharactersInBattle[i].getBottom ())){
							Debug.Log ("Defender: " + CharactersInBattle [i].getName ());
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth()- (Damage + 4*StackedNumber));
							CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
							break;
						}
					}
				}
				if (StackedNumber < 5){
					CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber+1);
				}
				break;

			case "Axe Throw":
				StackedNumber = CharactersInBattle[Attacker].getStackedNumberEffect("Anger Management");
				CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- (Damage + 4*StackedNumber));
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				if (StackedNumber < 5){
					CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber+1);
				}
				break;

			case "Wild Roar":
				StackedNumber = CharactersInBattle[Attacker].getStackedNumberEffect("Anger Management");
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[Attacker].getBottom() != CharactersInBattle[i].getBottom ()){
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth()- ((Damage*StackedNumber) + 4*StackedNumber));
							CharactersInBattle[i].setProgressIPBar(CharactersInBattle[i].getProgressIPBar() - (StackedNumber*20));
						}
					}
				}
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber/2);
				break;

			//


		}
	}

	public bool getNeedEnemy(){
		return NeedEnemy;
	}

	public string getName(){
		return this.Name;
	}

    public int getID()
    {
        return this.ID;
    }
}

