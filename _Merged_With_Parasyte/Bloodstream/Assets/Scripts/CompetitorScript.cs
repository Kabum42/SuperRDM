using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CompetitorScript : MonoBehaviour {
	
	public int controlledBy = 0;
	public MainScript main;
	public GameObject robot;

	public int positionInRace = 0;
	public string state = "idle";
	
	public Vector2 relativeCircle = new Vector2(0, 0);
	public Vector2 absoluteCircle = new Vector2(0, 0);
	
	public float controlSpeed = 20f;
	
	public float personalT = 1f;
	
	public float baseSpeed = 0.9f;
	public float bulletModifier = 0.01f;
	public float shreddingModifier = 0.5f;
	
	public float defaultRush = 0.5f;
	
	public float redBonusPerRed = 0.05f;
	public float redBonusCap = 2f;
	
	public float redEffectDefault = 1f;
	public float redEffectShreddingHandicap = 0.5f;
	
	public bool shredding = false;
	private float angle_shredding = 0;
	private bool clockwise_shredding = true;
	public float controlModifier = 0f;
	
	public float currentRush = 0f;
	
	public float redEffect = 0f;
	public float redBonusTotal = 1f;
	
	public float currentSpeed;
	public float targetSpeed;
	
	public bool collidingWhite = false;
	public float whiteModifier = 1f;
	
	public float loserModifier = 1f;

	public AudioSource turbo;
	public AudioSource open1;
	public AudioSource open2;
	public AudioSource open3;
	public AudioSource open4;
	public AudioSource close1;
	public AudioSource close2;
	public AudioSource close3;
	public AudioSource close4;


	//public float lastKnownSpeed = 0f;
	
	public float dead = 0f;
	
	private GameObject target;
	
	private Vector3 movement;
	private bool openClamps = false;
	private bool openClampsDown = false;
	
	// Use this for initialization
	void Start () {
		
		currentSpeed = baseSpeed;
		targetSpeed = baseSpeed;
		
		relativeCircle = new Vector2 (Random.Range (-1f, 1f), Random.Range (-1f, 1f));

		if (this.tag == "Player") {
			relativeCircle = new Vector2 (0f, 0f);
		}

		turbo = gameObject.AddComponent<AudioSource> ();
		turbo.clip = Resources.Load ("Turbo_1") as AudioClip;
		turbo.volume = 0.25f;
		turbo.spatialBlend = 1f;
		turbo.minDistance = 30f;
		turbo.maxDistance = 60f;

		open1 = gameObject.AddComponent<AudioSource> ();
		open1.clip = Resources.Load ("Open") as AudioClip;
		open1.volume = 0.25f;
		open1.spatialBlend = 1f;
		open1.minDistance = 30f;
		open1.maxDistance = 60f;
		
		open2 = gameObject.AddComponent<AudioSource> ();
		open2.clip = Resources.Load ("Open_2") as AudioClip;
		open2.volume = 0.25f;
		open2.spatialBlend = 1f;
		open2.minDistance = 30f;
		open2.maxDistance = 60f;

		open3 = gameObject.AddComponent<AudioSource> ();
		open3.clip = Resources.Load ("Open_3") as AudioClip;
		open3.volume = 0.25f;
		open3.spatialBlend = 1f;
		open3.minDistance = 30f;
		open3.maxDistance = 60f;

		close1 = gameObject.AddComponent<AudioSource> ();
		close1.clip = Resources.Load ("Close_1") as AudioClip;
		close1.volume = 0.25f;
		close1.spatialBlend = 1f;
		close1.minDistance = 30f;
		close1.maxDistance = 60f;

		close2 = gameObject.AddComponent<AudioSource> ();
		close2.clip = Resources.Load ("Close_2") as AudioClip;
		close2.volume = 0.25f;
		close2.spatialBlend = 1f;
		close2.minDistance = 30f;
		close2.maxDistance = 60f;

		close3 = gameObject.AddComponent<AudioSource> ();
		close3.clip = Resources.Load ("Close_3") as AudioClip;
		close3.volume = 0.25f;
		close3.spatialBlend = 1f;
		close3.minDistance = 30f;
		close3.maxDistance = 60f;

		
		//relativeCircle = new Vector2 (0f, 0f);
		
	}
	
	
	void FixedUpdate() {
		
		//CAMBIOS DE VELOCIDAD
		
		float deadModifier = 1f;
		
		if (dead > 0f) {
			deadModifier = 0f;
		}

		if (main.sortedCompetitors.Count > 0) {
			if (positionInRace == 1) {
				loserModifier = 1;
			} else {

				CompetitorScript competitorInFront = main.sortedCompetitors[main.sortedCompetitors.Count -positionInRace +1].GetComponentInParent<CompetitorScript>();

				float idealDistanceBetweenCompetitors = 0.2f;
				float distanceBetweenThese2 = competitorInFront.personalT - personalT;

				//Debug.Log(distanceBetweenThese2);

				float extraLosingModifier = 0f;
				if (distanceBetweenThese2 > idealDistanceBetweenCompetitors) {

					extraLosingModifier = distanceBetweenThese2 - idealDistanceBetweenCompetitors;
					if (extraLosingModifier > 0.5f) {
						extraLosingModifier = 0.5f;
					}

				}

				loserModifier = competitorInFront.loserModifier +extraLosingModifier;
				
			}
		}


		
		//loserModifier = main.leadingT - personalT +1;
		//loserModifier = Mathf.Sqrt (loserModifier);
		
		if (collidingWhite) {
			whiteModifier -= 0.05f;
			if (shredding) { 
				whiteModifier -= 0.15f; 
				if (whiteModifier <= 0f) { whiteModifier = 0f; }
			}
			else {
				if (whiteModifier <= 0.1f) { whiteModifier = 0.1f; }
			}
			
		}
		else {
			whiteModifier += 0.05f;
			if (!shredding) { whiteModifier += 0.05f; }
			if (whiteModifier >= 1f) {
				whiteModifier = 1f;
			}
		}
		
		controlSpeed = (20f + controlModifier*60f)* (whiteModifier);
		
		
		if (currentSpeed < targetSpeed) {
			// LA VELOCIDAD TIENE QUE AUMENTAR SUPER RAPIDO
			float modifier = shreddingModifier;
			
			currentSpeed = (currentSpeed*(1f -modifier) + targetSpeed*(modifier)*whiteModifier)*deadModifier;
			
		} else {
			float modifier = 1f;
			
			if (shredding && currentRush <= 0) {
				// LA VELOCIDAD TINE QUE BAJAR SUPER RAPIDO
				modifier = shreddingModifier;
			} else {
				// LA VELOCIDAD TIENE QUE BAJAR LENTAMENTE
				modifier = bulletModifier;
			}
			
			currentSpeed = (currentSpeed*(1f -modifier) + targetSpeed*(modifier)*whiteModifier)*deadModifier;
			
		}
		
		
		
		collidingWhite = false;
		
		/*
		evaluatePriority();

		if (state == "idle") {
			openClampsDown = false;
			openClamps = false;
		}
		else if (state == "kill") {
			// TIENEN QUE IR A POR SU TARGET, QUE ES UN PARASITO
			chaseTarget();
		}
		else if (state == "escape") {
			avoidTarget();
		}
		else if (state == "avoid") {
			// TIENEN QUE EVITAR AL GLOBULO BLANCO
			avoidTarget();
		}
		else if (state == "chase1") {

		}
		else if (state == "chase2") {
			// TIENEN QUE IR A POR SU TARGET, QUE ES EL GLOBULO ROJO
			chaseTarget();
		}
		*/
		
	}
	
	// Update is called once per frame
	
	
	void Update () {

		if (dead > 0f) {
			dead -= Time.deltaTime;
			this.transform.FindChild("Robot/Parasyte/Cube.003").GetComponent<Renderer>().material.SetFloat("_Amount", -dead);
			this.transform.FindChild("Robot").GetComponent<TrailRenderer>().enabled = false;
			if (dead <= 0f) {
				dead = 0f;
				this.transform.FindChild("Robot").GetComponent<TrailRenderer>().enabled = true;
			}
			if (this.tag == "Player") {
				Camera.main.GetComponent<MotionBlur>().blurAmount = dead*(0.8f);
			}
		}
		else {
			float aux_amount = Mathf.Lerp(this.transform.FindChild("Robot/Parasyte/Cube.003").GetComponent<Renderer>().material.GetFloat("_Amount"), 0f, Time.deltaTime*10f);
			this.transform.FindChild("Robot/Parasyte/Cube.003").GetComponent<Renderer>().material.SetFloat("_Amount", aux_amount);
			if (this.tag == "Player") {
				Camera.main.GetComponent<MotionBlur>().blurAmount = 0f;
			}
		}

		if (dead >= 0.8f) {
			this.transform.FindChild ("Robot").GetComponent<ParticleSystem> ().enableEmission = true;} 
		else {this.transform.FindChild ("Robot").GetComponent<ParticleSystem> ().enableEmission = false;}
				                                                                                                

		
		// MODIFICADORES VELOCIDAD
		
		if (controlledBy == 0) {
			// EL CONTROLOADOR 0 ES EL ORDENADOR
			
			evaluatePriority();
			
			if (dead > 0) {
				// SE QUEDA EN IDLE QUE PARA ALGO ESTA MUERTO
				state = "idle";
			}
			
			if (state == "idle") {
				openClampsDown = false;
				openClamps = false;
			}
			else if (state == "kill") {
				// TIENEN QUE IR A POR SU TARGET, QUE ES UN PARASITO
				chaseTarget();
			}
			else if (state == "escape") {
				avoidTarget();
			}
			else if (state == "avoid") {
				// TIENEN QUE EVITAR AL GLOBULO BLANCO
				avoidTarget();
			}
			else if (state == "chase1") {
				
			}
			else if (state == "chase2") {
				// TIENEN QUE IR A POR SU TARGET, QUE ES EL GLOBULO ROJO
				chaseTarget();
			}
		}
		
		
		
		if (currentRush > 0f) {
			currentRush -= Time.deltaTime;
			if (currentRush < 0f) { currentRush = 0f; }
		}
		
		if (controlModifier > 0f) {
			controlModifier -= Time.deltaTime;
			if (controlModifier < 0f) { controlModifier = 0f; }
		}



		if (redEffect > 0f) {
			redEffect -= Time.deltaTime;
			if (redEffect <= 0f) { redEffect = 0f; }
		}
		
		
		if (redEffect <= 0) {
			redBonusTotal = 1f;
		}
		
		targetSpeed = (baseSpeed*redBonusTotal*whiteModifier*loserModifier);
		
		// FIN MODIFICADORES VELOCIDAD
		if (!main.menu) {
			personalT += Time.deltaTime/2 * currentSpeed;
		}
		
		
		Vector3 aux_point = main.getPointBSpline (personalT);
		Vector3 direction = main.getPointBSpline (personalT + 0.001f) - aux_point;
		direction.Normalize();

		if (!main.menu || this.tag != "Player") {
			transform.position = new Vector3(aux_point.x, aux_point.y, aux_point.z);
			robot.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
			robot.transform.localPosition = new Vector3(0, 0, 0);
		}


		
		
		float t2 = personalT + 0.01f;
		
		
		robot.transform.LookAt(main.getPointBSpline(t2));
		
		
		Vector3 aux_point_player = new Vector3(0, 0, 0);
		
		if (controlledBy == 1) {
			
			float movement_x = 0f;
			float movement_y = 0f;
			
			if (Input.GetAxis ("Horizontal2") != 0 || Input.GetAxis ("Vertical2") != 0) {

				movement_x += Input.GetAxis ("Horizontal2");
				movement_y += Input.GetAxis ("Vertical2");
				
				movement = new Vector3 (movement_x, movement_y, 0);
				
			} else {
				
				if (Input.GetKey("a") || Input.GetKey("left")) {
					movement_x += -1;
				}
				if (Input.GetKey("d") || Input.GetKey("right")) {
					movement_x += +1;
				}
				
				if (Input.GetKey("s") || Input.GetKey("down")) {
					movement_y += -1;
				}
				if (Input.GetKey("w") || Input.GetKey("up")) {
					movement_y += +1;
				}
				
				movement = new Vector3 (movement_x, movement_y, 0);
				movement.Normalize ();
				
			}
			
		}
		
		movement = movement*controlSpeed*Time.deltaTime;
		
		absoluteCircle = new Vector2 (relativeCircle.x*main.currentRadius, relativeCircle.y*main.currentRadius);
		
		absoluteCircle = new Vector2 (absoluteCircle.x + movement.x, absoluteCircle.y + movement.y);
		
		if (absoluteCircle.magnitude >= (main.currentRadius-1f)) {
			absoluteCircle.Normalize();
			absoluteCircle = new Vector2(absoluteCircle.x*(main.currentRadius-1f), absoluteCircle.y*(main.currentRadius-1f));
		}

		
		if (!main.menu || this.tag != "Player") {
			// CONDICIONES NORMALES
			aux_point_player += robot.transform.right*absoluteCircle.x;
			aux_point_player += robot.transform.up*absoluteCircle.y;
			relativeCircle = absoluteCircle/main.currentRadius;
			robot.transform.localPosition = aux_point_player;
		} else {
			robot.transform.eulerAngles = new Vector3(0f, 0f, 0f);

			aux_point_player += robot.transform.right*absoluteCircle.x;
			aux_point_player += robot.transform.up*absoluteCircle.y;
			relativeCircle = absoluteCircle/main.currentRadius;
			robot.transform.localPosition = aux_point_player;
		}

		
		if (controlledBy == 1) {
			
			if (Input.GetKeyDown("space") || Input.GetButtonDown("Fire1")) {
				currentRush = defaultRush;
				controlModifier = 0.5f;

				float soundOpen = Random.Range(0f, 100f);
				
				if (soundOpen <= (100f/3f)*1f) {
					open1.Play ();
				}
				else if (soundOpen <= (100f/3f)*2f) {
					open2.Play ();
				}
				else if (soundOpen <= (100f/3f)*3f) {
					open3.Play ();
				}

				if (movement.x != 0 || movement.y != 0) {
					// SE PUEDE CAMBIAR EL CLOCKWISE
					float aux_angle = Mathf.Rad2Deg*Mathf.Atan2(movement.y, movement.x);
					if (aux_angle < 0) { aux_angle += 360; }
					
					if (aux_angle < 90 || aux_angle >= 270) {
						clockwise_shredding = false;
					}
					else {
						clockwise_shredding = true;
					}
				}
			}
			else if (Input.GetKey("space") || Input.GetButton("Fire1")) {
				//Debug.Log ("LOL");
				//this.gameObject.transform.FindChild ("Player/Parasyte").GetComponent<Animator> ().Play ("Open");
				this.transform.FindChild("Robot").gameObject.GetComponent<SphereCollider>().radius = 10f;
				this.gameObject.transform.FindChild("Robot/Parasyte").GetComponent<Animator>().CrossFade("Open", 1f);
				shredding = true;



			} else {
				//this.gameObject.transform.FindChild ("Player/Parasyte").GetComponent<Animator> ().Play ("Closed");
				this.transform.FindChild("Robot").gameObject.GetComponent<SphereCollider>().radius = 5f;
				this.gameObject.transform.FindChild("Robot/Parasyte").GetComponent<Animator>().CrossFade("Closed", 1f);

				if (shredding) { 

					float soundClose = Random.Range(0f, 100f);
					
					if (soundClose <= (100f/3f)*1f) {
						close1.Play ();
					}
					else if (soundClose <= (100f/3f)*2f) {
						close2.Play ();
					}
					else if (soundClose <= (100f/3f)*3f) {
						close3.Play ();
					}
				}

				shredding = false;
			}
			
		}

		if (controlledBy == 0) {
			if (openClampsDown) {
				currentRush = defaultRush;
				controlModifier = 0.5f;
				if (movement.x != 0 || movement.y != 0) {
					// SE PUEDE CAMBIAR EL CLOCKWISE
					float aux_angle = Mathf.Rad2Deg*Mathf.Atan2(movement.y, movement.x);
					if (aux_angle < 0) { aux_angle += 360; }
					
					if (aux_angle < 90 || aux_angle >= 270) {
						clockwise_shredding = false;
					}
					else {
						clockwise_shredding = true;
					}
				}
			}
			else if (openClamps) {
				this.gameObject.transform.FindChild("Robot/Parasyte").GetComponent<Animator>().CrossFade("Open", 1f);
				shredding = true;
			} else {
				this.gameObject.transform.FindChild("Robot/Parasyte").GetComponent<Animator>().CrossFade("Closed", 1f);
				shredding = false;
			}
		}

		
		
		if (currentRush > 0f) {
			
			if (clockwise_shredding) {
				angle_shredding += Time.deltaTime*currentRush*2000f;
			}
			else {
				angle_shredding -= Time.deltaTime*currentRush*2000f;
			}
			
			
			if (angle_shredding > 360f) {
				angle_shredding -= 360f;
			}
			else if (angle_shredding < 0f) {
				angle_shredding += 360f;
			}
			
		}
		
		this.gameObject.transform.FindChild("Robot").transform.RotateAround (this.gameObject.transform.FindChild("Robot").transform.position, this.gameObject.transform.FindChild("Robot").transform.forward, -angle_shredding);
		
	}
	
	
	
	
	void evaluatePriority() {
		
		state = "idle";
		
		if (dead > 0) {
			// SE QUEDA EN IDLE QUE PARA ALGO ESTA MUERTO
			state = "idle";
		}

		else if (lookForAPrey()) {
			// DETECTADO UN PARASITO DELANTE QUE PUEDE MATAR
			state = "kill";
		} 

		else if (isBeingChased()) {
			// DETECTADO UN PARASITO DETRAS QUE PUEDE MATARLO
			state = "escape";
		}
		/*
		else if (lookForAPrey()) {
			// DETECTADO UN PARASITO DELANTE QUE PUEDE MATAR
			state = "kill";
		}
		*/
		else if (lookForAWhite()) {
			// DETECTADO UN GLOBULO BLANCO DELANTE QUE SE VA A COMER CON PATATAS
			state = "avoid";
		}
		else if (lookForAPowerUp()) {
			// DETECTADO UN POWER UP QUE PUEDE PILLAR
			state = "chase1";
		}
		else if (lookForARed()) {
			// DETECTADO UN GLOBULO ROJO QUE PUEDE PILLAR
			state = "chase2";
		}

	}
	
	bool lookForAPrey() {
		
		float defaultMinimumDistance = 10f;
		float minimumDistance = defaultMinimumDistance;
		
		for (int i = 0; i < main.competitors.Count; i++) {
			if (main.competitors[i].transform.FindChild("Robot")) {
				// ES UN ROBOT
				float difference = main.competitors[i].transform.FindChild("Robot").transform.position.z - this.transform.FindChild("Robot").transform.position.z;
				
				if (difference > 0 && difference < minimumDistance) {
					minimumDistance = difference;
					target = main.competitors[i].transform.FindChild("Robot").gameObject;
				}
				
			}
			else if (main.competitors[i].transform.FindChild("Player")) {
				// ES UN PLAYER
				float difference = main.competitors[i].transform.FindChild("Player").transform.position.z - this.transform.FindChild("Robot").transform.position.z;
				
				if (difference > 0 && difference < minimumDistance) {
					minimumDistance = difference;
					target = main.competitors[i].transform.FindChild("Player").gameObject;
				}
				
			}
		}
		
		if (minimumDistance < defaultMinimumDistance) {
			return true;
		}
		else {
			return false;
		}
		
	}
	
	
	bool isBeingChased() {
		
		float defaultMinimumDistance = 9f;
		float minimumDistance = defaultMinimumDistance;
		
		for (int i = 0; i < main.competitors.Count; i++) {
			if (main.competitors[i].transform.FindChild("Robot")) {
				// ES UN ROBOT
				float difference = main.competitors[i].transform.FindChild("Robot").transform.position.z - this.transform.FindChild("Robot").transform.position.z;
				
				if (difference > 0 && difference < minimumDistance) {
					minimumDistance = difference;
					target = main.competitors[i].transform.FindChild("Robot").gameObject;
				}
				
			}
			else if (main.competitors[i].transform.FindChild("Player")) {
				// ES UN PLAYER
				float difference = main.competitors[i].transform.FindChild("Player").transform.position.z - this.transform.FindChild("Robot").transform.position.z;
				
				if (difference > 0 && difference < minimumDistance) {
					minimumDistance = difference;
					target = main.competitors[i].transform.FindChild("Player").gameObject;
				}
				
			}
		}
		
		if (minimumDistance < defaultMinimumDistance) {
			return true;
		}
		else {
			return false;
		}
		
	}
	
	bool lookForAWhite() {
		
		float defaultMinimumDistance = 15f;
		float minimumDistance = defaultMinimumDistance;
		
		for (int i = 0; i < main.whites.Count; i++) {
			
			if (main.whites[i].transform.position.z > this.gameObject.transform.position.z) {
				// EL GLOBULO ESTA DELANTE
				
				if (Vector3.Distance(main.whites[i].transform.position, this.gameObject.transform.position) < minimumDistance) {
					// EL EL GLOBULO ESTA CERCA
					
					minimumDistance = Vector3.Distance(main.whites[i].transform.position, this.gameObject.transform.position);
					target = main.whites[i];
					
				}
				
			}
			
		}
		
		if (minimumDistance < defaultMinimumDistance) {
			return true;
		}
		else {
			return false;
		}
		
	}
	
	bool lookForAPowerUp() {
		
		return false;
		
	}
	
	
	bool lookForARed() {
		
		float defaultMinimumDistance = 15f;
		float minimumDistance = defaultMinimumDistance;
		
		
		for (int i = 0; i < main.reds.Count; i++) {
			
			if (main.reds[i].transform.position.z > this.gameObject.transform.position.z) {
				// EL GLOBULO ESTA DELANTE
				
				if (Vector3.Distance(main.reds[i].transform.position, this.gameObject.transform.position) < minimumDistance) {
					// EL EL GLOBULO ESTA CERCA
					
					minimumDistance = Vector3.Distance(main.reds[i].transform.position, this.gameObject.transform.position);
					target = main.reds[i];
					
				}
				
			}
			
		}
		
		if (minimumDistance < defaultMinimumDistance) {
			return true;
		}
		else {
			return false;
		}
		
	}
	
	void chaseTarget() {
		
		Vector3 virtualTarget = target.transform.position;
		
		movement = new Vector3 (virtualTarget.x - robot.transform.position.x, virtualTarget.y - robot.transform.position.y);
		
		movement.Normalize();
		
		if (!openClamps) {
			openClampsDown = true;
		}
		else {
			openClampsDown = false;
		}
		openClamps = true;
		
	}
	
	void avoidTarget() {
		
		Vector3 virtualTarget = target.transform.position;
		
		movement = new Vector3 (robot.transform.position.x - virtualTarget.x, robot.transform.position.y - virtualTarget.y);
		
		movement.Normalize();
		
		openClampsDown = false;
		openClamps = false;
		
	}
	
	
	
}
