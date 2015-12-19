using UnityEngine;
using System.Collections.Generic;

public class SpeechBubble
{

    private static float lineWidth = 0.035f;
    public SpeechManager m;
    public GameObject root;
    public GameObject text;
    public GameObject bubble;
    public GameObject bubble2;
    public GameObject triangle;
    public GameObject triangle2;
    public int currentLetter = 0;
    public float timer = 0f;

    public SpeechBubble(SpeechManager auxM)
    {
        m = auxM;
        root = MonoBehaviour.Instantiate(Resources.Load("Prefabs/SpeechBubble")) as GameObject;
        text = root.transform.FindChild("Text").gameObject;
        bubble = root.transform.FindChild("Bubble").gameObject;
        bubble2 = root.transform.FindChild("Bubble2").gameObject;
        triangle = root.transform.FindChild("Triangle").gameObject;
        triangle2 = root.transform.FindChild("Triangle2").gameObject;
    }

    public void Write(Bubble b)
    {

        if (!root.activeInHierarchy)
        {
            text.GetComponent<TextMesh>().text = "";
            bubble.transform.localScale = new Vector3(0f, 0f, 0);
            bubble2.transform.localScale = new Vector3(0f, 0f, 0.1f);
            timer = -0.1f;
            root.SetActive(true);
        }

        string target = b.text;
        string currentString = text.GetComponent<TextMesh>().text;
        text.GetComponent<TextMesh>().text = target;
        text.transform.position = new Vector3(b.position.x - text.GetComponent<Renderer>().bounds.size.x / 2f, b.position.y + text.GetComponent<Renderer>().bounds.size.y / 2f, text.transform.position.z);
        float scaleX = text.GetComponent<Renderer>().bounds.size.x;
        float scaleY = text.GetComponent<Renderer>().bounds.size.y;
        if (scaleX > scaleY) { scaleY += 0.5f; }
        if (scaleY > scaleX) { scaleX += 0.5f; }
        //if (scaleX < scaleY * 0.5f) { scaleX = scaleY * 0.5f; }
        //if (scaleY < scaleX * 0.25f) { scaleY = scaleX * 0.25f; }
        bubble.transform.localScale = new Vector3(Mathf.Lerp(bubble.transform.localScale.x, (scaleX) * 0.7f, Time.deltaTime * 20f), Mathf.Lerp(bubble.transform.localScale.y, (scaleY) * 0.7f, Time.deltaTime * 20f), 1f);
        bubble.transform.position = new Vector3(text.GetComponent<Renderer>().bounds.center.x, text.GetComponent<Renderer>().bounds.center.y, bubble.transform.position.z);
        bubble2.transform.localScale = new Vector3(bubble.transform.localScale.x + lineWidth, bubble.transform.localScale.y + lineWidth, bubble2.transform.localScale.z);
        bubble2.transform.position = bubble.transform.position + new Vector3(0f, 0f, 0.1f);

        float min = bubble.transform.localScale.x;
        if (bubble.transform.localScale.y < min) { min = bubble.transform.localScale.y; }

        Vector3 diff = triangle.transform.position - m.speakers[b.speaker].transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        triangle.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);//speakers [b.speaker].transform;
        triangle2.transform.rotation = triangle.transform.rotation;

        triangle.transform.localScale = new Vector3(min / 3f, min / 3f, min / 3f);

        Vector2 director = new Vector2(m.speakers[b.speaker].transform.position.x - bubble.GetComponent<Renderer>().bounds.center.x, m.speakers[b.speaker].transform.position.y - bubble.GetComponent<Renderer>().bounds.center.y);
        director.Normalize();

        triangle.transform.position = new Vector3(bubble.GetComponent<Renderer>().bounds.center.x + (bubble.GetComponent<Renderer>().bounds.size.y / 2f) * director.x, bubble.GetComponent<Renderer>().bounds.center.y - bubble.GetComponent<Renderer>().bounds.size.y / 2f, triangle.transform.position.z);

        triangle2.transform.localScale = new Vector3(triangle.transform.localScale.x + lineWidth, triangle.transform.localScale.y + lineWidth, triangle2.transform.localScale.z);
        triangle2.transform.position = new Vector3(triangle.transform.position.x, triangle.transform.position.y, triangle2.transform.position.z) + triangle2.transform.up * -0.04f;

        text.GetComponent<TextMesh>().text = currentString;

        if (Input.GetKeyDown(KeyCode.Return) && currentLetter > 0)
        {
            if (true)
            {
                if (currentLetter < target.Length)
                {
                    currentLetter = target.Length - 1;
                    m.nextPhase = -1;
                }
                else
                {
                    if (m.nextPhase == 0)
                    {
                        m.nextPhase = 1;
                    }

                    if (m.globalPhase + 1 > b.endPhase && m.nextPhase == 1)
                    {
                        m.toRecycle.Add(m.speechBubblePool.IndexOf(this));
                    }

                }
            }
        }

        if (currentLetter < target.Length)
        {

            timer += Time.deltaTime;

            if (timer > 0.05f)
            {

                currentLetter++;

                while (currentLetter < target.Length - 1 && target[currentLetter] == ' ')
                {
                    currentLetter++;
                }

                timer = 0f;
                text.GetComponent<TextMesh>().text = target.Substring(0, currentLetter);

                m.textSounds[Random.Range(0, m.textSounds.Length)].Play();

            }

        }

    }

}