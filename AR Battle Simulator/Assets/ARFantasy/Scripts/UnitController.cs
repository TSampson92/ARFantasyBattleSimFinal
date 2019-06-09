using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoogleARCore.Examples.CloudAnchors;
#pragma warning disable 618
public class UnitController : MonoBehaviour
{

    private List<GameObject> mFirstObjList;
    private List<GameObject> mSecondObjList;
    private NetworkInstanceId mFirstID;
    private NetworkInstanceId mSecondID;
    private bool isFirstSet = false;
    private bool isSecondSet = false;

    public int ArrayOneCount = 0;
    public int ArrayTwoCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        mFirstObjList = new List<GameObject>();
        mSecondObjList = new List<GameObject>();
    }

    void Update()
    {
        ArrayOneCount = mFirstObjList.Count;
        ArrayTwoCount = mSecondObjList.Count;
    }

    public void AddNewUnit(GameObject obj)
    {
        NetworkInstanceId id = obj.GetComponent<BasicUnitScript>().GetOwnerID();
        //First unit for Player1
        if (isFirstSet == false)
        {
            isFirstSet = true;
            mFirstID = id;
            mFirstObjList.Add(obj);
        }
        //Second unit for player2
        else if (mFirstID == id)
        {
            mFirstObjList.Add(obj);
        }
        //First unit for player2
        else if (isSecondSet == false)
        {
            isSecondSet = true;
            mSecondID = id;
            mSecondObjList.Add(obj);
        }
        //Second unit for player2
        else if (mSecondID == id)
        {
            mSecondObjList.Add(obj);
        }
    }

    public void SetUnitColors()
    {
        foreach (var obj in mFirstObjList)
        {
            GameObject[] mChildArray = obj.GetComponent<LifeControl>().GetChildArray();
            foreach (GameObject child in mChildArray)
            {
                foreach (Transform c in child.transform)
                {
                    if (c.tag == "Color")
                        c.GetComponent<Renderer>().material.color = new Color((float)155/255, 0, 0);
                }
            }
        }

        foreach (var obj in mSecondObjList)
        {
            GameObject[] mChildArray = obj.GetComponent<LifeControl>().GetChildArray();
            foreach (GameObject child in mChildArray)
            {
                foreach (Transform c in child.transform)
                {
                    if (c.tag == "Color")
                        c.GetComponent<Renderer>().material.color = new Color(0, 0, (float)155/255);
                }
            }
        }
    }

    public List<GameObject> GetPlayerOneUnits()
    {
        return mFirstObjList;
    }

    public List<GameObject> GetPlayerTwoUnits()
    {
        return mSecondObjList;
    }

    public int ListOneCount()
    {
        if (mFirstObjList == null)
            return 0;
        else
            return mFirstObjList.Count;
    }
    public int ListTwoCount()
    {
        if (mSecondObjList == null)
            return 0;
        else
            return mSecondObjList.Count;
    }
    
    public void RemoveObj(GameObject obj)
    {
        GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("RpcRemoveObj Reached");
        //mFirstObjList.RemoveAll(item => item == null);
        //mSecondObjList.RemoveAll(item => item == null);

        //foreach (GameObject obj in mFirstObjList)
        //{
        //    if (obj == null)
        //        mFirstObjList.Remove(obj);

        //   else if (obj.GetComponent<LifeControl>().GetHP() <= 0)
        //        mFirstObjList.Remove(obj);
        //}

        //foreach (GameObject obj2 in mSecondObjList)
        //{
        //    if (obj2 == null)
        //       mSecondObjList.Remove(obj2);
        //   else
        //        if (obj2.GetComponent<LifeControl>().GetHP() <= 0)
        //        mSecondObjList.Remove(obj2);
        //}
    
         if (mFirstObjList.Contains(obj))
              mFirstObjList.Remove(obj);

         if (mSecondObjList.Contains(obj))
              mSecondObjList.Remove(obj);

        GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("RpcRemoveObj Finished");
    }

    public void DestroyLists()
    {
        foreach (GameObject obj in mFirstObjList)
        {
            if (obj != null)
                Destroy(obj);
        }

        foreach (GameObject obj2 in mSecondObjList)
        {
            if (obj2 != null)
                Destroy(obj2);
        }

        mFirstObjList = null;
        mSecondObjList = null;

        mFirstObjList = new List<GameObject>();
        mSecondObjList = new List<GameObject>();
    }
}
