using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player_Move : NetworkBehaviour
{
    public Animator anim;
    public GameObject knife;
    private bool isAttack = true;
    private float attackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        knife = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (isOwned)
        {
            OnRun();
            OnJump();
            if (isAttack)
            {
                OnAttack();
            }
            else if (!isAttack)
            {
                attackTime += Time.deltaTime;
                if (attackTime > 2f)
                {
                    isAttack = true;
                    attackTime = 0f;
                }
            }
        }
    }

    private void OnRun()
    {
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isRun", true);
        }
        else
        {
            anim.SetBool("isRun", false);
        }
    }

    private void OnJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("isJump");
        }
    }

    private void OnAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CmdAttack();
        }
    }

    [Command]
    private void CmdAttack()
    {
        RpcDoAttack();
    }

    [ClientRpc]
    private void RpcDoAttack()
    {
        knife.transform.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        isAttack = false;
        goAttack();
        Invoke("disAttack", 1.5f);
    }

    private void goAttack()
    {
        anim.SetTrigger("isAttack");
    }

    private void disAttack()
    {
        knife.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }
}
