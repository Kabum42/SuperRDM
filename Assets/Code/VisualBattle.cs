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

	public GameObject fading;
	public GameObject fading2;

	private bool endingBattle = false;

	private AudioSource musicBattle;

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


		/*
		fading2.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f);
		fading2.GetComponent<SpriteRenderer> ().material.SetColor("_Color", new Color(1f, 1f, 1f));
		fading2.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);

		int resolution = 512;
		Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
		
		float offsetX = Random.Range (1000f, 2000f);
		float offsetY = Random.Range (1000f, 2000f);
		float offsetZ = 0f;
		
		float division = (float)resolution;
		
		for (int y = 0; y < resolution; y++) {
			for (int x = 0; x < resolution; x++) {
				Vector3 point = new Vector3(offsetX + (float)x/division, offsetY + (float)y/division, offsetZ);
				float value = Noise.Sum(Noise.valueMethods[1], point, 2f, 10, 3f, 0.75f); 
				//float value = Noise.Perlin2D(n, 1f);
				texture.SetPixel(x, y, new Color(value, value, value));
			}
		}
		
		texture.Apply();
		
		fading2.GetComponent<SpriteRenderer> ().material.SetTexture ("_AlphaTex", texture);
		fading2.GetComponent<SpriteRenderer> ().sprite = Sprite.Create(GlobalData.auxTexture, new Rect(0, 0, GlobalData.auxTexture.width, GlobalData.auxTexture.height), new Vector2(0.5f, 0.5f));
		fading2.GetComponent<SpriteRenderer> ().material.SetTexture ("_MainTex", GlobalData.auxTexture);
		*/

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

		bool aux = false;

		for (int i = 0; i < visualCharacters.Length; i++) {
			if (visualCharacters [i] != null) {
				if (visualCharacters[i].performing != -1 || visualCharacters[i].representing != -1) {
					aux = true;
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
