using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkScript : MonoBehaviour {


    private AudioSource backgroundMusic;
	private AudioSource backgroundMusic2;
    private AudioSource menuOk;

	private GameObject background;

	private string target;

    private SpeechManager speechManager;

    private GameObject fading;
    private float fadingMode = 0f;

    private int currentEvent;

    private float partyHard = 6.1f;
    private int maxPartyPoints = 0;
    private int partyPoints = 0;
    private int partyCombo = 0;
    private float timePartying = 0f;
    private List<arrowParty> arrowsParty = new List<arrowParty>();
    private int currentArrowParty = 0;


	private List<List<Bubble>> randomGreetings = new List<List<Bubble>>();
	private List<List<Bubble>> randomRiddles = new List<List<Bubble>>();


    private GameObject eventRonG;
    private GameObject eventDouchebardsG;
    private GameObject eventExpertG;

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

    private string expertSubject;

    private GameObject expertItem1;
    private GameObject expertItem2;
    private GameObject expertItem3;
    private GameObject expertTheBest;

    private GameObject playerAnimated;

    private float lineWidth = 0.035f;

    private float endCondition = 0f;

	// Use this for initialization
	void Start () {

        currentEvent = GlobalData.currentSpecialEvent;

        speechManager = new SpeechManager(this.gameObject, currentEvent);

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

        expertItem1 = GameObject.Find("EventExpert/Item1");
        expertItem2 = GameObject.Find("EventExpert/Item2");
        expertItem3 = GameObject.Find("EventExpert/Item3");
        expertTheBest = GameObject.Find("EventExpert/TheBest"); ;

        eventExpertG = GameObject.Find("EventExpert");
        eventExpertG.SetActive(false);
        
        menuOk = gameObject.AddComponent<AudioSource>();
        menuOk.clip = Resources.Load("Music/Menu/MenuOk") as AudioClip;
        menuOk.volume = 0.45f;

		initializeEvent (currentEvent);

        //target = bubbles[0].text;

        GlobalData.worldObject.SetActive(false);

	}

	void initializeEvent(int eventID) {

		if (eventID == GlobalData.eventRon)
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

			
			speechManager.addSpeaker(GameObject.Find("Player"));
            speechManager.addSpeaker(GameObject.Find("Ron"));
			
			int localPhase;
			List<Bubble> auxBubbles;

			// GREETINGS
			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "That's not what I was going to say", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Sorry", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Sometimes I've got premonitions...\nI just said out loud what\n you where going to say", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "That's not what I was going to say", new Vector2(-3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f)));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I'm Ron Weasel, but\n you can call me Ron Weasel", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f)));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Your face looks familiar,\nI think we've met before\nin another dream", new Vector2(3f, 3f)));
			localPhase++;
            auxBubbles.Add(new Bubble(0, localPhase, localPhase, "(ಠ_ಠ)", new Vector2(-3f, 3f)));
            localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "One man's dead cat is \nanother man's free dinner", new Vector2(3f, 3f)));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Wait a moment", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Didn't I owe you 3 gold coins?", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Eh... I mean...", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "... 0 gold coins", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Forget it", new Vector2(3f, 3f)));
			localPhase++;
            auxBubbles.Add(new Bubble(0, localPhase, localPhase, "...", new Vector2(-3f, 3f)));
            localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I should stop drinking", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "My hallucinations\nare getting uglier", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "(ಠ_ಠ)", new Vector2(-3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Wait, are you for real?\nHaha, I was just kidding!", new Vector2(3f, 3f)));
			localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "There are 2 infinite things\nin the world :", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "The Universe...", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "...and my magic wand", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f)));
			localPhase++;
            auxBubbles.Add(new Bubble(0, localPhase, localPhase, "...", new Vector2(-3f, 3f)));
            localPhase++;
			randomGreetings.Add(auxBubbles);

			localPhase = 0;
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I bet you're naked\nunder those clothes", new Vector2(3f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f)));
			localPhase++;
			randomGreetings.Add(auxBubbles);


			int randomInt = Random.Range(0, randomGreetings.Count);
			localPhase = 0;

			for (int i = 0; i < randomGreetings[randomInt].Count; i++) {
				speechManager.bubbles.Add(randomGreetings[randomInt][i]);
				localPhase = randomGreetings[randomInt][i].beginPhase +1;
			}


            speechManager.bubbles.Add(new Bubble(1, localPhase, localPhase, "Answer my riddle correctly\nand I will give you\nsomething VERY special", new Vector2(3f, 3f)));
			localPhase++;

			// RIDDLES
			int auxLocalPhase = localPhase;

			List<string> responses = new List<string>();
			List<string> auxResponses;
			string[] currentResponses;
			int auxCorrect;
			// SON RESPUESTAS
			responses.Add("A mushroom");
			responses.Add("The outside");
			responses.Add("One of the two is green");
			responses.Add("A bubblegum");
			responses.Add("An egg");
			responses.Add("Your sister");
			responses.Add("Tomorrow");
			responses.Add("Senpai");
			responses.Add("Water");
			responses.Add("A pussy");
			responses.Add("An armpit");
			responses.Add("Fire");
			responses.Add("A table");
			// EXTRA
			responses.Add("Fuck you, Ron");
			responses.Add("( ͡° ͜ʖ ͡°)");

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "One of the two is green";
			auxResponses.Remove("One of the two is green");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What's the difference\nbetween a duck?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "A mushroom";
			auxResponses.Remove("A mushroom");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What kind of room\nhas no doors or windows?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "The outside";
			auxResponses.Remove("The outside");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "Which side of a cat\nhas the most fur?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "A bubblegum";
			auxResponses.Remove("A bubblegum");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "Blow me hard\nand I will get bigger\nWhat am I?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "An egg";
			auxResponses.Remove("An egg");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What has to be broken\nbefore you can use it?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "Your sister";
			auxResponses.Remove("Your sister");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "You are my brother\nbut I am not your brother\nWho am I?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "Tomorrow";
			auxResponses.Remove("Tomorrow");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What's always coming\nbut never arrives?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "Senpai";
			auxResponses.Remove("Senpai");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "Notice me", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "Water";
			auxResponses.Remove("Water");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "I can't get any wetter\nWhat am I?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "A pussy";
			auxResponses.Remove("A pussy");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "I steal your anchovies\nand I say 'meow'\nWhat am I?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "An armpit";
			auxResponses.Remove("An armpit");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What's my favorite\nbody part?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "Fire";
			auxResponses.Remove("Fire");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What always eats\nbut is always hungry?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);

			localPhase = auxLocalPhase;
			auxResponses = copyList(responses);
			currentResponses = new string[3];
			auxCorrect = Random.Range(0, 3);
			currentResponses[auxCorrect] = "A table";
			auxResponses.Remove("A table");
			assignResponses(currentResponses, auxResponses);
			auxBubbles = new List<Bubble>();
			auxBubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What has four legs\nand can't walk?", new Vector2(5f, 3f)));
			localPhase++;
			auxBubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{currentResponses[0], currentResponses[1], currentResponses[2]}, auxCorrect));
			localPhase++;
			randomRiddles.Add(auxBubbles);




			randomInt = Random.Range(0, randomRiddles.Count);
			localPhase = auxLocalPhase;
			
			for (int i = 0; i < randomRiddles[randomInt].Count; i++) {
                speechManager.bubbles.Add(randomRiddles[randomInt][i]);
				localPhase = randomRiddles[randomInt][i].beginPhase +1;
			}
			


		}
		else if (eventID == GlobalData.eventDouchebards)
		{
			eventDouchebardsG.SetActive(true);
			
			backgroundMusic = gameObject.AddComponent<AudioSource>();
			backgroundMusic.clip = Resources.Load("Music/Douchebards") as AudioClip;
			backgroundMusic.volume = 1f;
			backgroundMusic.loop = false;

            speechManager.addSpeaker(GameObject.Find("Player"));
            speechManager.addSpeaker(GameObject.Find("EventDouchebards/Leader"));

            int localPhase;
            List<Bubble> auxBubbles;

            // GREETINGS
            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "You came to the wrong\nneighbourhood, buddy", new Vector2(2f, 3f)));
            localPhase++;
            randomGreetings.Add(auxBubbles);

            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "PARTY HARD !!", new Vector2(2f, 3f)));
            localPhase++;
            randomGreetings.Add(auxBubbles);

            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Stand aside, partycrasher\nthe fun is about to start!", new Vector2(2f, 3f)));
            localPhase++;
            randomGreetings.Add(auxBubbles);

            int randomInt = Random.Range(0, randomGreetings.Count);
            localPhase = 0;

            for (int i = 0; i < randomGreetings[randomInt].Count; i++)
            {
                speechManager.bubbles.Add(randomGreetings[randomInt][i]);
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
        else if (currentEvent == GlobalData.eventExpert)
        {

            eventExpertG.SetActive(true);

            backgroundMusic = gameObject.AddComponent<AudioSource>();
            backgroundMusic.clip = Resources.Load("Music/Expert") as AudioClip;
            backgroundMusic.volume = 0f;
            backgroundMusic.loop = true;
            backgroundMusic.Play();

            speechManager.addSpeaker(GameObject.Find("Player"));
            speechManager.addSpeaker(GameObject.Find("EventExpert/Expert"));

            int localPhase;
            List<Bubble> auxBubbles;

            // EXPERT SUBJECTS
            List<string> subjects = new List<string>();
            //subjects.Add("jelly");
            //subjects.Add("chicken");
            subjects.Add("dumi");

            expertSubject = subjects[Random.Range(0, subjects.Count)];

            // GREETINGS
            localPhase = 0;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Oh hi", new Vector2(4f, 3f)));
            localPhase++;
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "I'm the\n"+expertSubject+" expert", new Vector2(4f, 3f)));
            localPhase++;
            auxBubbles.Add(new Bubble(0, localPhase, localPhase, "...", new Vector2(-3f, 3f)));
            localPhase++;
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "What?\nAren't you surprised?", new Vector2(4f, 3f)));
            localPhase++;
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Pfft, I bet you couldn't\nidentify the best "+expertSubject+"\nif I do this", new Vector2(4f, 3f)));
            localPhase++;

            localPhase = 0;

            for (int i = 0; i < auxBubbles.Count; i++)
            {
                speechManager.bubbles.Add(auxBubbles[i]);
                localPhase = auxBubbles[i].beginPhase + 1;
            }

        }

	}

	List<string> copyList(List<string> list1) {

		List<string> list2 = new List<string>();

		for (int i = 0; i < list1.Count; i++) {
			list2.Add(list1[i]);
		}

		return list2;

	}

	void assignResponses(string[] stringArray, List<string> stringList) {

		for (int i = 0; i < stringArray.Length; i++) {

			if (stringArray[i] == "" || stringArray[i] == null) {

				int aux = Random.Range(0, stringList.Count);
				stringArray[i] = stringList[aux];
				stringList.RemoveAt(aux);

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

        if (currentEvent == GlobalData.eventDouchebards)
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

        speechManager.update();

        if (!speechManager.writingSomething && currentEvent == GlobalData.eventRon && speechManager.options.auxFloat1 == 50f)
        {
            endCondition = 100f;
        }
        else if (!speechManager.writingSomething && currentEvent == GlobalData.eventDouchebards && endCondition == 50f)
        {
            endCondition = 100f;
        }
		
		

        if (!speechManager.writingSomething && currentEvent == GlobalData.eventDouchebards && !backgroundMusic.isPlaying)
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
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "You're not at our\nlevel, gobermouch", new Vector2(2f, 3f)));
                        }
                        else if (auxRand < 2f / 3f)
                        {
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "You ruined all the fun\nGet lost, smell-feast!", new Vector2(2f, 3f)));
                        }
                        else
                        {
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "GTFO, cumberworld!", new Vector2(2f, 3f)));
                        }
                    }
                    else if ((float)partyPoints / (float)maxPartyPoints < 80f/100f)
                    {
                        // BIEN (RECOMPENSA)
						float auxRand = Random.Range(0f, 1f);
						if (auxRand < 1f / 3f)
						{
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "Not bad... for a beginner\nHere's your reward", new Vector2(2f, 3f)));
						}
						else if (auxRand < 2f / 3f)
						{
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "I've seen worse...\nTake this, it's dangerous\nto go alone", new Vector2(2f, 3f)));
						}
						else {
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "Very impressive for a noob\nHave this and get lost", new Vector2(2f, 3f)));
						}
                    }
                    else if ((float)partyPoints / (float)maxPartyPoints <  100f/100f)
                    {
                        // MUY BIEN (RECOMPENSA)
                        float auxRand = Random.Range(0f, 1f);
                        if (auxRand < 1f / 3f)
                        {
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "Beautiful! Almost perfect!\n", new Vector2(2f, 3f)));
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 1, speechManager.globalPhase + 1, "You're the most effulgent\nmotherfucker I've seen in a while", new Vector2(2f, 3f)));
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 2, speechManager.globalPhase + 2, "Take this cool\nrandomly selected item", new Vector2(2f, 3f)));
                        }
                        else if (auxRand < 2f / 3f)
                        {
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "Bro, so close\n to perfect score!", new Vector2(2f, 3f)));
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 1, speechManager.globalPhase + 1, "But you're a one recalcitrant\nasshole, that's for sure", new Vector2(2f, 3f)));
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 2, speechManager.globalPhase + 2, "Take this item, I can't\nuse it since I'm an NPC", new Vector2(2f, 3f)));
                        }
                        else
                        {
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "That was so close!", new Vector2(2f, 3f)));
                            speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 1, speechManager.globalPhase + 1, "Such a tenacious scumbag\ndeserves an appropriate reward", new Vector2(2f, 3f)));
                        }
                    }
                    else 
                    {
                        // PERFECTO
                        float auxRand = Random.Range(0f, 1f);
                        speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase, speechManager.globalPhase, "HOLY SHIT !!\n100% PERFECT", new Vector2(2f, 3f)));

                        speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 1, speechManager.globalPhase + 1, "I'm going to tattoo\nyour face on my butt!", new Vector2(2f, 3f)));
                        speechManager.bubbles.Add(new Bubble(1, speechManager.globalPhase + 2, speechManager.globalPhase + 2, "Take this\nyou've earned it", new Vector2(2f, 3f)));

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
