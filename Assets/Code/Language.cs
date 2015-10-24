using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Language {

    public static int currentLanguage = 0;

    public static int English = 0;
    public static int Spanish = 1;

    public static Dictionary<string, string>[] languages = new Dictionary<string, string>[2];

    public static string getText(string tag) {

        return languages[currentLanguage][tag];

    }

    public static void Start()
    {

        languages[English] = new Dictionary<string, string>();
        languages[Spanish] = new Dictionary<string, string>();

        //string loadedInfo = System.IO.File.ReadAllText("Languages.txt", System.Text.Encoding.GetEncoding("iso-8859-1"));
        TextAsset text = Resources.Load("Languages") as TextAsset;
        string loadedInfo = text.ToString();

        loadedInfo.Replace("\r\n", "\n");
        loadedInfo.Replace("\r", "\n");

        bool writingTag = false;
        string currentTag = "";
        int currentLang = English;
        bool writingLang = false;
        string currentWrittenLang = "";
        bool writingKey = false;
        string currentKey = "";

        for (int i = 0; i < loadedInfo.Length; i++)
        {
            if (writingTag)
            {
                if (loadedInfo[i] == ' ' || loadedInfo[i] == '\n' || (int)loadedInfo[i] == 13)
                {
                    writingTag = false;
                }
                else
                {
                    currentTag += loadedInfo[i].ToString();
                }
            }
            else if (writingLang)
            {
                if (loadedInfo[i] == ' ' || loadedInfo[i] == '\n' || (int)loadedInfo[i] == 13)
                {
                    writingLang = false;
                    if (currentWrittenLang == "English") { currentLang = English; }
                    else if (currentWrittenLang == "Spanish") { currentLang = Spanish; }
                }
                else
                {
                    currentWrittenLang = currentWrittenLang + loadedInfo[i].ToString();
                }
            }
            else if (writingKey)
            {
                if (loadedInfo[i] == '}')
                {
                    languages[currentLang].Add(currentTag, currentKey);
                    writingKey = false;
                }
                else
                {
                    currentKey += loadedInfo[i].ToString();
                }
            }

            
            if (loadedInfo[i] == '#') { 
                writingTag = true;
                currentTag = "";
            }
            else if (loadedInfo[i] == '@')
            {
                writingLang = true;
                currentWrittenLang = "";
            }
            else if (loadedInfo[i] == '{')
            {
                writingKey = true;
                currentKey = "";
            }
        }

    }

}
