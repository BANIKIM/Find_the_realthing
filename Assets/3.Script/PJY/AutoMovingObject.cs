using UnityEngine;
using Mirror;

public class AutoMovingObject : NetworkBehaviour
{
    public float moveRange = 5f; // 이동 범위
    public float moveSpeed = 2f; // 이동 속도
    public float minStopTime = 3f; // 이동 멈춤 최소 시간
    public float maxStopTime = 5f; // 이동 멈춤 최대 시간
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float startTime;
    private bool willTurn = false; // 방향 전환 여부
    private float stopStartTime; // 이동 멈춤 시작 시간

    private bool isColliding = false; // 충돌 여부를 나타내는 변수

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkIdentity>())
            gameObject.AddComponent<NetworkIdentity>();

        InvokeRepeating("SetTargetPosition", 0f, Random.Range(minStopTime, maxStopTime)); // 랜덤 시간마다 목표 위치 변경
    }

    void SetTargetPosition()
    {
        // 방향 전환
        willTurn = !willTurn;

        // 현재 위치에서 랜덤 범위 내의 값을 더해 새로운 위치 설정
        float newX = willTurn ? 0f : Random.Range(-moveRange, moveRange);
        float newZ = Random.Range(-moveRange, moveRange);
        targetPosition = transform.position + new Vector3(newX, 0f, newZ);

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
        if (Time.time - startTime > Random.Range(minStopTime, maxStopTime))
        {
            // 멈추는 시간이 지나면 새로운 위치로 이동 시작
            SetTargetPosition();
            return;
        }

        // 보간을 통한 부드러운 이동
        float t = (Time.time - startTime) / moveSpeed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        if (isColliding)
        {
            // 충돌 중에는 이동 멈춤
            transform.position = startPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Will"))
        {
            // Will 태그를 가진 오브젝트와 충돌 시 isColliding 변수를 활성화하여 이동을 멈춥니다.
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Will"))
        {
            // Will 태그를 가진 오브젝트와 충돌을 빠져나가면 이동을 재개합니다.
            isColliding = false;
        }
    }
}
