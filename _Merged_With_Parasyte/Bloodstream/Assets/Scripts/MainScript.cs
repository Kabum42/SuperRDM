using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MainScript : MonoBehaviour {


	public class PointPlusMore {

		public Vector3 point;
		public float radius;
		public int drawed;
		public bool end;

		public PointPlusMore(Vector3 aux_point, float aux_radius) {

			point = aux_point;
			radius = aux_radius;
			drawed = 0;
			end = false;

		}

	}
	

	public bool menu = true;
	private bool debug = true;
	private GameObject menu2;

	public float t = 1.00f;

	public float leadingT = 0f;
	public float endingT = 0f;
	public int lastingPoints = 30;

	public List<GameObject> competitors = new List<GameObject> ();
	public List<GameObject> sortedCompetitors = new List<GameObject> ();
	public List<PointPlusMore> points = new List<PointPlusMore> ();
	public int currentDrawingPoint = 0;
	private List<GameObject> tubeObjects = new List<GameObject> ();
	private int currentTubeObject = 0;
	//public int current_point = 1;
	public List<GameObject> reds = new List<GameObject> ();
	public List<GameObject> whites = new List<GameObject> ();

	public float currentRadius = 20f;

	private Vector3[] newVertices = new Vector3[4];
	private Vector2[] newUV = new Vector2[4];
	private int[] newTriangles = new int[6];

	public GameObject theObj;
	public GameObject main;

	public GameObject player;

	public GameObject sourceRobot;
	//private Mesh mesh;

	float timeA;
	public int fps;
	public int lastFPS;
	public GUIStyle textStyle;

	private int playerPosition = 1;

	private int numOfTubes = 10;

	public float slowMo = 0f;

	private GameObject textPosition;
	private GameObject textSpeed;

	public Vector3 direcAux = new Vector3 (0f, 0f, 0f);
	private Vector3 lastMousePosition;
	public float separacionAux = 0f;
	
	// Use this for initialization
	void Start () {

		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		textPosition = GameObject.Find ("Canvas/TextPosition");
		textPosition.SetActive (false);

		textSpeed = GameObject.Find ("Canvas/TextSpeed");
		textSpeed.SetActive (false);

		Physics.gravity = new Vector3 (0, 0, 0);

		competitors.Add (player.transform.parent.gameObject);

		menu2 = GameObject.Find ("Menu2");

		for (int i = 0; i < 10; i++) {

			this.transform.eulerAngles = new Vector3 (0, 0, 0);
			this.transform.position = new Vector3(0, 0, 0);
			GameObject newCompetitor = Instantiate (sourceRobot);
			newCompetitor.GetComponent<CompetitorScript>().main = this;
			newCompetitor.GetComponent<CompetitorScript>().personalT = Random.Range (1.3f, 2.5f);


			for (int j = 0; j < competitors.Count; j++) {
				Physics.IgnoreCollision(newCompetitor.transform.FindChild("Robot").GetComponent<Collider>(), competitors[j].transform.FindChild("Robot").GetComponent<Collider>());
			}




			competitors.Add(newCompetitor);
		}



		timeA = Time.timeSinceLevelLoad;

		//points.Add (new BSpline3D (new Vector3 (0, 0, 0), new Vector3 (0, 0, 100), new Vector3 (100, 0, 200), new Vector3 (0, -200, 300), 10f, 10f));
		//points.Add (new BSpline3D (new Vector3 (0, 0, 100), new Vector3 (100, 0, 200), new Vector3 (0, -200, 300), new Vector3 (-30, 50, 400), 10f, 10f));


		points.Add (new PointPlusMore (new Vector3 (0, 0, 0), 20f));
		/*
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 100), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 200), 10f));
		points.Add (new PointPlusRadius (new Vector3 (100, 0, 300), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, -200, 400), 10f));
		points.Add (new PointPlusRadius (new Vector3 (30, 50, 500), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 600), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 700), 10f));
		points.Add (new PointPlusRadius (new Vector3 (100, 0, 800), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, -200, 900), 10f));
		points.Add (new PointPlusRadius (new Vector3 (30, 50, 1000), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 1100), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 1200), 10f));
		points.Add (new PointPlusRadius (new Vector3 (100, 0, 1300), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, -200, 1400), 10f));
		points.Add (new PointPlusRadius (new Vector3 (30, 50, 1500), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 1600), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, 0, 1700), 10f));
		points.Add (new PointPlusRadius (new Vector3 (100, 0, 1800), 10f));
		points.Add (new PointPlusRadius (new Vector3 (0, -200, 1900), 10f));
		points.Add (new PointPlusRadius (new Vector3 (30, 50, 2000), 10f));
		*/

		Generate (42);

	}

	public void Tube(GameObject obj, Vector3 origin1, Vector3 origin2, Vector3 origin3, float radius1, float radius2, bool overrideObject) {

		GameObject newObj;

		//Mesh mesh = new Mesh();

		if (overrideObject) {
			newObj = obj;
		} else {
			newObj = Instantiate (obj);
			//GetComponent<MeshFilter>().mesh = mesh;
		}

		newObj.GetComponent<MeshFilter> ().mesh = new Mesh ();


		int nbSides = 24;
		
		// Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
		float bottomRadius1 = radius1;
		float topRadius1 = radius2;
		Vector3 direction = new Vector3(origin2.x - origin1.x, origin2.y - origin1.y, origin2.z - origin1.z);
		direction.Normalize ();
		Vector3 direction2 = new Vector3(origin3.x - origin2.x, origin3.y - origin2.y, origin3.z - origin2.z);
		direction2.Normalize ();

		//Debug.Log ("X: "+direction.x);
		//Debug.Log ("Y: "+direction.y);
		//Debug.Log ("Z: "+direction.z);

		int nbVerticesSides = nbSides * 2 + 2;
		if (!debug) { nbVerticesSides = nbSides*1 +2; }

		#region Vertices
		
		// bottom + top + sides
		Vector3[] vertices = new Vector3[nbVerticesSides * 2];
		int vert = 0;
		float _2pi = Mathf.PI * 2f;
		
		// Sides (out)
		int sideCounter = 0;

		if (debug) {
			while (vert < nbVerticesSides )
			{
				sideCounter = sideCounter == nbSides ? 0 : sideCounter;
				
				float r1 = (float)(sideCounter++) / nbSides * _2pi;
				float cos = Mathf.Cos(r1);
				float sin = Mathf.Sin(r1);
				
				
				Vector3 aux_bottom;
				Vector3 aux_top;
				
				Vector3 rotation_aux = main.transform.localEulerAngles;
				rotation_aux.x = 0;
				rotation_aux.y = 0;
				rotation_aux.z = 0;
				main.transform.localEulerAngles = rotation_aux;
				
				main.transform.position = new Vector3(0 +(1)*cos*bottomRadius1, 0 , 0 +(1)*sin*bottomRadius1);
				Vector3 main_rotation = main.transform.localEulerAngles;
				//main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0);
				bool greater = false;
				
				if (direction.y >= 0) {
					greater = true;
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction.y, 2))));
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction.x));
				}
				else {
					greater = false;
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction.y, 2))));
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction.x));
				}
				
				
				
				
				
				aux_top = main.transform.position;
				
				rotation_aux = main.transform.localEulerAngles;
				rotation_aux.x = 0;
				rotation_aux.y = 0;
				rotation_aux.z = 0;
				main.transform.localEulerAngles = rotation_aux;
				
				main.transform.position = new Vector3(0 +(1)*cos*topRadius1, 0 , 0 +(1)*sin*topRadius1);
				main_rotation = main.transform.localEulerAngles;
				
				//main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0);
				
				if (direction2.y >= 0) {
					if (greater) {
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2))));
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
					}
					else {
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -90 -(90 - Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2)))));
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
					}
				}
				else {
					if (greater) {
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 90 +(90 - Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2)))));
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
					}
					else {
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2))));
						main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
					}
				}
				
				
				
				
				
				aux_bottom = main.transform.position;
				
				vertices[vert] = new Vector3(origin2.x + aux_bottom.x, origin2.y + aux_bottom.y, origin2.z + aux_bottom.z /*-sin*direction2.y*bottomRadius1 -cos*direction2.x*bottomRadius1*/);
				vertices[vert + 1] = new Vector3(origin1.x + aux_top.x, origin1.y + + aux_top.y, origin1.z + aux_top.z /*-sin*direction.y*topRadius1 -cos*direction.x*topRadius1*/);
				vert+=2;
			}
		}


		
		// Sides (in)
		sideCounter = 0;
		while (vert < vertices.Length )
		{
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
			
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
			float cos = Mathf.Cos(r1);
			float sin = Mathf.Sin(r1);


			Vector3 aux_bottom;
			Vector3 aux_top;
			
			Vector3 rotation_aux = main.transform.localEulerAngles;
			rotation_aux.x = 0;
			rotation_aux.y = 0;
			rotation_aux.z = 0;
			main.transform.localEulerAngles = rotation_aux;
			
			main.transform.position = new Vector3(0 +(1)*cos*bottomRadius1, 0 , 0 +(1)*sin*bottomRadius1);
			Vector3 main_rotation = main.transform.localEulerAngles;
			//main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0);
			bool greater = false;
			

			if (direction.y >= 0) {
				greater = true;
				main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction.y, 2))));
				main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction.x));
			}
			else {
				greater = false;
				main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction.y, 2))));
				main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction.x));
			}

			
			aux_top = main.transform.position;
			
			rotation_aux = main.transform.localEulerAngles;
			rotation_aux.x = 0;
			rotation_aux.y = 0;
			rotation_aux.z = 0;
			main.transform.localEulerAngles = rotation_aux;
			
			main.transform.position = new Vector3(0 +(1)*cos*topRadius1, 0 , 0 +(1)*sin*topRadius1);
			main_rotation = main.transform.localEulerAngles;
			
			//main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0);


			if (direction2.y >= 0) {
				if (greater) {
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2))));
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
				}
				else {
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -90 -(90 - Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2)))));
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
				}
			}
			else {
				if (greater) {
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 90 +(90 - Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2)))));
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
				}
				else {
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(1, 0, 0), -Mathf.Rad2Deg*Mathf.Asin(Mathf.Sqrt(1 - Mathf.Pow(direction2.y, 2))));
					main.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Mathf.Rad2Deg*Mathf.Asin(direction2.x));
				}
			}
			
			aux_bottom = main.transform.position;
			
			vertices[vert] = new Vector3(origin2.x + aux_bottom.x, origin2.y + aux_bottom.y, origin2.z + aux_bottom.z /*-sin*direction2.y*bottomRadius1 -cos*direction2.x*bottomRadius1*/);
			vertices[vert + 1] = new Vector3(origin1.x + aux_top.x, origin1.y + + aux_top.y, origin1.z + aux_top.z /*-sin*direction.y*topRadius1 -cos*direction.x*topRadius1*/);
			vert+=2;
		}
		#endregion
		
		#region Normales
		
		// bottom + top + sides
		Vector3[] normales = new Vector3[vertices.Length];
		vert = 0;
		

		
		// Sides (out)
		sideCounter = 0;

		if (debug) {
			while (vert < nbVerticesSides )
			{
				sideCounter = sideCounter == nbSides ? 0 : sideCounter;
				
				float r1 = (float)(sideCounter++) / nbSides * _2pi;
				
				normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
				normales[vert+1] = normales[vert];
				vert+=2;
			}
		}


		
		// Sides (in)
		sideCounter = 0;
		while (vert < vertices.Length )
		{
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
			
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
			
			normales[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
			normales[vert+1] = normales[vert];
			vert+=2;
		}
		#endregion
		
		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		
		vert = 0;
		
		// Sides (out)
		sideCounter = 0;

		if (debug) {
			while (vert < nbVerticesSides )
			{
				float t = (float)(sideCounter++) / nbSides;
				uvs[ vert++ ] = new Vector2( t, 0f );
				uvs[ vert++ ] = new Vector2( t, 1f );
			}
		}


		
		// Sides (in)
		sideCounter = 0;
		while (vert < vertices.Length )
		{
			float t = (float)(sideCounter++) / nbSides;
			uvs[ vert++ ] = new Vector2( t, 0f );
			uvs[ vert++ ] = new Vector2( t, 1f );
		}
		#endregion
		
		#region Triangles

		int nbFace = 0;

		if (debug) {
			nbFace = nbSides * 2;
		}
		else {
			nbFace = nbSides;
		}

		int nbTriangles = nbFace * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[nbIndexes];
		

		int i = 0;
		sideCounter = 0;

		
		// Sides (out)

		if (debug) {
			while( sideCounter < nbSides * 1 )
			{
				int current = sideCounter * 2 ;
				int next = sideCounter * 2 + 2;
				
				triangles[ i++ ] = current;
				triangles[ i++ ] = next;
				triangles[ i++ ] = next + 1;
				
				triangles[ i++ ] = current;
				triangles[ i++ ] = next + 1;
				triangles[ i++ ] = current + 1;
				
				sideCounter++;
			}
			
			
			// Sides (in)
			while( sideCounter < nbSides * 2 )
			{
				int current = sideCounter * 2 + 2;
				int next = sideCounter * 2 + 4;
				
				triangles[ i++ ] = next + 1;
				triangles[ i++ ] = next;
				triangles[ i++ ] = current;
				
				triangles[ i++ ] = current + 1;
				triangles[ i++ ] = next + 1;
				triangles[ i++ ] = current;
				
				sideCounter++;
			}
		}
		else {
			// Sides (in)
			while( sideCounter < nbSides * 1 )
			{
				int current = sideCounter * 2;
				int next = sideCounter * 2 + 2;
				
				triangles[ i++ ] = next + 1;
				triangles[ i++ ] = next;
				triangles[ i++ ] = current;
				
				triangles[ i++ ] = current + 1;
				triangles[ i++ ] = next + 1;
				triangles[ i++ ] = current;
				
				sideCounter++;
			}
		}


		#endregion

		//newObj.GetComponent<MeshFilter>().mesh;

		newObj.GetComponent<MeshFilter> ().mesh.vertices = vertices;
		newObj.GetComponent<MeshFilter> ().mesh.normals = normales;
		newObj.GetComponent<MeshFilter> ().mesh.uv = uvs;
		newObj.GetComponent<MeshFilter> ().mesh.triangles = triangles;

		newObj.GetComponent<MeshFilter> ().mesh.RecalculateBounds();
		newObj.GetComponent<MeshFilter> ().mesh.Optimize();
		
		newObj.GetComponent<MeshFilter> ().mesh.RecalculateNormals();


		newObj.name = "BLA";

		if (newObj.GetComponent<Rigidbody> () == null) {
			newObj.AddComponent<Rigidbody> ();
			newObj.GetComponent<Rigidbody> ().useGravity = false;
			newObj.GetComponent<Rigidbody> ().isKinematic = true;
			newObj.AddComponent<MeshCollider> ();
			for (int k = 0; k < competitors.Count; k++) {
				Physics.IgnoreCollision(newObj.GetComponent<Collider>(), competitors[k].GetComponentInChildren<Collider>());
			}
		} else {

			if (newObj.GetComponent<MeshCollider>() != null) {
				newObj.GetComponent<MeshCollider>().sharedMesh = null;
				newObj.GetComponent<MeshCollider>().sharedMesh = newObj.GetComponent<MeshFilter> ().mesh;
			}

		}

	}





	public Vector3 getPointBSpline(int target_point, float t) {

		while (t < 0) {
			target_point -= 1;
			t += 1;
		}
		if (target_point < 0) {
			target_point = 0;
		}

		while (t > 1) {
			target_point += 1;
			t -= 1;
		}
		
		if (target_point >= points.Count-3) {
			target_point = points.Count -4;
			t = 1.01f;
		}
		
		double[] a = new double[4];
		double[] b = new double[4];
		double[] c = new double[4];

		Vector3 p1 = points [target_point].point;
		Vector3 p2 = points [target_point+1].point;
		Vector3 p3 = points [target_point+2].point;
		Vector3 p4 = points [target_point+3].point;

		a[0] = (-p1.x + 3 * p2.x - 3 * p3.x + p4.x) / 6.0;
		a[1] = (3 * p1.x - 6 * p2.x + 3 * p3.x) / 6.0;
		a[2] = (-3 * p1.x + 3 * p3.x) / 6.0;
		a[3] = (p1.x + 4 * p2.x + p3.x) / 6.0;

		b[0] = (-p1.y + 3 * p2.y - 3 * p3.y + p4.y) / 6.0;
		b[1] = (3 * p1.y - 6 * p2.y + 3 * p3.y) / 6.0;
		b[2] = (-3 * p1.y + 3 * p3.y) / 6.0;
		b[3] = (p1.y + 4 * p2.y + p3.y) / 6.0;

		c[0] = (-p1.z + 3 * p2.z - 3 * p3.z + p4.z) / 6.0;
		c[1] = (3 * p1.z - 6 * p2.z + 3 * p3.z) / 6.0;
		c[2] = (-3 * p1.z + 3 * p3.z) / 6.0;
		c[3] = (p1.z + 4 * p2.z + p3.z) / 6.0;

		float value_x = (float) ( (a[2] + t * (a[1] + t * a[0]))*t+a[3] );
		float value_y = (float) ( (b[2] + t * (b[1] + t * b[0]))*t+b[3] );
		float value_z = (float) ( (c[2] + t * (c[1] + t * c[0]))*t+c[3] );

		//Debug.Log("X: "+ value_x);
		//Debug.Log("Y: "+ value_y);
		//Debug.Log("Z: "+ value_z);

		return new Vector3 (value_x, value_y, value_z);
	}

	public Vector3 getPointBSpline(float t) {

		return getPointBSpline (0, t);

	}
	/*
	void OnGUI()
	{

		GUI.Label(new Rect( 450,5, 30,30),""+lastFPS);

		GUI.Label (new Rect (450, 25, 100, 30), "Position: "+playerPosition);

		GUI.Label (new Rect (450, 45, 150, 30), "Finish Lane: "+(endingT-player.GetComponentInParent<CompetitorScript>().personalT).ToString("F2"));

	}
	*/
	
	// Update is called once per frame
	void Update () {
		
		float percent = player.GetComponentInParent<CompetitorScript>().personalT / endingT;
		Color finalColor = new Color (1f, 1f, 1f);
		Color color1 = new Color (40f/255f, 135f/255f, 199f/255f);
		Color color2 = new Color (36f/255f, 174f/255f, 162f/255f);
		Color color3 = new Color (232f/255f, 197f/255f, 87f/255f);
		Color color4 = new Color (207f/255f, 50f/255f, 61f/255f);

		if (percent < 1f/3f) {
			// PRIMER Y SEGUNDO COLOR
			finalColor = new Color(color1.r*(1f -(percent/(1f/3f))) + color2.r*(percent/(1f/3f)), color1.g*(1f -(percent/(1f/3f))) + color2.g*(percent/(1f/3f)), color1.b*(1f -(percent/(1f/3f))) + color2.b*(percent/(1f/3f)));
		}
		else if (percent < 2f/3f) {
			// SEGUNDO Y TERCER COLOR
			finalColor = new Color(color2.r*(1f -(percent-1f/3f)/(1f/3f)) + color3.r*(percent-1f/3f)/(1f/3f), color2.g*(1f -(percent-1f/3f)/(1f/3f)) + color3.g*(percent-1f/3f)/(1f/3f), color2.b*(1f -(percent-1f/3f)/(1f/3f)) + color3.b*(percent-1f/3f)/(1f/3f));
		}
		else {
			// TERCER Y CUARTO COLOR
				finalColor = new Color(color3.r*(1f -(percent-2f/3f)/(1f/3f)) + color4.r*(percent-2f/3f)/(1f/3f), color3.g*(1f -(percent-2f/3f)/(1f/3f)) + color4.g*(percent-2f/3f)/(1f/3f), color3.b*(1f -(percent-2f/3f)/(1f/3f)) + color4.b*(percent-2f/3f)/(1f/3f));
		}

		tubeObjects [currentTubeObject].GetComponent<MeshRenderer> ().sharedMaterial.color = finalColor;


		if (slowMo == 0.35f) {
			Time.timeScale = 0.1f;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
			slowMo = 0.349999f;
		}
		else if (slowMo > 0f) {
			Time.timeScale = 0.1f;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
			slowMo -= Time.deltaTime / Time.timeScale;
			// HACER SLOW MOTION
			if (slowMo <= 0f) {
				// QUITAR SLOW MOTION
				Time.timeScale = 1f;
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
				slowMo = 0f;
			}
		}

		if(Time.timeSinceLevelLoad  - timeA <= 1)
		{
			fps++;
		}
		else
		{
			lastFPS = fps + 1;
			timeA = Time.timeSinceLevelLoad;
			fps = 0;
		}

		if (player.GetComponentInParent<CompetitorScript>().collidingWhite) {
			player.transform.parent.gameObject.GetComponent<AudioSource>().volume = 0.01f;
			menu2.transform.gameObject.GetComponent<AudioSource>().volume = 0.01f;
		}
		else {
			player.transform.parent.gameObject.GetComponent<AudioSource>().volume = 0.25f;
			menu2.transform.gameObject.GetComponent<AudioSource>().volume = 0.15f;

			player.transform.parent.gameObject.GetComponent<AudioSource>().volume = 0f;
		}

		sortedCompetitors = competitors.OrderBy(go => go.GetComponentInParent<CompetitorScript>().personalT).ToList();

		for (int i = 0; i < sortedCompetitors.Count; i++) {
			sortedCompetitors[i].GetComponentInParent<CompetitorScript>().positionInRace = sortedCompetitors.Count - i;
		}

		if (player.GetComponentInParent<CompetitorScript>().personalT < endingT) {
			playerPosition = player.GetComponentInParent<CompetitorScript> ().positionInRace;
			leadingT = sortedCompetitors [sortedCompetitors.Count - 1].GetComponentInParent<CompetitorScript>().personalT;

			textPosition.GetComponent<Text>().text = ""+playerPosition;
			
			float currentSpeed = GameObject.Find("CentralPointPlayer").GetComponent<CompetitorScript>().currentSpeed*42f;
			textSpeed.GetComponent<Text>().text = currentSpeed.ToString("F0")+" ml/s";
		}
		else {
			textPosition.GetComponent<RectTransform>().anchoredPosition =  new Vector2(Mathf.Lerp (textPosition.GetComponent<RectTransform>().anchoredPosition.x, 210f, Time.deltaTime*5f), Mathf.Lerp (textPosition.GetComponent<RectTransform>().anchoredPosition.y, -Screen.height/2f -145f, Time.deltaTime*5f));
			textPosition.GetComponent<Text>().fontSize = (int) Mathf.Ceil(Mathf.Lerp(textPosition.GetComponent<Text>().fontSize, 400f, Time.deltaTime*5f));
			textSpeed.GetComponent<Text>().text = "";
		}


		if (!menu) {
			//t += Time.deltaTime/2;

			if (currentDrawingPoint < player.GetComponentInParent<CompetitorScript>().personalT +8f) {
				if (currentDrawingPoint >= points.Count -lastingPoints+8) {
					// YA NO SE DIBUJA NADA MAS
				}
				else {
					PointPlusMore nextTube = points[currentDrawingPoint];

					if (nextTube.drawed < (player.GetComponentInParent<CompetitorScript>().personalT +8f -currentDrawingPoint)*10f ) {

						float aux = (float) nextTube.drawed / (float) numOfTubes;
						//float aux2 = (float) (i+1) / (float) numOfTubes;
						
						//float first_radius = (points[j].begin_point_radius *(1f - aux)) + (points[j].end_point_radius *aux);
						//float last_radius = (points[j].begin_point_radius *(1f - aux2)) + (points[j].end_point_radius *aux2);
						float first_radius = currentRadius;
						float last_radius = currentRadius;
						
						GameObject recycledObject = tubeObjects[currentTubeObject];
						currentTubeObject++;
						if (currentTubeObject >= tubeObjects.Count) {
							currentTubeObject = 0;
						}

						Tube (recycledObject, getPointBSpline(currentDrawingPoint, aux), getPointBSpline(currentDrawingPoint, aux + (float) 1 / (float) numOfTubes), getPointBSpline(currentDrawingPoint, aux + (float) 2 / (float) numOfTubes), first_radius, last_radius, true);

						nextTube.drawed++;

					}

					if (nextTube.drawed == 10) { currentDrawingPoint++; }

				}
			}



			/*
			if (playerPosition == 1) {
				textPosition.GetComponent<Text>().color = new Color(0.8f, 0.77f, 0f);
			}
			else if (playerPosition == 2) {
				textPosition.GetComponent<Text>().color = new Color(0.41f, 0.41f, 0.41f);
			}
			else if (playerPosition == 3) {
				textPosition.GetComponent<Text>().color = new Color(0.51f, 0.37f, 0f);
			}
			else {
				textPosition.GetComponent<Text>().color = new Color(1f, 1f, 1f);
			}
			*/

		}
		else if (Input.GetKeyDown("space") || Input.GetButtonDown("Fire1")) {

			if (player.transform.position.y > 9.5f) {
				Camera.main.transform.localPosition = new Vector3(0f, 0f, 0f);
				menu = false;
				GameObject.Find("Menu2").SetActive(false);
				textPosition.SetActive(true);
				textSpeed.SetActive(true);
				Camera.main.farClipPlane = 500f;
			}
			else if (player.transform.position.y < -15) {
				Application.Quit ();
			}


		}


		if (Input.mouseScrollDelta.y != 0f) {
			separacionAux -= Input.mouseScrollDelta.y*0.5f;
		}

		if (Input.GetMouseButtonDown (1)) {
			if (Time.timeScale > 0.1f) {
				Time.timeScale = 0.1f;
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
			else if (Time.timeScale < 1f) {
				Time.timeScale = 1f;
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
		}
		if (Input.GetMouseButtonDown (0)) {
			lastMousePosition = Input.mousePosition;
		}
		if (Input.GetMouseButton (0)) {
			direcAux = (Input.mousePosition - lastMousePosition)/1000f;
			if (direcAux.x > 0.5f) { direcAux = new Vector3(0.5f, direcAux.y, direcAux.z);}
			if (direcAux.x < -0.5f) { direcAux = new Vector3(-0.5f, direcAux.y, direcAux.z);}
			if (direcAux.y > 0.5f) { direcAux = new Vector3(direcAux.x, 0.5f, direcAux.z);}
			if (direcAux.y < -0.5f) { direcAux = new Vector3(direcAux.x, -0.5f, direcAux.z);}
		}

	}


	public void Generate (int num_sections) {

		//points.Add (new QuadBezier3D (new Vector3 (0, 0, 0), new Vector3 (50, 50, 50), new Vector3 (0, 0, 100), 15f, 15f));
		//points.Add (new QuadBezier3D (new Vector3 (0, 0, 100), new Vector3 (-15, -15, 115), new Vector3 (0, 0, 200), 15f, 15f));


		Vector3 last_point = new Vector3 (0, 0, 0);

		//ADD POINTS

		for (int i = 0; i < num_sections; i++) {

			Vector3 new_point = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), last_point.z + 100);

			points.Add (new PointPlusMore(new_point, 20f));

			last_point = new_point;


		}

		points [points.Count - 1].end = true;
		endingT = points.Count;

		for (int i = 0; i < lastingPoints; i++) {
			
			Vector3 new_point = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), last_point.z + 100);
			
			points.Add (new PointPlusMore(new_point, 20f));
			
			last_point = new_point;
			
			
		}


		

		//CREATE GEOMETRY


		for (int j = 0; j < 9f; j++) {

			for (int i = 0; i < numOfTubes; i++) {
				float aux = (float) i / (float) numOfTubes;
				float aux2 = (float) (i+1) / (float) numOfTubes;
				
				//float first_radius = (points[j].begin_point_radius *(1f - aux)) + (points[j].end_point_radius *aux);
				//float last_radius = (points[j].begin_point_radius *(1f - aux2)) + (points[j].end_point_radius *aux2);
				float first_radius = currentRadius;
				float last_radius = currentRadius;

				GameObject newObj = Instantiate(theObj);

				tubeObjects.Add(newObj);


				//GameObject sourceObj = (GameObject)Instantiate (theObj);
				Tube (newObj, getPointBSpline(j, aux), getPointBSpline(j, aux + (float) 1 / (float) numOfTubes), getPointBSpline(j, aux + (float) 2 / (float) numOfTubes), first_radius, last_radius, true);
			}

			//points[j].drawed = numOfTubes;
		}

		currentDrawingPoint = 9;


	}


}
