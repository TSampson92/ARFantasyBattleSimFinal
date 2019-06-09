using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class BlobProperties : MonoBehaviour
{
    public List<GameObject> enemiesInSight = new List<GameObject>(); //In sight
    public List<GameObject> enemiesInRange = new List<GameObject>(); //Fighting distance
    public float inSightRadius = .5f; //TODO: update when pegmen die or join the blob
    public float inRangeRadius = .3f; //TODO: change based on blob size?
    public int numEnemiesInSight = 0; //For debugging
    public int numEnemiesInRange = 0; //For debugging
    public bool sightEnemies = false; //For debugging
    public bool rangeEnemies = false; //For debugging

    private NetworkInstanceId mAllyNetID;
    private int mCounter = 0;
    private List<GameObject> mFirstObjList;
    private List<GameObject> mSecondObjList;
    private UnitController mUnitController;


    private void Start()
    {
        mAllyNetID = gameObject.GetComponent<BasicUnitScript>().GetOwnerID();
        mUnitController = GameObject.Find("UnitController").GetComponent<UnitController>();
    }

    void Update()
    {
       // if (!isServer)
        //    return;

        //Updates once a second
        mCounter++;
        if (mCounter < 60)
        {
            return;
        }
        mCounter = 0;
        RescanSurroundings();
    }

    void RescanSurroundings()
    {
        mFirstObjList = mUnitController.GetPlayerOneUnits();
        mSecondObjList = mUnitController.GetPlayerTwoUnits();

        //If either is empty, no need to scan, game is over.
        if (mFirstObjList.Count == 0 || mSecondObjList.Count == 0)
            return;

        FillBoth();
        return;

    }

    void FillBoth()
    {
        enemiesInRange.Clear();
        enemiesInSight.Clear();
        Vector3 mCurrPos = gameObject.transform.position;
        foreach (GameObject unit in mFirstObjList)
        {
            if (unit == null)
                return;

            Vector3 mUnitPos = unit.transform.position;
            if ((mCurrPos - mUnitPos).magnitude < inRangeRadius)
            {
                enemiesInRange.Add(unit);
            }
            if ((mCurrPos - mUnitPos).magnitude < inSightRadius)
            {
                enemiesInSight.Add(unit);
            }
        }

        foreach (GameObject unit in mSecondObjList)
        {
            if (unit == null)
                return;

            Vector3 mUnitPos = unit.transform.position;
            if ((mCurrPos - mUnitPos).magnitude < inRangeRadius)
            {
                enemiesInRange.Add(unit);
            }
            if ((mCurrPos - mUnitPos).magnitude < inSightRadius)
            {
                enemiesInSight.Add(unit);
            }
        }

    }

    public NetworkInstanceId GetAllyID() { return mAllyNetID; }
    public List<GameObject> GetEnemyInSight() { return enemiesInSight; }
    public List<GameObject> GetEnemyInRange() { return enemiesInRange; }
}