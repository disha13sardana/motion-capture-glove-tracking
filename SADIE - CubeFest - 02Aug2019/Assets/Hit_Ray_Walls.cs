using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Ray_Walls : MonoBehaviour {
    public GameObject Test_RI;
    public GameObject LWall;
    public GameObject RWall;

    public Vector3 first;
    public Vector3 second;

    public Collider coll_l;
    public Collider coll_r;

    // Use this for initialization
    void Start () {
        LWall = GameObject.Find("LWall");
        RWall = GameObject.Find("RWall");
        //LWall.transform.position = new Vector3(-2.145166f,0.0f,0.0f);
        //LWall.transform.position.x = -2.145166f;
        //second.x = 20f;
        coll_l = LWall.GetComponent<Collider>();
        coll_r = RWall.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //float posx = Test_RI.transform.position.x;
        //print(posx);

        Vector3 forward = (first - second) * (-5);
        Ray ray = new Ray(first, forward);
        Debug.DrawRay(first, forward, Color.red, 1, false);
        RaycastHit hit1;
        RaycastHit hit2;
        if (coll_l.Raycast(ray, out hit1, 30.0F))
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            print("boo");

        }

        if (coll_r.Raycast(ray, out hit2, 30.0F))
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            print("baaaa");

        }

        if (Physics.Raycast(ray, out hit1, 100))
        {
            print("Found LWall - distance: " + hit1.distance + hit1.point);
            Debug.DrawLine(ray.origin, hit1.point, Color.green, 1, false);
        }



        if (Physics.Raycast(ray, out hit2, 100))
        {
            print("Found RWall - distance: " + hit2.distance + hit2.point);
            Debug.DrawLine(ray.origin, hit2.point, Color.green, 1, false);
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
