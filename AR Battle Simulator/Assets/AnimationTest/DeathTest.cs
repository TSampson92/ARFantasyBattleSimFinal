using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTest : MonoBehaviour

   
{
    private List<GameObject> pegList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in this.gameObject.transform)
        {
            pegList.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            int size = pegList.Count;
            Debug.Log(size);
            if (size > 0)
            {
                int rand = Random.Range(0, size);
                pegList[rand].GetComponent<Death>().OnDeath();
                pegList.RemoveAt(rand);
            } else
            {
                Debug.Log("Nothing left to kill!");
            }
            
        }
    }
}
