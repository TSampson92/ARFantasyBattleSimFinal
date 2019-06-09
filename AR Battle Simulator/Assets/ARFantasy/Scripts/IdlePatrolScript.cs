using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePatrolScript : MonoBehaviour
{
    private Vector3 mOffset;
    private Vector3 mArmyCenter;
    private GameObject mParentObj;
    private float mTimer = 0;
    public float mPatrolDelay = 240;
    public float mIdleSpeed = (float)0.01;
    public float mMoveSpeed = (float)0.04;
    public float mAttackSpeed = 0.1f;
    private Vector3 mPatrolPos;
    private bool isDirectionSet = false;
    private bool isParentSet = false;

    // Start is called before the first frame update
    void Start()
    {
        mArmyCenter = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isParentSet)
            return;
        if (isParentSet && mParentObj == null)
        {
            Destroy(gameObject);
            return;
        }

        //If not in idle animation
        Animator anim = gameObject.GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            //My attempt to improve network transform
            FollowTheParent();
            return;
        }

        if (mParentObj.name == "Mage" && anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
        {
            AttackMovement();
            return;
        }

        if (mTimer >= mPatrolDelay)
        {
            isDirectionSet = true;
            mTimer = 0;
            SetNewDirection();
        }
        mTimer += Random.Range(0, 3);

        if (isDirectionSet == true)
            MoveToPatrol();
    }

    void AttackMovement()
    {
        if (mTimer >= mPatrolDelay)
        {
            isDirectionSet = true;
            mTimer = 0;
            SetNewAttackDirection();
        }
        mTimer += Random.Range(7, 10);

        if (isDirectionSet == true)
            MoveToAttack();
    }

    void FollowTheParent()
    {
        
        mArmyCenter = mParentObj.transform.position;
        //Expected position for this unit
        Vector3 mExpected = mArmyCenter + mOffset;
        //Current position
        Vector3 mCurrent = gameObject.transform.position;

        if ((mExpected - mCurrent).magnitude > 0.001)
        {
            float step = mMoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, mExpected, step);
        }
    }

    void MoveToAttack()
    {
        float step = mAttackSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mPatrolPos, step);
    }

    void MoveToPatrol()
    {
        float step = mIdleSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mPatrolPos, step);
    }

    void SetNewDirection()
    {
        mArmyCenter = mParentObj.transform.position;
        Vector3 mCenter = mArmyCenter + mOffset;
        mPatrolPos = new Vector3(mCenter.x + Random.Range(-0.01f, 0.01f), mCenter.y, mCenter.z + Random.Range(-0.01f, 0.01f));

    }

    void SetNewAttackDirection()
    {
        mArmyCenter = mParentObj.transform.position;
        Vector3 mCenter = mArmyCenter + mOffset;
        mPatrolPos = new Vector3(mCenter.x + Random.Range(-0.03f, 0.03f), mCenter.y + Random.Range(0, 0.01f), mCenter.z + Random.Range(-0.03f, 0.03f));
    }

    public void SetOffset(float x, float y, float z)
    {

        Vector3 tempVec = new Vector3(x, y, z);
        mOffset = tempVec;

    }

    public void SetParentObj (GameObject obj)
    {
        mParentObj = obj;
        mMoveSpeed = mParentObj.GetComponent<MovementControlScript>().GetSpeed();
        isParentSet = true;
    }

    public GameObject GetParentObj()
    {
        return mParentObj;
    }
}
