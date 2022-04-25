using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleExample : MonoBehaviour {
    public GameObject RI;
    public GameObject RKL;
    //public Transform target;

    // Use this for initialization
    void Start()
    {

    }
    // prints "close" if the z-axis of this transform looks
    // almost towards the target

    void Update()
    {
        if (RI == null && RKL == null)
        {
            RI = GameObject.Find("RI");
            RKL = GameObject.Find("RKL");
        }

        else
        {
            // Vector3 targetDir = target.position - transform.position;
            Vector3 targetDir = RI.transform.position - RKL.transform.position;
            //float angle = Vector3.Angle(targetDir, RKL.transform.forward);

            print(targetDir);
        }      
    }
    
}
