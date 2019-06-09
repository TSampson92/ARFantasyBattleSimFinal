using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.CloudAnchors;

public class DebugTextScript : MonoBehaviour
{
    public Text mPlayerPOSText;
    public Text mPlayerInfo;
    public Text mPlayerOneUnits;
    public Text mPlayerTwoUnits;

    public GameObject GameControlObj;
    public GameObject UnitControlObj;
    private GameController GC;
    private UnitController UC;

    string mPosText = "";
    string mPlayerInfoText = "";
    string mPlayerOneUnitsText = "";
    string mPlayerTwoUnitsText = "";

    Vector3 mCamPos;
    Vector3 mCamRot;
    
    int playerCount;

    private Button ToggleDebugButton;
    private bool isTextOn = true;


    // Use this for initialization
    void Start()
    {
        ToggleDebugButton = GameObject.Find("ToggleTextButton").GetComponent<Button>();
        ToggleDebugButton.onClick.AddListener(delegate { TurnOffText(); });

        UC = UnitControlObj.GetComponent<UnitController>();
        GC = GameControlObj.GetComponent<GameController>();

        TurnOffText();
    }

    void TurnOffText()
    {
        if (isTextOn == true)
        {
            mPlayerPOSText.gameObject.SetActive(false);
            mPlayerInfo.gameObject.SetActive(false);
            mPlayerOneUnits.gameObject.SetActive(false);
            mPlayerTwoUnits.gameObject.SetActive(false);
            isTextOn = false;            
        }
        else
        {
            mPlayerPOSText.gameObject.SetActive(true);
            mPlayerInfo.gameObject.SetActive(true);
            mPlayerOneUnits.gameObject.SetActive(true);
            mPlayerTwoUnits.gameObject.SetActive(true);
            isTextOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerPosText();
        UpdateDebugText();
        UpdatePlayerOneUnitText();
        UpdatePlayerTwoUnitText();
    }

    void UpdatePlayerOneUnitText()
    {
        if (UC.ListOneCount() == 0)
            return;

        List<GameObject> mUnitList = UC.GetPlayerOneUnits();
        if (mUnitList == null)
            return;
        int unitcount = 0;
        mPlayerOneUnitsText = "Player One's Units \n";
        foreach (var obj in mUnitList)
        {
            BasicUnitScript BS = obj.GetComponent<BasicUnitScript>();
            LifeControl LC = obj.GetComponent<LifeControl>();
            BlobProperties BP = obj.GetComponent<BlobProperties>();

            unitcount++;
            mPlayerOneUnitsText = mPlayerOneUnitsText +
                 "Unit " + unitcount + ": " + BS.GetUnitName() +
                ". OwnerID: " + BS.GetOwnerID() + ". UnitID: " + BS.GetUnitID() + "\n"
                + " HP: " + LC.GetHP() + " Enemeies in Range: " + BP.enemiesInRange.Count + " Enemies in Sight: " + BP.enemiesInSight.Count +" \n";
        }
        mPlayerOneUnits.text = mPlayerOneUnitsText;
    }

    void UpdatePlayerTwoUnitText()
    {
        if (UC.ListTwoCount() == 0)
            return;

        List<GameObject> mUnitList = UC.GetPlayerTwoUnits();
        if (mUnitList == null)
            return;

        int unitcount = 0;
        mPlayerTwoUnitsText = "Player Two's Units \n";
        if (mUnitList.Count == 0)
            return;
        foreach (var obj in mUnitList)
        {
            BasicUnitScript BS = obj.GetComponent<BasicUnitScript>();
            LifeControl LC = obj.GetComponent<LifeControl>();
            BlobProperties BP = obj.GetComponent<BlobProperties>();

            unitcount++;
            mPlayerTwoUnitsText = mPlayerTwoUnitsText +
                     "Unit " + unitcount + ": " + BS.GetUnitName() +
                ". OwnerID: " + BS.GetOwnerID() + ". UnitID: " + BS.GetUnitID() + "\n"
                + " HP: " + LC.GetHP() + " Enemeies in Range: " + BP.enemiesInRange.Count + " Enemies in Sight: " + BP.enemiesInSight.Count + " \n";
        }
        mPlayerTwoUnits.text = mPlayerTwoUnitsText;
    }

    void UpdatePlayerPosText()
    {
        GameObject[] mPlayerCam = GameObject.FindGameObjectsWithTag("MainCamera");
        mPosText = "";
        playerCount = 0;

        foreach(GameObject p in mPlayerCam)
        {
            playerCount++;
            mCamPos = p.transform.position;
            mCamRot = p.transform.rotation.eulerAngles;
            mPosText = mPosText +
                      "Player: " + playerCount + ": Cam Pos: " + mCamPos.x.ToString("0.00") + ", " + mCamPos.y.ToString("0.00") + ", " + mCamPos.z.ToString("0.00") + "\t" +
                       "Cam Rot: " + mCamRot.x.ToString("0.00") + ", " + mCamRot.y.ToString("0.00") + ", " + mCamRot.z.ToString("0.00") + "\n";
        }
        mPlayerPOSText.text = mPosText;
    }

    void UpdateDebugText()
    {
        if (GameObject.Find("LocalPlayer") == null)
            return;
        mPlayerInfoText = "";
        mPlayerInfoText = mPlayerInfoText + "GameMode = " + GC.GetGameMode() + "\n\n";


        GameObject[] mLocalPlayers = GameObject.FindGameObjectsWithTag("Player");
        int count1 = 0;
        foreach (GameObject p in mLocalPlayers) {
            count1++;           
            LocalPlayerController mLocalPlayer = p.GetComponent<LocalPlayerController>();           
            mPlayerInfoText = mPlayerInfoText + "Player NetID = " + mLocalPlayer.GetNetID() + "\n";
            mPlayerInfoText = mPlayerInfoText + "isLocalPlayer = " + mLocalPlayer.isLocalPlayer + "\n";
            mPlayerInfoText = mPlayerInfoText + "isClient = " + mLocalPlayer.isClient + "\n";
            mPlayerInfoText = mPlayerInfoText + "isServer = " + mLocalPlayer.isServer + "\n";
            mPlayerInfoText = mPlayerInfoText + "hasAuthority = " + mLocalPlayer.hasAuthority + "\n\n";
            mPlayerInfo.text = mPlayerInfoText;
        }
        mPlayerInfo.text = mPlayerInfoText;
    }
}