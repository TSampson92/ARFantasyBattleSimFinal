using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class BasicUnitScript : MonoBehaviour
{
    private NetworkInstanceId mUnitID;
    private NetworkInstanceId mOwnerID;
    private string mUnitTypeName;

    // Start is called before the first frame update
    void Start()
    {
        mUnitID = this.GetComponent<NetworkIdentity>().netId;
        mUnitTypeName = this.name;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwnerID(NetworkInstanceId id) { mOwnerID = id; }
    public NetworkInstanceId GetOwnerID() { return mOwnerID; }
    public NetworkInstanceId GetUnitID() { return mUnitID; }
    public string GetUnitName() { return mUnitTypeName; }
}
