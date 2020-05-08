using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraBehaviour : MonoBehaviour
{
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    Vector3 center = new Vector3(0, 0, 0);
    Vector3 position = new Vector3(0, 0, 0);

    // Update camer position depending on mouse movement
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))    //right click maintained
        {
            float fixedZRotation = transform.rotation.z;

            //get mouse movement
            yaw = speedH * Input.GetAxis("Mouse X");    
            pitch = speedV * Input.GetAxis("Mouse Y");

            //move camera to position
            transform.Translate(Vector3.up * pitch);
            transform.Translate(Vector3.right * yaw);
            position = transform.position;

            //adjust camera distance to the center of the 3D function
            double ecDist = ecDist = Math.Pow(position.x, 2) + Math.Pow(position.y, 2) + Math.Pow(position.z, 2);
            double diff = 8000 / ecDist;
            position.x = (float)Math.Sqrt(Math.Pow(position.x, 2) * diff) * (position.x / Math.Abs(position.x));
            position.y = (float)Math.Sqrt(Math.Pow(position.y, 2) * diff) * (position.y / Math.Abs(position.y));
            position.z = (float)Math.Sqrt(Math.Pow(position.z, 2) * diff) * (position.z / Math.Abs(position.z));

            if((float.IsNaN(position.x)| float.IsNaN(position.y)| float.IsNaN(position.z))==false)      //check for occasionnal value error
            {
                transform.position = position;      //update position
                transform.LookAt(center);         //face the center of the 3D function
            }
        }
    }
}
