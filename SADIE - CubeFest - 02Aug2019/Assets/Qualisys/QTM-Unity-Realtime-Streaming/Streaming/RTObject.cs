// Unity SDK for Qualisys Track Manager. Copyright 2015-2017 Qualisys AB
//
using UnityEngine;
using System.Collections;
using QTMRealTimeSDK;

namespace QualisysRealTime.Unity
{
    class RTObject : MonoBehaviour
    {
        public string ObjectName = "Put QTM 6DOF object name here";
        public Vector3 PositionOffset = new Vector3(0, 0, 0);
        public Vector3 EulerOffset = new Vector3(0, 0, 0);

        private RTClient rtClient;

        // Use this for initialization
        void Start()
        {
            rtClient = RTClient.GetInstance();
        }

        // Update is called once per frame
        void Update()
        {
            if (rtClient == null) rtClient = RTClient.GetInstance();

            SixDOFBody body = rtClient.GetBody(ObjectName);
            if (body != null)
            {
                if (body.Position.magnitude > 0) //just to avoid error when position is NaN
                {
                    transform.position = body.Position + PositionOffset;
                    if (transform.parent) transform.position += transform.parent.position;
                    transform.rotation = body.Rotation * Quaternion.Euler(EulerOffset);
                    if (transform.parent) transform.rotation *= transform.parent.rotation;
                }
            }
        }
    }
}