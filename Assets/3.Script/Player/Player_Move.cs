using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM.Controllers;
using Mirror;

public class Player_Move : NetworkBehaviour
{
    //[SyncVar(hook = nameof(OnAnimationStateUpdated))]
    [SyncVar] private bool isRunning;
    [SyncVar] private bool isJumping;
    [SyncVar] private bool isAtking;

    [SyncVar] Vector3 syncPosition;

    public Animator anim;
    public BaseCharacterController controller;
    [Header("Player")]
    public GameObject knife;
    private bool isAttack = true;
    private float AttackTime = 0f;

    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        anim = GameObject.Find("Player_modle").transform.GetComponent<Animator>(); ;
        controller = FindObjectOfType<BaseCharacterController>();
        knife = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;

        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        bool isRunningLocal = OnRun();
        bool isJumpingLocal = OnJump();
        bool isAtkingLocal = OnAttack();

        if (isRunning != isRunningLocal)
        {
            isRunning = isRunningLocal;
            CmdSetAnimationState(isRunning);
        }
        if (isJumping != isJumpingLocal)
        {
            isJumping = isJumpingLocal;
            CmdSetAnimationState_j(isJumping);
        }
        if (isAtking != isAtkingLocal)
        {
            isAtking = isAtkingLocal;
            CmdSetAnimationState_a(isAtking);
        }

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

    private void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }

    [Command]
    private void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPosition = pos;
    }
    [ClientCallback]
    private void CmdSetAnimationState(bool running)
    {
        isRunning = running;
    }

    [ClientCallback]
    private void CmdSetAnimationState_j(bool jumping)
    {
        isJumping = jumping;
    }
    private void CmdSetAnimationState_a(bool atking)
    {
        isAtking = atking;
    }

    [ClientCallback]
    private void OnAnimationStateUpdated(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            // Set the running animation
            anim.SetBool("IsRun", true);
        }
        else
        {
            // Set the idle animation
            anim.SetBool("IsRun", false);
        }
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvidePositionToServer(transform.position);
        }
    }
    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * 10);
        }
    }
    private bool OnRun()
    {
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isRun", true);
            return true;
        }
        else
        {
            anim.SetBool("isRun", false);
            return false;
        }
    }

    private bool OnJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            anim.SetTrigger("isJump");
            return true;
        }
        else
        {
            anim.SetBool("isJump", false);
            return false;
        }
    }

    private bool OnAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("isAttack");
            knife.transform.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            isAttack = false;
            Invoke("disAttack", 3f);

            return true;
        }
        else
        {
            anim.SetBool("isAttack", false);
            return false;
        }
    }

    private void disAttack()
    {
        knife.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

}
