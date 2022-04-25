// This the latest version at 5:55 pm, Aug 21, 2018
// This the latest version at 4:49 pm, July 31, 2018

// While figuring out Qualysis to Unity data transmission, Zach & I commented out two lines (65,66) in RTClient.cs in Qualysis-Unity SDK for getting the correct orientation of the USR.

// RTMarkerStream script -  Vector offset changed to new Vector3(0.0f, 1.1f, 2.1f), line 81; 
// This was done while doing first user study so that marker positions are shown in Unity at correct positions (Done around Oct/Nov in 2017)

// Binaural v5 has all the dummy version too

// Calibration in Cube- Left handed system, positive z-axis is towards double-doors

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTMRealTimeSDK;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QualisysRealTime.Unity{

    public class Binaural_hands_headphones: MonoBehaviour {

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
    public string ObjectName = "Binaural_Head";
    private RTClient rtClient;
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
        rtClient = RTClient.GetInstance();

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


        if (RI != null && RKL != null && RKR != null && RT != null && RWL != null)
        {
            Vector3 height_offset = new Vector3(0f, 1.7526f, 0f);
            RI.transform.position = RI.transform.position - height_offset;
            RT.transform.position = RT.transform.position - height_offset;
            RKL.transform.position = RKL.transform.position - height_offset;
            RKR.transform.position = RKR.transform.position - height_offset;
            RWL.transform.position = RWL.transform.position - height_offset;
            

            //print("RI: " +  RI.transform.position);
            Vector3 right_ray_first = RI.transform.position;
            Vector3 right_ray_second = RKL.transform.position;
            Vector3 right_forward = (right_ray_first - right_ray_second) * (500);
            Ray right_ray = new Ray(right_ray_first, right_forward);

            Vector3 right_hand_ray_first = RKL.transform.position;
            Vector3 right_hand_ray_second = RWL.transform.position;
            Vector3 right_hand_forward = (right_hand_ray_first - right_hand_ray_second) * (500);
            Ray right_hand_ray = new Ray(right_hand_ray_first, right_hand_forward);

            right_lr.SetPosition(0, right_ray_first);
            right_lr.SetPosition(1, right_ray.GetPoint(10000));

            right_hand_lr.SetPosition(0, right_hand_ray_first);
            right_hand_lr.SetPosition(1, right_hand_ray.GetPoint(10000));

            Vector3 xDir = new Vector3(1, 0, 0);
            Vector3 yDir = new Vector3(0, 1, 0);
            Vector3 zDir = new Vector3(0, 0, 1);

            RaycastHit right_hit;
            if (Physics.Raycast(right_ray, out right_hit, 10000.0f, layer_mask))
            {
                right_impact.transform.position = right_hit.point;

                // Subtract 6'9 height from it to get correct elevation
                // Vector3 right_impactDir_elev = right_impact.transform.position - new Vector3(0f, 1.7526f, 0f);
                Vector3 right_impactDir_elev = right_impact.transform.position;
                right_projection.transform.position = new Vector3(right_hit.point.x, 0, right_hit.point.z);
                Vector3 right_projDir = right_projection.transform.position;

                float right_elev_angle = Vector3.Angle(right_impactDir_elev, right_projDir);
                float right_azim_angle = Vector3.Angle(-zDir, right_projDir);

// CHANGES: Calculate angle with the -zDir and change right_hit.point.z < 0 to right_hit.point.x > 0, removed 90
               
                if (right_hit.point.x > 0)
                {
                    right_azim_angle =  360 - right_azim_angle;
                }

                // right_azim_angle = (right_azim_angle + 90) % 360;

                sendString("right pointazel " + right_azim_angle + " " + right_elev_angle + "\n");
                print("Right hand" + right_azim_angle + right_elev_angle);







            }



            RaycastHit right_hand_hit;
            if (Physics.Raycast(right_hand_ray, out right_hand_hit, 10000.0f, layer_mask))
            {
                // Subtract 6'9 height from it to get correct elevation
                //Vector3 right_hand_impactDir_elev = right_hand_hit.point - new Vector3(0f, 1.7526f, 0f);
                Vector3 right_hand_impactDir_elev = right_hand_hit.point;
                Vector3 right_hand_projDir = new Vector3(right_hand_hit.point.x, 0, right_hand_hit.point.z);

                float right_hand_elev_angle = Vector3.Angle(right_hand_impactDir_elev, right_hand_projDir);
                float right_hand_azim_angle = Vector3.Angle(-zDir, right_hand_projDir);

// CHANGES: Calculate angle with the -zDir and change _hit.point.z < 0 to _hit.point.x > 0, removed 90
                if (right_hand_hit.point.x > 0)
                {
                    right_hand_azim_angle = 360 - right_hand_azim_angle;
                }

                //right_hand_azim_angle = (right_hand_azim_angle + 90) % 360;

                sendString("right handazel " + right_hand_azim_angle + " " + right_hand_elev_angle + "\n");


            }

            // Distance between right right knuckle and right thumb
            Vector3 right_targetDir_thumb = RT.transform.position - RKR.transform.position;
            float right_mag_thumb = Vector3.Magnitude(right_targetDir_thumb);
            float right_sqrLen_thumb = right_targetDir_thumb.sqrMagnitude;

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
        if (LI != null && LKL != null && LKR != null && LT != null && LWR != null)
        {

            Vector3 height_offset = new Vector3(0f, 1.7526f, 0f);
            LI.transform.position = LI.transform.position - height_offset;
            LT.transform.position = LT.transform.position - height_offset;
            LKL.transform.position = LKL.transform.position - height_offset;
            LKR.transform.position = LKR.transform.position - height_offset;
            LWR.transform.position = LWR.transform.position - height_offset;

            Vector3 left_ray_first = LI.transform.position;
            Vector3 left_ray_second = LKR.transform.position;
            Vector3 left_forward = (left_ray_first - left_ray_second) * (500);
            Ray left_ray = new Ray(left_ray_first, left_forward);

            Vector3 xDir = new Vector3(1, 0, 0);
            Vector3 yDir = new Vector3(0, 1, 0);
            Vector3 zDir = new Vector3(0, 0, 1);

            Vector3 left_hand_ray_first = LKR.transform.position;
            Vector3 left_hand_ray_second = LWR.transform.position;
            Vector3 left_hand_forward = (left_hand_ray_first - left_hand_ray_second) * (500);
            Ray left_hand_ray = new Ray(left_hand_ray_first, left_hand_forward);

            left_lr.SetPosition(0, left_ray_first);
            left_lr.SetPosition(1, left_ray.GetPoint(10000));

            left_hand_lr.SetPosition(0, left_hand_ray_first);
            left_hand_lr.SetPosition(1, left_hand_ray.GetPoint(10000));

            RaycastHit left_hit;
            if (Physics.Raycast(left_ray, out left_hit, 10000.0f, layer_mask))
            {
                left_impact.transform.position = left_hit.point;

                // Subtract 6'9 height from it to get correct elevation
                //Vector3 left_impactDir_elev = left_impact.transform.position - new Vector3(0f, 1.7526f, 0f);
                Vector3 left_impactDir_elev = left_impact.transform.position;
                left_projection.transform.position = new Vector3(left_hit.point.x, 0, left_hit.point.z);
                Vector3 left_projDir = left_projection.transform.position;

                float left_elev_angle = Vector3.Angle(left_impactDir_elev, left_projDir);
                float left_azim_angle = Vector3.Angle(-zDir, left_projDir);

// // CHANGES: Calculate angle with the -zDir and change _hit.point.z < 0 to _hit.point.x > 0, removed 90
                if (left_hit.point.x > 0)
                {
                    left_azim_angle = 360 - left_azim_angle;
                }

                //left_azim_angle = (left_azim_angle + 90) % 360;

                sendString("left pointazel " + left_azim_angle + " " + left_elev_angle + "\n");
                //print("left pointazel " + left_azim_angle + " " + left_elev_angle);


            }


            RaycastHit left_hand_hit;
            if (Physics.Raycast(left_hand_ray, out left_hand_hit, 10000.0f, layer_mask))
            {
                //Vector3 left_hand_impactDir_elev = left_hand_hit.point - new Vector3(0f, 1.7526f, 0f);
                Vector3 left_hand_impactDir_elev = left_hand_hit.point;
                Vector3 left_hand_projDir = new Vector3(left_hand_hit.point.x, 0, left_hand_hit.point.z);

                float left_hand_elev_angle = Vector3.Angle(left_hand_impactDir_elev, left_hand_projDir);
                float left_hand_azim_angle = Vector3.Angle(-zDir, left_hand_projDir);

// CHANGES: Calculate angle with the -zDir and change _hit.point.z < 0 to _hit.point.x > 0, removed 90
                if (left_hand_hit.point.x > 0)
                {
                    left_hand_azim_angle = 360 - left_hand_azim_angle;
                }

                //left_hand_azim_angle = (left_hand_azim_angle + 90) % 360;

                sendString("left handazel " + left_hand_azim_angle + " " + left_hand_elev_angle + "\n");
                //print("left handazel " + left_hand_azim_angle + " " + left_hand_elev_angle);


            }

            // Distance between left left knuckle and left thumb
            Vector3 left_targetDir_thumb = LT.transform.position - LKL.transform.position;
            float left_mag_thumb = Vector3.Magnitude(left_targetDir_thumb);
            float left_sqrLen_thumb = left_targetDir_thumb.sqrMagnitude;

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


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        if (rtClient == null) rtClient = RTClient.GetInstance();

        SixDOFBody body = rtClient.GetBody(ObjectName);
        if (body != null)
        {

            if (!qualisys_connected) qualisys_connected = true;

            if (body.Position.magnitude > 0) //just to avoid error when position is NaN
            {
                //Qualysis to Unity 
                USR.transform.position = body.Position - new Vector3(0f, 1.7526f, 0f);
                USR.transform.rotation = body.Rotation; //TODO: should this be a USR.transform.localRotation instead of rotation?
                //print("Angles of rigid body:" + USR.transform.rotation + " | " + USR.transform.localEulerAngles );
            }
        }
            

        //ICO DEBUG, LATER: consider removing this together with qualisys_connected
       //  if (!qualisys_connected)  
       // {
       //     USR.transform.localPosition = new Vector3(usr_x, usr_y, usr_z);
       //     USR.transform.localEulerAngles = new Vector3(usr_rot_x, usr_rot_y, usr_rot_z);
       // }       

        if (body != null) PRJ.transform.rotation = body.Rotation;
        else PRJ.transform.rotation = USR.transform.rotation;

        // if (body != null) Ear.transform.rotation = body.Rotation;
        // else Ear.transform.rotation = USR.transform.rotation;


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Visuals added

        // Vector3 A_ray_first = USR.transform.position;
        Vector3 A_ray_first = Ear.transform.position;
        Vector3 A_ray_second = SPK.transform.position;
        Vector3 A_forward = (A_ray_first - A_ray_second) * (-10);
        Ray A_ray = new Ray(A_ray_first, A_forward);

        Vector3 B_ray_first = SPK.transform.position;
        Vector3 B_ray_second = PRJ.transform.position;
        Vector3 B_forward = (B_ray_first - B_ray_second) * (-10);
        Ray B_ray = new Ray(B_ray_first, B_forward);

        // Vector3 C_ray_first = USR.transform.position;
        Vector3 C_ray_first = Ear.transform.position;
        Vector3 C_ray_second = PRJ.transform.position;
        Vector3 C_forward = (C_ray_first - C_ray_second) * (-10);
        Ray C_ray = new Ray(C_ray_first, C_forward);


        A_lr.SetPosition(0, A_ray_first);
        A_lr.SetPosition(1, A_ray.GetPoint(10000));

        B_lr.SetPosition(0, B_ray_first);
        B_lr.SetPosition(1, B_ray.GetPoint(10000));

        C_lr.SetPosition(0, C_ray_first);
        C_lr.SetPosition(1, C_ray.GetPoint(10000));


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Projection added

        Vector3 planeNormal = new Vector3(0, 1, 0);
        Vector3 planeXaxis = new Vector3(1, 0, 0);
        Vector3 planeZaxis = new Vector3(0, 0, 1);

        Vector3 xDirection = new Vector3(1, 0, 0);
        Vector3 yDirection = new Vector3(0, 1, 0);
        Vector3 zDirection = new Vector3(0, 0, 1);

        planeXaxis = USR.transform.rotation * planeXaxis;
        planeNormal = USR.transform.rotation * planeNormal;
        planeZaxis = USR.transform.rotation * planeZaxis;

        // float head2ear_angle_theta = Vector3.Angle(planeNormal, yDirection);
        // float head2ear_angle_phi = Vector3.Angle(planeZaxis, zDirection);
        // float earX = Mathf.Sin(Mathf.Deg2Rad * head2ear_angle_theta) * Mathf.Sin(Mathf.Deg2Rad * head2ear_angle_phi);
        // float earZ = Mathf.Sin(Mathf.Deg2Rad * head2ear_angle_theta) * Mathf.Cos(Mathf.Deg2Rad * head2ear_angle_phi);
        // float earY = Mathf.Cos(Mathf.Deg2Rad * head2ear_angle_theta);
        // Ear.transform.position.x = USR.transform.position.x * earX;
        // Ear.transform.position.z = USR.transform.position.z * earZ;
        // Ear.transform.position.y = (USR.transform.position.y - 0.15) * earY;

        // print("Head to Ear Angle: " + head2ear_angle + " |  cosine: " + (Mathf.Cos(Mathf.Deg2Rad * head2ear_angle_theta)) + " | Difference in height: " + 0.15f*(Mathf.Cos(Mathf.Deg2Rad * head2ear_angle_theta)));

        // float head2ear_height_diff =  0.15f*(Mathf.Cos(Mathf.Deg2Rad * head2ear_angle));

        // if (body != null)
        // {

        //     if (!qualisys_connected) qualisys_connected = true;

        //     if (body.Position.magnitude > 0) //just to avoid error when position is NaN
        //     {
        //         //Qualysis to Unity 
        //         USR.transform.position = body.Position - new Vector3(0f, 1.7526f, 0f) - new Vector3(0f, head2ear_height_diff, 0f);
        //     }
        // }
          



        // // Point on Ear level plane
        // Vector3 planePoint_earLevel = Ear.transform.position; //????????????????????????????? HOW TO GET THIS POSITION?

        // //Get the shortest distance between a point and a plane. The output is signed so it holds information as to which side of the plane normal the point is.
        // float SignedDistanceEarLevelUser = Vector3.Dot(planeNormal, (USR.transform.position - planePoint_earLevel));

        // //Reverse the sign of the distance
        // float distanceEarUser = -1 * SignedDistanceEarLevelUser;

        // //Get a translation vector
        // Vector3 translationVectorEarUser = Vector3.Normalize(planeNormal) * distanceEarUser;

        // PRJ.transform.position = USR.transform.position + translationVectorEarUser;

        
        //print("Ear position: " + Ear.transform.position + " | Difference in User head and ear: " + (USR.transform.position - Ear.transform.position));

        // Point on plane
        // Vector3 planePoint = USR.transform.position;
        Vector3 planePoint = Ear.transform.position;

        //Get the shortest distance between a point and a plane. The output is signed so it holds information as to which side of the plane normal the point is.
        float SignedDistancePlaneSpeaker = Vector3.Dot(planeNormal, (SPK.transform.position - planePoint));

        //Reverse the sign of the distance
        float distance = -1 * SignedDistancePlaneSpeaker;

        //Get a translation vector
        Vector3 translationVector = Vector3.Normalize(planeNormal) * distance;

        PRJ.transform.position = SPK.transform.position + translationVector;

        float binaural_elev_angle_Disha = Vector3.Angle(A_forward, C_forward);
        float binaural_azim_angle_Disha = Vector3.Angle(planeZaxis, C_forward);

        //ICO: Figure out if the source is left or right, or above or below the user's orientation
        // float leftright = AngleDir(USR.transform.forward, PRJ.transform.position - USR.transform.position, USR.transform.up);
        // float updown = AngleDir(USR.transform.forward, SPK.transform.position - USR.transform.position, USR.transform.right);
        float leftright = AngleDir(Ear.transform.forward, PRJ.transform.position - Ear.transform.position, Ear.transform.up);
        float updown = AngleDir(Ear.transform.forward, SPK.transform.position - Ear.transform.position, Ear.transform.right);

        binaural_azim_angle_Disha = binaural_azim_angle_Disha * leftright;
        binaural_elev_angle_Disha = binaural_elev_angle_Disha * -updown;

        //print("D: " + binaural_azim_angle_Disha + " | " + binaural_elev_angle_Disha);
        //print("Z: " + binaural_azim_angle_Zach + " " + binaural_elev_angle_Zach + " | D: " + binaural_azim_angle_Disha + " " + binaural_elev_angle_Disha);

        //ICO DEBUG
        // if (body == null)
        // {
        //     USR.transform.localPosition = new Vector3(usr_x, usr_y, usr_z);
        //     USR.transform.eulerAngles = new Vector3(usr_rot_x, usr_rot_y, usr_rot_z);
        // }

        //Send binaural angles only when receive source angles from Max TODO: see if we still need -999 (ICO: this should be fixed on Max side)
        if (source_azim_angle != -999)
        {
            sendBinauralAngles(binaural_elev_angle_Disha, binaural_azim_angle_Disha);
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


}
