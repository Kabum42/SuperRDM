using UnityEngine;
using System.Collections;

public class VisualBattle : MonoBehaviour {

	private BattleScript bs;

	private GameObject background;
	public VisualCharacter[] visualCharacters = new VisualCharacter[12];

	// Use this for initialization
	void Start () {

		background = Instantiate (Resources.Load ("Prefabs/VisualBackground")) as GameObject;
		background.transform.parent = this.gameObject.transform;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setBattleScript(BattleScript auxBs) {

		bs = auxBs;

		int currentLeft = 0;
		int currentRight = 0;

		for (int i = 0; i < bs.getCurrentCharacters().Length; i++) {

			if (bs.getCurrentCharacters()[i] != null) {

				visualCharacters[i] = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
				visualCharacters[i].gameObject.transform.parent = this.gameObject.transform;
				visualCharacters[i].setClass(bs.getCurrentCharacters()[i].getOwnClass());

				if (bs.getCurrentCharacters()[i].getBottom()) {
					// ESTA A LA IZQUIERDA
					visualCharacters[i].SetBattlePosition(true, currentLeft);
					currentLeft++;
				}
				else {
					// ESTA A LA DERECHA
					visualCharacters[i].SetBattlePosition(false, currentRight);
					currentRight++;
				}




			}

		}

	}
}
