using UnityEngine;
using UnityEngine.AI;

public class AI_State : MonoBehaviour
{
    public AI_Move ai_Move;
    public Animator animator;

    private bool isDie = false;     // �׾�����
    //�׺�޽�
    private NavMeshAgent nav;

    private void Awake()
    {
        TryGetComponent<AI_Move>(out ai_Move);
        //ai_Move = transform.GetChild(0).GetComponent<AI_Move>();//1207 mhkim �߰�
        //animator = transform.GetChild(0).GetComponent<Animator>();
         TryGetComponent<Animator>(out animator);
        TryGetComponent(out nav);

    }

    private void Update()
    {
        if (!isDie)
        {
            ai_Move.Move();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attack") && !isDie)
        {
            Debug.Log("dddd");
            OnDie();    // �÷��̾�� �¾��� �� OnDie �޼ҵ� ȣ��
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Attack") && !isDie)
        {
            Debug.Log("������");
        }
    }

    private void OnDie()
    {
        isDie = true;
        animator.SetTrigger("isDie");
        //������ �ڵ�
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // ���� �� y���� �ٲ�� ���� ������ y�� ����
        Rigidbody rig = GetComponent<Rigidbody>();
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        NavmeshStop();
        rig.isKinematic = true;
        cap.isTrigger = true;
    }

    private void NavmeshStop()//������ �޼���
    {
        // don't slide
        nav.isStopped = true;
        nav.updatePosition = false;
        nav.updateRotation = false;
        nav.velocity = Vector3.zero;
    }
}
