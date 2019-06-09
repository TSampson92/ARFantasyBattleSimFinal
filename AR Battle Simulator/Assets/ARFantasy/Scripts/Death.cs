using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public float timeToDespawn = 2;
    public float speed = 100;
    private bool isDead = false;

   

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnDeath();
        }
    }
    */

    public void OnDeath()
    {
        //From: https://answers.unity.com/questions/1111106/add-force-in-random-direction-with-set-speed.html
        Debug.Log("Calling Death");
        this.gameObject.AddComponent<Rigidbody>();
        Rigidbody pegBody = this.gameObject.GetComponent<Rigidbody>();
        Vector3 force = new Vector3(Random.Range(-35, 35), Random.Range(50, 100), Random.Range(-35, 35));
        pegBody.drag = 15;
        pegBody.AddRelativeForce(force * speed);
        Debug.Log("Dead peg");
        isDead = true;
        Destroy(this.gameObject, timeToDespawn);
    }
}
