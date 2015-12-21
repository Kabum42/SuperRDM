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
    private float LastDamage;
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
		int EnemyAttacked = EnemyFocused;
		switch (this.Name) {

			// Boar Ryder abilities
			case "Hack":
				StackedNumber = CharactersInBattle[Attacker].getStackedNumberEffect("Anger Management");
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (!CharactersInBattle[i].getAerial() && (CharactersInBattle[Attacker].getBottom() != CharactersInBattle[i].getBottom ())){
                            LastDamage = (Damage + 2 * StackedNumber);
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getCurrentHealth() - LastDamage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
                            EnemyAttacked = i;
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
                LastDamage = (Damage + 2 * StackedNumber);
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - LastDamage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				if (StackedNumber < 5){
					CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber+1);
				}
				break;

			case "Axe Dunk":
				StackedNumber = CharactersInBattle[Attacker].getStackedNumberEffect("Anger Management");
                LastDamage = ((Damage * StackedNumber) + 2 * StackedNumber);
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth()- LastDamage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				CharactersInBattle[EnemyAttacked].setProgressIPBar(CharactersInBattle[EnemyAttacked].getProgressIPBar() - (StackedNumber*20));
				CharactersInBattle[Attacker].setStackedNumberEffect("Anger Management", StackedNumber/2);
				break;

			// Pilumantic abilities
			case "Pilosity":
				StackedNumber = CharactersInBattle[EnemyAttacked].getStackedNumberEffect("Pilosity Stacks");
                LastDamage = this.Damage;
				if (StackedNumber < 5){
					CharactersInBattle[EnemyAttacked].setStackedNumberEffect("Pilosity Stacks", StackedNumber+1);
				}
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - LastDamage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				CharactersInBattle[EnemyAttacked].setProgressIPBar(CharactersInBattle[EnemyAttacked].getProgressIPBar() - CharactersInBattle[EnemyAttacked].getProgressIPBar()/10);
				break;

			case "Laser Depilation":
				StackedNumber = CharactersInBattle[EnemyAttacked].getStackedNumberEffect("Pilosity Stacks");
                LastDamage = StackedNumber * Damage;
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth()- LastDamage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				if (StackedNumber == 5){
					CharactersInBattle[EnemyAttacked].setProgressIPBar(CharactersInBattle[EnemyAttacked].getProgressIPBar() - CharactersInBattle[EnemyAttacked].getProgressIPBar()/2);
				}	
				CharactersInBattle[EnemyAttacked].setStackedNumberEffect("Pilosity Stacks", 0);
				break;

			case "Fashion Victim":
				StackedNumber = CharactersInBattle[EnemyAttacked].getStackedNumberEffect("Pilosity Stacks");
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
				CharactersInBattle[EnemyAttacked].setStackedNumberEffect("Pilosity Stacks", StackedNumber);
				break;

			// Dreamwalker abilities

			case "Pierce":
				StackedNumber = CharactersInBattle[EnemyAttacked].getStackedNumberEffect("Pierce Stacks");

				if ((CharactersInBattle[EnemyAttacked].getMaxHealth()*0.30) > CharactersInBattle[EnemyAttacked].getCurrentHealth()){
					StackedNumber += 1;
				}
				if (CharactersInBattle[EnemyAttacked].getAerial()){
					if (StackedNumber != 1){
						StackedNumber /= 2;
					}
				}
				if (StackedNumber > 3){
					StackedNumber = 3;
				}

                LastDamage = StackedNumber * Damage;
				
				for (int i = 0; i<StackedNumber; i++){
					CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - LastDamage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				}

				break;

			case "Daymare":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						CharactersInBattle[i].setStackedNumberEffect("Pierce Stacks", 1);
						CharactersInBattle[EnemyAttacked].setDurationEffect("Pierce Stacks", 0);
					}
				}
				CharactersInBattle[EnemyAttacked].setStackedNumberEffect("Pierce Stacks", 3);
				CharactersInBattle[EnemyAttacked].setDurationEffect("Pierce Stacks", 4);
				break;

			case "Deep Dream":
				CharactersInBattle[EnemyAttacked].setDurationEffect("DeepDream Effect", 4);
				break;

			// Hemancer abilities

			case "Summon Hen":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] == null){
						if (CharactersInBattle[CharactersInBattle.Length-1] == null){
							CharactersInBattle[i] = new EventCharacter (8, "Chicken", 10, 100, GlobalData.Classes [8], Biome.Prairie);
							CharactersInBattle[i].setBottom(CharactersInBattle[Attacker].getBottom());
							CharactersInBattle[i].setCurrentHealth(CharactersInBattle[i].getMaxHealth());
							CharactersInBattle[i].setPreviousHealth(CharactersInBattle[i].getMaxHealth());
							CharactersInBattle[i].setProgressIPBar(1);
						}
						break;
					}
				}
				break;

			case "Roast Chicken":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getName () == "Chicken"){
							EnemyAttacked = i;
							CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - Damage);
							CharactersInBattle[Attacker].setCurrentHealth(CharactersInBattle[Attacker].getCurrentHealth()+ 10 * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
							break;
						}
					}
				}
				break;

			case "Kamikahen":
				for (int i = 0; i<CharactersInBattle.Length; i++){
					if (CharactersInBattle[i] != null){
						if (CharactersInBattle[i].getName () == "Chicken"){
							EnemyAttacked = i;
							CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - 10);
							CharactersInBattle[EnemyFocused].setCurrentHealth(CharactersInBattle[EnemyFocused].getCurrentHealth() - Damage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
                            EnemyAttacked = EnemyFocused;
                            //Debug.Log (true);
							break;
						}
					}
				}
				break;

			// Disembodied abilities:

			case "Haunt":
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - Damage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				CharactersInBattle[EnemyAttacked].setDurationEffect("Haunt Effect", 4);
				break;

			case "Death Kiss":
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - Damage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
				break;

			case "Dark Pact":
				CharactersInBattle[Attacker].setDurationEffect ("Dark Pact Effect", 4);
				break;

            // Dummie

            case "Full HP":
                CharactersInBattle[Attacker].setCurrentHealth(CharactersInBattle[Attacker].getMaxHealth());
                break;

            case "Half HP":
                CharactersInBattle[Attacker].setCurrentHealth(CharactersInBattle[Attacker].getMaxHealth()/2);
                break;

            case "Cuadra HP":
                CharactersInBattle[Attacker].setCurrentHealth(CharactersInBattle[Attacker].getMaxHealth()/4);
                break;
            /*
            case "Simple":
				CharactersInBattle[EnemyAttacked].setCurrentHealth(CharactersInBattle[EnemyAttacked].getCurrentHealth() - Damage * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel())));
                break;
			*/
			
		}
		CharactersInBattle[Attacker].setCurrentHealth(CharactersInBattle[Attacker].getCurrentHealth() - (CostHealth  * (Mathf.Pow (GlobalData.LevelModifier, CharactersInBattle[Attacker].getCurrentLevel()))));
		CharactersInBattle[Attacker].setProgressIPBar(CharactersInBattle[Attacker].getProgressIPBar() - CostIP);
        CharactersInBattle[Attacker].setLastEnemyAttacked(EnemyAttacked);
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

    public float getLastDamage()
    {
        return LastDamage;
    }
}

