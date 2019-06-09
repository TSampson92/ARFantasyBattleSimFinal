using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSpawner : MonoBehaviour
{
    public float mUnitSpread = (float)0.03;
    public float mUnitSize = 0.02f;
    public GameObject mUnitPrefab;
    private GameObject[] mUnitArr = new GameObject[12];


    // Start is called before the first frame update
    void Start()
    {
        //scales spawn distance with world scale
        Vector3 mWorldScale = GameObject.Find("World").GetComponent<WorldController>().GetWorldScale();
        mUnitSpread = mUnitSpread * mWorldScale.x;
        mUnitSize = mUnitSize * mWorldScale.x;

        gameObject.name = "Mage";

        SpawnUnits();
        CorrectTheSpawn();
        SendChildArray();
    }

    void SendChildArray()
    {
        gameObject.GetComponent<MovementControlScript>().SetChildArray(mUnitArr);
        gameObject.GetComponent<LifeControl>().SetChildArray(mUnitArr);
        gameObject.GetComponent<AttackScript>().SetChildArray(mUnitArr);
    }

    void SpawnUnits()
    {
        //Every unit is a child of the parent empty object
        Vector3 CenterPos = gameObject.transform.position;
        Vector3 mTempPos = new Vector3(0, 0, 0);
        IdlePatrolScript PS;


        //Unit 1
        mTempPos.x = (-mUnitSpread * 1) + CenterPos.x;
        mTempPos.z = (mUnitSpread * 2) + CenterPos.z;
        GameObject mUnit1 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[0] = mUnit1;
        PS = mUnit1.GetComponent<IdlePatrolScript>();
        PS.SetOffset((-mUnitSpread * 1), mUnitSpread * 3, (mUnitSpread * 2));
        PS.SetParentObj(gameObject);


        //Unit 2
        mTempPos.x = (mUnitSpread * 1) + CenterPos.x;
        mTempPos.z = (mUnitSpread * 2) + CenterPos.z;
        GameObject mUnit2 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[1] = mUnit2;
        PS = mUnit2.GetComponent<IdlePatrolScript>();
        PS.SetOffset((mUnitSpread * 1), mUnitSpread * 3, (mUnitSpread * 2));
        PS.SetParentObj(gameObject);

        //Unit 3
        mTempPos.x = (-mUnitSpread * 2) + CenterPos.x;
        mTempPos.z = (mUnitSpread * 1) + CenterPos.z;
        GameObject mUnit3 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[2] = mUnit3;
        PS = mUnit3.GetComponent<IdlePatrolScript>();
        PS.SetOffset((-mUnitSpread * 2), mUnitSpread * 3, (mUnitSpread * 1));
        PS.SetParentObj(gameObject);

        //Unit 4
        mTempPos.x = CenterPos.x;
        mTempPos.z = (mUnitSpread * 1) + CenterPos.z;
        GameObject mUnit4 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[3] = mUnit4;
        PS = mUnit4.GetComponent<IdlePatrolScript>();
        PS.SetOffset(0, mUnitSpread * 3, (mUnitSpread * 1));
        PS.SetParentObj(gameObject);

        //Unit 5
        mTempPos.x = (mUnitSpread * 2) + CenterPos.x;
        mTempPos.z = (mUnitSpread * 1) + CenterPos.z;
        GameObject mUnit5 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[4] = mUnit5;
        PS = mUnit5.GetComponent<IdlePatrolScript>();
        PS.SetOffset((mUnitSpread * 2), mUnitSpread * 3, (mUnitSpread * 1));
        PS.SetParentObj(gameObject);

        //Unit 6
        mTempPos.x = (-mUnitSpread * 1) + CenterPos.x;
        mTempPos.z = CenterPos.z;
        GameObject mUnit6 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[5] = mUnit6;
        PS = mUnit6.GetComponent<IdlePatrolScript>();
        PS.SetOffset((-mUnitSpread * 1), mUnitSpread * 3, 0);
        PS.SetParentObj(gameObject);

        //Unit 7
        mTempPos.x = (mUnitSpread * 1) + CenterPos.x;
        mTempPos.z = CenterPos.z;
        GameObject mUnit7 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[6] = mUnit7;
        PS = mUnit7.GetComponent<IdlePatrolScript>();
        PS.SetOffset((mUnitSpread * 1), mUnitSpread * 3, 0);
        PS.SetParentObj(gameObject);

        //Unit 8
        mTempPos.x = (-mUnitSpread * 2) + CenterPos.x;
        mTempPos.z = (-mUnitSpread * 1) + CenterPos.z;
        GameObject mUnit8 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[7] = mUnit8;
        PS = mUnit8.GetComponent<IdlePatrolScript>();
        PS.SetOffset((-mUnitSpread * 2), mUnitSpread * 3, (-mUnitSpread * 1));
        PS.SetParentObj(gameObject);

        //Unit 9
        mTempPos.x = CenterPos.x;
        mTempPos.z = (-mUnitSpread * 1) + CenterPos.z;
        GameObject mUnit9 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[8] = mUnit9;
        PS = mUnit9.GetComponent<IdlePatrolScript>();
        PS.SetOffset(0, mUnitSpread * 3, (-mUnitSpread * 1));
        PS.SetParentObj(gameObject);

        //Unit 10
        mTempPos.x = (mUnitSpread * 2) + CenterPos.x;
        mTempPos.z = (-mUnitSpread * 1) + CenterPos.z;
        GameObject mUnit10 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[9] = mUnit10;
        PS = mUnit10.GetComponent<IdlePatrolScript>();
        PS.SetOffset((mUnitSpread * 2), mUnitSpread * 3, (-mUnitSpread * 1));
        PS.SetParentObj(gameObject);

        //Unit 11
        mTempPos.x = (-mUnitSpread * 1) + CenterPos.x;
        mTempPos.z = (-mUnitSpread * 2) + CenterPos.z;
        GameObject mUnit11 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[10] = mUnit11;
        PS = mUnit11.GetComponent<IdlePatrolScript>();
        PS.SetOffset((-mUnitSpread * 1), mUnitSpread * 3, (-mUnitSpread * 2));
        PS.SetParentObj(gameObject);

        //Unit12
        mTempPos.x = (mUnitSpread * 1) + CenterPos.x;
        mTempPos.z = (-mUnitSpread * 2) + CenterPos.z;
        GameObject mUnit12 = Instantiate(mUnitPrefab, mTempPos, gameObject.transform.rotation) as GameObject;
        mUnitArr[11] = mUnit12;
        PS = mUnit12.GetComponent<IdlePatrolScript>();
        PS.SetOffset((mUnitSpread * 1), mUnitSpread * 3, (-mUnitSpread * 2));
        PS.SetParentObj(gameObject);
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
}
