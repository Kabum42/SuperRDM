using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleScript : MonoBehaviour {

	private static int MaxCharacters = 6;
	private int ActualCharacters = 0;
	private Character[] CurrentCharacters = new Character[MaxCharacters];
	private MainCharacter Player;
	private Effect[] auxEffects;
	private int MaxTop;
	private int MaxBottom;
	private int[] PositionTops = new int[3];
	private int[] PositionBottoms = new int[3];
	private int CharacterTurn = -1;
	private int EnemyAttacked = -1;
	private int PlayerCharacter = 0;
	private bool TurnoActivo = true;
	private Skill lastSkill;
	private GameObject[] Skills = new GameObject[3];
	private GameObject[] StatsTop = new GameObject[3];
	private GameObject[] StatsBottom = new GameObject[1];
	private GameObject ActualTurn;
	private GameObject SkillUsed;
	private int SkillSelected = 0;
	private bool SelectedSkill = false;
	private bool NeedEnemy = false;
	private int CurrentEnemy = 0;
	private float TimerTurn = 0;
	private float TimerIPBar = 0;
	private float TimerHealth = 0;
	private bool UpdateHealth = false;

	private int Top;
	private int Bottom;

	private VisualBattle vb;

	// Use this for initialization
	void Start () {
		if (!GlobalData.started)
		{
			GlobalData.Start();
		}

		InitializeGameObjects();
		AsignCharacters ();
		InitializeBars();
		CheckPositions ();
		UpdateInformation ();
		LogCharacters ();
		Player = (MainCharacter) CurrentCharacters [PlayerCharacter];

		vb = (Instantiate(Resources.Load("Prefabs/VisualBattleObject")) as GameObject).GetComponent<VisualBattle>();
		vb.gameObject.transform.parent = GameObject.Find ("Battle").transform;
		vb.setBattleScript (this);

	}
	
	// Update is called once per frame
	void Update () {

        if (vb.hasOrders)
        {
            SelectedSkill = true;
            SkillSelected = vb.skillOrder;
            CurrentEnemy = vb.targetOrder;
            vb.hasOrders = false;
        }

		if (CharacterTurn == -1) {
			TimerIPBar += Time.deltaTime;
			if (TimerIPBar > 0.01) {
				CalculateProgressIP ();
				TimerIPBar = 0;
			}
		} 
		else {
			if (UpdateHealth){
				TimerHealth += Time.deltaTime;
				if (TimerHealth > 0.01){
					CheckHealth ();
					TimerHealth = 0;
				}
			}
			else {
				if ((SelectedSkill && PlayerCharacter == CharacterTurn) || (PlayerCharacter != CharacterTurn)) {
					TimerTurn += Time.deltaTime;
					if (TimerTurn > 1) {
						TimerTurn = 0;
						Turn ();
						// LogEffects();
						SelectedSkill = false;
					}
				} 
				else {
                    vb.AllowInteraction();
				}
			}
		}
		UpdateInformation ();
	}

	public Character[] getCurrentCharacters() {

		return CurrentCharacters;

	}

	void InitializeGameObjects(){
		Skills [0] = GameObject.Find ("Skill1");
		Skills [1] = GameObject.Find ("Skill2");
		Skills [2] = GameObject.Find ("Skill3");
		StatsTop [0] = GameObject.Find ("StatsTop1");
		StatsTop [1] = GameObject.Find ("StatsTop2");
		StatsTop [2] = GameObject.Find ("StatsTop3");
		StatsBottom [0] = GameObject.Find ("StatsBottom1");
		ActualTurn = GameObject.Find ("Turn");
		SkillUsed = GameObject.Find ("SkillUsed");

	}

	void AsignCharacters(){
        int auxposition = 0;
        CurrentCharacters[auxposition] = GlobalData.agents[GlobalData.positionCharacterCombat[0]];
        CurrentCharacters[auxposition].setBottom(true);
        auxposition++;

        if (GlobalData.positionCharacterCombat[1] != -1)
        {
            CurrentCharacters[auxposition] = GlobalData.agents[GlobalData.positionCharacterCombat[1]];
            CurrentCharacters[auxposition].setBottom(false);
        }
        else
        {
            switch (GlobalData.currentBiome)
            {
                case Biome.Prairie:
                    CurrentCharacters[auxposition] = GlobalData.RandomEnemies[0];
                    break;

                case Biome.Forest:
                    CurrentCharacters[auxposition] = GlobalData.RandomEnemies[1];
                    break;

                case Biome.Swamp:
                    CurrentCharacters[auxposition] = GlobalData.RandomEnemies[2];
                    break;

                default:
                    break;
            }
            CurrentCharacters[auxposition].setBottom(false);
        }

		// Setting Effects
		MainCharacter auxCharacter;
		Class auxClass;
		for (int i = 0; i<MaxCharacters; i++){
			if (CurrentCharacters[i] != null){
				auxClass = CurrentCharacters[i].getOwnClass();
				switch (auxClass.getName ()){
					case "Pilumantic": 
						auxCharacter = (MainCharacter) CurrentCharacters[i];
						auxCharacter.addEffect("Pilosity Stacks", ref CurrentCharacters, -1, CurrentCharacters[i].getID ());
						break;

					case "Dreamwalker":
						auxCharacter = (MainCharacter) CurrentCharacters[i];
						auxCharacter.addEffect("Pierce Stacks", ref CurrentCharacters, -1, CurrentCharacters[i].getID ());
						auxCharacter.addEffect("DeepDream Effect", ref CurrentCharacters, -1, CurrentCharacters[i].getID ());
						break;
				}
			}
		}
	}

	void InitializeBars(){
		for (int i = 0; i<MaxCharacters; i++){
			if (CurrentCharacters[i] != null){
				CurrentCharacters[i].setCurrentHealth(CurrentCharacters[i].getMaxHealth());
				CurrentCharacters[i].setPreviousHealth(CurrentCharacters[i].getMaxHealth());
				CurrentCharacters[i].setProgressIPBar(0);
				ActualCharacters += 1;
			}
		}
		Debug.Log ("ActualCharacters: " + ActualCharacters.ToString ()); 

	}

	void CheckPositions(){
		Top = 0;
		Bottom = 0;
		for (int i = 0; i<MaxCharacters; i++){
			if (CurrentCharacters[i] != null){
				if (CurrentCharacters[i].getBottom ()){
					PositionBottoms[Bottom] = i;
					Bottom += 1;
				}
				else {
					PositionTops[Top] = i;
					Top += 1;
				}
			}
		}
		MaxTop = Top-1;
		MaxBottom = Bottom-1;

	}

	void UpdateInformation(){
		Top = 0;
		Bottom = 0;
		string Text;

		for (int i = 0; i<3; i++) {
			Text = "-";
			StatsTop[i].GetComponent<TextMesh>().text = Text;
		}

		for (int i = 0; i<MaxCharacters; i++){
			if (CurrentCharacters[i] != null){
				Text = "Name: " + CurrentCharacters[i].getName() + "\n" + 
						"Health: " + CurrentCharacters[i].getPreviousHealth() + "/" + CurrentCharacters[i].getMaxHealth() + "\n" + 
						"IP: " + CurrentCharacters[i].getProgressIPBar() + "/" + CurrentCharacters[i].getMaxIPBar() + "\n"; 
				if (CurrentCharacters[i].getBottom ()){
					StatsBottom[Bottom].GetComponent<TextMesh>().text = Text;
					Bottom += 1;
				}
				else {
					StatsTop[Top].GetComponent<TextMesh>().text = Text;
					Top += 1;
				}
			}
		}

		for (int i = 0; i<Skills.Length; i++) {
			Skills[i].GetComponent<TextMesh>().text = CurrentCharacters[PlayerCharacter].getSkill(i).getName();
			Skills[i].GetComponent<TextMesh> ().color = Color.black;
		}
	
		for (int i = 0; i<StatsTop.Length; i++) {
			StatsTop[i].GetComponent<TextMesh> ().color = Color.black;
		}

		if (NeedEnemy) {
			StatsTop [CurrentEnemy].GetComponent<TextMesh> ().color = Color.red;
		}

		Skills[SkillSelected].GetComponent<TextMesh> ().color = Color.red;

		ActualTurn.GetComponent<TextMesh> ().text = "Turn: " + CharacterTurn;
	}

	void CalculateProgressIP(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null && CurrentCharacters[i].IsNotStun()){
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

	void LogEffects(){
		string Effectstring = "";
		for (int i = 0; i<MaxCharacters; i++) {
			if (CurrentCharacters[i] != null){
				auxEffects = CurrentCharacters[i].getCurrentEffects();
				Effectstring += CurrentCharacters[i].getName() + "\n";
				for (int j = 0; j<auxEffects.Length; j++){
					if (auxEffects[j] != null){
						Effectstring += auxEffects[j].getName() + ": " ;
						Effectstring += auxEffects[j].getStackedNumber() + ", " + auxEffects[j].getDuration() + "\n\n";
					}
				}
			}
		}
		Debug.Log (Effectstring);
	}

	void Turn(){
		Attack ();
		lastSkill = CurrentCharacters [CharacterTurn].getLastSkillUsed ();
        if (lastSkill != null)
        {
            vb.visualCharacters[CharacterTurn].Perform(lastSkill, vb.visualCharacters[CurrentCharacters[CharacterTurn].getLastEnemyAttacked()], new float[] { lastSkill.getLastDamage() });
        }
        UpdateEffects ();
		if (lastSkill != null) {
			SkillUsed.GetComponent<TextMesh> ().text = CurrentCharacters [CharacterTurn].getName () + " used: " + lastSkill.getName ();
			if (CheckInstantHealth()) {
				UpdateHealth = true;
			} 
			else {
				CharacterTurn = -1;
				UpdateHealth = false;
			}
		} 
		else {
			SkillUsed.GetComponent<TextMesh> ().text = CurrentCharacters [CharacterTurn].getName () + " used: " 
				+ "Simple Attack";
			UpdateHealth = true;
		}

	}

	void Attack(){
		if (CurrentCharacters [CharacterTurn].GetType() == typeof(EventCharacter)) {
			CurrentEnemy = 0;
			CurrentCharacters [CharacterTurn].UseSkill (SkillSelected, CharacterTurn, ref CurrentCharacters, CurrentEnemy);
		} 
		else {
			if (CharacterTurn == PlayerCharacter){
				CurrentCharacters [CharacterTurn].UseSkill (SkillSelected, CharacterTurn, ref CurrentCharacters, CurrentEnemy);
			}
			else {
				MainCharacter CurrentNPC = (MainCharacter) CurrentCharacters[CharacterTurn];
				CurrentNPC.ApplyEnemyIA(CharacterTurn, ref CurrentCharacters);
			}
		}

	}
			                   

	bool CheckInstantHealth(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null){
				if (CurrentCharacters[i].getCurrentHealth() < CurrentCharacters[i].getPreviousHealth()){
					return true;
				}
				else if (CurrentCharacters[i].getCurrentHealth() > CurrentCharacters[i].getPreviousHealth()){
					return true;
				}
			}
		}
		return false;
	}

	void CheckHealth(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null){
				if (CurrentCharacters[i].getCurrentHealth() < CurrentCharacters[i].getPreviousHealth()){
					CurrentCharacters[i].setPreviousHealth(CurrentCharacters[i].getPreviousHealth()-1);
					if (CurrentCharacters[i].getCurrentHealth() == CurrentCharacters[i].getPreviousHealth()){
						UpdateHealth = false;
						CharacterTurn = -1;
						CheckLifes ();
						CheckEnd ();
					}
				}
				else if (CurrentCharacters[i].getCurrentHealth() > CurrentCharacters[i].getPreviousHealth()){
					CurrentCharacters[i].setPreviousHealth(CurrentCharacters[i].getPreviousHealth()+1);
					if (CurrentCharacters[i].getCurrentHealth() == CurrentCharacters[i].getPreviousHealth()){
						UpdateHealth = false;
						CharacterTurn = -1;
					}
				}
			}
		}
	}

	void CheckLifes(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null){
				if (CurrentCharacters[i].getCurrentHealth() <= 0){
					if (CurrentCharacters[i].getBottom () != CurrentCharacters[PlayerCharacter].getBottom ()){
						Player.setExperience(70);
					}
					CurrentCharacters[i] = null;
					CheckPositions();
				}
			}
		}
	}

	void UpdateEffects(){
		for (int i = 0; i<ActualCharacters; i++) {
			if (CurrentCharacters[i] != null){
				CurrentCharacters[i].UpdateEffects(CharacterTurn, ref CurrentCharacters, i);
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
			vb.EndBattle();
		} 
		else if (Top == 0){
			vb.EndBattle();
		}
	}

	public List<int> getTargets(int caster, int position) {

		List<int> aux = new List<int> ();
		//aux.Add (0);
        if (CurrentCharacters[caster].CheckEnemies(position))
        {
            for (int i = 0; i < ActualCharacters; i++)
            {
                if (CurrentCharacters[i] != null)
                {
                    if (CurrentCharacters[i].getBottom() != CurrentCharacters[caster].getBottom())
                    {
                        aux.Add(i);
                    }
                }
            }
        }
		return aux;

	}

    public Character[] GetCurrentCharacters()
    {
        return CurrentCharacters;
    }

    public static void simulateBattle(int i) {

        GlobalData.agents[i].setExperience(70);
        GlobalData.agents[i].setCurrentFatigue(GlobalData.agents[i].getCurrentFatigue() + Random.Range(0f, 0.5f));

    }

}

