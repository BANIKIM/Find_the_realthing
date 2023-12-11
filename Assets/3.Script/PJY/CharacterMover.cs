using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]

public class CharacterMover : NetworkBehaviour
{
    public float walkingSpeed = 3f;
    public float runningSpeed = 5f;
    public float jumpSpeed = 5f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public GameObject knife;
    private bool isAttack = true;
    private float attackTime = 0f;
    private bool _isRun = false;

    public AudioSource audio; 

    public GameObject[] Win_Effect;
    public GameObject Win_E;

    private GameUI gameUI;

    [Header("Die")]
    public Animator anim;
    public CharacterController coll;
    public Rigidbody rig;
    [SyncVar(hook = nameof(OnDieChanged))]
    private bool isDie = false;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private bool a = false;

    void Start()
    {
        PlayerPrefs.SetString("Win", "승 리");
        characterController = GetComponent<CharacterController>();
        knife = transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;
        gameUI = GameUI.FindObjectOfType<GameUI>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anim = transform.GetChild(1).GetComponent<Animator>();
        coll = transform.GetComponent<CharacterController>();
        rig = transform.GetComponent<Rigidbody>();
        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }
        Win_Effect = new GameObject[4];
        Win_E = transform.GetChild(2).gameObject;
        for (int i = 0; i < Win_Effect.Length; i++)
        {
            Win_Effect[i] = transform.GetChild(2).gameObject.transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        if (!isLocalPlayer )
        {
            return;
        }
        if (isDie) return;
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        if ((curSpeedX != 0f || curSpeedY != 0) && !_isRun)
        {
            _isRun = true;
            CmdSetIsRun(true);
        }
        else if ((curSpeedX == 0f && curSpeedY == 0) && _isRun)
        {
            _isRun = false;
            CmdSetIsRun(false);
        }
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
            anim.SetTrigger("isJump");
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }



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

        //추가부분
        if(gameUI.isGameEnded && !a)
        {
            StartCoroutine(Winer());
        }


    }
    void OnDieChanged(bool oldValue, bool newValue)
    {
        if (newValue && !oldValue)
        {
            anim.SetTrigger("isDie");

            // Disable collision and physics for the character when it dies

            coll.enabled = false;
            rig.isKinematic = true;


        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return; // Only process this on the server

        if (other.CompareTag("Attack") && !isDie)
        {
           // PlayerPrefs.SetString("Win", "패 배");
           
            isDie = true;
            // Set isDie on the server so it gets synchronized to all clients
            CmdDie();
            gameUI.OnPlayerDie();
            
        }
    }

    [Command]
    void CmdDie()
    {
        isDie = true;
        
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

    [Command]
    private void CmdSetIsRun(bool isRunning)
    {
        RpcSetIsRun(isRunning);
    }

    [ClientRpc]
    private void RpcSetIsRun(bool isRunning)
    {
        anim.SetBool("isRun", isRunning);
    }


    IEnumerator Winer()
    {
        
        Win_E.SetActive(true);
        a = true;
        while (gameUI.isGameEnded)
        {
            int random = Random.Range(0, 4);
            Win_Effect[random].SetActive(true);
            audio.PlayOneShot(AudioManager.instance.SFXClip[6]);
            yield return new WaitForSeconds(1.5f);
            for (int i = 0; i < Win_Effect.Length; i++)
            {
                Win_Effect[i].SetActive(false);
            }
            yield return null;
        }
       
    }
}