using UnityEngine;
using Mirror;

public class AutoMovingObject : NetworkBehaviour
{
    public float moveRange = 5f; // �̵� ����
    public float moveSpeed = 2f; // �̵� �ӵ�
    public float minStopTime = 3f; // �̵� ���� �ּ� �ð�
    public float maxStopTime = 5f; // �̵� ���� �ִ� �ð�
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float startTime;
    private bool willTurn = false; // ���� ��ȯ ����
    private float stopStartTime; // �̵� ���� ���� �ð�

    private bool isColliding = false; // �浹 ���θ� ��Ÿ���� ����

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkIdentity>())
            gameObject.AddComponent<NetworkIdentity>();

        InvokeRepeating("SetTargetPosition", 0f, Random.Range(minStopTime, maxStopTime)); // ���� �ð����� ��ǥ ��ġ ����
    }

    void SetTargetPosition()
    {
        // ���� ��ȯ
        willTurn = !willTurn;

        // ���� ��ġ���� ���� ���� ���� ���� ���� ���ο� ��ġ ����
        float newX = willTurn ? 0f : Random.Range(-moveRange, moveRange);
        float newZ = Random.Range(-moveRange, moveRange);
        targetPosition = transform.position + new Vector3(newX, 0f, newZ);

        // ��Ʈ��ũ �󿡼� ��ġ ������ ����ȭ
        RpcSetTargetPosition(targetPosition);
    }

    [ClientRpc]
    void RpcSetTargetPosition(Vector3 newPosition)
    {
        // Ŭ���̾�Ʈ������ �����κ��� ���޹��� ��ġ�� ������ �̵�
        startPosition = transform.position;
        startTime = Time.time;
        targetPosition = newPosition;
    }

    void Update()
    {
        if (Time.time - startTime > Random.Range(minStopTime, maxStopTime))
        {
            // ���ߴ� �ð��� ������ ���ο� ��ġ�� �̵� ����
            SetTargetPosition();
            return;
        }

        // ������ ���� �ε巯�� �̵�
        float t = (Time.time - startTime) / moveSpeed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        if (isColliding)
        {
            // �浹 �߿��� �̵� ����
            transform.position = startPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Will"))
        {
            // Will �±׸� ���� ������Ʈ�� �浹 �� isColliding ������ Ȱ��ȭ�Ͽ� �̵��� ����ϴ�.
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Will"))
        {
            // Will �±׸� ���� ������Ʈ�� �浹�� ���������� �̵��� �簳�մϴ�.
            isColliding = false;
        }
    }
}
