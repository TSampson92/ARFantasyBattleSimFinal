using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordmanSpawner : MonoBehaviour
{
    public float mUnitSize = (float)0.03;
    public GameObject mUnitPrefab;
    private GameObject[] mUnitArr = new GameObject[25];

    // Start is called before the first frame update
    void Start()
    {
        //scales spawn distance with world scale
        Vector3 mWorldScale = GameObject.Find("World").GetComponent<WorldController>().GetWorldScale();
        mUnitSize = mUnitSize * mWorldScale.x;

        SpawnUnits();
        CorrectTheSpawn();
        SendChildArray();
    }
    void SpawnUnits()
    {
        //Every unit is a child of the parent empty object
        Vector3 CenterPos = gameObject.transform.position;
        Vector3 mTempPos = new Vector3(0, 0, 0);
        IdlePatrolScript PS;
        int mCounter = 0;

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                mTempPos.x = (mUnitSize * i * 2) + CenterPos.x;
                mTempPos.z = (mUnitSize * j * 2) + CenterPos.z;
                GameObject mUnit = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
                mUnitArr[mCounter] = mUnit;
                mCounter++;
                PS = mUnit.GetComponent<IdlePatrolScript>();
                PS.SetOffset((mUnitSize * i * 2), mUnitSize * 3, (mUnitSize * j * 2));
                PS.SetParentObj(gameObject);
            }
        }
    }

    void CorrectTheSpawn()
    {
        foreach (GameObject child in mUnitArr)
        {
            child.transform.localScale = new Vector3(mUnitSize, mUnitSize, mUnitSize);

            Quaternion tempRot = child.transform.rotation;
            tempRot.x = 0;
            tempRot.z = 0;
            tempRot.y = 0;
            child.transform.rotation = tempRot;

            Vector3 tempPos = child.transform.position;
            tempPos.y = (mUnitSize * 3);
            child.transform.position = tempPos;
        }
    }

    void SendChildArray()
    {
        gameObject.GetComponent<MovementControlScript>().SetChildArray(mUnitArr);
        gameObject.GetComponent<LifeControl>().SetChildArray(mUnitArr);
        gameObject.GetComponent<AttackScript>().SetChildArray(mUnitArr);
    }
}

