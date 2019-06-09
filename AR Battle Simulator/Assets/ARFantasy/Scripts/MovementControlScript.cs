using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class MovementControlScript : NetworkBehaviour
{
    public float mSpeed = 0.04f;
    public float mTrackDistance = 0.19f;

    [SyncVar(hook = "OnIdleChange")]
    private bool isIdle = true;
    void OnIdleChange(bool mode) { isIdle = mode; }

    [SyncVar(hook = "OnFleeingChange")]
    private bool isFleeing = false;
    void OnFleeingChange(bool mode) { isFleeing = mode; }

    [SyncVar(hook = "OnHaltedChange")]
    private bool isHalted = false;
    void OnHaltedChange(bool mode) { isHalted = mode; }

    [SyncVar(hook = "OnAttackingChange")]
    private bool isAttacking = false;
    void OnAttackingChange(bool mode) { isAttacking = mode; }

    [SyncVar(hook = "OnTrackingChange")]
    private bool isTrackingUnit = false;
    void OnTrackingChange(bool mode) { isTrackingUnit = mode; }



    private int mUnitTrackingTimer = 0;
    private GameObject mCurrentTrackingTarget;
    private Animator mAnim;
    private Vector3 mCurrentGoal;
    private GameObject[] mChildArray;

    private float mDistanceToTarget;
    private bool isEngaged = false;

    GoogleARCore.Examples.CloudAnchors.NetworkManagerUIController Snackbar;

    void Start()
    {
        Snackbar = GameObject.Find("Network Manager").GetComponent<GoogleARCore.Examples.CloudAnchors.NetworkManagerUIController>();
    }

    void Update()
    {
       // if (mCurrentTrackingTarget != null)
       // {
       //     mDistanceToTarget = (gameObject.transform.position - mCurrentTrackingTarget.transform.position).magnitude;
       //     Snackbar.ShowErrorMessage("TargetOwnerID: " + mCurrentTrackingTarget.GetComponent<BasicUnitScript>().GetOwnerID() + " ID: " + mCurrentTrackingTarget.GetComponent<BasicUnitScript>().GetUnitID()
       //         + " Distance: " + mDistanceToTarget + " CurPos: " + gameObject.transform.position + " OtherPos: " + mCurrentTrackingTarget.transform.position);
       // }
        UpdatePosition();
    }

    public float GetSpeed() { return mSpeed; }

    public void SetChildArray(GameObject[] arr)
    {
        mChildArray = arr;
    }

    [ClientRpc]
    void RpcSetMovingAnim(bool mode)
    {
        foreach (GameObject c in mChildArray)
        {
            if (c == null)
                return;

            mAnim = c.GetComponent<Animator>();
            mAnim.SetBool("isMoving", mode);
        }
    }

    [ClientRpc]
    void RpcSetAttackingAnim(bool mode)
    {
        foreach (GameObject c in mChildArray)
        {
            if (c == null)
                return;
            mAnim = c.GetComponent<Animator>();
            mAnim.SetBool("isAttacking", mode);
        }
    }

    //public void SetTrackDistance(float n) { mTrackDistance = n; }

    public void Move(Vector3 pos)
    {
        mCurrentTrackingTarget = null;
        mCurrentGoal = pos;
        isIdle = false;
        isFleeing = true;
        isHalted = false;
        isAttacking = false;
        isTrackingUnit = false;
        RpcSetMovingAnim(true);
        RpcSetAttackingAnim(false);
    }

    public void Halt()
    {
        mCurrentTrackingTarget = null;
        mCurrentGoal = transform.position;
        isIdle = true;
        if (isHalted == false)
            isHalted = true;
        else
            isHalted = false;
        isFleeing = false;
        isAttacking = false;
        isTrackingUnit = false;
        RpcSetMovingAnim(false);
    }

    public void Attack(Vector3 pos)
    {
        mCurrentTrackingTarget = null;
        mCurrentGoal = pos;
        isIdle = false;
        isFleeing = false;
        isHalted = false;
        isAttacking = true;
        isTrackingUnit = false;
        RpcSetMovingAnim(true);
    }

    public void AttackUnit(GameObject obj)
    {
        mCurrentGoal = obj.transform.position;
        mCurrentTrackingTarget = obj;
        isIdle = false;
        isFleeing = false;
        isHalted = false;
        isAttacking = true;
        isTrackingUnit = true;
        RpcSetMovingAnim(true);
    }

    void UpdatePosition()
    {
        if (!isServer)
            return;

        if (mCurrentTrackingTarget == null)
        {
            if (isEngaged == true)
            {
                RpcSetAttackingAnim(false);
                RpcSetMovingAnim(false);
                isIdle = true;
            }
            isEngaged = false;

            isTrackingUnit = false;
        }
        if (isIdle == true)
            return;
        if (isTrackingUnit == true)
        {
            if ((mCurrentTrackingTarget.transform.position - gameObject.transform.position).magnitude <= mTrackDistance)
            {
                if (isEngaged == false)
                {
                    RpcSetAttackingAnim(true);
                    RpcSetMovingAnim(false);
                }
                isEngaged = true;
                return;
            }
            else
            {
                if (isEngaged == true)
                {
                    RpcSetAttackingAnim(false);
                    RpcSetMovingAnim(true);
                }
                isEngaged = false;
                mCurrentGoal = mCurrentTrackingTarget.transform.position;
                TakeStep();
                return;
            }

        }
        else if ((transform.position - mCurrentGoal).magnitude <= 0.01)
        {
            isIdle = true;
            isAttacking = false;
            isFleeing = false;
            isTrackingUnit = false;

            RpcSetMovingAnim(false);
            return;
        }
        else
            TakeStep();
    }
    void TakeStep()
    {
        float step = mSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mCurrentGoal, step);
    }

    public void SetFleeing(bool mode) { isFleeing = mode; }
    public bool GetFleeing() { return isFleeing; }
    public void SetHalted(bool mode) { isHalted = mode; }
    public bool GetHalted() { return isHalted; }
    public void SetTracking(bool mode) { isTrackingUnit = mode; }
    public bool GetTracking() { return isTrackingUnit; }
    public bool GetAttacking() { return isAttacking; }
}
