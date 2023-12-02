using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    

    private Vector3 CameraPosition;
   // private float yaw = 0.0f;
   // private float pitch = 0.0f;
    [Header("CameraSettings")]
    public float CameraSpeed;
    public float CameraScrollSpeed;
    // public float speedH = 2.0f;
    //public float speedV = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
        CameraPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Camera mainCamera = GetComponent<Camera>();
        Vector3 cameraForward = mainCamera.transform.forward;

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && CameraPosition.z < 40)
        {
            CameraPosition.z += CameraSpeed * Time.deltaTime;
        }
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && CameraPosition.z > 28)
        {
            CameraPosition.z -= CameraSpeed * Time.deltaTime;
        }
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && CameraPosition.x > -45)
        {
            CameraPosition.x -= CameraSpeed * Time.deltaTime;
        }
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && CameraPosition.x < -10)
        {
            CameraPosition.x += CameraSpeed * Time.deltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && CameraPosition.y > -1)
        {

            CameraPosition += cameraForward * CameraScrollSpeed * Time.deltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && CameraPosition.y < 5)
        {
            CameraPosition -= cameraForward * CameraScrollSpeed * Time.deltaTime;
        }


        /* if (Input.GetKey(KeyCode.Mouse2))
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        } */

        this.transform.position = CameraPosition;

    }
}
