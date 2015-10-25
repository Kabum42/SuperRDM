using UnityEngine;
using System.Collections;

public class TalkScript : MonoBehaviour {

	private GameObject test;
	private int currentLetter = 0;
	private float timer = 0f;
	private AudioSource[] textSounds = new AudioSource[3];
	private bool nextSound = true;

	// Use this for initialization
	void Start () {

		test = GameObject.Find("Test");
		test.GetComponent<TextMesh>().text = "";

		for (int i = 0; i < textSounds.Length; i++) {
			textSounds[i] = gameObject.AddComponent<AudioSource>();
	        textSounds[i].clip = Resources.Load("Music/Text/Text_"+i.ToString("00")) as AudioClip;
	        textSounds[i].volume = 1f;
	        textSounds[i].pitch = 1.1f;
	        textSounds[i].Play();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		string target = "Hello bro, my name is Ron.\nRon Weasel, y'know, hahahaha!";

		if (currentLetter < target.Length) {

			timer += Time.deltaTime;
			
			if (timer > 0.05f) {

				currentLetter++;
				timer = 0f;
				test.GetComponent<TextMesh>().text = target.Substring(0, currentLetter);

				textSounds[Random.Range(0, textSounds.Length)].Play();

			}

		}

		


	
	}
}
