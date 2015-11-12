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
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth() - (Damage + 2*StackedNumber));
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
				CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- (Damage + 2*StackedNumber));
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				if (StackedNumber < 5){
					CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber+1);
				}
				break;

			case "Axe Dunk":
				StackedNumber = CharactersInBattle[Attacker].getStackedNumberEffect("Anger Management");
				CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- ((Damage*StackedNumber) + 2*StackedNumber));
				CharactersInBattle[EnemyFocused].setProgressIPBar(CharactersInBattle[EnemyFocused].getProgressIPBar() - (StackedNumber*20));
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber/2);
				break;

			// Pilumantic abilities
			case "Pilosity":
				StackedNumber = CharactersInBattle[EnemyFocused].getStackedNumberEffect("Pilosity Stacks");
				if (StackedNumber < 5){
					CharactersInBattle[EnemyFocused].setStackedNumberEffect("Pilosity Stacks", StackedNumber+1);
				}
				CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- this.Damage);
				CharactersInBattle[EnemyFocused].setProgressIPBar(CharactersInBattle[EnemyFocused].getProgressIPBar() - CharactersInBattle[EnemyFocused].getProgressIPBar()/10);
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				break;

			case "Laser Depilation":
				StackedNumber = CharactersInBattle[EnemyFocused].getStackedNumberEffect("Pilosity Stacks");
				CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- (StackedNumber * Damage));
				if (StackedNumber == 5){
					CharactersInBattle[EnemyFocused].setProgressIPBar(CharactersInBattle[EnemyFocused].getProgressIPBar() - CharactersInBattle[EnemyFocused].getProgressIPBar()/2);
				}	
				CharactersInBattle[EnemyFocused].setStackedNumberEffect("Pilosity Stacks", 0);
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				break;

			case "Fashion Victim":
				StackedNumber = CharactersInBattle[EnemyFocused].getStackedNumberEffect("Pilosity Stacks");
				if (StackedNumber == 0){
					for (int i = 0; i<CharactersInBattle.Length; i++){
						if (CharactersInBattle[i] != null){
							StackedNumber += CharactersInBattle[i].getStackedNumberEffect("Pilosity Stacks");
							CharactersInBattle[i].setStackedNumberEffect("Pilosity Stacks", 0);
						}
					}
				}
				if (StackedNumber > 5){
					StackedNumber = 5;
				}
				CharactersInBattle[EnemyFocused].setStackedNumberEffect("Pilosity Stacks", StackedNumber);
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				break;

			// Dreamwalker abilities

			case "Pierce":
				StackedNumber = CharactersInBattle[EnemyFocused].getStackedNumberEffect("Pierce Stacks");

				if ((CharactersInBattle[EnemyFocused].getMaxHealth()*0.30) > CharactersInBattle[EnemyFocused].getCurrentHealth()){
					StackedNumber += 1;
				}
				if (CharactersInBattle[EnemyFocused].getAerial()){
					if (StackedNumber != 1){
						StackedNumber /= 2;
					}
				}
				if (StackedNumber > 3){
					StackedNumber = 3;
				}	
				
				for (int i = 0; i<StackedNumber; i++){
					CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth()- (StackedNumber * Damage));
				}

				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				break;

			case "Daymare":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						CharactersInBattle[i].setStackedNumberEffect("Pierce Stacks", 1);
						CharactersInBattle[EnemyFocused].setDurationEffect("Pierce Stacks", 0);
					}
				}
				CharactersInBattle[EnemyFocused].setStackedNumberEffect("Pierce Stacks", 3);
				CharactersInBattle[EnemyFocused].setDurationEffect("Pierce Stacks", 4);
				CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
				break;

			case "Deep Dream":
				CharactersInBattle[EnemyFocused].setDurationEffect("DeepDream Effect", 4);
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

    public int getID()
    {
        return this.ID;
    }

	public float getDamage(){
		return this.Damage;
	}
}

