using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Binaural_hands_head: MonoBehaviour {
    // for right hand
    public GameObject RI;
    public GameObject RKL;
    public GameObject RKR;
    public GameObject RT;
    public GameObject RWL;

    // for left hand
    public GameObject LI;
    public GameObject LKL;
    public GameObject LKR;
    public GameObject LT;
    public GameObject LWL;
    public GameObject LWR;

    // for head
    public GameObject HT;
    public GameObject HF;
    public GameObject HRL;
    public GameObject HRR;

    // for headphones
    public GameObject USR;
    public GameObject SPK;
    public GameObject PRJ;
    public GameObject Horizon;
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

    private float yaw_usr = 0.0f;

    private float source_elev_angle = 30.0f;
    private float source_azim_angle = 0.0f;

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

        /*
        right_hand_impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        right_hand_impact.GetComponentInChildren<Collider>().enabled = false;
        right_hand_impact.GetComponent<Renderer>().material.color = new Color(0,1f,0);

        left_hand_impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        left_hand_impact.GetComponentInChildren<Collider>().enabled = false;
        left_hand_impact.GetComponent<Renderer>().material.color = new Color(0,0.5f,0);
        
        right_hand_projection = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        right_hand_projection.GetComponentInChildren<Collider>().enabled = false;
        right_hand_projection.GetComponent<Renderer>().material.color = new Color(0,0.5f,0);

        left_hand_projection = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        left_hand_projection.GetComponentInChildren<Collider>().enabled = false;
        left_hand_projection.GetComponent<Renderer>().material.color = new Color(0,0.5f,0);
        */

        SPK = GameObject.Find("SPK");
        PRJ = GameObject.Find("PRJ");
        USR = GameObject.Find("USR");

        USR.AddComponent<OSCMessageSender>();
        USR.AddComponent<OSCMessageReceiver>();

        Horizon = GameObject.Find("Horizon");

        // SPK.transform.position = new Vector3(7.52348f, 4.6482f,0f);
        SPK.transform.position = new Vector3(7.52348f, 7.52348f,0f);
        // PRJ.transform.position = new Vector3(7.52348f, 0f,0f);
        PRJ.transform.position = new Vector3(SPK.transform.position.x, 0f,SPK.transform.position.z);

        USR.transform.position = new Vector3(0f, 0f, 0f);
        Horizon.transform.position = new Vector3(0f, 0f, 0f);

        SPK.GetComponent<Renderer>().material.color = new Color(1f,0.5f,1);
        PRJ.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0);
        USR.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0.5f);
        Horizon.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0.5f);
    
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
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define
        //IP="127.0.0.1";
        IP = "172.29.106.209";
        port = 9001;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    // sendData
    public void sendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
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
	void Update () {
        //if (RI == null && RKL == null && RKR == null && RT == null) {

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
        // Head top
        HT = GameObject.Find("HT");
        // Head front
        HF = GameObject.Find("HF");
        // Head rear left
        HRL = GameObject.Find("HRL");
        // Head rear right
        HRR = GameObject.Find("HRR");
        //}

        //else {
        if (RI != null && RKL != null && RKR != null && RT != null && RWL != null) {
            
            Vector3 right_ray_first = RI.transform.position ;
            Vector3 right_ray_second = RKL.transform.position ;
            Vector3 right_forward = (right_ray_first - right_ray_second) * (500);
            Ray right_ray = new Ray(right_ray_first, right_forward);
            
            Vector3 right_hand_ray_first = RKL.transform.position ;
            Vector3 right_hand_ray_second = RWL.transform.position ;
            Vector3 right_hand_forward = (right_hand_ray_first - right_hand_ray_second) * (500);
            Ray right_hand_ray = new Ray(right_hand_ray_first, right_hand_forward);

            right_lr.SetPosition(0, right_ray_first);
            right_lr.SetPosition(1, right_ray.GetPoint(10000));

            right_hand_lr.SetPosition(0, right_hand_ray_first);
            right_hand_lr.SetPosition(1, right_hand_ray.GetPoint(10000));

            Vector3 xDir = new Vector3(1,0,0);
            Vector3 yDir = new Vector3(0,1,0);

            RaycastHit right_hit;

	        if (Physics.Raycast(right_ray, out right_hit, 10000.0f, layer_mask))
    	    {
        	    //print("Found an object - distance: " + hit.distance + hit.point);
            	right_impact.transform.position = right_hit.point ;

                // Subtract 6'9 height from it to get correct elevation
            	Vector3 right_impactDir_elev = right_impact.transform.position - new Vector3(0f, 1.7526f, 0f) ;
                right_projection.transform.position = new Vector3(right_hit.point.x, 0, right_hit.point.z);
            	Vector3 right_projDir = right_projection.transform.position;

            	float right_elev_angle = Vector3.Angle(right_impactDir_elev, right_projDir);
            	float right_azim_angle = Vector3.Angle(xDir, right_projDir);
                
                if (right_hit.point.z > 0){
                    right_azim_angle = 360 - right_azim_angle ;
                }

                right_azim_angle = (right_azim_angle+90)%360;

                sendString("right pointazel " + right_azim_angle + " " + right_elev_angle + "\n");
                //print("right pointazel " + right_azim_angle + " " + right_elev_angle);

                
            }

            

            RaycastHit right_hand_hit;

            if (Physics.Raycast(right_hand_ray, out right_hand_hit, 10000.0f, layer_mask))
            {
              
                // Subtract 6'9 height from it to get correct elevation
                Vector3 right_hand_impactDir_elev = right_hand_hit.point - new Vector3(0f, 1.7526f, 0f) ;
                Vector3 right_hand_projDir = new Vector3(right_hand_hit.point.x, 0, right_hand_hit.point.z);
                
                /*            
                right_hand_impact.transform.position = right_hand_hit.point ;
                // Subtract 6'9 height from it to get correct elevation
                Vector3 right_hand_impactDir_elev = right_hand_impact.transform.position - new Vector3(0f, 1.7526f, 0f) ;
                right_hand_projection.transform.position = new Vector3(right_hand_hit.point.x, 0, right_hand_hit.point.z);
                Vector3 right_hand_projDir = right_hand_projection.transform.position;
                */

                float right_hand_elev_angle = Vector3.Angle(right_hand_impactDir_elev, right_hand_projDir);
                float right_hand_azim_angle = Vector3.Angle(xDir, right_hand_projDir);
                
                if (right_hand_hit.point.z > 0){
                    right_hand_azim_angle = 360 - right_hand_azim_angle ;
                }

                right_hand_azim_angle = (right_hand_azim_angle+90)%360;

                sendString("right handazel " + right_hand_azim_angle + " " + right_hand_elev_angle + "\n");
                print("right handazel " + right_hand_azim_angle + " " + right_hand_elev_angle);

            }

            // Distance between right right knuckle and right thumb
            Vector3 right_targetDir_thumb = RT.transform.position - RKR.transform.position;
            float right_mag_thumb = Vector3.Magnitude(right_targetDir_thumb);
            float right_sqrLen_thumb = right_targetDir_thumb.sqrMagnitude;
            //print("right thumb " + right_mag_thumb + "  " + right_sqrLen_thumb);


            // Distance between right index finger and right thumb
            Vector3 right_targetDir_pinch = RI.transform.position - RT.transform.position;
            float right_mag_pinch = Vector3.Magnitude(right_targetDir_pinch);
            float right_sqrLen_pinch = right_targetDir_pinch.sqrMagnitude;
                        
            Vector3 right_targetDir_roll = RKL.transform.position - RKR.transform.position;
            float right_roll_angle = Vector3.Angle(yDir, right_targetDir_roll);

            sendString("right pinch " + right_mag_pinch + "\n");
            sendString("right thumb " + right_mag_thumb + "\n");
            sendString("right roll " + right_roll_angle + "\n");

    

        }


        // Left hand
        if (LI != null && LKL != null && LKR != null && LT != null && LWR != null) {
            

            Vector3 left_ray_first = LI.transform.position ;
            Vector3 left_ray_second = LKR.transform.position ;
            Vector3 left_forward = (left_ray_first - left_ray_second) * (500);
            Ray left_ray = new Ray(left_ray_first, left_forward);

            Vector3 xDir = new Vector3(1,0,0);
            Vector3 yDir = new Vector3(0,1,0);

            Vector3 left_hand_ray_first = LKR.transform.position ;
            Vector3 left_hand_ray_second = LWR.transform.position ;
            Vector3 left_hand_forward = (left_hand_ray_first - left_hand_ray_second) * (500);
            Ray left_hand_ray = new Ray(left_hand_ray_first, left_hand_forward);

            left_lr.SetPosition(0, left_ray_first);
            left_lr.SetPosition(1, left_ray.GetPoint(10000));

            left_hand_lr.SetPosition(0, left_hand_ray_first);
            left_hand_lr.SetPosition(1, left_hand_ray.GetPoint(10000));

            RaycastHit left_hit;

            if (Physics.Raycast(left_ray, out left_hit, 10000.0f, layer_mask))
            {
                //print("Found an object - distance: " + hit.distance + hit.point);
                left_impact.transform.position = left_hit.point ;
                // Subtract 6'9 height from it to get correct elevation
                Vector3 left_impactDir_elev = left_impact.transform.position - new Vector3(0f, 1.7526f, 0f) ;
                left_projection.transform.position = new Vector3(left_hit.point.x, 0, left_hit.point.z);
                Vector3 left_projDir = left_projection.transform.position;

                float left_elev_angle = Vector3.Angle(left_impactDir_elev, left_projDir);
                float left_azim_angle = Vector3.Angle(xDir, left_projDir);
                
                if (left_hit.point.z > 0){
                    left_azim_angle = 360 - left_azim_angle ;
                }

                left_azim_angle = (left_azim_angle+90)%360;

                sendString("left pointazel " + left_azim_angle + " " + left_elev_angle + "\n");
                //print("left pointazel " + left_azim_angle + " " + left_elev_angle);

                               
            }


            RaycastHit left_hand_hit;

            if (Physics.Raycast(left_hand_ray, out left_hand_hit, 10000.0f, layer_mask))
            {
                
                /*
                // Subtract 6'9 height from it to get correct elevation
                left_hand_impact.transform.position = left_hand_hit.point ;
                // Subtract 6'9 height from it to get correct elevation
                Vector3 left_hand_impactDir_elev = left_hand_impact.transform.position - new Vector3(0f, 1.7526f, 0f) ;
                left_hand_projection.transform.position = new Vector3(left_hand_hit.point.x, 0, left_hand_hit.point.z);
                Vector3 left_hand_projDir = left_hand_projection.transform.position;
                */

                Vector3 left_hand_impactDir_elev = left_hand_hit.point  - new Vector3(0f, 1.7526f, 0f) ;
                Vector3 left_hand_projDir = new Vector3(left_hand_hit.point.x, 0, left_hand_hit.point.z);

                float left_hand_elev_angle = Vector3.Angle(left_hand_impactDir_elev, left_hand_projDir);
                float left_hand_azim_angle = Vector3.Angle(xDir, left_hand_projDir);
                
                if (left_hand_hit.point.z > 0){
                    left_hand_azim_angle = 360 - left_hand_azim_angle ;
                }

                left_hand_azim_angle = (left_hand_azim_angle+90)%360;

                sendString("left handazel " + left_hand_azim_angle + " " + left_hand_elev_angle + "\n");
                print("left handazel " + left_hand_azim_angle + " " + left_hand_elev_angle);
               

            }

                          
            // Distance between left left knuckle and left thumb
            Vector3 left_targetDir_thumb = LT.transform.position - LKL.transform.position;
            float left_mag_thumb = Vector3.Magnitude(left_targetDir_thumb);
            float left_sqrLen_thumb = left_targetDir_thumb.sqrMagnitude;
            //print("left thumb " + left_mag_thumb + "  " + left_sqrLen_thumb);


            // Distance between left index finger and left thumb
            Vector3 left_targetDir_pinch = LI.transform.position - LT.transform.position;
            float left_mag_pinch = Vector3.Magnitude(left_targetDir_pinch);
            float left_sqrLen_pinch = left_targetDir_pinch.sqrMagnitude;
                

            Vector3 left_targetDir_roll = LKR.transform.position - LKL.transform.position;
            float left_roll_angle = Vector3.Angle(yDir, left_targetDir_roll);


            sendString("left pinch " + left_mag_pinch + "\n");
            sendString("left thumb " + left_mag_thumb + "\n");            
            sendString("left roll " + left_roll_angle + "\n");


        }

        if (HT != null && HF != null && HRR != null && HRL != null) {
            Vector3 head_top = HT.transform.position;
            Vector3 head_front = HF.transform.position;
            Vector3 head_left = HRL.transform.position;
            Vector3 head_right = HRR.transform.position;

            print("HT: " + head_top + "HF: " + head_front + "HRL: " + head_left + "HRR: " + head_right);

        }




        // User
        USR = GameObject.Find("USR");
        // Speaker
        SPK = GameObject.Find("SPK");
        // Projection of Speaker
        PRJ = GameObject.Find("PRJ");

            
        Vector3 A_ray_first = USR.transform.position ;
        Vector3 A_ray_second = SPK.transform.position ;
        Vector3 A_forward = (A_ray_first - A_ray_second) * (-10);
        Ray A_ray = new Ray(A_ray_first, A_forward);

        Vector3 B_ray_first = SPK.transform.position ;
        Vector3 B_ray_second = PRJ.transform.position ;
        Vector3 B_forward = (B_ray_first - B_ray_second) * (-10);
        Ray B_ray = new Ray(B_ray_first, B_forward);

        Vector3 C_ray_first = USR.transform.position ;
        Vector3 C_ray_second = PRJ.transform.position ;
        Vector3 C_forward = (C_ray_first - C_ray_second) * (-10);
        Ray C_ray = new Ray(C_ray_first, C_forward);


        A_lr.SetPosition(0, A_ray_first);
        A_lr.SetPosition(1, A_ray.GetPoint(10000));

        B_lr.SetPosition(0, B_ray_first);
        B_lr.SetPosition(1, B_ray.GetPoint(10000));

        C_lr.SetPosition(0, C_ray_first);
        C_lr.SetPosition(1, C_ray.GetPoint(10000));


        // Vector3 xDir = new Vector3(1,0,0);
        // Vector3 yDir = new Vector3(0,1,0);
        Vector3 zDir = new Vector3(0,0,1);

        Vector3 planeNormal = new Vector3(0,1,0);
        Vector3 planeXaxis =  new Vector3(1,0,0);
        Vector3 planeZaxis =  new Vector3(0,0,1);


        if (Input.GetKey(KeyCode.R))
            roll++;
        if (Input.GetKey(KeyCode.T))
            roll--;

        if (Input.GetKey(KeyCode.F))
            yaw_usr++;
        if (Input.GetKey(KeyCode.G))
            yaw_usr--;

        //USR.transform.eulerAngles = new Vector3(pitch, yaw, roll);
        //USR.transform.Rotate(0, roll_usr/2.0f, 0);

        USR.transform.Translate(x, 0, z);
        USR.transform.localEulerAngles = new Vector3(0, 0, roll);
        USR.transform.Rotate(0, yaw_usr, 0);
        
        Horizon.transform.Rotate(0, yaw_usr, 0);
        //Horizon.transform.localEulerAngles = new Vector3(0, 0, roll);
     

        PRJ.transform.localEulerAngles = new Vector3(0, 0, roll);
        //PRJ.transform.Rotate(0, yaw_usr, 0);
        
        
        //print("PYR: " + pitch + " " + yaw + " " + roll + "\n");
    
        rotation = Quaternion.Euler(0, 0, roll);
        planeNormal = rotation*planeNormal;
        planeXaxis = rotation*planeXaxis;

        // planeNormal = Horizon.transform.localRotation*planeNormal;
        // planeXaxis = Horizon.transform.localRotation*planeXaxis;

        //print(planeNormal);

        x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
        z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

   
        yaw_usr = 0f;

       
        // Point on plane
        Vector3 planePoint = USR.transform.position;

        //Get the shortest distance between a point and a plane. The output is signed so it holds information
        //as to which side of the plane normal the point is.
        float SignedDistancePlaneSpeaker = Vector3.Dot(planeNormal, (SPK.transform.position - planePoint));

        //Reverse the sign of the distance
        float distance = -1*SignedDistancePlaneSpeaker;

        //Get a translation vector
        Vector3 translationVector = Vector3.Normalize(planeNormal)*distance;

        PRJ.transform.position = SPK.transform.position + translationVector;

        float binaural_elev_angle = Vector3.Angle(A_forward, C_forward);
        float binaural_azim_angle = Vector3.Angle(planeXaxis, C_forward);
        //sendString("binauralazel " + binaural_azim_angle + " " + binaural_elev_angle + "\n");
        

        //sendString("sourceazel " + source_azim_angle + " " + source_elev_angle + "\n");

        // if (PRJ.transform.position.x > 0)
        //     {
        //       azim_angle = 360 - azim_angle ;
        //     }

        // azim_angle = (azim_angle+90)%360;


        // create random elevation angle and azimuthal angle
        if (Input.GetKey(KeyCode.B))
        {   source_elev_angle = UnityEngine.Random.Range(30f, 90.0f);
            source_azim_angle = UnityEngine.Random.Range(0f, 360.0f);
            print("New randomly generated source angles : " + source_azim_angle + " " + source_elev_angle);
        }

        Vector3 SourceRay = new Vector3(Mathf.Cos(Mathf.Deg2Rad*source_elev_angle)*Mathf.Cos(Mathf.Deg2Rad*source_azim_angle), Mathf.Sin(Mathf.Deg2Rad*source_elev_angle), Mathf.Cos(Mathf.Deg2Rad*source_elev_angle)*Mathf.Sin(Mathf.Deg2Rad*source_azim_angle));
        print(SourceRay);

        Vector3 D_ray_first = new Vector3(0,0,0);
        //Vector3 D_ray_second = Vector3 SourceRay;
        Vector3 D_forward = (D_ray_first - SourceRay) * (-10);
        Ray D_ray = new Ray(D_ray_first, D_forward);

        D_lr.SetPosition(0, D_ray_first);
        D_lr.SetPosition(1, D_ray.GetPoint(10000));

        RaycastHit D_hit;

        if (Physics.Raycast(D_ray, out D_hit, 10000.0f, layer_mask))
        {
            //print("Found an object - distance: " + hit.distance + hit.point);
            SPK.transform.position = D_hit.point ;

            // Subtract 6'9 height from it to get correct elevation
            //Vector3 right_impactDir_elev = right_impact.transform.position - new Vector3(0f, 1.7526f, 0f) ;
            
        }

        // Vector3 Dir = Quaternion.AngleAxis( 54.0f, transform.up) * transform.forward;
        // RaycastHit2D Hit = Physics2D.Raycast(CubeOrigin, Dir, 300.0f, layer_mask);

        // if (Hit)
        //  {
        //      print("hit");
        //      Debug.DrawRay(CubeOrigin, Dir, Color.yellow);
        //  }

    
        //sendSourceAngles();
        float[] sourceAngles = getSourceAngles();
        //print("Whole array");
        print(sourceAngles);

        // float test_angle = sourceAngles[0];
        // print("test : " + test_angle);

        source_azim_angle = sourceAngles[0];
        print("Source Azim Angle: " + source_azim_angle);
        source_elev_angle = sourceAngles[1];
        print("Source Elev Angle: " + source_elev_angle);

        if(source_azim_angle!=-999){
            sendBinauralAngles(binaural_elev_angle, binaural_azim_angle);
            //print("Binaural Angles: " + binaural_azim_angle + " " + binaural_elev_angle);
        }

        // sendBinauralAngles(binaural_elev_angle, binaural_azim_angle);
    }
    
    private void sendBinauralAngles(float binaural_elev_angle,float binaural_azim_angle){
        List<object> angles = new List<object>();
        string messageAddress = "/BinauralAngles/";
        angles.Add(binaural_azim_angle + " " + binaural_elev_angle); 
        USR.GetComponent<OSCMessageSender>().AppendMessage(messageAddress,angles);
    }
    // private void sendSourceAngles(){
    //     List<object> angles = new List<object>();
    //     string messageAddress = "/SourceAngles/";
    //     angles.Add("sourceazel 45 45");
    //     USR.GetComponent<OSCMessageSender>().AppendMessage(messageAddress,angles);
    // }

    private float[] getSourceAngles(){
        string source_angles = USR.GetComponent<OSCMessageReceiver>().sourceMessage;
        Debug.Log(source_angles);
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
            return new float[2] {-999,-999};
        }

    }

}
