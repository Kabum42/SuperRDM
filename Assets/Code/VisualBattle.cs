using UnityEngine;
using System.Collections;

public class VisualBattle : MonoBehaviour {

	public BattleScript bs;

	private GameObject background;
	public VisualCharacter[] visualCharacters = new VisualCharacter[12];
    public int myCharacter = 0;
    private VisualInterface vInterface;

    public bool hasOrders = false;
    public int skillOrder = -1;
    public int targetOrder = -1;

	// Use this for initialization
	void Start () {

		background = Instantiate (Resources.Load ("Prefabs/VisualBackground")) as GameObject;
		background.transform.parent = this.gameObject.transform;

        vInterface = (Instantiate(Resources.Load("Prefabs/VisualInterface")) as GameObject).GetComponent<VisualInterface>();
        vInterface.root.transform.parent = this.gameObject.transform;
        vInterface.vBattle = this;
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AllowInteraction();
        }

        Character[] temp = GetCurrentCharacters();

        for (int i = 0; i < visualCharacters.Length; i++)
        {
            if (visualCharacters[i] != null)
            {
                float percentHealth = (float)temp[i].getCurrentHealth() / (float)temp[i].getMaxHealth();
                visualCharacters[i].health.GetComponent<Animator>().Play("Health", 0, percentHealth);
            }
        }
	
	}

    public Character[] GetCurrentCharacters()
    {
        return bs.getCurrentCharacters();
    }

    public void AllowInteraction()
    {
        vInterface.AllowInteraction();
    }

    public void setOrders(int skill, int target)
    {
        skillOrder = skill;
        targetOrder = target;
        hasOrders = true;
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
