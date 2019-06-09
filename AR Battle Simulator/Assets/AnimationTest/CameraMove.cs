using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    [Header("Camera Speed")]
    public int Speed = 50;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
        }

        float xAxisValue = Input.GetAxis("Horizontal") * Speed;
        float zAxisValue = Input.GetAxis("Vertical") * Speed;
        float rotation = Input.GetAxis("Mouse ScrollWheel") * Speed;

        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + rotation, transform.rotation.z, transform.rotation.w);
        transform.position = new Vector3(transform.position.x + xAxisValue, transform.position.y, transform.position.z + zAxisValue);
    }
}
