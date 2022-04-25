using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Ray : MonoBehaviour {
    public GameObject LWall;
    public GameObject RWall;
    public GameObject FWall;
    public GameObject BWall;
    public GameObject Ceiling;

    public Vector3 first;
    public Vector3 second;

    public Collider coll;

    public GameObject rayLine;
    public LineRenderer lr;

    private GameObject impact;
    private GameObject CubeOrigin;
    private GameObject projection;

	// Use this for initialization
	void Start () {

        LWall = GameObject.Find("LWall");
        RWall = GameObject.Find("RWall");
        FWall = GameObject.Find("FWall");
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

        /*CubeOrigin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        CubeOrigin.GetComponentInChildren<Collider>().enabled = false;
        CubeOrigin.transform.position = new Vector3(0f, 1.7526f, 0f);
        CubeOrigin.GetComponent<Renderer>().material.color = new Color(0.5f,0,0);
        */

        //Vector3 UnityOrigin = new Vector3(0,0,0);

        
        //LWall.transform.scale.x = 2;
        //FWall.transform.scale = new Vector3(1,1,2);
        //BWall.transform.scale = new Vector3(1,1,2);
        

        rayLine = new GameObject("rayLine");
        rayLine.transform.position = new Vector3(0f, 0f, 0f);

        lr = rayLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(Color.red, Color.green);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, new Vector3(0f, 0f, 0f));
        lr.SetPosition(1, new Vector3(1f, 0f, 0f));

        impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impact.GetComponentInChildren<Collider>().enabled = false;
        impact.GetComponent<Renderer>().material.color = new Color(0,0,0.5f);
        
        projection = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projection.GetComponentInChildren<Collider>().enabled = false;
        projection.GetComponent<Renderer>().material.color = new Color(1f,0.5f,0.5f);
        

        //coll = LWall.GetComponent<Collider>();
        //coll = RWall.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //float posx = Test_RI.transform.position.x;
        //print(posx);

        Vector3 forward = (first - second) * (-500);
        Ray ray = new Ray(first, forward);
        //print(first + " " + second);
        //Debug.DrawRay(first, forward, Color.red, 1, false);
        lr.SetPosition(0, first);
        lr.SetPosition(1, ray.GetPoint(100));


        RaycastHit hit;
        /*if (coll.Raycast(ray, out hit, 30.0F))
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            print("boo");

        }*/

        if (Physics.Raycast(ray, out hit))
        {
            print("Found an object - distance: " + hit.distance + hit.point);
            impact.transform.position = hit.point;
            projection.transform.position = new Vector3(hit.point.x, 1.7526f, hit.point.z);
            Vector3 impactDir = impact.transform.position;
            Vector3 projDir = projection.transform.position;
            Vector3 xDir = new Vector3(1,1.7526f,0);

            float elev_angle = Vector3.Angle(impactDir, projDir);
            float azim_angle = Vector3.Angle(xDir, projDir);
            //projection.transform.position.z = hit.point.z;
            //projection.transform.position.y = 0;
            //print(elev_angle + "&" + azim_angle);
            
            //print("RI:"+RI.transform.position+"Hit point = " + hit.point +  " & Azimuthal Angle = " + azim_angle + "& Elevation Angle = " + elev_angle);
            //Debug.DrawLine(ray.origin, hit.point, Color.green, 1, false);
        }


        /*if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (coll.Raycast(ray, out hit, 100.0F))
            {
                print("ouch");
            }

        }*/
       

    }

}
