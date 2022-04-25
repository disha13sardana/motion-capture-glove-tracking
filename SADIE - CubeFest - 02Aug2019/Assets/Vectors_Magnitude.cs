using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectors_Magnitude : MonoBehaviour
{
    public GameObject RI;
    public GameObject RKL;
    public GameObject TAR;
    public GameObject RT;

    public GameObject LWall;
    public GameObject RWall;
    public GameObject FWall;

    public Vector3 first;
    public Vector3 second;

    public Collider coll;

    // Use this for initialization
    void Start()
    {
        LWall = GameObject.Find("LWall");
        FWall = GameObject.Find("FWall");
        RWall = GameObject.Find("RWall");
        RWall = GameObject.Find("RWall");
        //second.x = 20f;
        //coll = LWall.GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (RI == null && RKL == null && TAR == null)
        {
            // Index finger
            RI = GameObject.Find("RI");
            // Left knuckle
            RKL = GameObject.Find("RKL");
            // Target
            TAR = GameObject.Find("TAR");
            // Thumb
            RT = GameObject.Find("RT");
        }
        else
        {

            //Vector3 targetDir1 = RI.transform.position - TAR.transform.position;
            //Vector3 targetDir2 = RKL.transform.position - RI.transform.position;
            //float angle = Vector3.Angle(targetDir1, TAR.transform.forward);
            //float angle = Vector3.Angle(targetDir2, TAR.transform.TransformDirection(targetDir1));
            // print(angle);


            // Draw ray
            Vector3 first = RI.transform.position;
            Vector3 second = RKL.transform.position;
            Vector3 forward = (first - second) * (-500);
            Ray ray = new Ray(first, forward);
            Debug.DrawRay(first, forward, Color.red, 1, false);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 500))
            {
                print("boo");
                print("Found an object - distance: " + hit.distance + hit.point);
                Debug.DrawLine(ray.origin, hit.point, Color.green, 1, false);
            }


            // Distance between index finger and thumb
            /*Vector3 targetDir3 = RI.transform.position - RT.transform.position;
            float mag = Vector3.Magnitude(targetDir3);
            float sqrLen = targetDir3.sqrMagnitude;
            print(mag + "  " + sqrLen);*/

            // Ray casting
            /*Ray ray = new Ray(RI.transform.position, RI.transform.forward);
            print(ray.GetPoint(10));*/


            //Vector3 forward = RI.transform.TransformDirection(targetDir1) * (-2);
            // Debug.DrawRay(RI.transform.position, forward, Color.red, 1, false);

            /* Vector3 forward = RKL.transform.TransformDirection(targetDir2) * (-50);
             Debug.DrawRay(RKL.transform.position, forward, Color.red, 1, false);*/

            //Vector3 origin = Ray.origin(RI.transform.position);
            //print(origin);
            //print(TAR.transform.position);
            //print(targetDir1 + "" + angle);
            //print(RI.transform.eulerAngles + "" + RI.transform.position.x + "" + RI.transform.position.y + "" + RI.transform.position.z);
            print(" X: " + RKL.transform.position.x + " Y: " + RKL.transform.position.y + " Z: " + RKL.transform.position.z);
        }

    }
}
