using UnityEngine;
using System.Collections;

public class PreludeScript : MonoBehaviour {

	private GameObject story;
	private int phase = 0;
	private float timeInPhase = 0f;
	private float acceleration_y = 0f;

	// Use this for initialization
	void Start () {

		story = GameObject.Find("Story");
	
	}
	
	// Update is called once per frame
	void Update () {

		float target_y = 0f;

		if (phase == 0) {
			if (timeInPhase >= 1f) { phase = 1; timeInPhase = 0f; }
		}
		if (phase == 1) {
			target_y = 9f;
			if (timeInPhase >= 3f) { phase = 2; }
		}
		if (phase == 2) {
			target_y = 15f;
		}

		if (story.transform.position.y < target_y) {
			acceleration_y += Time.deltaTime;
			if (acceleration_y > 1f) { acceleration_y = 1f; }
		}
		else {
			acceleration_y -= Time.deltaTime;
			if (acceleration_y < 0f) { acceleration_y = 0f; }
			timeInPhase += Time.deltaTime;
		}

		story.transform.position = new Vector3(story.transform.position.x, story.transform.position.y + Time.deltaTime*acceleration_y*2.5f, story.transform.position.z);
	
	}
}
