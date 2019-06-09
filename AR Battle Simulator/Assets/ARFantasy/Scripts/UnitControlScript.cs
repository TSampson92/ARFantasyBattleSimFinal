using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using GoogleARCore.Examples.CloudAnchors;

#pragma warning disable 618
public class UnitControlScript : NetworkBehaviour
{
    //UI objects
    public GameObject mUnitControlCanvas;
    public GameObject mCancelCanvas;
    public Button mAttackButton;
    public Button mMoveButton;
    public Button mHaltButton;
    public Button mCancelButton;
    public Button mSpellButton;

    //Highlight color
    private Color mOldColor;
    private bool isOldColorSet = false;

    //To determine if a UI has been pressed
    private bool isAttackPressed = false;
    private bool isMovePressed = false;
    private bool isSpellPressed = false; 

    //To indicate if the unit currently has orders
    private bool isAttackState = false;
    private bool isMoveState = false;
    private bool isHaltState = false;
    private bool isSpellState = false;

    //script wont run until in gamemode 3 and a bunch of other things used for this 
    private int mGameMode = 0;
    private bool isUnitSelected;
    private GameObject mSelectedUnit;
    private int fingerID = -1;
    GameObject hitObj;
    private NetworkInstanceId mNetID;
    private bool isIDSet = false;
    private RaycastHit hit;
    private NetworkManagerUIController SnackBarUI;
    public GameObject iceRainGameObject;

    private void Awake()
    {
#if !UNITY_EDITOR
        fingerID = 0; 
#endif
    }

    // Start is called before the first frame update
    void Start()
    { 
        ToggleActionUI(true);
        ToggleCancelUI(true);
        mCancelButton.onClick.AddListener(delegate { CancelButtonPressed(); });
        mAttackButton.onClick.AddListener(delegate { AttackButtonPressed(); });
        mMoveButton.onClick.AddListener(delegate { MoveButtonPressed(); });
        mHaltButton.onClick.AddListener(delegate { HaltButtonPressed(); });
        mSpellButton.onClick.AddListener(delegate { SpellButtonPressed(); });
        SnackBarUI = GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>();
        ToggleActionUI(false);
        ToggleCancelUI(false);
    }

