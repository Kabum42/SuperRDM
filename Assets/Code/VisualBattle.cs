using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private bool finalBoss = false;
    private int finalBossPhase = -1;

    public SkillBar skillBar;

    private int currentLeft = 0;
    private int currentRight = 0;

    private SpeechManager speechManager;

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
        skillBar.transform.parent = this.gameObject.transform;

		musicBattle = gameObject.AddComponent<AudioSource>();
        musicBattle.clip = Resources.Load("Music/Battle_Boss") as AudioClip;
        musicBattle.volume = 1f;
        musicBattle.loop = true;

		GlobalData.worldObject.SetActive (false);
	
	}
	
	// Update is called once per frame
	void Update () {

        if (speechManager != null)
        {
            speechManager.update();
            if (speechManager.writingSomething)
            {
                vInterface.DisallowInteraction();
            }
        }

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

        if (finalBoss)
        {
            if (finalBossPhase == -1 && !speechManager.writingSomething)
            {
                finalBossPhase = 0;
                musicBattle.Play();
            }
            else if (finalBossPhase == 0)
            {
                float percentHealth = visualCharacters[1].previousHealth / visualCharacters[1].currentTrueMaxHealth;
                if (percentHealth <= 2f / 3f)
                {
                    finalBossPhase = 1;
                    speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "I have less than\n66% of life left", new Vector2(3f, 3f)));
                }
            }
            else if (finalBossPhase == 1)
            {
                float percentHealth = visualCharacters[1].previousHealth / visualCharacters[1].currentTrueMaxHealth;
                if (percentHealth <= 1f / 3f)
                {
                    finalBossPhase = 2;
                    speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "I have less than\n33% of life left", new Vector2(3f, 3f)));
                }
            }
            else if (finalBossPhase == 2)
            {
                float percentHealth = visualCharacters[1].previousHealth / visualCharacters[1].currentTrueMaxHealth;
                if (percentHealth <= 0f / 3f)
                {
                    finalBossPhase = 3;
                    speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "Now I'm dead", new Vector2(3f, 3f)));
                }
            }
        }

	
	}

    public Character[] GetCurrentCharacters()
    {
        return bs.getCurrentCharacters();
    }

    public int GetCurrentCharactersNotNull()
    {
        int aux = 0;

        Character[] temp = GetCurrentCharacters();
        for (int i = 0; i < bs.getCurrentCharacters().Length; i++)
        {
            if (bs.getCurrentCharacters()[i] != null)
            {
                aux++;
            }
        }

        return aux;

    }

    public void AllowInteraction()
    {
        if (!skillBar.isActive && (speechManager == null || !speechManager.writingSomething))
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

        if (temp [1].getOwnClass ().getName () == "FinalBoss") {

			finalBoss = true;

			speechManager = new SpeechManager (this.gameObject, GlobalData.eventFinalBoss);

			Debug.Log (visualCharacters [0].gameObject);
			Debug.Log (speechManager);
            
			speechManager.addSpeaker (visualCharacters [0].gameObject);
			speechManager.addSpeaker (visualCharacters [1].gameObject);

			speechManager.bubbles.Add (new Bubble (1, speechManager.globalPhase, speechManager.globalPhase, "Prepare to die!", new Vector2 (3f, 3f)));
			speechManager.bubbles.Add (new Bubble (1, speechManager.globalPhase + 1, speechManager.globalPhase + 1, "My power keeps\ngetting bigga", new Vector2 (3f, 3f)));
			speechManager.bubbles.Add (new Bubble (1, speechManager.globalPhase + 2, speechManager.globalPhase + 2, "because Jesuschrist\nis my nigga", new Vector2 (3f, 3f)));

		} else {

			musicBattle = gameObject.AddComponent<AudioSource>();
			musicBattle.clip = Resources.Load("Music/Battle_Players") as AudioClip;
			musicBattle.volume = 1f;
			musicBattle.loop = true;
			musicBattle.Play();

		}
        

	}

    public void addVisualCharacter(int ID)
    {
        if (ID == 0)
        {
            // ES UNA PUTA GALLINA
            int i = GetCurrentCharactersNotNull();

            visualCharacters[i] = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
            visualCharacters[i].gameObject.transform.parent = this.gameObject.transform;
            visualCharacters[i].setClass(bs.getCurrentCharacters()[i].getOwnClass());
            //visualCharacters[i].previousHealth = (float)temp[i].getCurrentHealth();
            visualCharacters[i].currentTrueHealth = visualCharacters[i].previousHealth;
            visualCharacters[i].currentVirtualHealth = visualCharacters[i].previousHealth;
            visualCharacters[i].positionInArray = i;

            float percentHealth = Mathf.Clamp(visualCharacters[i].previousHealth / visualCharacters[i].currentTrueMaxHealth, 0.000001f, 0.9999999f);
            visualCharacters[i].health.GetComponent<Animator>().Play("Health", 0, percentHealth);

            if (bs.getCurrentCharacters()[i].getBottom())
            {
                // ESTA A LA IZQUIERDA
                visualCharacters[i].SetBattlePosition(true, currentLeft);
                currentLeft++;
            }
            else
            {
                // ESTA A LA DERECHA
                visualCharacters[i].SetBattlePosition(false, currentRight);
                currentRight++;
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
