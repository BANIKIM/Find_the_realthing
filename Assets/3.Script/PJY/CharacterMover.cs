using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMover : NetworkBehaviour
{
    public bool isMoveable;

    [SyncVar]
    [SerializeField] private float lookSensitivity;
    [SyncVar]
    [SerializeField] private float cameraRotationLimit;

    private float currentCameraRotationX = 0;
    private Camera theCamera;

    private Rigidbody playerRigid;

    [SyncVar]
    public float walkSpeed = 2f;

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        // 캡슐 콜라이더의 경우 중심을 적절히 조절해야 할 수 있습니다.

        if (hasAuthority)
        {
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 5f, -9f);
            cam.orthographicSize = 2.5f;
        }
     


    }

    private void FixedUpdate()
    {
        Move();
        //CameraRotation();
        PlayerRotation();
    }

    private void Move()
    {
        if (hasAuthority && isMoveable)
        {
            
            
                float _moveDirX = Input.GetAxisRaw("Horizontal");
                float _moveDirZ = Input.GetAxisRaw("Vertical");

                Vector3 _moveHorizontal = transform.right * _moveDirX;
                Vector3 _moveVertical = transform.forward * _moveDirZ;

                Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

                playerRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

                //(0,0,1)+(1,0,0)
                //(1,0,1)=2
                //(0.5,0,0.5)=1
            
        }
    }
/*    private void CameraRotation()
    {
        //상하 카메라 회전

        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;                    //-45                  45
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }*/
    private void PlayerRotation()
    {
        if (hasAuthority && isMoveable)
        {
            //좌우 플레이어 회전 
            float _yRotation = Input.GetAxisRaw("Mouse X");

            Vector3 _playerRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            playerRigid.MoveRotation(playerRigid.rotation * Quaternion.Euler(_playerRotationY));
        }
    }
}
