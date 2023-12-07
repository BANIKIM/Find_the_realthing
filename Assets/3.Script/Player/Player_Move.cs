using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM.Controllers;

public class Player_Move : MonoBehaviour
{

    public Animator anim;
    public BaseCharacterController controller;
    [Header("Player")]
    public GameObject knife;
    private bool isAttack = true;
    private float AttackTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GameObject.Find("Player_modle").transform.GetComponent<Animator>(); ;
        controller = FindObjectOfType<BaseCharacterController>();
        knife = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        OnRun();
        OnJump();
        if (isAttack)
        {
            OnAttack();

        }
        else if (!isAttack)
        {
            AttackTime += Time.deltaTime;
            if (AttackTime > 2f)
            {

                isAttack = true;
                AttackTime = 0f;
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
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            anim.SetTrigger("isJump");
        }
    }

    private void OnAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {

            anim.SetTrigger("isAttack");
            knife.transform.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            isAttack = false;
        }
        Invoke("disAttack", 3f);
    }

    private void disAttack()
    {
        knife.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }


}
