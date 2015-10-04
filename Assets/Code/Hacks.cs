using UnityEngine;
using System.Collections;

public static class Hacks {

    // TEXT ALPHA
    public static void TextAlpha(GameObject g, float a)
    {
        g.GetComponent<TextMesh>().color = new Color(g.GetComponent<TextMesh>().color.r, g.GetComponent<TextMesh>().color.g, g.GetComponent<TextMesh>().color.b, a);
    }

    public static void TextAlpha(TextMesh t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }

    // SPRITE RENDERER ALPHA
    public static void SpriteRendererAlpha(GameObject g, float a)
    {
        g.GetComponent<SpriteRenderer>().color = new Color(g.GetComponent<SpriteRenderer>().color.r, g.GetComponent<SpriteRenderer>().color.g, g.GetComponent<SpriteRenderer>().color.b, a);
    }

    public static void SpriteRendererAlpha(SpriteRenderer s, float a)
    {
        s.color = new Color(s.color.r, s.color.g, s.color.b, a);
    }

    // XBOX CONTROLLER
    public static bool ControllerAnyConnected()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] != "")
            {
                return true;
            }
        }
        return false;
    }

}
