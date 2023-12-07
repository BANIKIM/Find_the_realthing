using UnityEngine;
using Mirror;

public class AutoMovingObject : NetworkBehaviour
{
    public float moveRange = 5f; // 이동 범위
    public float moveSpeed = 2f; // 이동 속도
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkIdentity>())
            gameObject.AddComponent<NetworkIdentity>();

        InvokeRepeating("SetTargetPosition", 0f, 5f); // 일정 주기마다 목표 위치 변경
    }

    void SetTargetPosition()
    {
        // 현재 위치에서 랜덤 범위 내의 값을 더해 새로운 위치 설정
        targetPosition = transform.position + new Vector3(Random.Range(-moveRange, moveRange), 0f, 0f);

        // 충돌 검사
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, moveRange))
        {
            // 충돌이 감지되면 목표 위치를 충돌 지점으로 조절
            targetPosition = hit.point;
        }

        // 네트워크 상에서 위치 변경을 동기화
        RpcSetTargetPosition(targetPosition);
    }

    [ClientRpc]
    void RpcSetTargetPosition(Vector3 newPosition)
    {
        // 클라이언트에서는 서버로부터 전달받은 위치로 서서히 이동
        startPosition = transform.position;
        startTime = Time.time;
        targetPosition = newPosition;
    }

    void Update()
    {
        // 보간을 통한 부드러운 이동
        float t = (Time.time - startTime) / moveSpeed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
    }
}
