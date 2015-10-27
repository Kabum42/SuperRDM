using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkScript : MonoBehaviour {


	private AudioSource[] textSounds = new AudioSource[3];
	private bool nextSound = true;

	private GameObject background;

	private string target;

    private static List<SpeechBubble> speechBubblePool = new List<SpeechBubble>();
    private static OptionsSquare options;

    private GameObject[] speakers;
    private List<Bubble> bubbles = new List<Bubble>();
    private int currentBubble = 0;
    private int globalPhase = 0;

    private static int eventRon = 0;

	// Use this for initialization
	void Start () {

		background = GameObject.Find("Background");

		for (int i = 0; i < textSounds.Length; i++) {
			textSounds[i] = gameObject.AddComponent<AudioSource>();
	        textSounds[i].clip = Resources.Load("Music/Text/Text_"+i.ToString("00")) as AudioClip;
	        textSounds[i].volume = 1f;
	        textSounds[i].pitch = 1.1f;
	        textSounds[i].Play();
		}

        SpeechBubble s = new SpeechBubble();
        speechBubblePool.Add(s);

        options = new OptionsSquare();

        InitializeSpeakers(eventRon);

        target = bubbles[0].text;

	}

    void InitializeSpeakers(int eventID)
    {
        if (eventID == eventRon)
        {
			speakers = new GameObject[2];
            speakers[0] = GameObject.Find ("Player");
            speakers[1] = GameObject.Find ("Ron");

            bubbles.Add(new Bubble(1, 0, "Hi bro, my name is Ron Weasel.\nDid you see my friend, Harry the Otter?", new Vector2(3f, 3f), null));
            bubbles.Add(new Bubble(0, 1, "...", new Vector2(-3f, 3f), null));
            bubbles.Add(new Bubble(1, 2, "Ron Ron Ron Ron\nRon Ron Ron Ron", new Vector2(5f, 3f), null));
            bubbles.Add(new Bubble(0, 2, "", new Vector2(-5.5f, 2.5f), new string[]{"This is a long text to test something\nso looong", "EJEM", "OMG"}));
        }
    }
	
	// Update is called once per frame
	void Update () {

        int currentSpeechBubble = 0;

        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i].phase > globalPhase)
            {
                break;
            }
            else if (bubbles[i].phase == globalPhase)
            {

                if (bubbles[i].options == null)
                {
                    // ES SOLO TEXTO
                    if (speechBubblePool.Count <= currentSpeechBubble)
                    {
                        SpeechBubble s = new SpeechBubble();
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

        for (int i = currentSpeechBubble; i < speechBubblePool.Count; i++)
        {
            speechBubblePool[i].root.SetActive(false);
        }
        

	
	}

    private void Write(Bubble b, SpeechBubble s)
    {

        if (!s.root.activeInHierarchy)
        {
            s.text.GetComponent<TextMesh>().text = "";
            s.bubble.transform.localScale = new Vector3(0f, 0f, 0f);
            s.bubble2.transform.localScale = new Vector3(0f, 0f, 0f);
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
        s.bubble2.transform.localScale = new Vector3(s.bubble.transform.localScale.x + 0.035f, s.bubble.transform.localScale.y + 0.035f, s.bubble2.transform.localScale.z);
        s.bubble2.transform.position = s.bubble.transform.position;
      	
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

		s.triangle2.transform.localScale = new Vector3 (s.triangle.transform.localScale.x + 0.035f, s.triangle.transform.localScale.y + 0.035f, s.triangle2.transform.localScale.z);
		s.triangle2.transform.position = new Vector3 (s.triangle.transform.position.x, s.triangle.transform.position.y, s.triangle2.transform.position.z) +s.triangle2.transform.up*-0.04f;

		s.text.GetComponent<TextMesh>().text = currentString;

        if (Input.GetKeyDown(KeyCode.Return) && s.currentLetter > 0)
        {
            if (globalPhase < 3)
            {
                if (s.currentLetter < target.Length)
                {
                    s.currentLetter = target.Length - 1;
                }
                else
                {
                    globalPhase = b.phase + 1;
                    s.text.GetComponent<TextMesh>().text = "";
                    s.currentLetter = 0;

                    if (b.phase + 1 > currentBubble)
                    {
                        currentBubble = b.phase + 1;
                    }

                    s.root.SetActive(false);

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
            o.timer = -0.1f;
            o.root.SetActive(true);
        }

        float scaleX = 0;
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
        }

        o.squareMiddle.transform.localScale = new Vector3(Mathf.Lerp(o.squareMiddle.transform.localScale.x, (scaleX) * 105f, Time.deltaTime * 20f), Mathf.Lerp(o.squareMiddle.transform.localScale.y, (scaleY) * 105f, Time.deltaTime * 20f), o.squareMiddle.transform.localScale.z);

        /*
        string target = b.text;
        string currentString = s.text.GetComponent<TextMesh>().text;
        s.text.GetComponent<TextMesh>().text = target;
        s.text.transform.position = new Vector3(b.position.x - s.text.GetComponent<Renderer>().bounds.size.x / 2f, b.position.y + s.text.GetComponent<Renderer>().bounds.size.y / 2f, s.text.transform.position.z);
        float scaleX = s.text.GetComponent<Renderer>().bounds.size.x;
        float scaleY = s.text.GetComponent<Renderer>().bounds.size.y;
        if (scaleX > scaleY) { scaleY += 0.5f; }
        if (scaleY > scaleX) { scaleX += 0.5f; }
        //if (scaleX < scaleY * 0.5f) { scaleX = scaleY * 0.5f; }
        //if (scaleY < scaleX * 0.25f) { scaleY = scaleX * 0.25f; }
        s.bubble.transform.localScale = new Vector3(Mathf.Lerp(s.bubble.transform.localScale.x, (scaleX) * 0.7f, Time.deltaTime * 20f), Mathf.Lerp(s.bubble.transform.localScale.y, (scaleY) * 0.7f, Time.deltaTime * 20f), 1f);
        s.bubble.transform.position = new Vector3(s.text.GetComponent<Renderer>().bounds.center.x, s.text.GetComponent<Renderer>().bounds.center.y, s.bubble.transform.position.z);
        s.bubble2.transform.localScale = new Vector3(s.bubble.transform.localScale.x + 0.035f, s.bubble.transform.localScale.y + 0.035f, s.bubble2.transform.localScale.z);
        s.bubble2.transform.position = s.bubble.transform.position;

        float min = s.bubble.transform.localScale.x;
        if (s.bubble.transform.localScale.y < min) { min = s.bubble.transform.localScale.y; }

        Vector3 diff = s.triangle.transform.position - speakers[b.speaker].transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        s.triangle.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);//speakers [b.speaker].transform;
        s.triangle2.transform.rotation = s.triangle.transform.rotation;

        s.triangle.transform.localScale = new Vector3(min / 3f, min / 3f, min / 3f);

        Vector2 director = new Vector2(speakers[b.speaker].transform.position.x - s.bubble.GetComponent<Renderer>().bounds.center.x, speakers[b.speaker].transform.position.y - s.bubble.GetComponent<Renderer>().bounds.center.y);
        director.Normalize();

        s.triangle.transform.position = new Vector3(s.bubble.GetComponent<Renderer>().bounds.center.x + (s.bubble.GetComponent<Renderer>().bounds.size.y / 2f) * director.x, s.bubble.GetComponent<Renderer>().bounds.center.y - s.bubble.GetComponent<Renderer>().bounds.size.y / 2f, s.triangle.transform.position.z);

        s.triangle2.transform.localScale = new Vector3(s.triangle.transform.localScale.x + 0.035f, s.triangle.transform.localScale.y + 0.035f, s.triangle2.transform.localScale.z);
        s.triangle2.transform.position = new Vector3(s.triangle.transform.position.x, s.triangle.transform.position.y, s.triangle2.transform.position.z) + s.triangle2.transform.up * -0.04f;

        s.text.GetComponent<TextMesh>().text = currentString;

        if (Input.GetKeyDown(KeyCode.Return) && s.currentLetter > 0)
        {
            if (globalPhase < 3)
            {
                if (s.currentLetter < target.Length)
                {
                    s.currentLetter = target.Length - 1;
                }
                else
                {
                    globalPhase = b.phase + 1;
                    s.text.GetComponent<TextMesh>().text = "";
                    s.currentLetter = 0;

                    if (b.phase + 1 > currentBubble)
                    {
                        currentBubble = b.phase + 1;
                    }

                    s.root.SetActive(false);

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
        */
    }


    private class Bubble
    {

        public int speaker;
        public int phase;
        public string text;
        public Vector2 position;
        public string[] options;

        public Bubble(int auxSpeaker, int auxPhase, string auxText, Vector2 auxPosition, string[] auxOptions)
        {
            speaker = auxSpeaker;
            phase = auxPhase;
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
        public int currentLetter = 0;
        public float timer = 0f;

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
            root.SetActive(false);
            
        }

    }

}
