using UnityEngine;
using System.Collections;

public class TalkScript : MonoBehaviour {

	private GameObject test;
	private int currentLetter = 0;
	private float timer = 0f;
	private AudioSource[] textSounds = new AudioSource[3];
	private bool nextSound = true;

	private GameObject background;
	private GameObject bubble;

	private string target;

	// Use this for initialization
	void Start () {

		test = GameObject.Find("Test");
		test.GetComponent<TextMesh>().text = "";

		background = GameObject.Find("Background");
		bubble = GameObject.Find ("Bubble");

		for (int i = 0; i < textSounds.Length; i++) {
			textSounds[i] = gameObject.AddComponent<AudioSource>();
	        textSounds[i].clip = Resources.Load("Music/Text/Text_"+i.ToString("00")) as AudioClip;
	        textSounds[i].volume = 1f;
	        textSounds[i].pitch = 1.1f;
	        textSounds[i].Play();
		}

        target = "Ron\nRon\nRon\nRon\nRon\nRon";
		target = Hacks.TextMultilineCentered (test, target);

	}
	
	// Update is called once per frame
	void Update () {


		string currentString = test.GetComponent<TextMesh>().text;
		test.GetComponent<TextMesh> ().text = target;
        float scaleX = test.GetComponent<Renderer>().bounds.size.x;
        float scaleY = test.GetComponent<Renderer>().bounds.size.y;
        if (scaleX < scaleY * 0.5f) { scaleX = scaleY * 0.5f; }
        if (scaleY < scaleX * 0.25f) { scaleY = scaleX * 0.25f; }
		bubble.transform.localScale = new Vector3(scaleX*0.8f, scaleY*0.8f, 1f) ;
		bubble.transform.position = new Vector3(test.GetComponent<Renderer> ().bounds.center.x, test.GetComponent<Renderer> ().bounds.center.y, bubble.transform.position.z);
		//background.transform.localScale = test.GetComponent<Renderer> ().bounds.size*105f;
		//background.transform.position = new Vector3(test.GetComponent<Renderer> ().bounds.center.x, test.GetComponent<Renderer> ().bounds.center.y, background.transform.position.z);
		test.GetComponent<TextMesh> ().text = currentString;

		if (currentLetter < target.Length) {

			timer += Time.deltaTime;
			
			if (timer > 0.05f) {

				currentLetter++;

				while(currentLetter < target.Length-1 && target[currentLetter] == ' ') {
					currentLetter++;
				}

				timer = 0f;
				test.GetComponent<TextMesh>().text = target.Substring(0, currentLetter);

				textSounds[Random.Range(0, textSounds.Length)].Play();

			}

		}



		


	
	}
}
