using UnityEngine;
using System.Collections.Generic;

public class SpeechManager
{

    public GameObject physical;
    public List<GameObject> speakers;
    public List<Bubble> bubbles;
    public int currentBubble = 0;
    public int nextPhase = 0;
    public int globalPhase = 0;

    public List<SpeechBubble> speechBubblePool;
    public List<int> toRecycle;
    public OptionsSquare options;

    public AudioSource[] textSounds = new AudioSource[3];
    public AudioSource closeSpeech;

    public bool writingSomething = false;

    public int eventSpeech = -1;

    public SpeechManager(GameObject auxPhysical, int auxEventSpeech)
    {
        physical = auxPhysical;
        eventSpeech = auxEventSpeech;
        bubbles = new List<Bubble>();
        speechBubblePool = new List<SpeechBubble>();
        toRecycle = new List<int>();
        speakers = new List<GameObject>();

        SpeechBubble s = new SpeechBubble(this);
        s.root.transform.parent = physical.transform;
        speechBubblePool.Add(s);
        s.root.SetActive(false);

        options = new OptionsSquare(this);
        options.root.transform.parent = physical.transform;

        for (int i = 0; i < textSounds.Length; i++)
        {
            textSounds[i] = physical.AddComponent<AudioSource>();
            textSounds[i].clip = Resources.Load("Music/Text/Text_" + i.ToString("00")) as AudioClip;
            textSounds[i].volume = 1.5f;
            textSounds[i].pitch = 1.1f;
            textSounds[i].Play();
        }

        closeSpeech = physical.AddComponent<AudioSource>();
        closeSpeech.clip = Resources.Load("Sounds/whip_01") as AudioClip;
        closeSpeech.volume = 1f;
        closeSpeech.playOnAwake = false;
    }

    public void addSpeaker(GameObject g) {
        speakers.Add(g);
    }

    public void update()
    {

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
        writingSomething = false;

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
                        SpeechBubble s = new SpeechBubble(this);
                        s.root.transform.parent = physical.transform;
                        s.root.SetActive(false);
                        speechBubblePool.Add(s);
                    }
                    speechBubblePool[currentSpeechBubble].Write(bubbles[i]);
                    currentSpeechBubble++;
                }
                else
                {
                    // SON OPCIONES
                    options.Write(bubbles[i]);
                }

            }
        }

        for (int i = currentSpeechBubble; i < speechBubblePool.Count; i++)
        {
            speechBubblePool[i].root.SetActive(false);
        }

    }

}