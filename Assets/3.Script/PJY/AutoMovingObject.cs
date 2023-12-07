using UnityEngine;
using Mirror;

public class AutoMovingObject : NetworkBehaviour
{
    public float moveRange = 5f; // �̵� ����
    public float moveSpeed = 2f; // �̵� �ӵ�
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkIdentity>())
            gameObject.AddComponent<NetworkIdentity>();

        InvokeRepeating("SetTargetPosition", 0f, 5f); // ���� �ֱ⸶�� ��ǥ ��ġ ����
    }

    void SetTargetPosition()
    {
        // ���� ��ġ���� ���� ���� ���� ���� ���� ���ο� ��ġ ����
        targetPosition = transform.position + new Vector3(Random.Range(-moveRange, moveRange), 0f, 0f);

        // �浹 �˻�
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, moveRange))
        {
            // �浹�� �����Ǹ� ��ǥ ��ġ�� �浹 �������� ����
            targetPosition = hit.point;
        }

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
        // ������ ���� �ε巯�� �̵�
        float t = (Time.time - startTime) / moveSpeed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
    }
}
