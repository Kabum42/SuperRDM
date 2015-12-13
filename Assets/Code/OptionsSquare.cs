using UnityEngine;
using System.Collections.Generic;

public class OptionsSquare
{

    private static float lineWidth = 0.035f;
    public SpeechManager m;
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

    public OptionsSquare(SpeechManager auxM)
    {

        m = auxM;
        root = MonoBehaviour.Instantiate(Resources.Load("Prefabs/OptionsSquare")) as GameObject;
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

    public void Write(Bubble b)
    {

        if (!root.activeInHierarchy)
        {
            optionText[0].GetComponent<TextMesh>().text = "";
            optionText[1].GetComponent<TextMesh>().text = "";
            optionText[2].GetComponent<TextMesh>().text = "";
            squareMiddle.transform.localScale = new Vector3(0f, 0f, 0f);
            squareTop.transform.localScale = new Vector3(0f, 0f, 0f);
            squareBottom.transform.localScale = new Vector3(0f, 0f, 0f);
            corner1.transform.localScale = new Vector3(0f, 0f, 0f);
            corner2.transform.localScale = new Vector3(0f, 0f, 0f);
            corner3.transform.localScale = new Vector3(0f, 0f, 0f);
            corner4.transform.localScale = new Vector3(0f, 0f, 0f);
            squareMiddleBack.transform.localScale = new Vector3(0f, 0f, 0f);
            squareTopBack.transform.localScale = new Vector3(0f, 0f, 0f);
            squareBottomBack.transform.localScale = new Vector3(0f, 0f, 0f);
            corner1Back.transform.localScale = new Vector3(0f, 0f, 0f);
            corner2Back.transform.localScale = new Vector3(0f, 0f, 0f);
            corner3Back.transform.localScale = new Vector3(0f, 0f, 0f);
            corner4Back.transform.localScale = new Vector3(0f, 0f, 0f);
            timer = -0.1f;
            root.SetActive(true);
        }

        float scaleX = 0f;
        float scaleY = 0;

        for (int i = 0; i < b.options.Length; i++)
        {
            string target = b.options[i];
            string currentString = optionText[i].GetComponent<TextMesh>().text;
            optionText[i].GetComponent<TextMesh>().text = target;
            if (optionText[i].GetComponent<Renderer>().bounds.size.x > scaleX)
            {
                scaleX = optionText[i].GetComponent<Renderer>().bounds.size.x;
            }
            optionText[i].transform.localPosition = new Vector3(optionText[i].transform.localPosition.x, -scaleY - optionText[i].GetComponent<Renderer>().bounds.size.y / 2f, optionText[i].transform.localPosition.z);
            scaleY += optionText[i].GetComponent<Renderer>().bounds.size.y;
        }

        for (int i = 0; i < b.options.Length; i++)
        {
            optionText[i].transform.localPosition = new Vector3(-scaleX / 2f, optionText[i].transform.localPosition.y + scaleY / 2f, optionText[i].transform.localPosition.z);
            optionText[i].GetComponent<TextMesh>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            if (selected == i)
            {
                optionText[i].GetComponent<TextMesh>().color = new Color(0f, 0f, 0f, 1f);
            }
        }

        scaleX += 0.5f;

        squareMiddle.transform.localScale = new Vector3(Mathf.Lerp(squareMiddle.transform.localScale.x, (scaleX) * 105f, Time.deltaTime * 20f), Mathf.Lerp(squareMiddle.transform.localScale.y, (scaleY) * 105f, Time.deltaTime * 20f), squareMiddle.transform.localScale.z);
        squareMiddleBack.transform.localScale = new Vector3(squareMiddle.transform.localScale.x + lineWidth * 195f, squareMiddle.transform.localScale.y + lineWidth * 195f, squareMiddle.transform.localScale.z);

        corner1.transform.localScale = new Vector3(Mathf.Lerp(corner1.transform.localScale.x, 0.2f, Time.deltaTime * 20f), Mathf.Lerp(corner1.transform.localScale.y, 0.2f, Time.deltaTime * 20f), 1f);
        corner1.transform.localPosition = new Vector3(+corner1.GetComponent<Renderer>().bounds.size.x / 2f - squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, corner1.GetComponent<Renderer>().bounds.size.y / 2f + squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        corner1Back.transform.localScale = new Vector3(corner1.transform.localScale.x + lineWidth, corner1.transform.localScale.y + lineWidth, corner1.transform.localScale.z);
        corner1Back.transform.localPosition = new Vector3(corner1.transform.localPosition.x, corner1.transform.localPosition.y, corner1Back.transform.localPosition.z);

        corner2.transform.localScale = new Vector3(Mathf.Lerp(corner2.transform.localScale.x, -0.2f, Time.deltaTime * 20f), Mathf.Lerp(corner2.transform.localScale.y, 0.2f, Time.deltaTime * 20f), 1f);
        corner2.transform.localPosition = new Vector3(-corner2.GetComponent<Renderer>().bounds.size.x / 2f + squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, corner2.GetComponent<Renderer>().bounds.size.y / 2f + squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        corner2Back.transform.localScale = new Vector3(corner2.transform.localScale.x - lineWidth, corner2.transform.localScale.y + lineWidth, corner2.transform.localScale.z);
        corner2Back.transform.localPosition = new Vector3(corner2.transform.localPosition.x, corner2.transform.localPosition.y, corner2Back.transform.localPosition.z);

        corner3.transform.localScale = new Vector3(Mathf.Lerp(corner3.transform.localScale.x, 0.2f, Time.deltaTime * 20f), Mathf.Lerp(corner3.transform.localScale.y, -0.2f, Time.deltaTime * 20f), 1f);
        corner3.transform.localPosition = new Vector3(corner3.GetComponent<Renderer>().bounds.size.x / 2f - squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, -corner3.GetComponent<Renderer>().bounds.size.y / 2f - squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        corner3Back.transform.localScale = new Vector3(corner3.transform.localScale.x + lineWidth, corner3.transform.localScale.y - lineWidth, corner3.transform.localScale.z);
        corner3Back.transform.localPosition = new Vector3(corner3.transform.localPosition.x, corner3.transform.localPosition.y, corner3Back.transform.localPosition.z);

        corner4.transform.localScale = new Vector3(Mathf.Lerp(corner4.transform.localScale.x, -0.2f, Time.deltaTime * 20f), Mathf.Lerp(corner4.transform.localScale.y, -0.2f, Time.deltaTime * 20f), 1f);
        corner4.transform.localPosition = new Vector3(-corner4.GetComponent<Renderer>().bounds.size.x / 2f + squareMiddle.GetComponent<Renderer>().bounds.size.x / 2f, -corner4.GetComponent<Renderer>().bounds.size.y / 2f - squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        corner4Back.transform.localScale = new Vector3(corner4.transform.localScale.x - lineWidth, corner4.transform.localScale.y - lineWidth, corner4.transform.localScale.z);
        corner4Back.transform.localPosition = new Vector3(corner4.transform.localPosition.x, corner4.transform.localPosition.y, corner4Back.transform.localPosition.z);

        squareTop.transform.localScale = new Vector3(Mathf.Lerp(squareTop.transform.localScale.x, (scaleX - corner1.GetComponent<Renderer>().bounds.size.x * 1.9f) * 105f, Time.deltaTime * 20f), Mathf.Lerp(squareTop.transform.localScale.y, 0.4f * 105f * 0.96f, Time.deltaTime * 20f), squareTop.transform.localScale.z);
        squareTop.transform.localPosition = new Vector3(0f, squareTop.GetComponent<Renderer>().bounds.size.y / 2f * 0.98f + squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        squareTopBack.transform.localScale = new Vector3(squareTop.transform.localScale.x + lineWidth * 195f, squareTop.transform.localScale.y + lineWidth * 195f, squareTopBack.transform.localScale.z);
        squareTopBack.transform.localPosition = new Vector3(squareTop.transform.localPosition.x, squareTop.transform.localPosition.y, squareTopBack.transform.localPosition.z);

        squareBottom.transform.localScale = new Vector3(Mathf.Lerp(squareBottom.transform.localScale.x, (scaleX - corner1.GetComponent<Renderer>().bounds.size.x * 1.9f) * 105f, Time.deltaTime * 20f), Mathf.Lerp(squareBottom.transform.localScale.y, 0.4f * 105f * 0.96f, Time.deltaTime * 20f), squareBottom.transform.localScale.z);
        squareBottom.transform.localPosition = new Vector3(0f, -squareBottom.GetComponent<Renderer>().bounds.size.y / 2f * 0.98f - squareMiddle.GetComponent<Renderer>().bounds.size.y / 2f, 0f);
        squareBottomBack.transform.localScale = new Vector3(squareBottom.transform.localScale.x + lineWidth * 195f, squareBottom.transform.localScale.y + lineWidth * 195f, squareBottomBack.transform.localScale.z);
        squareBottomBack.transform.localPosition = new Vector3(squareBottom.transform.localPosition.x, squareBottom.transform.localPosition.y, squareBottomBack.transform.localPosition.z);

        pointer.transform.localPosition = new Vector3(-scaleX / 2f - pointer.GetComponent<Renderer>().bounds.size.x / 2f + 0.15f, Mathf.Lerp(pointer.transform.localPosition.y, optionText[selected].transform.localPosition.y, Time.deltaTime * 20f), pointer.transform.localPosition.z);

        float height = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f)).y;
        root.transform.position = new Vector3(0f, -height + scaleY / 2f + squareBottom.GetComponent<Renderer>().bounds.size.y * 3f, root.transform.position.z);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selected++;
            if (selected >= b.options.Length)
            {
                selected = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selected--;
            if (selected < 0)
            {
                selected = b.options.Length - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (m.nextPhase == 0)
            {
                m.nextPhase = 1;
            }

            //menuOk.Play();

            int localPhase = m.globalPhase + 1;

            m.bubbles.Add(new Bubble(0, localPhase, localPhase + 1, b.options[selected], new Vector2(-3f, 3f)));
            localPhase++;

            if (b.options[selected] == b.options[b.correctOption])
            {
                m.bubbles.Add(new Bubble(1, localPhase, localPhase, "How did you know it?!", new Vector2(3f, 3f)));
                localPhase++;
                m.bubbles.Add(new Bubble(1, localPhase, localPhase, "Well, it's clear now\nthat you deserve this prize", new Vector2(3f, 3f)));
                localPhase++;
            }
            else if (b.options[selected] == "( ͡° ͜ʖ ͡°)")
            {
                m.bubbles.Add(new Bubble(1, localPhase, localPhase, "You...", new Vector2(3f, 3f)));
                localPhase++;
                m.bubbles.Add(new Bubble(1, localPhase, localPhase, "I like you", new Vector2(3f, 3f)));
                localPhase++;
                m.bubbles.Add(new Bubble(1, localPhase, localPhase, "That was not the correct answer\nbut take the prize anyway", new Vector2(3f, 3f)));
                localPhase++;
            }
            else if (b.options[selected] == "Fuck you, Ron")
            {
                m.bubbles.Add(new Bubble(1, localPhase, localPhase, "( ͡° ͜ʖ ͡°)", new Vector2(3f, 3f)));
                localPhase++;
            }
            else
            {
                float randomFloat = Random.Range(0f, 1f);
                float extraOptions = 2f;

                if (randomFloat <= 0.5f)
                {
                    m.bubbles.Add(new Bubble(1, localPhase, localPhase, "That's the most fucked up\nshit I ever heard", new Vector2(3f, 3f)));
                    localPhase++;
                }
                else if (randomFloat <= 0.5f + (1f / extraOptions) * 0.5f)
                {
                    m.bubbles.Add(new Bubble(1, localPhase, localPhase, "Whaaaat?!", new Vector2(3f, 3f)));
                    localPhase++;
                }
                else if (randomFloat <= 0.5f + (2f / extraOptions) * 0.5f)
                {
                    m.bubbles.Add(new Bubble(1, localPhase, localPhase, "...", new Vector2(3f, 3f)));
                    localPhase++;
                    m.bubbles.Add(new Bubble(1, localPhase, localPhase, "I admit that was funny\nbut still a wrong answer!", new Vector2(3f, 3f)));
                    localPhase++;
                }

            }

            root.SetActive(false);

            m.bubbles.Add(new Bubble(1, localPhase, localPhase, "And remember...", new Vector2(3f, 3f)));
            localPhase++;

            int originalLocalPhase = localPhase;
            List<Bubble> auxBubbles;

            // FAREWELLS
            List<List<Bubble>> randomFarewells = new List<List<Bubble>>();

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Cigar smoothies are\nbeyond good and evil", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Without X, life would\nbe an error", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "The last thing one\nloses... it's memories\n*sigh*", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Millennium Hand and Shrimp!", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Sometimes happiness is\nat the bottom of\na pickle jar", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Those who believe that\nmagic does everything", new Vector2(3f, 3f)));
            localPhase++;
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "end up doing\neverything for magic", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            localPhase = originalLocalPhase;
            auxBubbles = new List<Bubble>();
            auxBubbles.Add(new Bubble(1, localPhase, localPhase, "Jet fuel can't melt\nsteal beams", new Vector2(3f, 3f)));
            localPhase++;
            randomFarewells.Add(auxBubbles);

            int randomInt = Random.Range(0, randomFarewells.Count);

            for (int i = 0; i < randomFarewells[randomInt].Count; i++)
            {
                m.bubbles.Add(randomFarewells[randomInt][i]);
            }

            endCondition = 50f;

        }

    }

}