//Final script for March/ April 2019 user-studies
//This script includes null statements
//This script is written for including LM and RM new markers put on the middle fingers of right and left hand
//This script is written for Locus version 2.0 In this, in addition to pointazel and handazel, fingerazel is calculated as well,
//by putting an extra makrer in the middle of index finger. 
//Date: March 17, 2019, 7:16 PM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTMRealTimeSDK;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class locus_v5 : MonoBehaviour {

	// ICO DEBUG
	public float source_azim_angle = 0.0f;  //ICO: made public for debugging purposes
	public float source_elev_angle = 30.0f; //ICO: made public for debugging purposes

	// LATER: delete vars below (used for debugging only)

	public float usr_x = 0.0f;
	public float usr_y = 0.0f;
	public float usr_z = 0.0f;
	public float usr_rot_x = 0.0f;
	public float usr_rot_y = 0.0f;
	public float usr_rot_z = 0.0f;
	private bool qualisys_connected = false;
	// END ICO DEBUG

	// for right hand
	public GameObject RI;
	public GameObject RKL;
	public GameObject RKR;
	public GameObject RT;
	public GameObject RWL;
	public GameObject RIJ;
	public GameObject RM;

	// for left hand
	public GameObject LI;
	public GameObject LKL;
	public GameObject LKR;
	public GameObject LT;
	public GameObject LWL;
	public GameObject LWR;
	public GameObject LIJ;
	public GameObject LM;

	// for head
	public GameObject HT;
	public GameObject HF;
	public GameObject HRL;
	public GameObject HRR;
	public string ObjectName = "Binaural_Head";
	// private RTClient rtClient;
	public Vector3 PositionOffset = new Vector3(0, 0, 0);
	public Vector3 EulerOffset = new Vector3(0, 0, 0);

	// for headphones test with keyboard
	public GameObject USR;
	public GameObject SPK;
	public GameObject PRJ;
	public GameObject Horizon;
	public GameObject Ear;
	public GameObject earLevel;
	public Plane ThePlane;

	// for creating Cube 
	public GameObject LWall;
	public GameObject RWall;
	public GameObject FWall;
	public GameObject BWall;
	public GameObject Ceiling;

	public Collider coll;

	// for creating ray (hands)
	public GameObject right_rayLine;
	public LineRenderer right_lr;

	public GameObject left_rayLine;
	public LineRenderer left_lr;

	public GameObject right_hand_rayLine;
	public LineRenderer right_hand_lr;

	public GameObject left_hand_rayLine;
	public LineRenderer left_hand_lr;

	public GameObject right_finger_rayLine;
	public LineRenderer right_finger_lr;

	public GameObject left_finger_rayLine;
	public LineRenderer left_finger_lr;

	// for creating ray (binaural)
	public GameObject A_rayLine;
	public LineRenderer A_lr;

	public GameObject B_rayLine;
	public LineRenderer B_lr;

	public GameObject C_rayLine;
	public LineRenderer C_lr;

	public GameObject D_rayLine;
	public LineRenderer D_lr;

	// for showing impact on Cube walls when a ray hits the walls or ceiling
	public GameObject right_impact;
	public GameObject right_projection;

	public GameObject left_impact;
	public GameObject left_projection;

	public GameObject right_hand_impact;
	public GameObject right_hand_projection;

	public GameObject left_hand_impact;
	public GameObject left_hand_projection;

	public GameObject CubeOrigin;

	// binaural variables
	public GameObject A_impact;
	public GameObject A_projection;

	public Quaternion rotation;

	public float x = 0.0f;
	public float y = 0.0f;
	public float z = 0.0f;

	public float speedH = 2.0f;
	public float speedV = 2.0f;
	public float speedR = 2.0f;

	private float yaw = 0.0f;
	private float pitch = 0.0f;
	private float roll = 0.0f;

	private float binaural_elev_angle;
	private float binaural_azim_angle;

	public Vector3 right_oldRKR;
	public Vector3 right_oldFwd;
	public float right_deltaAngle = 0.0f;

	public Vector3 left_oldLKL;
	public Vector3 left_oldFwd;
	public float left_deltaAngle = 0.0f;


	// data transmission variables
	public int layer_mask;

	//
	public static int localPort;

	// prefs
	public string IP;  // define in init
	public int port;  // define in init

	// "connection" things
	IPEndPoint remoteEndPoint;
	UdpClient client;

	// gui
	string strMessage = "";

	// call it from shell (as program)
	public static void Main()
	{
		UDPSend sendObj = new UDPSend();
		sendObj.init();

	}


	// Use this for initialization
	void Start () {

		init();
		// rtClient = RTClient.GetInstance();

		float deltaAngle = 0.0f;
		layer_mask = LayerMask.GetMask("Walls");

		LWall = GameObject.Find("LWall");
		FWall = GameObject.Find("FWall");
		RWall = GameObject.Find("RWall");
		BWall = GameObject.Find("BWall");
		Ceiling = GameObject.Find("Ceiling");

		LWall.transform.position = new Vector3(-7.54126f, 4.6482f,0f);
		RWall.transform.position = new Vector3(7.52348f, 4.6482f,0f) ;
		FWall.transform.position = new Vector3(0f, 4.6482f,6.30682f);
		BWall.transform.position = new Vector3(0f, 4.6482f, -6.29412f);
		Ceiling.transform.position = new Vector3(0f, 9.2964f, 0f);

		LWall.GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,1);
		RWall.GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,1);
		FWall.GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,1);
		BWall.GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,1);
		Ceiling.GetComponent<Renderer>().material.color = new Color(0.5f,0.5f,1);

		right_rayLine = new GameObject("right_rayLine");
		right_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		left_rayLine = new GameObject("left_rayLine");
		left_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		right_hand_rayLine = new GameObject("right_hand_rayLine");
		right_hand_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		left_hand_rayLine = new GameObject("left_hand_rayLine");
		left_hand_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		right_finger_rayLine = new GameObject("right_finger_rayLine");
		right_finger_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		left_finger_rayLine = new GameObject("left_finger_rayLine");
		left_finger_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		right_lr = right_rayLine.AddComponent<LineRenderer>();
		right_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		right_lr.SetColors(Color.red, Color.green);
		right_lr.SetWidth(0.1f, 0.1f);
		right_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		right_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		left_lr = left_rayLine.AddComponent<LineRenderer>();
		left_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		left_lr.SetColors(Color.blue, Color.green);
		left_lr.SetWidth(0.1f, 0.1f);
		left_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		left_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		right_hand_lr = right_hand_rayLine.AddComponent<LineRenderer>();
		right_hand_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		right_hand_lr.SetColors(Color.green, Color.red);
		right_hand_lr.SetWidth(0.1f, 0.1f);
		right_hand_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		right_hand_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		left_hand_lr = left_hand_rayLine.AddComponent<LineRenderer>();
		left_hand_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		left_hand_lr.SetColors(Color.green, Color.blue);
		left_hand_lr.SetWidth(0.1f, 0.1f);
		left_hand_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		left_hand_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		right_finger_lr = right_finger_rayLine.AddComponent<LineRenderer>();
		right_finger_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		right_finger_lr.SetColors(Color.yellow, Color.red);
		right_finger_lr.SetWidth(0.1f, 0.1f);
		right_finger_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		right_finger_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		left_finger_lr = left_finger_rayLine.AddComponent<LineRenderer>();
		left_finger_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		left_finger_lr.SetColors(Color.yellow, Color.blue);
		left_finger_lr.SetWidth(0.1f, 0.1f);
		left_finger_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		left_finger_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		right_impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		right_impact.GetComponentInChildren<Collider>().enabled = false;
		right_impact.GetComponent<Renderer>().material.color = new Color(0.5f,0,0);

		left_impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		left_impact.GetComponentInChildren<Collider>().enabled = false;
		left_impact.GetComponent<Renderer>().material.color = new Color(0,0,0.5f);

		right_projection = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		right_projection.GetComponentInChildren<Collider>().enabled = false;
		right_projection.GetComponent<Renderer>().material.color = new Color(0.5f,0f,0.5f);

		left_projection = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		left_projection.GetComponentInChildren<Collider>().enabled = false;
		left_projection.GetComponent<Renderer>().material.color = new Color(0,0.5f,0.5f);

		SPK = GameObject.Find("SPK");
		PRJ = GameObject.Find("PRJ");
		USR = GameObject.Find("USR");

		USR.AddComponent<OSCMessageSender>();
		USR.AddComponent<OSCMessageReceiver>();

		Horizon = GameObject.Find("Horizon");
		Ear = GameObject.Find("Ear");
		earLevel = GameObject.Find("earLevel");

		SPK.transform.position = new Vector3(7.52348f, 4.6482f,0f);
		PRJ.transform.position = new Vector3(SPK.transform.position.x, 0f,SPK.transform.position.z);

		USR.transform.position = new Vector3(0f, 0f, 0f);
		Horizon.transform.position = new Vector3(0f, 0f, 0f);
		// Ear.transform.position = USR.transform.position - new Vector3(0f, 0.15f, 0f);
		// earLevel.transform.position = Horizon.transform.position - new Vector3(0f, 0.15f, 0f);

		SPK.GetComponent<Renderer>().material.color = new Color(1f,0.5f,1);
		PRJ.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0);
		USR.GetComponent<Renderer>().material.color = new Color(1f,0.2f,0.2f);
		//Horizon.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0.5f);
		Ear.GetComponent<Renderer>().material.color = new Color(1f,0.4f,0.2f);
		earLevel.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0.5f);


		A_rayLine = new GameObject("A_rayLine");
		A_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		B_rayLine = new GameObject("B_rayLine");
		B_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		C_rayLine = new GameObject("C_rayLine");
		C_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		D_rayLine = new GameObject("D_rayLine");
		D_rayLine.transform.position = new Vector3(0f, 0f, 0f);

		A_lr = A_rayLine.AddComponent<LineRenderer>();
		A_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		A_lr.SetColors(Color.magenta, Color.green);
		A_lr.SetWidth(0.1f, 0.1f);
		A_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		A_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		B_lr = B_rayLine.AddComponent<LineRenderer>();
		B_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		B_lr.SetColors(Color.cyan, Color.green);
		B_lr.SetWidth(0.1f, 0.1f);
		B_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		B_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		C_lr = C_rayLine.AddComponent<LineRenderer>();
		C_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		C_lr.SetColors(Color.grey, Color.red);
		C_lr.SetWidth(0.1f, 0.1f);
		C_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		C_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

		D_lr = D_rayLine.AddComponent<LineRenderer>();
		D_lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		D_lr.SetColors(Color.yellow, Color.red);
		D_lr.SetWidth(0.1f, 0.1f);
		D_lr.SetPosition(0, new Vector3(0f, 0f, 0f));
		D_lr.SetPosition(1, new Vector3(1f, 0f, 0f));

	}

	// init
	public void init()
	{
		//print("UDPSend.init()");

		// define
		IP = "192.168.1.9";
		port = 9001;

		remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
		client = new UdpClient();

		// status
		//print("Sending to " + IP + " : " + port);
		//print("Testing: nc -lu " + IP + " : " + port);

	}

	// sendData
	public void sendString(string message)
	{
		try
		{
			// Daten mit der UTF8-Kodierung in das Binärformat kodieren.
			byte[] data = Encoding.UTF8.GetBytes(message);

			// Den message zum Remote-Client senden.
			client.Send(data, data.Length, remoteEndPoint);

		}
		catch (Exception err)
		{
			print(err.ToString());
		}
	}


	// endless test
	public void sendEndless(string testStr)
	{
		do
		{
			sendString(testStr);
		}
		while (true);

	}


	// Update is called once per frame
	void Update()
	{

		// Right index finger
		RI = GameObject.Find("RI");
		// Right left knuckle
		RKL = GameObject.Find("RKL");
		// Right right knuckle
		RKR = GameObject.Find("RKR");
		// Right thumb
		RT = GameObject.Find("RT");
		// Right left wrist
		RWL = GameObject.Find("RWL");
		// Right index finger middle joint
		RIJ = GameObject.Find("RIJ");
		// Right middle finger 
		RM = GameObject.Find("RM");

		// Left index finger
		LI = GameObject.Find("LI");
		// Left left knuckle
		LKL = GameObject.Find("LKL");
		// Left right knuckle
		LKR = GameObject.Find("LKR");
		// Left thumb
		LT = GameObject.Find("LT");
		// Left right wrist
		LWR = GameObject.Find("LWR");
		// Left index finger middle joint
		LIJ = GameObject.Find("LIJ");
		// Left middle finger 
		LM = GameObject.Find("LM");

		// Head top
		HT = GameObject.Find("HT");
		// Head front
		HF = GameObject.Find("HF");
		// Head rear left
		HRL = GameObject.Find("HRL");
		// Head rear right
		HRR = GameObject.Find("HRR");


		if (RI != null && RIJ != null && RKL!= null && RKR != null && RT != null && RWL != null && RM != null)
		{
			Vector3 height_offset = new Vector3(0f, 1.7526f, 0f);
			RI.transform.position = RI.transform.position - height_offset;
			RT.transform.position = RT.transform.position - height_offset;
			RKL.transform.position = RKL.transform.position - height_offset;
			RKR.transform.position = RKR.transform.position - height_offset;
			RWL.transform.position = RWL.transform.position - height_offset;
			RIJ.transform.position = RIJ.transform.position - height_offset;
			RM.transform.position = RM.transform.position - height_offset;

			//point azel between index finger and index finger joint
			Vector3 right_ray_first = RI.transform.position;
			Vector3 right_ray_second = RIJ.transform.position;
			Vector3 right_forward = (right_ray_first - right_ray_second) * (500);
			Ray right_ray = new Ray(right_ray_first, right_forward);

			//hand azel between knuckle and wrist
			Vector3 right_hand_ray_first = RKL.transform.position;
			Vector3 right_hand_ray_second = RWL.transform.position;
			Vector3 right_hand_forward = (right_hand_ray_first - right_hand_ray_second) * (500);
			Ray right_hand_ray = new Ray(right_hand_ray_first, right_hand_forward);

			//finger azel between knuckle and index finger joint
			Vector3 right_finger_ray_first = RIJ.transform.position;
			Vector3 right_finger_ray_second = RKL.transform.position;
			Vector3 right_finger_forward = (right_hand_ray_first - right_hand_ray_second) * (500);
			Ray right_finger_ray = new Ray(right_hand_ray_first, right_hand_forward);

			right_lr.SetPosition(0, right_ray_first);
			right_lr.SetPosition(1, right_ray.GetPoint(10000));

			right_hand_lr.SetPosition(0, right_hand_ray_first);
			right_hand_lr.SetPosition(1, right_hand_ray.GetPoint(10000));

			right_finger_lr.SetPosition(0, right_finger_ray_first);
			right_finger_lr.SetPosition(1, right_finger_ray.GetPoint(10000));

			Vector3 xDir = new Vector3(1, 0, 0);
			Vector3 yDir = new Vector3(0, 1, 0);
			Vector3 zDir = new Vector3(0, 0, 1);

			RaycastHit right_hit;
			RaycastHit right_finger_hit;

			if (Physics.Raycast(right_ray, out right_hit, 10000.0f, layer_mask) && Physics.Raycast(right_finger_ray, out right_finger_hit, 10000.0f, layer_mask))
			{
				right_impact.transform.position = right_hit.point;
				Vector3 right_impactDir_elev = right_impact.transform.position;

				right_projection.transform.position = new Vector3(right_hit.point.x, 0, right_hit.point.z);
				Vector3 right_projDir = right_projection.transform.position;

				float right_elev_angle = Vector3.Angle(right_impactDir_elev, right_projDir);
				if (right_impactDir_elev.y < right_projDir.y) right_elev_angle = 0 - right_elev_angle;
				float right_azim_angle = Vector3.Angle(-zDir, right_projDir);

				// CHANGES: Calculate angle with the -zDir and change right_hit.point.z < 0 to right_hit.point.x > 0, removed 90
				if (right_hit.point.x > 0)
				{
					right_azim_angle =  360 - right_azim_angle;
				}
					
				Vector3 right_finger_impactDir_elev = right_finger_hit.point;
				Vector3 right_finger_projDir = new Vector3(right_finger_hit.point.x, 0, right_finger_hit.point.z);

				float right_finger_elev_angle = Vector3.Angle(right_finger_impactDir_elev, right_finger_projDir);
				if (right_finger_impactDir_elev.y < right_finger_projDir.y) right_finger_elev_angle = 0 - right_finger_elev_angle;
				float right_finger_azim_angle = Vector3.Angle(-zDir, right_finger_projDir);

				// CHANGES: Calculate angle with the -zDir and change _hit.point.z < 0 to _hit.point.x > 0, removed 90
				if (right_finger_hit.point.x > 0)
				{
					right_finger_azim_angle = 360 - right_finger_azim_angle;
				}

				// right_azim_angle = (right_azim_angle + 90) % 360;
				sendString("right pointazel " + right_azim_angle + " " + right_elev_angle +  " " + right_finger_azim_angle + " " + right_finger_elev_angle + "\n");
				print("Right hand: " + right_azim_angle + " "  + right_elev_angle + " " + right_finger_azim_angle + " " + right_finger_elev_angle);

			}
				
			// Distance between right right knuckle and right thumb
			Vector3 right_targetDir_thumb = RT.transform.position - RKR.transform.position;
			float right_mag_thumb = Vector3.Magnitude(right_targetDir_thumb);
			float right_sqrLen_thumb = right_targetDir_thumb.sqrMagnitude;

			// Distance between right index finger and right thumb
			Vector3 right_targetDir_pinch = RI.transform.position - RT.transform.position;
			float right_mag_pinch = Vector3.Magnitude(right_targetDir_pinch);
			float right_sqrLen_pinch = right_targetDir_pinch.sqrMagnitude;

			// Distance between right index finger and right middle finger
			Vector3 right_targetDir_middle_index_distance = RI.transform.position - RM.transform.position;
			float right_mag_middle_index_distance = Vector3.Magnitude(right_targetDir_middle_index_distance);

			// NIME Music from Gestures
			float right_rollDir = 0f;
			if (right_oldRKR != null)
			{
				Vector3 right_rollFwd = RKL.transform.position - RKR.transform.position;
				Vector3 right_rollRight = RKL.transform.position - RWL.transform.position;
				right_rollDir = -AngleDir(right_rollFwd, right_oldRKR - RKR.transform.position, right_rollRight);
				right_deltaAngle = Vector3.Angle(right_rollFwd, right_oldFwd) * right_rollDir;
			}

			right_oldRKR = new Vector3 (RKR.transform.position.x, RKR.transform.position.y, RKR.transform.position.z);
			right_oldFwd = RKL.transform.position - RKR.transform.position;
			//print("Right Hand Delta Angle : " + right_deltaAngle );

			sendString("right pinch " + right_mag_pinch + "\n");
			sendString("right thumb " + right_mag_thumb + "\n");
			sendString("right roll " + right_deltaAngle + "\n");
			sendString("right mi " + right_mag_middle_index_distance + "\n");
			print("Right MI : " + right_mag_middle_index_distance + "\n" );

		}
			

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Left hand

		if (LI != null && LIJ != null && LKL != null && LKR != null && LT != null && LWR != null && LM != null)
		{

			Vector3 height_offset = new Vector3(0f, 1.7526f, 0f);
			LI.transform.position = LI.transform.position - height_offset;
			LT.transform.position = LT.transform.position - height_offset;
			LKL.transform.position = LKL.transform.position - height_offset;
			LKR.transform.position = LKR.transform.position - height_offset;
			LWR.transform.position = LWR.transform.position - height_offset;
			LIJ.transform.position = LIJ.transform.position - height_offset;
			LM.transform.position = LM.transform.position - height_offset;

			//point azel between index finger and index finger joint
			Vector3 left_ray_first = LI.transform.position;
			Vector3 left_ray_second = LIJ.transform.position;
			Vector3 left_forward = (left_ray_first - left_ray_second) * (500);
			Ray left_ray = new Ray(left_ray_first, left_forward);

			//hand azel between knuckle and wrist
			Vector3 left_hand_ray_first = LKR.transform.position;
			Vector3 left_hand_ray_second = LWR.transform.position;
			Vector3 left_hand_forward = (left_hand_ray_first - left_hand_ray_second) * (500);
			Ray left_hand_ray = new Ray(left_hand_ray_first, left_hand_forward);

			//finger azel between knuckle and index finger joint
			Vector3 left_finger_ray_first = LIJ.transform.position;
			Vector3 left_finger_ray_second = LKR.transform.position;
			Vector3 left_finger_forward = (left_finger_ray_first - left_finger_ray_second) * (500);
			Ray left_finger_ray = new Ray(left_finger_ray_first, left_finger_forward);

			left_lr.SetPosition(0, left_ray_first);
			left_lr.SetPosition(1, left_ray.GetPoint(10000));

			left_hand_lr.SetPosition(0, left_hand_ray_first);
			left_hand_lr.SetPosition(1, left_hand_ray.GetPoint(10000));

			left_finger_lr.SetPosition(0, left_finger_ray_first);
			left_finger_lr.SetPosition(1, left_finger_ray.GetPoint(10000));

			Vector3 xDir = new Vector3(1, 0, 0);
			Vector3 yDir = new Vector3(0, 1, 0);
			Vector3 zDir = new Vector3(0, 0, 1);

			RaycastHit left_hit;
			RaycastHit left_finger_hit;

			if (Physics.Raycast(left_ray, out left_hit, 10000.0f, layer_mask) && Physics.Raycast(left_finger_ray, out left_finger_hit, 10000.0f, layer_mask))
			{
				left_impact.transform.position = left_hit.point;
				Vector3 left_impactDir_elev = left_impact.transform.position;

				left_projection.transform.position = new Vector3(left_hit.point.x, 0, left_hit.point.z);
				Vector3 left_projDir = left_projection.transform.position;

				float left_elev_angle = Vector3.Angle(left_impactDir_elev, left_projDir);
				if (left_impactDir_elev.y < left_projDir.y) left_elev_angle = 0 - left_elev_angle;
				float left_azim_angle = Vector3.Angle(-zDir, left_projDir);

				// // CHANGES: Calculate angle with the -zDir and change _hit.point.z < 0 to _hit.point.x > 0, removed 90
				if (left_hit.point.x > 0)
				{
					left_azim_angle = 360 - left_azim_angle;
				}
					
			    Vector3 left_finger_impactDir_elev = left_finger_hit.point;
				Vector3 left_finger_projDir = new Vector3(left_finger_hit.point.x, 0, left_finger_hit.point.z);

				float left_finger_elev_angle = Vector3.Angle(left_finger_impactDir_elev, left_finger_projDir);
				if (left_finger_impactDir_elev.y < left_finger_projDir.y) left_finger_elev_angle = 0 - left_finger_elev_angle;
				float left_finger_azim_angle = Vector3.Angle(-zDir, left_finger_projDir);

				// CHANGES: Calculate angle with the -zDir and change _hit.point.z < 0 to _hit.point.x > 0, removed 90
				if (left_finger_hit.point.x > 0)
				{
					left_finger_azim_angle = 360 - left_finger_azim_angle;
				}
				sendString("left pointazel " + left_azim_angle + " " + left_elev_angle + " " + left_finger_azim_angle + " " + left_finger_elev_angle + "\n");
			}

	
			// Distance between left left knuckle and left thumb
			Vector3 left_targetDir_thumb = LT.transform.position - LKL.transform.position;
			float left_mag_thumb = Vector3.Magnitude(left_targetDir_thumb);
			float left_sqrLen_thumb = left_targetDir_thumb.sqrMagnitude;

			// Distance between left index finger and left thumb
			Vector3 left_targetDir_pinch = LI.transform.position - LT.transform.position;
			float left_mag_pinch = Vector3.Magnitude(left_targetDir_pinch);
			float left_sqrLen_pinch = left_targetDir_pinch.sqrMagnitude;

			// Distance between left index finger and left middle finger
			Vector3 left_targetDir_middle_index_distance = LI.transform.position - LM.transform.position;
			float left_mag_middle_index_distance = Vector3.Magnitude(left_targetDir_middle_index_distance);

			// NIME Music from Gestures
			float left_rollDir = 0f;
			if (left_oldLKL != null)
			{
				Vector3 left_rollFwd = LKR.transform.position - LKL.transform.position;
				Vector3 left_rollRight = LKR.transform.position - LWR.transform.position;
				left_rollDir = AngleDir(left_rollFwd, left_oldLKL - LKL.transform.position, left_rollRight);
				left_deltaAngle = Vector3.Angle(left_rollFwd, left_oldFwd) * left_rollDir;
			}

			left_oldLKL = new Vector3 (LKL.transform.position.x, LKL.transform.position.y, LKL.transform.position.z);
			left_oldFwd = LKR.transform.position - LKL.transform.position;
			print("Left Hand Delta Angle : " + left_deltaAngle );

			sendString("left pinch " + left_mag_pinch + "\n");
			sendString("left thumb " + left_mag_thumb + "\n");
			sendString("left roll " + left_deltaAngle + "\n");
			sendString("left mi " + left_mag_middle_index_distance + "\n");
		}
			
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Source angles  

		// get the source angles from MAX           
		float[] sourceAngles = getSourceAngles();
		source_azim_angle = sourceAngles[0];
		source_elev_angle = sourceAngles[1];

		// show the speaker in Unity environment  
		Vector3 SourceRay = new Vector3(Mathf.Cos(Mathf.Deg2Rad * source_elev_angle) * Mathf.Sin(Mathf.Deg2Rad * source_azim_angle), Mathf.Sin(Mathf.Deg2Rad * source_elev_angle), Mathf.Cos(Mathf.Deg2Rad * source_elev_angle) * Mathf.Cos(Mathf.Deg2Rad * source_azim_angle));
		Vector3 D_ray_first = new Vector3(0f, 0f, 0f);
		Vector3 D_forward = (D_ray_first - SourceRay) * (-10);
		Ray D_ray = new Ray(D_ray_first, D_forward);

		D_lr.SetPosition(0, D_ray_first);
		D_lr.SetPosition(1, D_ray.GetPoint(10000));

		RaycastHit D_hit;
		if (Physics.Raycast(D_ray, out D_hit, 10000.0f, layer_mask))
		{
			SPK.transform.position = D_hit.point;
		} 

	}
		
	private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);

		if (dir > 0f)
		{
			return 1f;
		}
		else if (dir < 0f)
		{
			return -1f;
		}
		else
		{
			return 0f;
		}
	}

	private void sendBinauralAngles(float binaural_elev_angle,float binaural_azim_angle){
		List<object> angles = new List<object>();
		string messageAddress = "/BinauralAngles/";
		angles.Add(binaural_azim_angle + " " + binaural_elev_angle); 
		USR.GetComponent<OSCMessageSender>().AppendMessage(messageAddress,angles);
	}


	private float[] getSourceAngles(){
		string source_angles = USR.GetComponent<OSCMessageReceiver>().sourceMessage;
		//Debug.Log(source_angles);
		//Split string on spaces.
		//... This will separate all the words.
		string[] words = source_angles.Split(' ');
		if (words.Length>1){
			float[] sourceAngles = new float[2];
			for(int i=0;i<2;i++)
			{
				sourceAngles[i] = float.Parse(words[i]);
			}

			return sourceAngles;

		} else {
			return new float[2] {source_azim_angle, source_elev_angle }; //ICO later revert to -999, -999
		}
	}

	Vector2 CartesianToPolar(Vector3 point)
	{
		Vector2 polar;

		// Angle with respect to +z axis
		polar.x = Mathf.Atan2(point.x, point.z);
		var xzLen = new Vector2(point.x, point.z).magnitude;

		// -y because direction is USR-SPK and SPK is at higher location than that of USR head
		polar.y = Mathf.Atan2(-point.y, xzLen);

		polar *= Mathf.Rad2Deg;
		return polar;
	}
		
}