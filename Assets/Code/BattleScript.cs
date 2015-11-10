using UnityEngine;
using System.Collections;

public class BattleScript : MonoBehaviour {

	private static int MaxCharacters = 6;
	private int ActualCharacters = 0;
	private Character[] CurrentCharacters = new Character[MaxCharacters];
	private MainCharacter Player;
	private int MaxTop;
	private int MaxBottom;
	private int[] PositionTops = new int[3];
	private int[] PositionBottoms = new int[3];
	private int CharacterTurn = -1;
	private int EnemyAttacked = -1;
	private int PlayerCharacter = 0;
	private bool TurnoActivo = true;
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
		//vb.visualCharacters [0].Perform (GlobalData.Skills [1], vb.visualCharacters [3], new float[]{34f});

	}
	
	// Update is called once per frame
	void Update () {

        if (vb.hasOrders)
        {
            vb.visualCharacters[vb.myCharacter].Perform(GlobalData.Skills[vb.skillOrder], vb.visualCharacters[vb.targetOrder], new float[] { 34f });
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
						SelectedSkill = false;
					}
				} 
				else {
					SelectSkill ();
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
		CurrentCharacters [0] = GlobalData.agents [0];
		CurrentCharacters [0].setBottom (true);

		CurrentCharacters [1] = GlobalData.agents [1];
		CurrentCharacters [1].setBottom (false);

		CurrentCharacters [2] = GlobalData.RandomEnemies [1];
		CurrentCharacters [2].setBottom (false);

		CurrentCharacters [3] = GlobalData.RandomEnemies [1];
		CurrentCharacters [3].setBottom (false);
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
			Skills[i].GetComponent<TextMesh>().text = CurrentCharacters[PlayerCharacter].getSkillName(i);
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

	void SelectSkill(){
		if (!NeedEnemy) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (SkillSelected == 0) {
					SkillSelected = 2;
				} else {
					SkillSelected -= 1;
				}
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (SkillSelected == 2) {
					SkillSelected = 0;
				} else {
					SkillSelected += 1;
				}
			}
			if (Input.GetKeyDown (KeyCode.Return)) {
				if(CurrentCharacters [CharacterTurn].CheckEnemies(SkillSelected)){
					NeedEnemy = true;
				}
				else {
					SelectedSkill = true;
					CurrentEnemy = -1;
				}

			}
		} 
		else {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (CurrentEnemy == 0) {
					CurrentEnemy = MaxTop;
				} else {
					CurrentEnemy -= 1;
				}
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (CurrentEnemy == MaxTop) {
					CurrentEnemy = 0;
				} else {
					CurrentEnemy += 1;
				}
			}
			if (Input.GetKeyDown (KeyCode.Return)) {
				SelectedSkill = true;
			}
		}
	}

	void Turn(){
		Attack ();
		SkillUsed.GetComponent<TextMesh>().text = CurrentCharacters[CharacterTurn].getName() + " used: " 
				+ CurrentCharacters[CharacterTurn].getLastSkillUsed();
		UpdateHealth = true;
	}

	void Attack(){
		if (CurrentCharacters [CharacterTurn].GetType() == typeof(EventCharacter)) {
			CurrentEnemy = 0;
			CurrentCharacters [CharacterTurn].UseSkill (SkillSelected, CharacterTurn, ref CurrentCharacters, CurrentEnemy);
		} 
		else {
			if (CharacterTurn == PlayerCharacter){
				if (NeedEnemy){
					CurrentEnemy = PositionTops[CurrentEnemy];
					NeedEnemy = false;
				}
				CurrentCharacters [CharacterTurn].UseSkill (SkillSelected, CharacterTurn, ref CurrentCharacters, CurrentEnemy);
			}
			else {
				MainCharacter CurrentNPC = (MainCharacter) CurrentCharacters[CharacterTurn];
				CurrentNPC.ApplyEnemyIA(CharacterTurn, ref CurrentCharacters);
			}
			Debug.Log (CurrentCharacters[CharacterTurn].getName() + " passive: " + CurrentCharacters[CharacterTurn].getStackedNumberEffect("Anger Management"));
		}

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
            GlobalData.World();
            Destroy(GameObject.Find("Battle"));
		} 
		else if (Top == 0){
			Debug.Log ("Bottom Wins");
            GlobalData.World();
            Destroy(GameObject.Find("Battle"));
		}
	}

}

