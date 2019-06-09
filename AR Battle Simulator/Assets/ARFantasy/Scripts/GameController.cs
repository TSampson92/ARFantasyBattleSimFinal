using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using GoogleARCore.Examples.CloudAnchors;

// This script will begin running with the placement of the world object
#pragma warning disable 618
public class GameController : NetworkBehaviour
{
    public GameObject mPlacementUI;
    public GameObject mUnitControlUI;
    public Button mStartPlacementButton;
    public Button mResetMapButton;
    public GameObject UnitControlObject;
    private UnitController mUnitController;
    public UnitControlScript mUnitControlScript;
    public BattleControlScript mBattleController;
    public UnitPlacementScript mUnitPlacementController;
    public NewGameUIScript mNewGameUIController;
    private LocalPlayerController PlayerController;
   // private LocalPlayerControlScript PlayerSpawner;

    // Lobby = 0,
    // SettingWorld = 1,
    // PlacingUnits = 2,
    // GameStarted = 3
    [SyncVar (hook = "OnGameStateChange")]
    public int mGameMode = 0;
    void OnGameStateChange(int mode) { mGameMode = mode; }

    [SyncVar(hook = "OnPlayerReadyChange")]
    private int mPlayerReady = 0;
    void OnPlayerReadyChange(int count) { mPlayerReady = count; }

    private bool isMode1 = false;
    private bool isMode2 = false;
    private bool isMode3 = false;

    private float mTimer = 0;

    void Start()
    {
        mStartPlacementButton.onClick.AddListener(delegate { StartPlacement(); });
        mResetMapButton.onClick.AddListener(delegate { ResetMapPlacement(); });
        TogglePlacementUI(false);
        mUnitController = UnitControlObject.GetComponent<UnitController>();
    }



    // Update is called once per frame
    void Update()
    {
        //Out of lobby phase, local player is spawned
        if (mGameMode == 1 && isMode1 == false) { 
            isMode1 = true;
        }

        //Placement mode begins.
        if (mGameMode == 2 && isMode2 == false)
        {
            TogglePlacementUI(true);
            isMode2 = true;
        }

        //Game Begins
        if (mPlayerReady == 2 && isMode3 == false)
        {
            //Start Game here
            if(isServer)
                RpcStartGame();
            isMode3 = true;

            TogglePlacementUI(false);
            SetGameMode(3);
        }

        if (isMode3 == true)
        {
            mTimer++;
            if (mTimer > 120)
            {
                mTimer = 0;
                CheckForEndGame();
            }
        }

    }

    void CheckForEndGame()
    {
        string msg = "";
        if (mUnitController.ListOneCount() == 0)
        {
            //Red player wins
            msg = msg + "Red Wins!";
            mNewGameUIController.gameObject.SetActive(true);
            mNewGameUIController.SetWinnerText(msg);
            return;
        }
        if (mUnitController.ListTwoCount() == 0)
        {
            //Blue player wins
            msg = msg + "Blue Wins!";
            mNewGameUIController.gameObject.SetActive(true);
            mNewGameUIController.SetWinnerText(msg);
            return;
        }
    }

    public void TogglePlacementUI(bool mode)
    {
        mPlacementUI.SetActive(mode);
    }

    void StartPlacement()
    {
        if (GameObject.FindGameObjectWithTag("World") == null)
        {
            return;
        }
        else
        {
            RpcStartPlacement();
            SetGameMode(2);
        }
    }

    [ClientRpc]
    public void RpcResetGame()
    {
        //Turn on placement UI and reset the gold;
        TogglePlacementUI(true);
        mUnitPlacementController.ResetPlacement();

        //Tell Battle controller not to fight each other
        if (isServer)
            mBattleController.StartBattle(false);

        //Tell the game controller what stage its in
        SetGameMode(2);
        isMode3 = false;
        SetPlayerReady(-2);

        //Tell the unitcontroller to to reset the unit lists
        mUnitController.DestroyLists();

        //UnitControlScript
        mUnitControlScript.SetMode(2);

        //Turn off EndGame UI
        mNewGameUIController.gameObject.SetActive(false);

        //Set Snackbar msg
        GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("Pick a side and place your units with gold. Press Ready to start game.");

    }

    [ClientRpc]
    public void RpcStartGame()
    {
        //Make World disapear
        GameObject.Find("World").GetComponent<Renderer>().enabled = false;

        GameObject mWorld = GameObject.Find("World");
        foreach (Transform child in mWorld.transform)
        {
            if (child.name == "Wall")
            {
                child.GetComponent<WallDespawner>().StartFalling();
            } else if (child.name == "player1Spawn" || child.name == "player2Spawn")
            {
                child.gameObject.SetActive(false);
            }
        }

        //Change colors of units
        mUnitController.SetUnitColors();

        //Tell Unit controller it can now let user move units
        UnitControlScript CS = GameObject.Find("UnitController").GetComponent<UnitControlScript>();
        CS.SetMode(3);

        //Tell the Battle controller to start
        if (isServer)
        {
            BattleControlScript BS = GameObject.Find("BattleController").GetComponent<BattleControlScript>();
            BS.StartBattle(true);
        }

    }

    [ClientRpc]
    public void RpcStartPlacement()
    {       
        //Disable AR features
        GameObject w = GameObject.Find("ARCore World Origin Helper");
        ARCoreWorldOriginHelper helperScript = w.GetComponent<ARCoreWorldOriginHelper>();
        helperScript.SetNoPlanes(true);
        //Pointcloud
        GameObject pc = GameObject.Find("Point Cloud");
        pc.SetActive(false);
        GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
        foreach (GameObject m in meshes)
        {
            m.SetActive(false);
        }
        GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("Pick a side and place your units with gold. Press Ready to start game.");

    }

    void ResetMapPlacement()
    {
        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerController pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerController>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdDespawnWorld(false);
        }
    }

    public void SetGameMode(int mode)
    {
        mGameMode = mode;
    }

    public int GetGameMode()
    {
        return mGameMode;
    }


    public void SetPlayerReady(int i)
    {
        mPlayerReady += i;
    }

    public int GetPlayerReady()
    {
        return mPlayerReady;
    }
}
