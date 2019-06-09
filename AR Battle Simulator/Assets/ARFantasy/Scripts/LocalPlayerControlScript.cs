using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoogleARCore.Examples.CloudAnchors;

#pragma warning disable 618
public class LocalPlayerControlScript : NetworkBehaviour
{
    public GameObject Unit1Prefab;
    public GameObject Unit2Prefab;
    public GameObject Unit3Prefab;
    public GameObject IceSpellPrefab;

    private GameController mGameController;
    private NetworkManagerUIController SnackBarUI;
    private WorldController mWorldController;
    private UnitController mUnitController;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

    }

    void Update()
    {
        if (SnackBarUI == null)
            SnackBarUI = GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>();
        if (mGameController == null)
            mGameController = GameObject.Find("GameController").GetComponent<GameController>();
        if (mUnitController == null)
            mUnitController = GameObject.Find("UnitController").GetComponent<UnitController>();
        if (mGameController.GetGameMode() == 2)
        {
            if (mWorldController == null)
                mWorldController = GameObject.Find("World").GetComponent<WorldController>();
        }
    }

    [Command]
    public void CmdSpawnUnit1(Vector3 position, Quaternion rotation)
    {   
        //Correct for rotation
        Quaternion tempRot = rotation;
        //tempRot.x = tempRot.x - 90;
        rotation = tempRot;

        //Correct for height
        //Vector3 tempPos = position;
        //tempPos.y += (Unit1Prefab.transform.localScale.y / 2);
        //position = tempPos;

        //Spawn prefab
        var Unit1Object = Instantiate(Unit1Prefab, position, rotation);
        Vector3 tempScale = Vector3.Scale(Unit1Object.transform.localScale, mWorldController.GetWorldScale());
        //Unit1Object.transform.localScale = tempScale;
        NetworkServer.Spawn(Unit1Object);
        RpcSyncUnitScale(Unit1Object, tempScale);
        //SnackBarUI.ShowErrorMessage("Cmd Spawn: " + Unit1Object);
    }

    [Command]
    public void CmdSpawnUnit2(Vector3 position, Quaternion rotation)
    {
        //Correct for rotation
        Quaternion tempRot = rotation;
        //tempRot.x = tempRot.x - 90;
        rotation = tempRot;

        //Correct for height
        //Vector3 tempPos = position;
        //tempPos.y += (Unit2Prefab.transform.localScale.y / 2);
        //position = tempPos;

        //Spawn prefab
        var Unit2Object = Instantiate(Unit2Prefab, position, rotation);
        Vector3 tempScale = Vector3.Scale(Unit2Object.transform.localScale, mWorldController.GetWorldScale());
        //Unit2Object.transform.localScale = tempScale;
        NetworkServer.Spawn(Unit2Object);
        RpcSyncUnitScale(Unit2Object, tempScale);
        //SnackBarUI.ShowErrorMessage("Cmd Spawn: " + Unit2Object);
    }

    [Command]
    public void CmdSpawnUnit3(Vector3 position, Quaternion rotation)
    {
        //Correct for rotation
        Quaternion tempRot = rotation;
       // tempRot.x = tempRot.x + 90;
        rotation = tempRot;

        //Correct for height
       // Vector3 tempPos = position;
        //tempPos.y += (Unit3Prefab.transform.localScale.y / 2);
        //position = tempPos;

        //Spawn prefab
        var Unit3Object = Instantiate(Unit3Prefab, position, rotation);
        Vector3 tempScale = Vector3.Scale(Unit3Object.transform.localScale, mWorldController.GetWorldScale());
        //Unit3Object.transform.localScale = tempScale;
        NetworkServer.Spawn(Unit3Object);
        RpcSyncUnitScale(Unit3Object, tempScale);
        //SnackBarUI.ShowErrorMessage("Cmd Spawn: " + Unit3Object);
    }

    [Command]
    public void CmdWizardSpell(Vector3 pos)
    {
       GameObject obj = Instantiate(IceSpellPrefab, new Vector3(pos.x, pos.y + 0.15f, pos.z), Quaternion.identity);
       NetworkServer.Spawn(obj);
        
    }

    [Command]
    public void CmdMeleeMove(GameObject obj, Vector3 pos)
    {
        MovementControlScript m = obj.GetComponent<MovementControlScript>();
        m.Move(pos);
    }

    [Command]
    public void CmdMeleeHalt(GameObject obj)
    {
        MovementControlScript m = obj.GetComponent<MovementControlScript>();
        m.Halt();
    }

    [Command]
    public void CmdMeleeAttackGround(GameObject obj, Vector3 pos)
    {
        MovementControlScript m = obj.GetComponent<MovementControlScript>();
        m.Attack(pos);
    }

    [Command]
    public void CmdMeleeAttackUnit(GameObject obj, GameObject attobj)
    {
        MovementControlScript m = obj.GetComponent<MovementControlScript>();
        m.AttackUnit(attobj);
    }

    [Command]
    public void CmdDespawnObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    [Command]
    public void CmdPlayerReady(bool isReady)
    {
        if (isReady == false)
            mGameController.SetPlayerReady(-1);
        else
            mGameController.SetPlayerReady(1);
    }

    [Command]
    public void CmdSetGameMode(int mode)
    {
        mGameController.SetGameMode(mode);
    }

    [ClientRpc]
    public void RpcSyncUnitScale(GameObject obj, Vector3 scale)
    {      
        obj.transform.localScale = scale;
        obj.GetComponent<BasicUnitScript>().SetOwnerID(this.GetComponent<LocalPlayerController>().GetNetID());
        mUnitController.AddNewUnit(obj);     
    }

    [Command]
    public void CmdResetGame()
    {
        mGameController.RpcResetGame();
    }
}
