using UnityEngine;
using System.Collections;

public class IAScript : MonoBehaviour {

	public MainScript main;
	public GameObject robot;

	public string state = "idle";

	public Vector2 relativeCircle = new Vector2(0, 0);
	public Vector2 absoluteCircle = new Vector2(0, 0);
	
	public float controlSpeed = 20f;
	
	public float personalT = 0f;
	
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

		//relativeCircle = new Vector2 (0f, 0f);

	}


	void FixedUpdate() {
		
		//CAMBIOS DE VELOCIDAD

		float deadModifier = 1f;

		if (dead > 0f) {
			deadModifier = 0f;
		}

		loserModifier = main.leadingT - personalT +1;
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
			
			currentSpeed = currentSpeed*(1f -modifier) + targetSpeed*(modifier)*whiteModifier*deadModifier;
			
		} else {
			float modifier = 1f;
			
			if (shredding && currentRush <= 0) {
				// LA VELOCIDAD TINE QUE BAJAR SUPER RAPIDO
				modifier = shreddingModifier;
			} else {
				// LA VELOCIDAD TIENE QUE BAJAR LENTAMENTE
				modifier = bulletModifier;
			}
			
			currentSpeed = currentSpeed*(1f -modifier) + targetSpeed*(modifier)*whiteModifier*deadModifier;
			
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
		
		// MODIFICADORES VELOCIDAD

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
			if (redEffect < 0f) { redEffect = 0f; }
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
		
		transform.position = new Vector3(aux_point.x, aux_point.y, aux_point.z);
		
		robot.transform.localPosition = new Vector3(0, 0, 0);
		
		
		float t2 = personalT + 0.01f;
		
		
		robot.transform.LookAt(main.getPointBSpline(t2));
		
		
		Vector3 aux_point_player = new Vector3(0, 0, 0);
		
		movement = movement*controlSpeed*Time.deltaTime;
		
		Vector2 absoluteCircle = new Vector2 (relativeCircle.x*main.currentRadius, relativeCircle.y*main.currentRadius);
		
		absoluteCircle = new Vector2 (absoluteCircle.x + movement.x, absoluteCircle.y + movement.y);

		//Debug.Log (absoluteCircle);
		
		if (absoluteCircle.magnitude >= (main.currentRadius-1f)) {
			absoluteCircle.Normalize();
			absoluteCircle = new Vector2(absoluteCircle.x*(main.currentRadius-1f), absoluteCircle.y*(main.currentRadius-1f));
		}
		
		
		aux_point_player += robot.transform.right*absoluteCircle.x;
		aux_point_player += robot.transform.up*absoluteCircle.y;
		
		relativeCircle = absoluteCircle/main.currentRadius;
		
		
		robot.transform.localPosition = aux_point_player;
		
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
			//Debug.Log ("LOL");
			//this.gameObject.transform.FindChild ("Player/Parasyte").GetComponent<Animator> ().Play ("Open");
			this.gameObject.transform.FindChild("Robot/Parasyte").GetComponent<Animator>().CrossFade("Open", 1f);
			shredding = true;
		} else {
			//this.gameObject.transform.FindChild ("Player/Parasyte").GetComponent<Animator> ().Play ("Closed");
			this.gameObject.transform.FindChild("Robot/Parasyte").GetComponent<Animator>().CrossFade("Closed", 1f);
			shredding = false;
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
		/*
		else if (lookForAPrey()) {
			// DETECTADO UN PARASITO DELANTE QUE PUEDE MATAR
			state = "kill";
		} 
		*/
		else if (isBeingChased()) {
			// DETECTADO UN PARASITO DETRAS QUE PUEDE MATARLO
			state = "escape";
		}
		else if (lookForAPrey()) {
			// DETECTADO UN PARASITO DELANTE QUE PUEDE MATAR
			state = "kill";
		}
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
