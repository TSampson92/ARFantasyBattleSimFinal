using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDespawner : MonoBehaviour
{

    private bool isFalling = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -20)
            Destroy(gameObject);

        if (isFalling)
        {
            Vector3 mCurrPos = transform.position;
            mCurrPos.y = mCurrPos.y - (float)0.02;
            transform.position = mCurrPos;
        }
    }

    public void StartFalling()
    {
        isFalling = true;
    }
}
