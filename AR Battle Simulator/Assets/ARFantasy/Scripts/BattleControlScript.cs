using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class BattleControlScript : NetworkBehaviour
{ 
    private MovementControlScript mMovement;
    private BlobProperties mBlobProperties;
    private AttackScript mAttack;
    private List<GameObject> mFirstObjList = new List<GameObject>();
    private List<GameObject> mSecondObjList = new List<GameObject>();

    private bool isGameStarted = false;
    private int mUpdateThrottle = 0;

    public void SetUnitLists()
    {
        UnitController UC = GameObject.Find("UnitController").GetComponent<UnitController>();

        mFirstObjList = UC.GetPlayerOneUnits();
        mSecondObjList = UC.GetPlayerTwoUnits();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameStarted)
            return;

        mUpdateThrottle++;
        if (mUpdateThrottle <= 30)
        {
            return;
        }
        mUpdateThrottle = 0;

        ScanUnits1();
        ScanUnits2();

    }

    void ScanUnits1()
    {
        foreach( GameObject unit in mFirstObjList)
        {
            mMovement = unit.GetComponent<MovementControlScript>();
            mBlobProperties = unit.GetComponent<BlobProperties>();
            mAttack = unit.GetComponent<AttackScript>();

            //If the target is currently engaged, dont change their orders
            if (mAttack.GetEngage())
                return;

            //Something is in attack range. Fight it
            if (mBlobProperties.GetEnemyInRange().Count > 0)
            {
                foreach(GameObject obj in mBlobProperties.GetEnemyInRange())
                {
                    if (obj.GetComponent<BasicUnitScript>().GetOwnerID() != unit.GetComponent<BasicUnitScript>().GetOwnerID())
                    {
                        mAttack.SetAttackTarget(obj);
                        mMovement.AttackUnit(obj);
                        break;
                    }
                }


            }
            else
                mAttack.SetEngage(false);
        }

    }

    void ScanUnits2()
    {

        foreach (GameObject unit in mSecondObjList)
        {
            mMovement = unit.GetComponent<MovementControlScript>();
            mBlobProperties = unit.GetComponent<BlobProperties>();
            mAttack = unit.GetComponent<AttackScript>();

            //If the target is currently engaged, dont change their orders
            if (mAttack.GetEngage())
               return;

            //Something is in attack range. Fight it
            if (mBlobProperties.GetEnemyInRange().Count > 0)
            {
                foreach (GameObject obj in mBlobProperties.GetEnemyInRange())
                {
                    if (obj.GetComponent<BasicUnitScript>().GetOwnerID() != unit.GetComponent<BasicUnitScript>().GetOwnerID())
                    {
                        mAttack.SetAttackTarget(obj);
                        mMovement.AttackUnit(obj);
                        break;
                    }
                }
            }
            else
                mAttack.SetEngage(false);
        }
    }

    public void StartBattle(bool mode)
    {
        isGameStarted = mode;
        if (mode)
            SetUnitLists();
    }
}