    void SetOwnerID()
    {
        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        NetworkIdentity nid;
        foreach (GameObject p in mPlayers)
        {
            nid = p.GetComponent<NetworkIdentity>();
            if (!nid.isLocalPlayer)
                continue;
            mNetID = nid.netId;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If not in game started state, do nothing
        if (mGameMode != 3)
            return;

        if (isIDSet == false)
        {
            isIDSet = true;
            SetOwnerID();
        }

        //Update Button color
        SetButtonColor();
        //Determine Button state
        GetSelectedState();

        //If ray cast hits UI, ignore raycast
        if (EventSystem.current.IsPointerOverGameObject(fingerID))    // is the touch on the GUI
        {
            return;
        }

        //If player hasnt touched the screen, return
        if (Input.touchCount < 1 || Input.GetTouch(0).phase != TouchPhase.Began)
        {
            return;
        }

        //Something is Selected
        if (mSelectedUnit != null)
        {
            GetRaycast();

            if (isAttackPressed == true)
            {

                //Move to world position
                if (hitObj.name == "World")
                {
                    MeleeAttackGround(hit.point);
                    isAttackPressed = false;
                    isMovePressed = false;
                }

                //Attacking own unit, dont do anything
                if (hitObj.GetComponent<BasicUnitScript>().GetOwnerID() == mNetID)
                    return;

                //Attacking enemy unit.
                //if (hitObj.GetComponent<BasicUnitScript>().GetOwnerID() != mNetID)
                else
                {
                    MeleeAttackUnit(hitObj);
                    isAttackPressed = false;
                    isMovePressed = false;
                }
            }
            else if (isMovePressed == true)
            {

                //Move to world position
                if (hitObj.name == "World")
                {
                    MeleeMove(hit.point);
                    isAttackPressed = false;
                    isMovePressed = false;
                }
                //moved onto a enemy unit, uses that units position.
                if (hitObj.name != "World")
                {
                    MeleeMove(hitObj.transform.position);
                    isAttackPressed = false;
                    isMovePressed = false;
                }
            }
            else if (isSpellPressed == true) //Need to make sure it only works with wizard units in future
            {
                //Drop ice rain on position
                if (hitObj.name == "World")
                {
                    IceRain(iceRainGameObject, hit.point);
                    isAttackPressed = false;
                    isMovePressed = false;
                }
                //Location is enemy unit, uses that units position.
                if (hitObj.name != "World")
                {
                    IceRain(iceRainGameObject, hitObj.transform.position);
                    isAttackPressed = false;
                    isMovePressed = false;
                }
            }
            //Neither action button was pressed, this raycast determines new selected unit
            else
            {
                RemoveHighLightColor();
                ProcessSelectedUnit();           
            }

            return;
        }

        //Nothing selected Start of state tree
        GetRaycast();

        //A click occured, but no action was requested
        ProcessSelectedUnit();
    }

    void ProcessSelectedUnit()
    {

        //if hit obj is world, dont set selected
        if (hitObj.name == "World")
        {
            RemoveHighLightColor();
            mSelectedUnit = null;
            ToggleActionUI(false);
            return;
        }

        //Ally unit
        if (hitObj.GetComponent<BasicUnitScript>().GetOwnerID() == mNetID)
        {
            //Resets the button checks
            isAttackPressed = false;
            isMovePressed = false;
            isSpellPressed = false; 

            //Changes selected unit and turns on the UI
            mSelectedUnit = hitObj;
            ToggleActionUI(true);

            //If unit is a wizard turn spell button on, else turn it off
           // if (mSelectedUnit.name.Contains("Wizard"))
            //    mSpellButton.gameObject.SetActive(true);
           // else
            //    mSpellButton.gameObject.SetActive(false);


            //Input a form of indicator here? light or something?
            SetHighLightColor();
        }

        //Enemy unit
        else
        {
            //Indicator / ui for enemy unit when the time comes
            //For now it treats it the same as clicking the ground
            RemoveHighLightColor();
            mSelectedUnit = null;
            ToggleActionUI(false);
            return;

        }
    }

    void RemoveHighLightColor()
    {
        GameObject[] mChildArray = mSelectedUnit.GetComponent<LifeControl>().GetChildArray();
        foreach (GameObject child in mChildArray)
        {
            if (child == null)
                continue;
            foreach (Transform c in child.transform)
            {
                if (c.tag == "Color")
                {
                    c.GetComponent<Renderer>().material.color = mOldColor;
                }
            }
        }
    }

    void SetHighLightColor()
    {
        GameObject[] mChildArray = mSelectedUnit.GetComponent<LifeControl>().GetChildArray();
        foreach (GameObject child in mChildArray)
        {
            foreach (Transform c in child.transform)
            {
                if (c.tag == "Color")
                {
                    if (isOldColorSet == false)
                    {
                        isOldColorSet = true;
                        mOldColor = c.GetComponent<Renderer>().material.color;
                    }
                    c.GetComponent<Renderer>().material.color = new Color((float)220/255, (float)220/255, (float)40/255);
                }
            }
        }
    }

    public void AttackButtonPressed()
    {
        isAttackPressed = true;
        isMovePressed = false;
        isSpellPressed = false;
        ToggleActionUI(false);
        ToggleCancelUI(true);
    }

    public void SpellButtonPressed()
    {
        isAttackPressed = false;
        isMovePressed = false;
        isSpellPressed = true;
        ToggleActionUI(false);
        ToggleCancelUI(true);
    }

    public void MoveButtonPressed()
    {
        isAttackPressed = false;
        isMovePressed = true;
        isSpellPressed = false;
        ToggleActionUI(false);
        ToggleCancelUI(true);
    }

    public void HaltButtonPressed()
    {
        isAttackPressed = false;
        isMovePressed = false;
        isSpellPressed = false;
        MeleeHalt();
    }

    public void CancelButtonPressed()
    {
        isAttackPressed = false;
        isMovePressed = false;
        isSpellPressed = false;
        ToggleActionUI(true);
        ToggleCancelUI(false);
    }

    void GetSelectedState()
    {
        if (mSelectedUnit == null)
            return;
        MovementControlScript state = mSelectedUnit.GetComponent<MovementControlScript>();
        isAttackState = state.GetAttacking();
        isMoveState = state.GetFleeing();
        isHaltState = state.GetHalted();
        //Need one of these for spells?
    }

    void SetButtonColor()
    {
        //if ui is active, show button states by changing color of button.
        if (mUnitControlCanvas.activeSelf == true)
        {
            if (isAttackState == true)
            {
                mAttackButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                mAttackButton.GetComponent<Image>().color = Color.white;
            }
            if (isMoveState == true)
            {
                mMoveButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                mMoveButton.GetComponent<Image>().color = Color.white;
            }
            if (isHaltState == true)
            {
                mHaltButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                mHaltButton.GetComponent<Image>().color = Color.white;
            }

        }

    }

    void GetRaycast()
    {
        int layer_mask = LayerMask.GetMask("Pegmen", "World");
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
        {
            
            hitObj = hit.transform.gameObject;
           // SnackBarUI.ShowErrorMessage("Hit: " + hitObj);
            while (hitObj.transform.parent != null)
                hitObj = hitObj.transform.parent.gameObject;

            //If pegman, get parent node
            if (hitObj.tag == "PegTag")
            {
                hitObj = hitObj.GetComponent<IdlePatrolScript>().GetParentObj();
            }

        }
    }

    void ToggleActionUI(bool mode)
    {
        mUnitControlCanvas.SetActive(mode);
        if (mode == true)
            GetSelectedState();
    }

    void ToggleCancelUI(bool mode)
    {
        mCancelCanvas.SetActive(mode);
    }

    void MeleeMove(Vector3 pos)
    {
        ToggleActionUI(true);
        ToggleCancelUI(false);

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdMeleeMove(mSelectedUnit, pos);
        }
    }

    //Need to change for spell animation (currently copy of meleemove)
    void IceRain(GameObject spell, Vector3 pos)
    {
        ToggleActionUI(true);
        ToggleCancelUI(false);

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            //pc.CmdWizardSpell(spell, pos);
        }
    }

    void MeleeHalt()
    {
        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdMeleeHalt(mSelectedUnit);
        }

    }

    void MeleeAttackGround(Vector3 pos)
    {
        ToggleActionUI(true);
        ToggleCancelUI(false);

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdMeleeAttackGround(mSelectedUnit, pos);
        }
    }

    void MeleeAttackUnit(GameObject obj)
    {
        ToggleActionUI(true);
        ToggleCancelUI(false);

        GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
        LocalPlayerControlScript pc;
        foreach (GameObject p in mPlayers)
        {
            pc = p.GetComponent<LocalPlayerControlScript>();
            if (!pc.isLocalPlayer)
                continue;
            pc.CmdMeleeAttackUnit(mSelectedUnit, obj);
        }
    }

    public void SetMode(int mode)
    {
        mGameMode = mode;
        if (mode == 3)
            SnackBarUI.ShowErrorMessage("Tap Unit - Select Order - Tap again on World/Enemy");
    }
}