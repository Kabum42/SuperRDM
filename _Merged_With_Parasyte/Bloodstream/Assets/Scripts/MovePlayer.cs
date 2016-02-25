using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour {

	//main_object
	public MainScript main;
	public GameObject player;
	public Camera camera;
	public float pulseCooldown = 0f;
	public float pulseCooldownDefault = 0.000001f;
	public GameObject sourceMultiLine;
	public List<GameObject> multiLines = new List<GameObject> ();
	private GameObject menu2;
	private GameObject menuRobot;
	private float cooldownDisplacement = 0f;
	private Vector3 displacement;
	private GameObject credits1;
	private GameObject credits2;
	private GameObject targetPlane;
	private float tutorialStep = 0f;
	private float timeEmitting = 0.2f;

	public Vector3 direction;


	public Vector3 lockedCamera;


	// Use this for initialization
	void Start () {

		/*
		for (int i = 0; i < 6; i++) {
			GameObject newMultiline = Instantiate (sourceMultiLine);
			multiLines.Add (newMultiline);
		}
		*/

		menu2 = GameObject.Find ("Menu2");
		menuRobot = GameObject.Find ("Menu2/Robot");
		credits1 = GameObject.Find ("Menu2/Credits1");
		credits2 = GameObject.Find ("Menu2/Credits2");
		credits1.SetActive (false);
		credits2.SetActive (false);
		targetPlane = GameObject.Find ("Target");
		targetPlane.transform.FindChild("Text").gameObject.GetComponent<TextMesh>().text = "W-A-S-D";
		
	}
	

	// Update is called once per frame
	void Update () {

		if (pulseCooldown > 0) {
			pulseCooldown -= Time.deltaTime;
			if (pulseCooldown < 0) { 
				pulseCooldown = 0;
			}
		}
		
		player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		
		float personalT = this.gameObject.GetComponent<CompetitorScript> ().personalT;


		Vector3 aux_point = main.getPointBSpline (personalT);
		//direction = main.getPointBSpline (personalT + 0.001f) - aux_point;
		direction = main.getPointBSpline (personalT + 0.001f) - aux_point + main.GetComponent<MainScript> ().direcAux;
		direction.Normalize();




		if (!main.GetComponent<MainScript> ().menu) {
			float separacion = 1.5f; // ESTO PUEDE SER CUALQUIER VALOR A PARTIR DE 1
			float distanciaTrasera;
			float minDistanciaTrasera = 20f;
			float maxDistanciaTrasera = 20.5f;
			float currentFieldOfView;
			float minFieldOfView = 60f;
			float maxFieldOfView = 80f;
			distanciaTrasera = minDistanciaTrasera*(2f-this.gameObject.GetComponent<CompetitorScript>().currentSpeed) + maxDistanciaTrasera*(this.gameObject.GetComponent<CompetitorScript>().currentSpeed-1f);

			currentFieldOfView = minFieldOfView*((maxDistanciaTrasera - distanciaTrasera)/(maxDistanciaTrasera - minDistanciaTrasera)) + maxFieldOfView*((distanciaTrasera - minDistanciaTrasera)/(maxDistanciaTrasera - minDistanciaTrasera));

			if (distanciaTrasera < minDistanciaTrasera) { distanciaTrasera = minDistanciaTrasera; }

			if (currentFieldOfView < minFieldOfView) { currentFieldOfView = minFieldOfView; }

			if (personalT > main.endingT) {
				Vector3 camera_to_point = main.getPointBSpline(main.endingT);
				//camera.transform.localPosition = new Vector3(0, 0, 0);


				Vector3 median = (main.getPointBSpline (personalT) + player.transform.position) / 2; 
				if (personalT > main.endingT +1.0f) {

					if (lockedCamera == new Vector3(0, 0, 0)) {
						lockedCamera = median;
					}
					else {
						median = lockedCamera;
					}

					if (camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity == 0.1f && (Input.GetKeyDown("space") || Input.GetButtonDown("Fire1")) ) {
						Application.LoadLevel("Game");
					}

					//Application.LoadLevel("Scene1");

				}

				camera.transform.position = Vector3.Lerp(camera.transform.position, camera_to_point, 0.2f);
				camera.transform.LookAt (median);

				if (camera.transform.parent != null) {
					camera.transform.parent = null;
				}

			}
			else {

				distanciaTrasera += main.GetComponent<MainScript> ().separacionAux;
				camera.transform.position = Vector3.Lerp(camera.transform.position, player.transform.position - player.transform.localPosition + player.transform.localPosition / (separacion) - direction * distanciaTrasera, 0.1f);
				Vector3 median = (main.getPointBSpline (personalT) + player.transform.position) / 2; 
				camera.transform.LookAt (median);

				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, currentFieldOfView, Time.deltaTime);

			}

			float max_intensity = 1.5f;
			Color max_color = new Color(255f/255f, 112f/255f, 112f/255f);
			Color ambient = new Color(156f/255f, 156/255f, 156/255f);


			if(player.GetComponentInParent<CompetitorScript>().personalT > main.endingT) {
				if(camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity > 0.1f) {
					camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity -= Time.deltaTime/2f;
					if(camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity <= 0.1f) {
						camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity = 0.1f;
					}
					float proporcion = camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity / max_intensity;
					
					float r = 0*(1f-proporcion) + max_color.r*proporcion;
					float g = 0*(1f-proporcion) + max_color.g*proporcion;
					float b = 0*(1f-proporcion) + max_color.b*proporcion;

					r = 0f;
					g = 0f;
					b = 0f;
					
					camera.backgroundColor = new Color (r, g, b);
					
					float r2 = 0*(1f-proporcion) + ambient.r*proporcion;
					float g2 = 0*(1f-proporcion) + ambient.g*proporcion;
					float b2 = 0*(1f-proporcion) + ambient.b*proporcion;
					
					
					RenderSettings.ambientLight = new Color(r2, g2, b2);
					
					camera.GetComponent<AudioSource>().volume = 0.1f*(1f-proporcion) + 1f*proporcion;
					
					//camera.backgroundColor = max_color;
				}
			}
			else {
				if(camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity < max_intensity) {
					camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity += Time.deltaTime/2f;
					if(camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity > max_intensity) {
						camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity = max_intensity;
					}
					float proporcion = camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity / max_intensity;
					
					float r = 0*(1f-proporcion) + max_color.r*proporcion;
					float g = 0*(1f-proporcion) + max_color.g*proporcion;
					float b = 0*(1f-proporcion) + max_color.b*proporcion;

					r = 0f;
					g = 0f;
					b = 0f;
					
					camera.backgroundColor = new Color (r, g, b);
					
					float r2 = 0*(1f-proporcion) + ambient.r*proporcion;
					float g2 = 0*(1f-proporcion) + ambient.g*proporcion;
					float b2 = 0*(1f-proporcion) + ambient.b*proporcion;
					
					
					RenderSettings.ambientLight = new Color(r2, g2, b2);
					
					camera.GetComponent<AudioSource>().volume = 0.1f*(1f-proporcion) + 1f*proporcion;
					
					//camera.backgroundColor = max_color;
				}
			}



			AudioSource audio = camera.GetComponent<AudioSource>();
			float[] spectrum = audio.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
			float median_volume = 0f;
			for (int i = 0; i < spectrum.Length; i++) {
				median_volume += spectrum[i];
			}
			median_volume = median_volume / spectrum.Length;

			//Debug.Log (median_volume);

			camera.transform.FindChild("Point light").GetComponent<Light>().intensity = median_volume*10000f*((camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity-0.1f) / max_intensity);

			//Debug.Log (median_volume*10000f);


			if (median_volume*10000f > 6f && pulseCooldown == 0) {
				//Debug.Log ("LOL");
				pulseCooldown = pulseCooldownDefault;

			}


			if (!this.GetComponentInParent<AudioSource>().isPlaying) {
				this.GetComponentInParent<AudioSource>().Play();
			}


		} else {


			float speedMenu = 70f*player.GetComponentInParent<CompetitorScript>().redBonusTotal;

			if (player.GetComponentInParent<CompetitorScript>().collidingWhite && player.GetComponentInParent<CompetitorScript>().shredding) {
				speedMenu = 10f;

			}
			//Debug.Log (player.GetComponentInParent<CompetitorScript>().redBonusTotal);


			targetPlane.transform.position = player.transform.position + new Vector3(2.6f, 0f, 0f);

			menu2.transform.position = new Vector3 (menu2.transform.position.x, menu2.transform.position.y, menu2.transform.position.z + Time.deltaTime * speedMenu);
			player.transform.parent.transform.position = menu2.transform.position;

			cooldownDisplacement -= Time.deltaTime;
			if (cooldownDisplacement <= 0f) {
				cooldownDisplacement = 1f;
				displacement = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
				displacement.Normalize();
			}


			if (tutorialStep == 0f) {
				if (Input.GetKey("a") || Input.GetKey("left") || Input.GetKey("d") || Input.GetKey("right")
				    || Input.GetKey("s") || Input.GetKey("down") || Input.GetKey("w") || Input.GetKey("up")) {
					tutorialStep = 1f;
					targetPlane.transform.FindChild("Text").gameObject.GetComponent<TextMesh>().text = "SPACE";
					//targetPlane.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "SPACE";
				}
			}
			else if (tutorialStep == 1f) {
				if (Input.GetKey("space")) {
					tutorialStep = 2f;
					targetPlane.SetActive(false);
					//targetPlane.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "SPACE";
				}
			}

			if (menuRobot.GetComponent<ParticleSystem>().enableEmission && timeEmitting > 0f) {
				timeEmitting -= Time.deltaTime;
				if (timeEmitting <= 0f) {
					menuRobot.GetComponent<ParticleSystem>().enableEmission = false;
				}
			}

			menuRobot.transform.localPosition = menuRobot.transform.localPosition + displacement*Time.deltaTime*20f;
			if (menuRobot.transform.localPosition.x > 10f) { menuRobot.transform.localPosition = new Vector3(10f, menuRobot.transform.localPosition.y, menuRobot.transform.localPosition.z); }
			if (menuRobot.transform.localPosition.x < -10f) { menuRobot.transform.localPosition = new Vector3(-10f, menuRobot.transform.localPosition.y, menuRobot.transform.localPosition.z); }
			if (menuRobot.transform.localPosition.y > 10f) { menuRobot.transform.localPosition = new Vector3(menuRobot.transform.localPosition.x, 10f, menuRobot.transform.localPosition.z); }
			if (menuRobot.transform.localPosition.y < -10f) { menuRobot.transform.localPosition = new Vector3(menuRobot.transform.localPosition.x, -10f, menuRobot.transform.localPosition.z); }
			menuRobot.transform.localPosition = new Vector3(menuRobot.transform.localPosition.x, menuRobot.transform.localPosition.y, 2f);

			if (menuRobot.GetComponent<TrailRenderer>().enabled && Vector3.Distance(menuRobot.transform.position, player.transform.position) < 4f && player.GetComponentInParent<CompetitorScript>().shredding) {
				menuRobot.transform.FindChild("Parasyte").gameObject.SetActive(false);
				menuRobot.transform.FindChild("Text").gameObject.SetActive(false);
				menuRobot.GetComponent<TrailRenderer>().enabled = false;
				credits1.SetActive(true);
				credits1.transform.position = menuRobot.transform.position;
				credits2.SetActive(true);
				credits2.transform.position = menuRobot.transform.position;
				menuRobot.GetComponent<ParticleSystem>().enableEmission = true;
			}

			if (credits1.activeInHierarchy) {
				credits1.transform.localPosition = Vector3.Lerp(credits1.transform.localPosition, new Vector3(-25f, 16f, 2f), Time.deltaTime);
				credits2.transform.localPosition = Vector3.Lerp(credits2.transform.localPosition, new Vector3(25f, 16f, 2f), Time.deltaTime);
			}

			//player.transform.position = new Vector3(0, 0, -985);

			//camera.transform.position = new Vector3(0, 0, -200);
			camera.transform.LookAt (new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z +1f));
			camera.backgroundColor = new Color (0, 0, 0);
			camera.transform.FindChild("Spotlight").GetComponent<Light>().intensity = 0f;

			camera.GetComponent<AudioSource>().volume = 0.1f;

			AudioSource audio = camera.GetComponent<AudioSource>();
			float[] spectrum = audio.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
			float median_volume = 0f;
			for (int i = 0; i < spectrum.Length; i++) {
				median_volume += spectrum[i];
			}
			median_volume = median_volume / spectrum.Length;

			float aux_scale = 1f + median_volume*100f;

			//GameObject.Find("Menu").transform.localScale = new Vector3(aux_scale, aux_scale, aux_scale);
			GameObject.Find("Menu2/Jugar").transform.localScale = new Vector3(aux_scale, aux_scale, aux_scale);
			GameObject.Find("Menu2/Salir").transform.localScale = new Vector3(aux_scale, aux_scale, aux_scale);
			GameObject.Find("Menu2/start").transform.localScale = new Vector3(aux_scale, aux_scale, aux_scale);

		}



	}


	
}
