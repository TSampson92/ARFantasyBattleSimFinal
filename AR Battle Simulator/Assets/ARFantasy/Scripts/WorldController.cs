using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class WorldController : NetworkBehaviour
{
    [SyncVar(hook = "OnScaleChange")]
    private Vector3 mWorldScale = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "World";
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Command]
    public void CmdSetScale(Vector3 vec)
    {
        mWorldScale = vec;
    }

    private void OnScaleChange(Vector3 vec)
    {
        transform.localScale = vec;
    }

    public Vector3 GetWorldScale()
    {
        return mWorldScale;
    }
}
