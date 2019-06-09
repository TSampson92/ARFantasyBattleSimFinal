using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoogleARCore.Examples.CloudAnchors;

#pragma warning disable 618
public class LifeControl : NetworkBehaviour
{
    private GameObject[] mUnitArr;
    UnitController UC;

    [SyncVar(hook = "OnHpChange")]
    private int mHp;
    void OnHpChange(int x)
    {
        mHp = x;
        UpdateUnitSize(mHp);
    }

    void Start()
    {
        UC = GameObject.Find("UnitController").GetComponent<UnitController>();
    }

    public void TakeDmg(int dmg)
    {
        mHp -= dmg;
        UpdateUnitSize(mHp);
    }

    void UpdateUnitSize(int hp)
    {

        if (mUnitArr != null)
            if (hp == mUnitArr.Length)
                return;

        if (hp <= 0)
        {
            mUnitArr[hp].GetComponent<Death>().OnDeath();
            if (isServer)
                RpcRemoveObj();
            return;
        }

        if (mUnitArr[hp] == null)
            return;
        mUnitArr[hp].GetComponent<Death>().OnDeath();
    }

    public void SetChildArray(GameObject[] arr)
    {
        mUnitArr = arr;
        mHp = arr.Length;
    }

    public GameObject[] GetChildArray()
    {
        return mUnitArr;
    }

    [ClientRpc]
    void RpcRemoveObj()
    {
        GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("RpcRemoveObj called");
        UC.RemoveObj(gameObject);
        Destroy(gameObject);
        
    }

     public int GetHP() { return mHp; }
}
