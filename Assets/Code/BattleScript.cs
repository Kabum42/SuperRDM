using UnityEngine;
using System.Collections;

public class BattleScript : MonoBehaviour {

	private static int MaxCharacters = 6;
	private int ActualCharacters = 0;
	private Character[] CurrentCharacters = new Character[MaxCharacters];
	private int CharacterTurn = -1;
	private int EnemyAttacked = -1;
	private bool TurnoActivo = true;

	// Use this for initialization
	void Start () {
		if (!GlobalData.started)
		{
			GlobalData.Start();
		}

		AsignCharacters ();
		InitializeBars();
		LogCharacters ();

	}
	
	// Update is called once per frame
	void Update () {
		while (CharacterTurn == -1) {
			CalculateProgressIP ();
		}
		Debug.Log ("Turn: " + CharacterTurn.ToString ()); 
		Turn();
		CheckLifes ();
		CheckEnd ();
		LogCharacters ();
	}

	void AsignCharacters(){
		CurrentCharacters [0] = GlobalData.agents [0];
		CurrentCharacters [0].setBottom (true);

		CurrentCharacters [1] = GlobalData.RandomEnemies [0];
		CurrentCharacters [1].setBottom (false);

		CurrentCharacters [2] = GlobalData.RandomEnemies [1];
		CurrentCharacters [2].setBottom (false);
	}

	void InitializeBars(){
		for (int i = 0; i<MaxCharacters; i++){
			if (CurrentCharacters[i] != null){
				CurrentCharacters[i].setCurrentHealth(CurrentCharacters[i].getMaxHealth());
				CurrentCharacters[i].setProgressIPBar(0);
				ActualCharacters += 1;
			}
		}
		Debug.Log ("ActualCharacters: " + ActualCharacters.ToString ()); 

	}

	void CalculateProgressIP(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null){
				if (CurrentCharacters[i].getProgressIPBar() == CurrentCharacters[i].getMaxIPBar()){
					if (CharacterTurn == -1){
						CharacterTurn = i;
					}
				}
				else {
					if (CurrentCharacters[i].getProgressIPBar()+1 > CurrentCharacters[i].getMaxIPBar()){
						CurrentCharacters[i].setProgressIPBar(CurrentCharacters[i].getMaxIPBar());
					}
					else {
							CurrentCharacters[i].setProgressIPBar(CurrentCharacters[i].getProgressIPBar()+1);
					}
				}
			}
		}
	}

	void LogCharacters(){
		for (int i = 0; i<MaxCharacters; i++){
			if (CurrentCharacters[i] != null){
				Debug.Log("ID: " + CurrentCharacters[i].getID().ToString() + "\n" +
				          "Name: " + CurrentCharacters[i].getName().ToString() + "\n" +
				          "CurrentHealth: " + CurrentCharacters[i].getCurrentHealth().ToString() + "\n" +
				          "MaxHealth: " + CurrentCharacters[i].getMaxHealth().ToString() + "\n" + 
				          "CurrentIPBar: " + CurrentCharacters[i].getProgressIPBar().ToString() + "\n" +
				          "MaxIPBar: " + CurrentCharacters[i].getMaxIPBar().ToString() + "\n" +
						  "Aerial: " + CurrentCharacters[i].getAerial().ToString()); 
			}
		}
	}

	void Turn(){
		Attack (0);
		CharacterTurn = -1;
	}

	void Attack(int SkillSelected){
		int CurrentEnemy;
		if (CurrentCharacters [CharacterTurn].GetType() == typeof(EventCharacter)) {
			CurrentEnemy = 0;
			CurrentCharacters [CharacterTurn].UseSkill (SkillSelected, CharacterTurn, ref CurrentCharacters, CurrentEnemy);
		} 
		else {
			CurrentCharacters [CharacterTurn].UseSkill (SkillSelected, CharacterTurn, ref CurrentCharacters, -1);
		}

	}

	void CheckLifes(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null){
				if (CurrentCharacters[i].getCurrentHealth() <= 0){
					CurrentCharacters[i] = null;
				}
			}
		}
	}

	void CheckEnd(){
		int Bottom = 0;
		int Top = 0;
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters [i] != null) {
				if (CurrentCharacters [i].getBottom ()) {
					Bottom += 1;
				} else {
					Top += 1;
				}
			}
		}

		if (Bottom == 0) {
			Debug.Log ("Top Wins");
			Application.LoadLevel ("World");
		} 
		else if (Top == 0){
			Debug.Log ("Bottom Wins");
			Application.LoadLevel ("World");
		}
	}

}

