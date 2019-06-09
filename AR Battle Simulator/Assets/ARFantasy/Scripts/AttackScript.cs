using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoogleARCore.Examples.CloudAnchors;

#pragma warning disable 618
public class AttackScript : NetworkBehaviour
{
    [Header("Unit Power")]
    public int Attack_Power;
    public int Defense_Power;
    public int Armor_Type;             //For later use- rock paper modifier

    private Animator mAnim;
    private bool isEngaged = false;
    private GameObject mAttackTarget;
    private GameObject[] mChildArray;
    private MovementControlScript MS;

    private float mDmgThrottle = 0;
    private int mThrottleCap = 60;
    private bool isChildSent = false;

    private bool isIceSpell = false;
    private float iceCooldown = 90;
    private float iceTimer = 0;

    void Start()
    {
        MS = gameObject.GetComponent<MovementControlScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChildSent == false)
            return;

        if (mAttackTarget == null)
        {
            if (isServer)
                if(isEngaged)
                    SetEngage(false);
            return;
        }

        mDmgThrottle++;
        if (mDmgThrottle >= mThrottleCap)
        {
            mDmgThrottle = 0;
            if (isEngaged)
            {
                DealDmg();
            }
            if ((mAttackTarget.transform.position - gameObject.transform.position).magnitude > 0.2)
            {
                mAttackTarget = null;
            }
        }  

        if (isIceSpell == true)
        {
            iceTimer++;
            if (iceTimer > iceCooldown)
            {
                iceTimer = 0;

                    GameObject[] mPlayers = GameObject.FindGameObjectsWithTag("Player");
                    LocalPlayerControlScript pc;
                    foreach (GameObject p in mPlayers)
                    {
                        pc = p.GetComponent<LocalPlayerControlScript>();
                        if (!pc.isLocalPlayer)
                            continue;
                        pc.CmdWizardSpell(mAttackTarget.transform.position);
                    }
                
            }
        }
    }

    public void SetAttackTarget(GameObject obj)
    {
        mAttackTarget = obj;
        SetEngage(true);
    }

    void DealDmg()
    {
            mAttackTarget.GetComponent<LifeControl>().TakeDmg(1);
    }

    void SetDmgThrottle()
    {
        //Default for now .. base this off of this units attack power and the attack targets defenses
        int mTarDef = mAttackTarget.GetComponent<AttackScript>().GetDefense();

        int AttackScore = 0;
        AttackScore -= mTarDef;
        AttackScore += Attack_Power;
        //Attack scoring 
        //-2 == 90
        //-1 == 75
        // 0 == 60
        // 1 == 45
        // 2 == 30
        mThrottleCap = 60 + (-AttackScore * 15);


        //GameObject.Find("Network Manager").GetComponent<NetworkManagerUIController>().ShowErrorMessage("Throttle: " + mThrottleCap);
    }

    public void SetChildArray(GameObject[] arr) { mChildArray = arr; isChildSent = true; }


    [ClientRpc]
    void RpcSetAttackingAnim(bool mode)
    {
        foreach (GameObject c in mChildArray)
        {
            if (c == null)
                return;
            mAnim = c.GetComponent<Animator>();
            mAnim.SetBool("isAttacking", mode);
            mAnim.SetBool("isMoving", !mode);
        }
    }

    public GameObject GetAttackingTarget() { return mAttackTarget; }

    public void SetEngage(bool mode)
    {
        isEngaged = mode;

        if (gameObject.name == "Mage")
        {
            isIceSpell = mode;
        }

        if (mode == true)
            SetDmgThrottle();
    }
    public bool GetEngage() { return isEngaged; }
    public int GetDefense() { return Defense_Power; }
}
