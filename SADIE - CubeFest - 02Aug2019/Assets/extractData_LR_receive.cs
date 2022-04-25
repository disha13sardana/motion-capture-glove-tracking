using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class extractData_LR_receive : MonoBehaviour {
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

    // for creating Cube 
    public GameObject LWall;
    public GameObject RWall;
    public GameObject FWall;
    public GameObject BWall;
    public GameObject Ceiling;

    public Collider coll;

    // for creating ray
    public GameObject right_rayLine;
    public LineRenderer right_lr;

    public GameObject left_rayLine;
    public LineRenderer left_lr;

    public GameObject right_hand_rayLine;
    public LineRenderer right_hand_lr;

    public GameObject left_hand_rayLine;
    public LineRenderer left_hand_lr;

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

    public int layer_mask;

    public static int localPort;

    // prefs
    public string IP;  // define in init
    public int port;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

        // gui
    string strMessage = "";

    // receiving Thread
    Thread receiveThread;

        // infos
    public string lastReceivedUDPPacket="";
    public string allReceivedUDPPackets=""; // clean up this from time to time!





    // call it from shell (as program)
    public static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.init();

    }

    // Use this for initialization
    void Start () {

        init();

        initReceive();

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
       
    }

	// init
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define
        //IP="127.0.0.1";
        IP = "192.168.1.38";
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

    public void initReceive()
    {
 // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        // print("UDPSend.init()");
        //IP = "192.168.1.38";
        // define port
        port = 9002;

        // status
        //print("Sending to 127.0.0.1 : "+port);
        //print("Test-Sending to this Port: nc -u 127.0.0.1  "+port+"");


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
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



   // receive thread
    public void ReceiveData()
    {
        // const int SIO_CONNRESET = -1744830452; 
        client = new UdpClient(port);
        // client.Client.IOControl((IOControlCode)SIO_CONNRESET,new byte[]{0,0,0,0},null);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                //print(">> " + text);

                // latest UDPpacket
                lastReceivedUDPPacket=text;

                // ....
                allReceivedUDPPackets=allReceivedUDPPackets+text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }



    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets="";
        return lastReceivedUDPPacket;
    }

    // public float getLatestUDPPacket()
    // {
    //     allReceivedUDPPackets="";
    //     return lastReceivedUDPPacket;
    // }

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
                //print("left pointazel " + left_azim_angle + " " + left_elev_angle)
                               
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
                //print("left handazel " + left_hand_azim_angle + " " + left_hand_elev_angle);
               

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


            print(getLatestUDPPacket());

        // string dataFromMax = getLatestUDPPacket();
        
        // print(dataFromMax);



        }

    }
}
