using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkScript : MonoBehaviour {


	private AudioSource[] textSounds = new AudioSource[3];
	private bool nextSound = true;

	private GameObject background;

	private string target;

    private static List<SpeechBubble> speechBubblePool = new List<SpeechBubble>();
    private static List<int> toRecycle = new List<int>();
    private static OptionsSquare options;

    private GameObject[] speakers;
    private List<Bubble> bubbles = new List<Bubble>();
    private int currentBubble = 0;
    private int nextPhase = 0;
    private int globalPhase = 0;

    private static int eventRon = 0;

    private float lineWidth = 0.035f;

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

            int localPhase = 0;
            float randomHello = Random.Range(0f, 1f);

            if (randomHello < 1f / 3f)
            {
                bubbles.Add(new Bubble(1, localPhase, localPhase, "I'm Ron Weasel, but\n you can call me Ron Weasel", new Vector2(3f, 3f), null));
                localPhase++;
                bubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
                localPhase++;
            }
            else if (randomHello < 2f / 3f)
            {
                bubbles.Add(new Bubble(1, localPhase, localPhase, "Hey bro, don't trust Harry Otter", new Vector2(3f, 3f), null));
                localPhase++;
            }
            else
            {
                bubbles.Add(new Bubble(1, localPhase, localPhase, "This shoe tastes like... sweaty foot.\nFar from strawberry candy", new Vector2(3f, 3f), null));
                localPhase++;
            }

            bubbles.Add(new Bubble(0, localPhase, localPhase, "...", new Vector2(-3f, 3f), null));
            localPhase++;

            bubbles.Add(new Bubble(1, localPhase, localPhase, "If you answer my question correctly\n I will give you something VERY special", new Vector2(3f, 3f), null));
            localPhase++;

            float randomQuestion = Random.Range(0f, 1f);

            if (randomQuestion < 1f)
            {
                bubbles.Add(new Bubble(1, localPhase, localPhase + 1, "What's the difference between a duck?", new Vector2(5f, 3f), null));
                localPhase++;
            }

            bubbles.Add(new Bubble(0, localPhase, localPhase, "", new Vector2(-5.5f, 2.5f), new string[]{"I don't know", "A duck", "Fuck you Ron"}));
        
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (nextPhase == 1)
        {
            globalPhase++;
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

        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i].beginPhase <= globalPhase && bubbles[i].endPhase >= globalPhase)
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
        s.bubble2.transform.localScale = new Vector3(s.bubble.transform.localScale.x + lineWidth, s.bubble.transform.localScale.y + lineWidth, s.bubble2.transform.localScale.z);
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

        s.triangle2.transform.localScale = new Vector3(s.triangle.transform.localScale.x + lineWidth, s.triangle.transform.localScale.y + lineWidth, s.triangle2.transform.localScale.z);
		s.triangle2.transform.position = new Vector3 (s.triangle.transform.position.x, s.triangle.transform.position.y, s.triangle2.transform.position.z) +s.triangle2.transform.up*-0.04f;

		s.text.GetComponent<TextMesh>().text = currentString;

        if (Input.GetKeyDown(KeyCode.Return) && s.currentLetter > 0)
        {
            if (globalPhase < 10)
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

            bubbles.Add(new Bubble(0, globalPhase+1, globalPhase+2, b.options[o.selected], new Vector2(-3f, 3f), null));

            if (b.options[o.selected] == "I don't know")
            {
                bubbles.Add(new Bubble(1, globalPhase + 2, globalPhase + 2, "That's what she said\nWait, what?", new Vector2(3f, 3f), null));
            }
            if (b.options[o.selected] == "A duck")
            {
                bubbles.Add(new Bubble(1, globalPhase + 2, globalPhase + 2, "How did you know it?!", new Vector2(3f, 3f), null));
            }
            if (b.options[o.selected] == "Fuck you Ron")
            {
                bubbles.Add(new Bubble(1, globalPhase + 2, globalPhase + 2, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f), null));
            }
            
            o.root.SetActive(false);

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

}
