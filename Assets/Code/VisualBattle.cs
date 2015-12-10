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
    public float precisionOrder = -1f;

	public GameObject fading;
	public GameObject fading2;

	private bool endingBattle = false;

	private AudioSource musicBattle;

    public SkillBar skillBar;

	// Use this for initialization
	void Start () {

		background = Instantiate (Resources.Load ("Prefabs/VisualBackground")) as GameObject;
		background.transform.parent = this.gameObject.transform;

        vInterface = (Instantiate(Resources.Load("Prefabs/VisualInterface")) as GameObject).GetComponent<VisualInterface>();
        vInterface.root.transform.parent = this.gameObject.transform;
        vInterface.vBattle = this;

		fading = this.gameObject.transform.FindChild ("Fading").gameObject;
		Hacks.SpriteRendererAlpha (fading, 0f);

		fading2 = this.gameObject.transform.FindChild ("Fading2").gameObject;

		fading2.transform.localScale = new Vector3 (3000f, 3000f, 1.1f);

        skillBar = (Instantiate(Resources.Load("Prefabs/SkillBarObject")) as GameObject).GetComponent<SkillBar>();

		musicBattle = gameObject.AddComponent<AudioSource>();
		musicBattle.clip = Resources.Load("Music/Battle_Boss") as AudioClip;
		musicBattle.volume = 1f;
		musicBattle.loop = true;
		musicBattle.Play();

		GlobalData.worldObject.SetActive (false);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (endingBattle) {
			if (fading.GetComponent<SpriteRenderer> ().color.a < 1f) {
				Hacks.SpriteRendererAlpha (fading, fading.GetComponent<SpriteRenderer> ().color.a + Time.deltaTime*2f);
				if (fading.GetComponent<SpriteRenderer> ().color.a >= 1f) {
					GlobalData.World();
					Destroy(GameObject.Find("Battle"));
				}
			}
			fading2.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", 1f);
		} else {
			if (fading2.GetComponent<SpriteRenderer>().material.GetFloat("_Cutoff") < 1f) {
				fading2.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", fading2.GetComponent<SpriteRenderer>().material.GetFloat("_Cutoff") + Time.deltaTime/2f);
			}
		}



		Character[] temp = GetCurrentCharacters();


		
		for (int i = 0; i < visualCharacters.Length; i++)
		{
			if (visualCharacters[i] != null)
			{

				if (!endingBattle) {
					visualCharacters[i].currentTrueHealth = Mathf.Clamp((float)temp[i].getCurrentHealth(), 0.000001f, float.MaxValue);
					visualCharacters[i].currentTrueMaxHealth = (float)temp[i].getMaxHealth();
				}



				float percentHealth = 1;
				
				if (visualCharacters[i].previousHealth < visualCharacters[i].currentVirtualHealth-0.001f  ||  visualCharacters[i].previousHealth > visualCharacters[i].currentVirtualHealth+0.001f) {
					
					visualCharacters[i].previousHealth = Mathf.Lerp(visualCharacters[i].previousHealth, visualCharacters[i].currentVirtualHealth, Time.deltaTime*10f);

					percentHealth = Mathf.Clamp (visualCharacters[i].previousHealth / visualCharacters[i].currentTrueMaxHealth, 0.000001f, 0.9999999f);
					visualCharacters[i].health.GetComponent<Animator>().Play("Health", 0, percentHealth);
					
				}


			}
		}


	
	}

    public Character[] GetCurrentCharacters()
    {
        return bs.getCurrentCharacters();
    }

    public void AllowInteraction()
    {
        if (!skillBar.isActive)
        {
            vInterface.AllowInteraction();
        }
    }

    public void setOrders(int skill, int target)
    {
        skillOrder = skill;
        targetOrder = target;
    }

    public void setPrecision(float auxPrecision)
    {
        precisionOrder = auxPrecision;
        hasOrders = true;
    }

	public void setBattleScript(BattleScript auxBs) {

		bs = auxBs;

		int currentLeft = 0;
		int currentRight = 0;

		Character[] temp = GetCurrentCharacters();

		for (int i = 0; i < bs.getCurrentCharacters().Length; i++) {

			if (bs.getCurrentCharacters()[i] != null) {


				visualCharacters[i] = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
				visualCharacters[i].gameObject.transform.parent = this.gameObject.transform;
				visualCharacters[i].setClass(bs.getCurrentCharacters()[i].getOwnClass());
				visualCharacters[i].previousHealth = (float)temp[i].getCurrentHealth();
				visualCharacters[i].currentTrueHealth = visualCharacters[i].previousHealth;
				visualCharacters[i].currentVirtualHealth = visualCharacters[i].previousHealth;
				visualCharacters[i].positionInArray = i;

				float percentHealth = Mathf.Clamp (visualCharacters[i].previousHealth / visualCharacters[i].currentTrueMaxHealth, 0.000001f, 0.9999999f);
				visualCharacters[i].health.GetComponent<Animator>().Play("Health", 0, percentHealth);

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

	public bool isWaiting() {

		bool aux = true;

		for (int i = 0; i < visualCharacters.Length; i++) {
			if (visualCharacters [i] != null) {
				if (visualCharacters[i].performing != -1 || visualCharacters[i].representing != -1) {
					aux = false;
					break;
				}
			}
		}

		return aux;

	}

	public void EndBattle() {

		endingBattle = true;

	}

}
