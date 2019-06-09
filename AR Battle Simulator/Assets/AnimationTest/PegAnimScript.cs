using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PegAnimScript : MonoBehaviour
{

    Animator pegAnim;

    // Start is called before the first frame update
    void Start()
    {
        pegAnim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (pegAnim.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
            {
                pegAnim.SetBool("isMoving", false);
            }
            else
            {
                pegAnim.SetBool("isMoving", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (pegAnim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            {
                pegAnim.SetBool("isAttacking", false);
            } else
            {
                pegAnim.SetBool("isAttacking", true);
            }
        }
    }
    public void SetIsMoving(bool mode)
    {
        pegAnim.SetBool("isMoving", mode);
    }

    public void SetIsAttacking(bool mode)
    {
        pegAnim.SetBool("isAttacking", mode);
    }

}
