using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    bool up = true;

    void Update()
    {
        float newPos = .002f;
        if (transform.position.z <= .3 && transform.position.z >= -.3 && up) //within bounds going up
            newPos = transform.position.z + .005f;
        else if (transform.position.z <= 3 && transform.position.z >= -.3 && !up) //within bounds going down
            newPos = transform.position.z - .005f;
        else if (transform.position.z > .3 && up) //out of bounds going up
        {
            newPos = .28f;
            up = false;
        }
        else if (transform.position.z < -.3 && !up) //out of bounds going down
        {
            newPos = -.28f;
            up = true;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, newPos);
    }
}