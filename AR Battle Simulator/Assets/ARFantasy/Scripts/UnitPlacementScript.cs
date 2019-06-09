using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using GoogleARCore.Examples.CloudAnchors;

#pragma warning disable 618
public class UnitPlacementScript : NetworkBehaviour
{
    private Button Unit1Button;
    private Button Unit2Button;
    private Button Unit3Button;
    private Button ReadyButton;
    private Text PlayerReadyText;
    private Text PlayerGoldText;
    private int Unit1Cost = 100;
    private int Unit2Cost = 200;
    private int Unit3Cost = 300;
    private int PlayerCurrentGold = 1500;

    private bool isUnit1 = false;
    private bool isUnit2 = false;
    private bool isUnit3 = false;
    private bool isReady = false;
    private int fingerID = -1;
    GameObject hitObj;
    GameController GC;


    private void Awake()
    {
    #if !UNITY_EDITOR
        fingerID = 0; 
    #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        Unit1Button = GameObject.Find("Unit1Button").GetComponent<Button>();
        Unit2Button = GameObject.Find("Unit2Button").GetComponent<Button>();
        Unit3Button = GameObject.Find("Unit3Button").GetComponent<Button>();
        ReadyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        PlayerReadyText = GameObject.Find("PlayersReady").GetComponent<Text>();
        PlayerGoldText = GameObject.Find("GoldToSpend").GetComponent<Text>();
        Unit1Button.onClick.AddListener(delegate { Unit1ButtonPressed(); });
        Unit2Button.onClick.AddListener(delegate { Unit2ButtonPressed(); });
        Unit3Button.onClick.AddListener(delegate { Unit3ButtonPressed(); });
        ReadyButton.onClick.AddListener(delegate { ReadyButtonPressed(); });
       
        GC = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update() {
        UpdateReadyText();

        if (EventSystem.current.IsPointerOverGameObject(fingerID))    // is the touch on the GUI
        {
            return;
        }

        //If player hasnt touched the screen, return
        if (Input.touchCount < 1 || Input.GetTouch(0).phase != TouchPhase.Began)
        {
            return;
        }

        //If no button has been pressed, return;
        if (isUnit1 == false && isUnit2 == false && isUnit3 == false)
        {
            return;
        }

        //Cast a ray from first touch, get the object it hit
        int layer_mask = LayerMask.GetMask("World");
         Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
         RaycastHit hit;
         if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
         {
             hitObj = hit.transform.gameObject;
           
        }

         
        //Can only spawn on correct object, do the check here? or maybe with raycast filter later
        if (hitObj.name != "player1Spawn" && hitObj.name != "player2Spawn")
            return;

       // GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("Object hit is: " + hitObj.name +  " at point: " + hit.point);
        if (isUnit1)
        {
            SpawnUnit1(hit.point, hitObj.transform.rotation);
        }
        else if (isUnit2)
        {
            SpawnUnit2(hit.point, hitObj.transform.rotation);
        }
        else if (isUnit3)
        {
            SpawnUnit3(hit.point, hitObj.transform.rotation);
        }

    }

    public void ResetPlacement()
    {
        PlayerCurrentGold = 1500;
        PlayerGoldText.text = "" + PlayerCurrentGold + "g";
        isReady = false;
        ReadyButton.GetComponent<Image>().color = Color.white;
    }


    void ReadyButtonPressed()
    {
        if (isReady == true)
        {
            ReadyButton.GetComponent<Image>().color = Color.white;
            isReady = false;
            GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
            LocalPlayerControlScript pc;
            foreach (GameObject p in mPlayers)
            {
                pc = p.GetComponent<LocalPlayerControlScript>();
                if (!pc.isLocalPlayer)
                    continue;
                pc.CmdPlayerReady(isReady);
            }
        }
        else
        {
            ReadyButton.GetComponent<Image>().color = Color.red;
            isReady = true;
            GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
            LocalPlayerControlScript pc;
            foreach (GameObject p in mPlayers)
            {
                pc = p.GetComponent<LocalPlayerControlScript>();
                if (!pc.isLocalPlayer)
                    continue;
                pc.CmdPlayerReady(isReady);
            }
        }
    }

    public void SpawnUnit1(Vector3 point, Quaternion rotation)
    {
        PlayerCurrentGold = PlayerCurrentGold - Unit1Cost;
        if (PlayerCurrentGold < 0)
        {
            PlayerCurrentGold += Unit1Cost;
            return;
        }

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
           pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdSpawnUnit1(point, rotation);
        }
        PlayerGoldText.text = "" + PlayerCurrentGold + "g";
    }

    public void SpawnUnit2(Vector3 point, Quaternion rotation)
    {
        PlayerCurrentGold = PlayerCurrentGold - Unit2Cost;
        if (PlayerCurrentGold < 0)
        {
            PlayerCurrentGold += Unit2Cost;
            return;
        }

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdSpawnUnit2(point, rotation);
        }

        PlayerGoldText.text = "" + PlayerCurrentGold + "g";
    }

    public void SpawnUnit3(Vector3 point, Quaternion rotation)
    {
        PlayerCurrentGold = PlayerCurrentGold - Unit3Cost;
        if (PlayerCurrentGold < 0)
        {
            PlayerCurrentGold += Unit3Cost;
            return;
        }

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdSpawnUnit3(point, rotation);
        }

        PlayerGoldText.text = "" + PlayerCurrentGold + "g";
    }

    void Unit1ButtonPressed()
    {
        isUnit1 = true;
        isUnit2 = false;
        isUnit3 = false;
        Unit1Button.GetComponent<Image>().color = Color.red;
        Unit2Button.GetComponent<Image>().color = Color.white;
        Unit3Button.GetComponent<Image>().color = Color.white;
    }

    void Unit2ButtonPressed()
    {
        isUnit1 = false;
        isUnit2 = true;
        isUnit3 = false;
        Unit1Button.GetComponent<Image>().color = Color.white;
        Unit2Button.GetComponent<Image>().color = Color.red;
        Unit3Button.GetComponent<Image>().color = Color.white;
    }

    void Unit3ButtonPressed()
    {
        isUnit1 = false;
        isUnit2 = false;
        isUnit3 = true;
        Unit1Button.GetComponent<Image>().color = Color.white;
        Unit2Button.GetComponent<Image>().color = Color.white;
        Unit3Button.GetComponent<Image>().color = Color.red;
    }

    void UpdateReadyText()
    {
        int num = GC.GetPlayerReady();
        string mText = "Ready: " + num + "/2";
        PlayerReadyText.text = mText;
    }
}
