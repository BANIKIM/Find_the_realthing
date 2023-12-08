using UnityEngine;
using Mirror;

public class AutoMovingObject : NetworkBehaviour
{
    public float moveRange = 5f; // �̵� ����
    public float minStopTime = 3f; // �̵� ���� �ּ� �ð�
    public float maxStopTime = 5f; // �̵� ���� �ִ� �ð�
    public float walkingSpeed = 4f; // �ȱ� �ӵ�
    private Vector3 targetPosition;
    private float startTime;
    private bool willTurn = false; // ���� ��ȯ ����

    private bool isDie = false;

    private bool isColliding = false; // �浹 ���θ� ��Ÿ���� ����

    private Animator anim;
    private CapsuleCollider coll;
    private Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkIdentity>())
            gameObject.AddComponent<NetworkIdentity>();

        InvokeRepeating("SetTargetPosition", 0f, Random.Range(minStopTime, maxStopTime)); // ���� �ð����� ��ǥ ��ġ ����

        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider>();
        rig = GetComponent<Rigidbody>();
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
        startTime = Time.time;
        targetPosition = newPosition;
    }

    void Update()
    {
        if (isDie) return;
        if (Time.time - startTime > Random.Range(minStopTime, maxStopTime))
        {
            // ���ߴ� �ð��� ������ ���ο� ��ġ�� �̵� ����
            SetTargetPosition();
            return;
        }

        // ������ ���� �ε巯�� �̵�
        float distance = Vector3.Distance(transform.position, targetPosition);
        float t = walkingSpeed * Time.deltaTime / distance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);

        anim.SetBool("isRun", distance > 0.1f);

        if (isColliding)
        {
            // �浹 �߿��� �̵� ����
            transform.position = targetPosition;
            anim.SetBool("isRun", false);
        }
        else
        {
            // �浹���� ���� ���� �̵� ������ �ٶ󺸰� ����
            Vector3 dirToTarget = (targetPosition - transform.position).normalized;
            dirToTarget.y = 0; // ���� ������ y���� 0���� �����Ͽ� ���� ȸ�� ����
            if (dirToTarget != Vector3.zero) // ���� ���Ͱ� 0�� �ƴ� ��쿡�� ȸ��
            {
                Quaternion lookRotation = Quaternion.LookRotation(dirToTarget);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Will"))
        {
            // Will �±׸� ���� ������Ʈ�� �浹 �� isColliding ������ Ȱ��ȭ�Ͽ� �̵��� ����ϴ�.
            isColliding = true;
        }

        if(other.gameObject.CompareTag("Attack") && !isDie)
        {
            anim.SetTrigger("isDie");
            rig.isKinematic = true;
            isDie = true;
            coll.isTrigger = true;
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
