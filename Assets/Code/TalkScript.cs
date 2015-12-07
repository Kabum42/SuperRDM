using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkScript : MonoBehaviour {


	private AudioSource[] textSounds = new AudioSource[3];
	private bool nextSound = true;

    private AudioSource backgroundMusic;
	private AudioSource backgroundMusic2;
    private AudioSource menuOk;
    private AudioSource closeSpeech;

	private GameObject background;

	private string target;

    private static List<SpeechBubble> speechBubblePool;
    private static List<int> toRecycle;
    private static OptionsSquare options;

    private GameObject[] speakers;
    private List<Bubble> bubbles = new List<Bubble>();
    private int currentBubble = 0;
    private int nextPhase = 0;
    private int globalPhase = 0;

    private GameObject fading;
    private float fadingMode = 0f;

    public static int eventRon = 0;
    public static int eventDouchebards = 1;

    private int currentEvent = 0;

    private float partyHard = 6.1f;
    private int maxPartyPoints = 0;
    private int partyPoints = 0;
    private int partyCombo = 0;
    private float timePartying = 0f;
    private List<arrowParty> arrowsParty = new List<arrowParty>();
    private int currentArrowParty = 0;


	private List<List<Bubble>> randomGreetings = new List<List<Bubble>>();
	private List<List<Bubble>> randomFarewells = new List<List<Bubble>>();


    private GameObject eventRonG;
    private GameObject eventDouchebardsG;
    private GameObject light1;
    private GameObject light2;
    private GameObject light3;
    private GameObject silhouette1;
    private List<GameObject> arrows1 = new List<GameObject>();
    private GameObject arrowPositive1G;

    private GameObject silhouette2;
    private List<GameObject> arrows2 = new List<GameObject>();
    private GameObject arrowPositive2G;

    private GameObject silhouette3;
    private List<GameObject> arrows3 = new List<GameObject>();
    private GameObject arrowPositive3G;

    private GameObject silhouette4;
    private List<GameObject> arrows4 = new List<GameObject>();
    private GameObject arrowPositive4G;

    private GameObject arrowEpic;
    private GameObject arrowText;
    private float arrowTextShowing = 0f;

    private List<string> textGoodCombo = new List<string>();
    private List<string> textBadCombo = new List<string>();

    private float light1Z = 0f;
    private float light2Z = 0f;
    private float light3Z = 0f;

    private GameObject playerAnimated;

    private float lineWidth = 0.035f;

    private float endCondition = 0f;

	// Use this for initialization
	void Start () {

        currentEvent = GlobalData.currentSpecialEvent;

        speechBubblePool = new List<SpeechBubble>();
        toRecycle = new List<int>();

		background = GameObject.Find("Background");

        eventRonG = GameObject.Find("EventRon");
        eventRonG.SetActive(false);

        fading = this.gameObject.transform.FindChild("Fading").gameObject;
        fading.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);

        light1 = GameObject.Find("EventDouchebards/Light1/TrueLight");
        light1.SetActive(false);
        light2 = GameObject.Find("EventDouchebards/Light2/TrueLight");
        light2.SetActive(false);
        light3 = GameObject.Find("EventDouchebards/Light3/TrueLight");
        light3.SetActive(false);
        silhouette1 = GameObject.Find("EventDouchebards/ArrowsGame/Left/Silhouette1");
        silhouette1.SetActive(false);
        silhouette2 = GameObject.Find("EventDouchebards/ArrowsGame/Down/Silhouette2");
        silhouette2.SetActive(false);
        silhouette3 = GameObject.Find("EventDouchebards/ArrowsGame/Up/Silhouette3");
        silhouette3.SetActive(false);
        silhouette4 = GameObject.Find("EventDouchebards/ArrowsGame/Right/Silhouette4");
        silhouette4.SetActive(false);
        arrowPositive1G = GameObject.Find("EventDouchebards/ArrowsGame/Left/arrowPositive");
        arrowPositive1G.SetActive(false);
        arrowPositive2G = GameObject.Find("EventDouchebards/ArrowsGame/Down/arrowPositive");
        arrowPositive2G.SetActive(false);
        arrowPositive3G = GameObject.Find("EventDouchebards/ArrowsGame/Up/arrowPositive");
        arrowPositive3G.SetActive(false);
        arrowPositive4G = GameObject.Find("EventDouchebards/ArrowsGame/Right/arrowPositive");
        arrowPositive4G.SetActive(false);
        arrowEpic = GameObject.Find("EventDouchebards/ArrowsGame/Epic");
        arrowEpic.SetActive(false);
        arrowText = GameObject.Find("EventDouchebards/ArrowsGame/Text");
        arrowText.GetComponent<TextMesh>().color = new Color(1f, 0.4f, 0.4f);
        arrowText.SetActive(false);

        textGoodCombo.Add("Nice");
        textGoodCombo.Add("Good");
        textGoodCombo.Add("Great");
        textGoodCombo.Add("Excellent");
        textGoodCombo.Add("Perfect");
        textGoodCombo.Add("WOW!");
        textGoodCombo.Add("Amazing");
        textGoodCombo.Add("Wonderful");
        textGoodCombo.Add("Incredible");
        textGoodCombo.Add("Miracolous");
        textGoodCombo.Add("Headshot");
        textGoodCombo.Add("GOD MODE");

        textBadCombo.Add("Meh");
        textBadCombo.Add("Bad");
        textBadCombo.Add("Poor");
        textBadCombo.Add("Awful");
        textBadCombo.Add("Disgusting");
        textBadCombo.Add("Terrible");
        textBadCombo.Add("Horrible");
        textBadCombo.Add("Atrocious");
        textBadCombo.Add("Dont even try");
        textBadCombo.Add("Kill yourself");

        GameObject aux = Instantiate(Resources.Load("Prefabs/Barbarian")) as GameObject;
        aux.transform.parent = GameObject.Find("Player").transform;
        aux.transform.localPosition = new Vector3(0f, 0f, 0f);
        playerAnimated = aux.transform.FindChild("Animated").gameObject;
        

        for (int i = 1; i <= 6; i++)
        {
            arrows1.Add(GameObject.Find("EventDouchebards/ArrowsGame/Left/arrow"+i));
            arrows1[i - 1].SetActive(false);
            arrows2.Add(GameObject.Find("EventDouchebards/ArrowsGame/Down/arrow" + i));
            arrows2[i - 1].SetActive(false);
            arrows3.Add(GameObject.Find("EventDouchebards/ArrowsGame/Up/arrow" + i));
            arrows3[i - 1].SetActive(false);
            arrows4.Add(GameObject.Find("EventDouchebards/ArrowsGame/Right/arrow" + i));
            arrows4[i - 1].SetActive(false);
        }

        eventDouchebardsG = GameObject.Find("EventDouchebards");
        eventDouchebardsG.SetActive(false);
        

		for (int i = 0; i < textSounds.Length; i++) {
			textSounds[i] = gameObject.AddComponent<AudioSource>();
	        textSounds[i].clip = Resources.Load("Music/Text/Text_"+i.ToString("00")) as AudioClip;
	        textSounds[i].volume = 1.5f;
	        textSounds[i].pitch = 1.1f;
	        textSounds[i].Play();
		}

        menuOk = gameObject.AddComponent<AudioSource>();
        menuOk.clip = Resources.Load("Music/Menu/MenuOk") as AudioClip;
        menuOk.volume = 0.45f;

        closeSpeech = gameObject.AddComponent<AudioSource>();
        closeSpeech.clip = Resources.Load("Sounds/whip_01") as AudioClip;
        closeSpeech.volume = 1f;
        closeSpeech.playOnAwake = false;

        SpeechBubble s = new SpeechBubble();
        s.root.transform.parent = this.gameObject.transform;
        speechBubblePool.Add(s);

        options = new OptionsSquare();
        options.root.transform.parent = this.gameObject.transform;

		initializeEvent (currentEvent);

        target = bubbles[0].text;

        GlobalData.worldObject.SetActive(false);

	}

	void initializeEvent(int eventID) {

		if (eventID == eventRon)
		{
			background.GetComponent<SpriteRenderer>().color = new Color (138f/255f, 125f/255f, 188f/255f);

			eventRonG.SetActive(true);
			
			backgroundMusic = gameObject.AddComponent<AudioSource>();
			backgroundMusic.clip = Resources.Load("Music/Ron_Weasel") as AudioClip;
			backgroundMusic.volume = 0f;
			backgroundMusic.loop = true;
            backgroundMusic.pitch = 1f;
			backgroundMusic.Play();

			backgroundMusic2 = gameObject.AddComponent<AudioSource>();
			backgroundMusic2.clip = Resources.Load("Music/Ron_Fireplace") as AudioClip;
			backgroundMusic2.volume = 1f;
			backgroundMusic2.loop = true;
			backgroundMusic2.panStereo = 0.7f;
			backgroundMusic2.Play();

			
			speakers = new GameObject[2];
			speakers[0] = GameObject.Find ("Player");
			speakers[1] = GameObject.Find ("Ron");
			
			int localPhase;
			List<Bubble> auxBubbles;

			// GREETINGS
			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "That's not what I was going to say", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Sorry", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Sometimes I've got premonitions...\nI just said out loud what\n you where going to say", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "That's not what I was going to say", new Vector2(-3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I'm Ron Weasel, but\n you can call me Ron Weasel", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Your face looks familiar,\nI think we've met before\nin another dream", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "One man's dead cat is \nanother man's free dinner", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Wait a moment", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Didn't I owe you 3 gold coins?", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Eh... I mean...", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "... 0 gold coins", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Forget it", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I should stop drinking", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "My hallucinations\nare getting uglier", new Vector2(3f, 3f), null));
			localPhase++;
            auxBubbles.Add(new Bubble(0, localPhase, localPhase, "(ಠ_ಠ)", new Vector2(-3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Wait, are you for real?\nHaha, I was just kidding!", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "There are 2 infinite things\nin the world :", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "The Universe...", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "...and your mother.", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I bet you're naked\nunder those clothes", new Vector2(3f, 3f), null));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
			localPhase++;
			randomGreetings.Add(auxBubbles);


			int randomInt = Random.Range(0, randomGreetings.Count);
			localPhase = 0;

			for (int i = 0; i < randomGreetings[randomInt].Count; i++) {
				bubbles.Add(randomGreetings[randomInt][i]);
				localPhase = randomGreetings[randomInt][i].beginPhase +1;
			}

			bubbles.Add(new Bubble(0, localPhase, localPhase, "...", new Vector2(-3f, 3f), null));
			localPhase++;
			
			bubbles.Add(new Bubble(1, localPhase, localPhase, "Answer my riddle correctly\nand I will give you\nsomething VERY special", new Vector2(3f, 3f), null));
			localPhase++;
			
			float randomQuestion = Random.Range(0f, 1f);
			
			if (randomQuestion < 1f)
			{
				bubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What's the difference\n between a duck?", new Vector2(5f, 3f), null));
				localPhase++;
			}
			
			bubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{"That's not even a proper question", "One of the two is green", "The feathers"}));

		}
		if (eventID == eventDouchebards)
		{
			eventDouchebardsG.SetActive(true);
			
			backgroundMusic = gameObject.AddComponent<AudioSource>();
			backgroundMusic.clip = Resources.Load("Music/Douchebards") as AudioClip;
			backgroundMusic.volume = 1f;
			backgroundMusic.loop = false;
			
			speakers = new GameObject[2];
			speakers[0] = GameObject.Find("Player");
            speakers[1] = GameObject.Find("EventDouchebards/Leader");

            int localPhase;
            List<Bubble> auxBubbles;

            // GREETINGS
            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "You came to the wrong\nneighbourhood, buddy", new Vector2(2f, 3f), null));
            localPhase++;
            randomGreetings.Add(auxBubbles);

            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "PARTY HARD !!", new Vector2(2f, 3f), null));
            localPhase++;
            randomGreetings.Add(auxBubbles);

            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Stand aside, partycrasher\nthe fun is about to start!", new Vector2(2f, 3f), null));
            localPhase++;
            randomGreetings.Add(auxBubbles);

            int randomInt = Random.Range(0, randomGreetings.Count);
            localPhase = 0;

            for (int i = 0; i < randomGreetings[randomInt].Count; i++)
            {
                bubbles.Add(randomGreetings[randomInt][i]);
                localPhase = randomGreetings[randomInt][i].beginPhase + 1;
            }

            float current = 6.1f;
            float tempo = 0.7507f;


            current += 0.285f;

            // PHASE 1
            for (int i = 0; i < 24; i++)
            {
                string direction = "none";
                float auxRand = Random.Range(0f, 100f);
                if (auxRand > 75f) { direction = "left"; }
                else if (auxRand > 50f) { direction = "down"; }
                else if (auxRand > 25f) { direction = "up"; }
                else { direction = "right"; }

                arrowsParty.Add(new arrowParty(current, direction));
                maxPartyPoints++;
                current += tempo;
            }

            // EPIC PHASE


            // PHASE 2
            current = 30.421f;
            tempo = tempo / 2f;

            for (int i = 0; i < 18*2; i++)
            {
                string direction = "none";
                float auxRand = Random.Range(0f, 100f);
                if (auxRand > 75f) { direction = "left"; }
                else if (auxRand > 50f) { direction = "down"; }
                else if (auxRand > 25f) { direction = "up"; }
                else { direction = "right"; }

                arrowsParty.Add(new arrowParty(current, direction));
                maxPartyPoints++;
                current += tempo;
            }

		}

	}


    void addTag(GameObject g, string s)
    {
        addTag(g, s, null);
    }

    void addTag(GameObject g, string s, string s2)
    {

    }

	// Update is called once per frame
	void Update () {

        if (endCondition == 100f)
        {
            fadingMode = 2;
        }

        if (fadingMode == 0f)
        {
            float auxAlpha = fading.GetComponent<SpriteRenderer>().color.a - Time.deltaTime;
            Hacks.SpriteRendererAlpha(fading, auxAlpha);
            if (auxAlpha <= 0f)
            {
                fadingMode = 1;
            }
        }
        else if (fadingMode == 2)
        {
            float aux = fading.GetComponent<SpriteRenderer>().color.a + Time.deltaTime;
            Hacks.SpriteRendererAlpha(fading, aux);
            if (aux >= 1f)
            {
                GlobalData.World();
                Destroy(this.gameObject);
            }
        }

        updateEvent();

        if (currentEvent == eventDouchebards)
        {

            if (backgroundMusic.isPlaying)
            {

                timePartying += Time.deltaTime;
                if (currentArrowParty < arrowsParty.Count && arrowsParty[currentArrowParty].timeToShine - 2f <= timePartying)
                {
                    placeArrow(arrowsParty[currentArrowParty].direction);
                    currentArrowParty++;
                }

                if (!arrowEpic.activeInHierarchy)
                {
                    if (timePartying > 24f)
                    {
                        arrowEpic.SetActive(true);
                    }
                }
                else
                {
                    if (timePartying > 28f && arrowEpic.GetComponent<ParticleSystem>().emissionRate != 0f)
                    {
                        arrowEpic.GetComponent<ParticleSystem>().emissionRate = 0f;
                    }
                }

                moveArrows(ref arrows1, Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A), ref arrowPositive1G);
                moveArrows(ref arrows2, Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S), ref arrowPositive2G);
                moveArrows(ref arrows3, Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W), ref arrowPositive3G);
                moveArrows(ref arrows4, Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D), ref arrowPositive4G);

                checkExpanding(arrowPositive1G);
                checkExpanding(arrowPositive2G);
                checkExpanding(arrowPositive3G);
                checkExpanding(arrowPositive4G);

                checkText(arrowText);

                if (timePartying > 24f && playerAnimated.GetComponent<Animator>().speed == 1f)
                {
                    playerAnimated.GetComponent<Animator>().speed = 2f;
                    partyCombo = 0;
                    arrowText.GetComponent<TextMesh>().text = "FREESTYLE";
                    Hacks.TextAlpha(arrowText, 1f);
                    arrowText.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    arrowText.SetActive(true);
                    arrowTextShowing = -3f;
                }

            }

            if (backgroundMusic.isPlaying && partyHard > 0f)
            {
                partyHard -= Time.deltaTime;

                if (partyHard < 6f && !silhouette1.activeInHierarchy) { silhouette1.SetActive(true); }
                if (partyHard < 4.5f && !silhouette2.activeInHierarchy) { silhouette2.SetActive(true); }
                if (partyHard < 3f && !silhouette3.activeInHierarchy) { silhouette3.SetActive(true); }
                if (partyHard < 1.5f && !silhouette4.activeInHierarchy) { silhouette4.SetActive(true); }

                if (partyHard <= 0f)
                {
                    partyHard = 0f;
                    playerAnimated.GetComponent<Animator>().CrossFade("Dance", GlobalData.crossfadeAnimation / 2f, 0, 0f);
                    light1.SetActive(true);
                    light2.SetActive(true);
                    light3.SetActive(true);
                    Hacks.SpriteRendererColor(background, new Color(0.5f, 0.5f, 0.5f));
                }
            }
            else if (backgroundMusic.isPlaying && partyHard == 0f)
            {

                if (timePartying > 43.5f && silhouette1.activeInHierarchy) { silhouette1.SetActive(false); }
                if (timePartying > 43.9f && silhouette2.activeInHierarchy) { silhouette2.SetActive(false); }
                if (timePartying > 44.3f && silhouette3.activeInHierarchy) { silhouette3.SetActive(false); }
                if (timePartying > 44.7f && silhouette4.activeInHierarchy) { silhouette4.SetActive(false); }

                changePartyLight(light1, ref light1Z);
                changePartyLight(light2, ref light2Z);
                changePartyLight(light3, ref light3Z);

            }


        }	

	}

    private Color changeColor(Color source, float min, float speed)
    {
        Color newColor = source;

        if (source.r == 1f && source.g == min && source.b < 1f)
        {
            newColor = new Color(source.r, source.g, Mathf.Clamp(source.b + Time.deltaTime * speed, min, 1f), source.a);
        }
        else if (source.r > min && source.g == min && source.b == 1f)
        {
            newColor = new Color(Mathf.Clamp(source.r - Time.deltaTime * speed, min, 1f), source.g, source.b, source.a);
        }
        else if (source.r == min && source.g < 1f && source.b == 1f)
        {
            newColor = new Color(source.r, Mathf.Clamp(source.g + Time.deltaTime * speed, min, 1f), source.b, source.a);
        }
        else if (source.r == min && source.g == 1f && source.b > min)
        {
            newColor = new Color(source.r, source.g, Mathf.Clamp(source.b - Time.deltaTime * speed, min, 1f), source.a);
        }
        else if (source.r < 1f && source.g == 1f && source.b == min)
        {
            newColor = new Color(Mathf.Clamp(source.r + Time.deltaTime * speed, min, 1f), source.g, source.b, source.a);
        }
        else if (source.r == 1f && source.g > min && source.b == min)
        {
            newColor = new Color(source.r, Mathf.Clamp(source.g - Time.deltaTime * speed, min, 1f), source.b, source.a);
        }

        return newColor;
    }

    private void changePartyLight(GameObject light, ref float desiredAngle)
    {

        float min = 128f / 256f;
        float speed = 2f;

        Hacks.SpriteRendererColor(light, changeColor(light.GetComponent<SpriteRenderer>().color, min, speed));

        float fixedAngle = light.transform.parent.localEulerAngles.z;
        if (fixedAngle > 60f) { fixedAngle -= 360f; }

        if (Mathf.Abs((fixedAngle - desiredAngle)) < 1f)
        {
            desiredAngle = Random.Range(-60f, 60f);
        }

        light.transform.parent.localEulerAngles = new Vector3(light.transform.parent.localEulerAngles.x, light.transform.parent.localEulerAngles.y, Mathf.LerpAngle(light.transform.parent.localEulerAngles.z, desiredAngle, Time.deltaTime * 10f));
           

    }

	private void updateEvent() {

		if (backgroundMusic.volume < 0.5f)
		{
			backgroundMusic.volume += Time.deltaTime/2f;
			if (backgroundMusic.volume > 0.5f)
			{
				backgroundMusic.volume = 0.5f;
			}
		}
		
		if (nextPhase == 1)
		{
			globalPhase++;
			
			if (toRecycle.Count > 0)
			{
				closeSpeech.Play();
			}
			
			for (int i = 0; i < toRecycle.Count; i++)
			{
				speechBubblePool[toRecycle[i]].text.GetComponent<TextMesh>().text = "";
				speechBubblePool[toRecycle[i]].currentLetter = 0;
				speechBubblePool[toRecycle[i]].root.SetActive(false);
			}
		}
		nextPhase = 0;
		toRecycle = new List<int>();
		
		int currentSpeechBubble = 0;
        bool writingSomething = false;
		
		for (int i = 0; i < bubbles.Count; i++)
		{
			if (bubbles[i].beginPhase <= globalPhase && bubbles[i].endPhase >= globalPhase)
			{

                writingSomething = true;
				
				if (bubbles[i].options == null)
				{
					// ES SOLO TEXTO
					if (speechBubblePool.Count <= currentSpeechBubble)
					{
						SpeechBubble s = new SpeechBubble();
                        s.root.transform.parent = this.gameObject.transform;
						s.root.SetActive(false);
						speechBubblePool.Add(s);
					}
					Write(bubbles[i], speechBubblePool[currentSpeechBubble]);
					currentSpeechBubble++;
				}
				else
				{
					// SON OPCIONES
					Write(bubbles[i], options);
				}
				
			}
		}

        if (!writingSomething && currentEvent == eventRon && endCondition == 50f)
        {
            endCondition = 100f;
        }
        else if (!writingSomething && currentEvent == eventDouchebards && endCondition == 50f)
        {
            endCondition = 100f;
        }
		
		for (int i = currentSpeechBubble; i < speechBubblePool.Count; i++)
		{
			speechBubblePool[i].root.SetActive(false);
		}

        if (!writingSomething && currentEvent == eventDouchebards && !backgroundMusic.isPlaying)
        {
            if (partyHard > 0f)
            {
                if (partyHard != 42f) {
                    backgroundMusic.Play();
                }
            }
            else
            {

                float alpha = light1.GetComponent<SpriteRenderer>().color.a - Time.deltaTime*2f;
                if (alpha < 0f) { alpha = 0f; }

                Hacks.SpriteRendererAlpha(light1, alpha);
                Hacks.SpriteRendererAlpha(light2, alpha);
                Hacks.SpriteRendererAlpha(light3, alpha);

                float color = background.GetComponent<SpriteRenderer>().color.r +Time.deltaTime;
                if (color > 1f) { color = 1f; }
                Hacks.SpriteRendererColor(background, new Color(color, color, color));

                if (alpha == 0f && color == 1f)
                {
                    // ADD THE RESPONSE FROM DOUCHEBARDS
                    if ((float)partyPoints/(float)maxPartyPoints < 50f/100f)
                    {
                        // MAL
                        float auxRand = Random.Range(0f, 1f);
                        if (auxRand < 1f / 3f)
                        {
                            bubbles.Add(new Bubble(1, globalPhase, globalPhase, "You're not at our\nlevel, gobermouch", new Vector2(2f, 3f), null));
                        }
                        else if (auxRand < 2f / 3f)
                        {
                            bubbles.Add(new Bubble(1, globalPhase, globalPhase, "You ruined all the fun\nGet lost, smell-feast!", new Vector2(2f, 3f), null));
                        }
                        else
                        {
                            bubbles.Add(new Bubble(1, globalPhase, globalPhase, "GTFO, cumberworld!", new Vector2(2f, 3f), null));
                        }
                    }
                    else if ((float)partyPoints / (float)maxPartyPoints < 80f/100f)
                    {
                        // BIEN (RECOMPENSA)
                        bubbles.Add(new Bubble(1, globalPhase, globalPhase, "Not bad...\nHere's your reward", new Vector2(2f, 3f), null));
                    }
                    else if ((float)partyPoints / (float)maxPartyPoints <  100f/100f)
                    {
                        // MUY BIEN (RECOMPENSA)
                        float auxRand = Random.Range(0f, 1f);
                        if (auxRand < 1f / 3f)
                        {
                            bubbles.Add(new Bubble(1, globalPhase, globalPhase, "Beautiful! Almost perfect!\n", new Vector2(2f, 3f), null));
                            bubbles.Add(new Bubble(1, globalPhase + 1, globalPhase + 1, "You're the most effulgent\nmotherfucker I've seen in a while", new Vector2(2f, 3f), null));
                            bubbles.Add(new Bubble(1, globalPhase + 2, globalPhase + 2, "Take this cool\nrandomly selected item", new Vector2(2f, 3f), null));
                        }
                        else if (auxRand < 2f / 3f)
                        {
                            bubbles.Add(new Bubble(1, globalPhase, globalPhase, "Bro, so close\n to perfect score!", new Vector2(2f, 3f), null));
                            bubbles.Add(new Bubble(1, globalPhase + 1, globalPhase + 1, "But you're a one recalcitrant\nasshole, that's for sure", new Vector2(2f, 3f), null));
                            bubbles.Add(new Bubble(1, globalPhase + 2, globalPhase + 2, "Take this item, I can't\nuse it since I'm an NPC", new Vector2(2f, 3f), null));
                        }
                        else
                        {
                            bubbles.Add(new Bubble(1, globalPhase, globalPhase, "That was so close!", new Vector2(2f, 3f), null));
                            bubbles.Add(new Bubble(1, globalPhase +1 , globalPhase + 1, "Such a tenacious scumbag\ndeserves an appropriate reward", new Vector2(2f, 3f), null));
                        }
                    }
                    else 
                    {
                        // PERFECTO
                        float auxRand = Random.Range(0f, 1f);
                        bubbles.Add(new Bubble(1, globalPhase, globalPhase, "HOLY SHIT !!\n100% PERFECT", new Vector2(2f, 3f), null));

                        bubbles.Add(new Bubble(1, globalPhase + 1, globalPhase + 1, "I'm going to tattoo\nyour face on my butt!", new Vector2(2f, 3f), null));
                        bubbles.Add(new Bubble(1, globalPhase + 2, globalPhase + 2, "Take this\nyou've earned it", new Vector2(2f, 3f), null));

                    }

                    playerAnimated.GetComponent<Animator>().speed = 1f;
                    playerAnimated.GetComponent<Animator>().CrossFade("Idle", GlobalData.crossfadeAnimation / 2f, 0, 0f);

                    // ESTO ES PARA QUE NO VUELVA A SONAR LA MUSICA NI NADA
                    partyHard = 42f;

                    endCondition = 50f;

                }

            }
            
        }

	}

    private void Write(Bubble b, SpeechBubble s)
    {

        if (!s.root.activeInHierarchy)
        {
            s.text.GetComponent<TextMesh>().text = "";
            s.bubble.transform.localScale = new Vector3(0f, 0f, 0);
            s.bubble2.transform.localScale = new Vector3(0f, 0f, 0.1f);
            s.timer = -0.1f;
            s.root.SetActive(true);
        }

        string target = b.text;
        string currentString = s.text.GetComponent<TextMesh>().text;
        s.text.GetComponent<TextMesh>().text = target;
        s.text.transform.position = new Vector3(b.position.x - s.text.GetComponent<Renderer>().bounds.size.x / 2f,b.position.y + s.text.GetComponent<Renderer>().bounds.size.y / 2f, s.text.transform.position.z);
        float scaleX = s.text.GetComponent<Renderer>().bounds.size.x;
        float scaleY = s.text.GetComponent<Renderer>().bounds.size.y;
		if (scaleX > scaleY) { scaleY += 0.5f; }
		if (scaleY > scaleX) { scaleX += 0.5f; }
        //if (scaleX < scaleY * 0.5f) { scaleX = scaleY * 0.5f; }
        //if (scaleY < scaleX * 0.25f) { scaleY = scaleX * 0.25f; }
        s.bubble.transform.localScale = new Vector3(Mathf.Lerp(s.bubble.transform.localScale.x, (scaleX) * 0.7f, Time.deltaTime * 20f), Mathf.Lerp(s.bubble.transform.localScale.y, (scaleY) * 0.7f, Time.deltaTime * 20f), 1f);
        s.bubble.transform.position = new Vector3(s.text.GetComponent<Renderer>().bounds.center.x, s.text.GetComponent<Renderer>().bounds.center.y, s.bubble.transform.position.z);
        s.bubble2.transform.localScale = new Vector3(s.bubble.transform.localScale.x + lineWidth, s.bubble.transform.localScale.y + lineWidth, s.bubble2.transform.localScale.z);
        s.bubble2.transform.position = s.bubble.transform.position + new Vector3(0f, 0f, 0.1f);
      	
		float min = s.bubble.transform.localScale.x;
		if (s.bubble.transform.localScale.y < min) { min = s.bubble.transform.localScale.y; }

		Vector3 diff = s.triangle.transform.position - speakers [b.speaker].transform.position;
		diff.Normalize();
		
		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		
		s.triangle.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);//speakers [b.speaker].transform;
		s.triangle2.transform.rotation = s.triangle.transform.rotation;

		s.triangle.transform.localScale = new Vector3(min/3f, min/3f, min/3f);

		Vector2 director = new Vector2 (speakers [b.speaker].transform.position.x -s.bubble.GetComponent<Renderer>().bounds.center.x, speakers [b.speaker].transform.position.y -s.bubble.GetComponent<Renderer>().bounds.center.y);
		director.Normalize ();

		s.triangle.transform.position = new Vector3 (s.bubble.GetComponent<Renderer>().bounds.center.x + (s.bubble.GetComponent<Renderer>().bounds.size.y/2f)*director.x, s.bubble.GetComponent<Renderer>().bounds.center.y  -s.bubble.GetComponent<Renderer>().bounds.size.y/2f , s.triangle.transform.position.z);

        s.triangle2.transform.localScale = new Vector3(s.triangle.transform.localScale.x + lineWidth, s.triangle.transform.localScale.y + lineWidth, s.triangle2.transform.localScale.z);
		s.triangle2.transform.position = new Vector3 (s.triangle.transform.position.x, s.triangle.transform.position.y, s.triangle2.transform.position.z) +s.triangle2.transform.up*-0.04f;

		s.text.GetComponent<TextMesh>().text = currentString;

        if (Input.GetKeyDown(KeyCode.Return) && s.currentLetter > 0)
        {
            if (true)
            {
                if (s.currentLetter < target.Length)
                {
                    s.currentLetter = target.Length - 1;
                    nextPhase = -1;
                }
                else
                {
                    if (nextPhase == 0)
                    {
                        nextPhase = 1;
                    }

                    if (globalPhase + 1 > b.endPhase && nextPhase == 1)
                    {
                        toRecycle.Add(speechBubblePool.IndexOf(s));
                    }

                }
            }
        }

        if (s.currentLetter < target.Length)
        {

            s.timer += Time.deltaTime;

            if (s.timer > 0.05f)
            {

                s.currentLetter++;

                while (s.currentLetter < target.Length - 1 && target[s.currentLetter] == ' ')
                {
                    s.currentLetter++;
                }

                s.timer = 0f;
                s.text.GetComponent<TextMesh>().text = target.Substring(0, s.currentLetter);

                textSounds[Random.Range(0, textSounds.Length)].Play();

            }

        }

    }


    private void Write(Bubble b, OptionsSquare o)
    {

        if (!o.root.activeInHierarchy)
        {
            o.optionText[0].GetComponent<TextMesh>().text = "";
            o.optionText[1].GetComponent<TextMesh>().text = "";
            o.optionText[2].GetComponent<TextMesh>().text = "";
            o.squareMiddle.transform.localScale = new Vector3(0f, 0f, 0f);
            o.squareTop.transform.localScale = new Vector3(0f, 0f, 0f);
            o.squareBottom.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner1.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner2.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner3.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner4.transform.localScale = new Vector3(0f, 0f, 0f);
            o.squareMiddleBack.transform.localScale = new Vector3(0f, 0f, 0f);
            o.squareTopBack.transform.localScale = new Vector3(0f, 0f, 0f);
            o.squareBottomBack.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner1Back.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner2Back.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner3Back.transform.localScale = new Vector3(0f, 0f, 0f);
            o.corner4Back.transform.localScale = new Vector3(0f, 0f, 0f);
            o.timer = -0.1f;
            o.root.SetActive(true);
        }

        float scaleX = 0f;
        float scaleY = 0;

        for (int i = 0; i < b.options.Length; i++)
        {
            string target = b.options[i];
            string currentString = o.optionText[i].GetComponent<TextMesh>().text;
            o.optionText[i].GetComponent<TextMesh>().text = target;
            if (o.optionText[i].GetComponent<Renderer>().bounds.size.x > scaleX)
            {
                scaleX = o.optionText[i].GetComponent<Renderer>().bounds.size.x;
            }
            o.optionText[i].transform.localPosition = new Vector3(o.optionText[i].transform.localPosition.x, -scaleY -o.optionText[i].GetComponent<Renderer>().bounds.size.y/2f, o.optionText[i].transform.localPosition.z);
            scaleY += o.optionText[i].GetComponent<Renderer>().bounds.size.y;
        }

        for (int i = 0; i < b.options.Length; i++)
        {
            o.optionText[i].transform.localPosition = new Vector3(-scaleX / 2f, o.optionText[i].transform.localPosition.y +scaleY/2f, o.optionText[i].transform.localPosition.z);
            o.optionText[i].GetComponent<TextMesh>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            if (o.selected == i)
            {
                o.optionText[i].GetComponent<TextMesh>().color = new Color(0f, 0f, 0f, 1f);
            }
        }

        scaleX += 0.5f;

        o.squareMiddle.transform.localScale = new Vector3(Mathf.Lerp(o.squareMiddle.transform.localScale.x, (scaleX) * 105f, Time.deltaTime * 20f), Mathf.Lerp(o.squareMiddle.transform.localScale.y, (scaleY) * 105f, Time.deltaTime * 20f), o.squareMiddle.transform.localScale.z);
        o.squareMiddleBack.transform.localScale = new Vector3(o.squareMiddle.transform.localScale.x + lineWidth * 195f, o.squareMiddle.transform.localScale.y + lineWidth * 195f, o.squareMiddle.transform.localScale.z);

        o.corner1.transform.localScale = new Vector3(Mathf.Lerp(o.corner1.transform.localScale.x, 0.2f, Time.deltaTime * 20f), Mathf.Lerp(o.corner1.transform.localScale.y, 0.2f, Time.deltaTime * 20f), 1f);
        o.corner1.transform.localPosition = new Vector3(+o.corner1.GetComponent<Renderer>().bounds.size.x / 2f - o.squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, o.corner1.GetComponent<Renderer>().bounds.size.y / 2f + o.squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        o.corner1Back.transform.localScale = new Vector3(o.corner1.transform.localScale.x + lineWidth, o.corner1.transform.localScale.y + lineWidth, o.corner1.transform.localScale.z);
        o.corner1Back.transform.localPosition = new Vector3(o.corner1.transform.localPosition.x, o.corner1.transform.localPosition.y, o.corner1Back.transform.localPosition.z);

        o.corner2.transform.localScale = new Vector3(Mathf.Lerp(o.corner2.transform.localScale.x, -0.2f, Time.deltaTime * 20f), Mathf.Lerp(o.corner2.transform.localScale.y, 0.2f, Time.deltaTime * 20f), 1f);
        o.corner2.transform.localPosition = new Vector3(-o.corner2.GetComponent<Renderer>().bounds.size.x / 2f + o.squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, o.corner2.GetComponent<Renderer>().bounds.size.y / 2f + o.squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        o.corner2Back.transform.localScale = new Vector3(o.corner2.transform.localScale.x - lineWidth, o.corner2.transform.localScale.y + lineWidth, o.corner2.transform.localScale.z);
        o.corner2Back.transform.localPosition = new Vector3(o.corner2.transform.localPosition.x, o.corner2.transform.localPosition.y, o.corner2Back.transform.localPosition.z);

        o.corner3.transform.localScale = new Vector3(Mathf.Lerp(o.corner3.transform.localScale.x, 0.2f, Time.deltaTime * 20f), Mathf.Lerp(o.corner3.transform.localScale.y, -0.2f, Time.deltaTime * 20f), 1f);
        o.corner3.transform.localPosition = new Vector3(o.corner3.GetComponent<Renderer>().bounds.size.x / 2f - o.squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, -o.corner3.GetComponent<Renderer>().bounds.size.y / 2f - o.squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        o.corner3Back.transform.localScale = new Vector3(o.corner3.transform.localScale.x + lineWidth, o.corner3.transform.localScale.y - lineWidth, o.corner3.transform.localScale.z);
        o.corner3Back.transform.localPosition = new Vector3(o.corner3.transform.localPosition.x, o.corner3.transform.localPosition.y, o.corner3Back.transform.localPosition.z);

        o.corner4.transform.localScale = new Vector3(Mathf.Lerp(o.corner4.transform.localScale.x, -0.2f, Time.deltaTime * 20f), Mathf.Lerp(o.corner4.transform.localScale.y, -0.2f, Time.deltaTime * 20f), 1f);
        o.corner4.transform.localPosition = new Vector3(-o.corner4.GetComponent<Renderer>().bounds.size.x / 2f + o.squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, -o.corner4.GetComponent<Renderer>().bounds.size.y / 2f - o.squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        o.corner4Back.transform.localScale = new Vector3(o.corner4.transform.localScale.x - lineWidth, o.corner4.transform.localScale.y - lineWidth, o.corner4.transform.localScale.z);
        o.corner4Back.transform.localPosition = new Vector3(o.corner4.transform.localPosition.x, o.corner4.transform.localPosition.y, o.corner4Back.transform.localPosition.z);

        o.squareTop.transform.localScale = new Vector3(Mathf.Lerp(o.squareTop.transform.localScale.x, (scaleX - o.corner1.GetComponent<Renderer>().bounds.size.x * 1.9f) * 105f, Time.deltaTime * 20f), Mathf.Lerp(o.squareTop.transform.localScale.y, 0.4f * 105f * 0.96f, Time.deltaTime * 20f), o.squareTop.transform.localScale.z);
        o.squareTop.transform.localPosition = new Vector3(0f, o.squareTop.GetComponent<Renderer>().bounds.size.y / 2f * 0.98f + o.squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        o.squareTopBack.transform.localScale = new Vector3(o.squareTop.transform.localScale.x + lineWidth * 195f, o.squareTop.transform.localScale.y + lineWidth * 195f, o.squareTopBack.transform.localScale.z);
        o.squareTopBack.transform.localPosition = new Vector3(o.squareTop.transform.localPosition.x, o.squareTop.transform.localPosition.y, o.squareTopBack.transform.localPosition.z);

        o.squareBottom.transform.localScale = new Vector3(Mathf.Lerp(o.squareBottom.transform.localScale.x, (scaleX - o.corner1.GetComponent<Renderer>().bounds.size.x * 1.9f) * 105f, Time.deltaTime * 20f), Mathf.Lerp(o.squareBottom.transform.localScale.y, 0.4f * 105f * 0.96f, Time.deltaTime * 20f), o.squareBottom.transform.localScale.z);
        o.squareBottom.transform.localPosition = new Vector3(0f, -o.squareBottom.GetComponent<Renderer>().bounds.size.y / 2f * 0.98f - o.squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        o.squareBottomBack.transform.localScale = new Vector3(o.squareBottom.transform.localScale.x + lineWidth * 195f, o.squareBottom.transform.localScale.y + lineWidth * 195f, o.squareBottomBack.transform.localScale.z);
        o.squareBottomBack.transform.localPosition = new Vector3(o.squareBottom.transform.localPosition.x, o.squareBottom.transform.localPosition.y, o.squareBottomBack.transform.localPosition.z);

        o.pointer.transform.localPosition = new Vector3(-scaleX / 2f - o.pointer.GetComponent<Renderer>().bounds.size.x / 2f + 0.15f, Mathf.Lerp(o.pointer.transform.localPosition.y, o.optionText[o.selected].transform.localPosition.y, Time.deltaTime*20f), o.pointer.transform.localPosition.z);

        float height = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f)).y;
        o.root.transform.position = new Vector3(0f, -height + scaleY / 2f + o.squareBottom.GetComponent<Renderer>().bounds.size.y*3f, o.root.transform.position.z);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            o.selected++;
            if (o.selected >= b.options.Length)
            {
                o.selected = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            o.selected--;
            if (o.selected < 0)
            {
                o.selected = b.options.Length-1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return)) {

            if (nextPhase == 0)
            {
                nextPhase = 1;
            }

            //menuOk.Play();

            int localPhase = globalPhase + 1;

            bubbles.Add(new Bubble(0, localPhase, localPhase+1, b.options[o.selected], new Vector2(-3f, 3f), null));
            localPhase++;

			if (b.options[o.selected] == "That's not even a proper question")
            {
				bubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
                localPhase++;
            }
			if (b.options[o.selected] == "One of the two is green")
            {
                bubbles.Add(new Bubble(1, localPhase, localPhase, "How did you know it?!", new Vector2(3f, 3f), null));
                localPhase++;
            }
			if (b.options[o.selected] == "The feathers")
            {
                bubbles.Add(new Bubble(1, localPhase, localPhase, "Wrong answer!", new Vector2(3f, 3f), null));
                localPhase++;
            }
            
            o.root.SetActive(false);

            bubbles.Add(new Bubble(1, localPhase, localPhase, "And remember...", new Vector2(3f, 3f), null));
            localPhase++;

            int originalLocalPhase = localPhase;
            List<Bubble> auxBubbles;

            // FAREWELLS
            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Cigar smoothies are\nbeyond good and evil", new Vector2(3f, 3f), null));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Without X, life would\nbe an error", new Vector2(3f, 3f), null));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "The last thing one\nloses... it's memories\n*sigh*", new Vector2(3f, 3f), null));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Millennium Hand and Shrimp!", new Vector2(3f, 3f), null));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Sometimes happiness is\nat the bottom of\na pickle jar", new Vector2(3f, 3f), null));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Those who believe that\nmagic does everything", new Vector2(3f, 3f), null));
            localPhase++;
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "end up doing\neverything for magic", new Vector2(3f, 3f), null));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            int randomInt = Random.Range(0, randomFarewells.Count);

            for (int i = 0; i < randomFarewells[randomInt].Count; i++)
            {
                bubbles.Add(randomFarewells[randomInt][i]);
            }

            endCondition = 50f;

        }

    }


    private class Bubble
    {

        public int speaker;
        public int beginPhase;
        public int endPhase;
        public string text;
        public Vector2 position;
        public string[] options;

        public Bubble(int auxSpeaker, int auxBeginPhase, int auxEndPhase, string auxText, Vector2 auxPosition, string[] auxOptions)
        {
            speaker = auxSpeaker;
            beginPhase = auxBeginPhase;
            endPhase = auxEndPhase;
            text = Hacks.TextMultilineCentered(speechBubblePool[0].text, auxText);
            position = auxPosition;
            options = auxOptions;
        }

    }

    private class SpeechBubble
    {

        public GameObject root;
        public GameObject text;
        public GameObject bubble;
        public GameObject bubble2;
		public GameObject triangle;
		public GameObject triangle2;
        public int currentLetter = 0;
        public float timer = 0f;

        public SpeechBubble()
        {
            root = Instantiate(Resources.Load("Prefabs/SpeechBubble")) as GameObject;
            text = root.transform.FindChild("Text").gameObject;
            bubble = root.transform.FindChild("Bubble").gameObject;
            bubble2 = root.transform.FindChild("Bubble2").gameObject;
			triangle = root.transform.FindChild("Triangle").gameObject;
			triangle2 = root.transform.FindChild("Triangle2").gameObject;
        }

    }

    private class OptionsSquare
    {

        public GameObject root;
        public GameObject[] optionText = new GameObject[3];
        public GameObject squareMiddle;
        public GameObject squareTop;
        public GameObject squareBottom;
        public GameObject corner1;
        public GameObject corner2;
        public GameObject corner3;
        public GameObject corner4;
        public GameObject squareMiddleBack;
        public GameObject squareTopBack;
        public GameObject squareBottomBack;
        public GameObject corner1Back;
        public GameObject corner2Back;
        public GameObject corner3Back;
        public GameObject corner4Back;
        public GameObject pointer;
        public int currentLetter = 0;
        public float timer = 0f;
        public int selected = 0;

        public OptionsSquare()
        {

            root = Instantiate(Resources.Load("Prefabs/OptionsSquare")) as GameObject;
            optionText[0] = root.transform.FindChild("Option1").gameObject;
            optionText[1] = root.transform.FindChild("Option2").gameObject;
            optionText[2] = root.transform.FindChild("Option3").gameObject;
            squareMiddle = root.transform.FindChild("SquareMiddle").gameObject;
            squareTop = root.transform.FindChild("SquareTop").gameObject;
            squareBottom = root.transform.FindChild("SquareBottom").gameObject;
            corner1 = root.transform.FindChild("Corner1").gameObject;
            corner2 = root.transform.FindChild("Corner2").gameObject;
            corner3 = root.transform.FindChild("Corner3").gameObject;
            corner4 = root.transform.FindChild("Corner4").gameObject;
            squareMiddleBack = root.transform.FindChild("SquareMiddleBack").gameObject;
            squareTopBack = root.transform.FindChild("SquareTopBack").gameObject;
            squareBottomBack = root.transform.FindChild("SquareBottomBack").gameObject;
            corner1Back = root.transform.FindChild("Corner1Back").gameObject;
            corner2Back = root.transform.FindChild("Corner2Back").gameObject;
            corner3Back = root.transform.FindChild("Corner3Back").gameObject;
            corner4Back = root.transform.FindChild("Corner4Back").gameObject;
            pointer = root.transform.FindChild("Pointer").gameObject;
            root.SetActive(false);
            
        }

    }

    private class arrowParty
    {
        public float timeToShine = 0f;
        public string direction = "none";

        public arrowParty(float time, string dir)
        {
            timeToShine = time;
            direction = dir;
        }

    }

    private void placeArrow(string direction)
    {
        List<GameObject> targetList = new List<GameObject>();

        if (direction == "left") { targetList = arrows1; }
        else if (direction == "down") { targetList = arrows2; }
        else if (direction == "up") { targetList = arrows3; }
        else if (direction == "right") { targetList = arrows4; }

        for (int i = 0; i < targetList.Count; i++)
        {
            if (!targetList[i].activeInHierarchy)
            {
                targetList[i].transform.localPosition = new Vector3(targetList[i].transform.localPosition.x, -10f, targetList[i].transform.localPosition.z);
                targetList[i].SetActive(true);
                break;
            }
        }

    }

    private void moveArrows(ref List<GameObject> list, bool correctInput, ref GameObject positive)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].activeInHierarchy)
            {
                list[i].transform.localPosition = new Vector3(list[i].transform.localPosition.x, list[i].transform.localPosition.y + Time.deltaTime * 5f, list[i].transform.localPosition.z);
                if (list[i].transform.localPosition.y >= -0.8f && list[i].transform.localPosition.y <= 0.8f && correctInput)
                {
                    if (partyCombo < 0) { partyCombo = 0; }
                    partyCombo++;
                    int step = 3;
                    float auxScale = 0.05f + ((float)partyCombo/36f)*0.15f;

                    if (partyCombo % step == 0 && textGoodCombo.Count >= partyCombo/step)
                    {
                        arrowText.GetComponent<TextMesh>().text = textGoodCombo[-1 +partyCombo/step];
                        Hacks.TextAlpha(arrowText, 1f);
                        arrowText.transform.localScale = new Vector3(auxScale, auxScale, auxScale);
                        arrowText.SetActive(true);
                        arrowTextShowing = 0f;
                    }

                    partyPoints++;
                    list[i].SetActive(false);
                    positive.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    Hacks.SpriteRendererAlpha(positive, 1f);
                    positive.SetActive(true);
                }
                if (list[i].transform.localPosition.y > 0.8f && list[i].GetComponent<SpriteRenderer>().color.a == 1f)
                {
                    if (partyCombo > 0) { partyCombo = 0; }
                    partyCombo--;

                    int step = 3;
                    float auxScale = 0.05f + ((float)-partyCombo / 36f) * 0.15f;

                    if (-partyCombo % step == 0 && textBadCombo.Count >= -partyCombo / step)
                    {
                        arrowText.GetComponent<TextMesh>().text = textBadCombo[-1 - partyCombo / step];
                        Hacks.TextAlpha(arrowText, 1f);
                        arrowText.transform.localScale = new Vector3(auxScale, auxScale, auxScale);
                        arrowText.SetActive(true);
                        arrowTextShowing = 0f;
                    }
                    Hacks.SpriteRendererAlpha(list[i], 0.9999999f);
                }
                if (list[i].transform.localPosition.y >= 3f) {
                    Hacks.SpriteRendererAlpha(list[i], 1f);
                    list[i].SetActive(false); 
                }
            }
        }
    }

    private void checkExpanding(GameObject g)
    {
        if (g.activeInHierarchy)
        {
            Hacks.SpriteRendererAlpha(g, g.GetComponent<SpriteRenderer>().color.a - Time.deltaTime * 4f);
            float scale = g.transform.localScale.x + Time.deltaTime;
            g.transform.localScale = new Vector3(scale, scale, scale);
            if (g.GetComponent<SpriteRenderer>().color.a <= 0f)
            {
                g.SetActive(false);
            }
        }
    }

    private void checkText(GameObject g)
    {
        if (g.activeInHierarchy)
        {
            arrowTextShowing += Time.deltaTime;

            
            if (arrowTextShowing > 1f)
            {
                Hacks.TextAlpha(g, g.GetComponent<TextMesh>().color.a - Time.deltaTime * 3f);
            }

            if (arrowTextShowing > 0f)
            {
                float scale = g.transform.localScale.x + Time.deltaTime / 12f;
                g.transform.localScale = new Vector3(scale, scale, scale);
            }
            
            

            

            Color c = changeColor(g.GetComponent<TextMesh>().color, 0.4f, 2f);
            g.GetComponent<TextMesh>().color = c;

            if (g.GetComponent<TextMesh>().color.a <= 0f)
            {
                g.SetActive(false);
                arrowTextShowing = 0f;
            }
        }
    }

}
