using UnityEngine;

public class Bubble
{

    private static GameObject auxG;
    public int speaker;
    public int beginPhase;
    public int endPhase;
    public string text;
    public Vector2 position;
    public string[] options;
    public int correctOption;

    public Bubble(int auxSpeaker, int auxBeginPhase, int auxEndPhase, string auxText, Vector2 auxPosition)
    {
        if (auxG == null)
        {
            auxG = new GameObject();
            auxG.AddComponent<TextMesh>();
        }

        speaker = auxSpeaker;
        beginPhase = auxBeginPhase;
        endPhase = auxEndPhase;
        text = Hacks.TextMultilineCentered(auxG, auxText);
        position = auxPosition;
    }

    public Bubble(int auxSpeaker, int auxBeginPhase, int auxEndPhase, string auxText, Vector2 auxPosition, string[] auxOptions, int auxCorrectOption)
    {
        if (auxG == null)
        {
            auxG = new GameObject();
            auxG.AddComponent<TextMesh>();
        }

        speaker = auxSpeaker;
        beginPhase = auxBeginPhase;
        endPhase = auxEndPhase;
        text = Hacks.TextMultilineCentered(auxG, auxText);
        position = auxPosition;
        options = auxOptions;
        correctOption = auxCorrectOption;
    }

}