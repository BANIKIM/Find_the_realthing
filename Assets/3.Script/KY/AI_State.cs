using UnityEngine;

public class AI_State : MonoBehaviour
{
    private AI_Move ai_Move;
    public Animator animator;

    private bool isDie = false;     // �׾�����

    private void Awake()
    {
        TryGetComponent<AI_Move>(out ai_Move);
       
        animator = transform.GetChild(0).GetComponent<Animator>();
       // TryGetComponent<Animator>(out animator);
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
        if(other.gameObject.CompareTag("Attack") && !isDie)
        {
            OnDie();    // �÷��̾�� �¾��� �� OnDie �޼ҵ� ȣ��
        }
            
    }

    private void OnDie()
    {
        isDie = true;
        animator.SetTrigger("isDie");
        Rigidbody rig = GetComponent<Rigidbody>();
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        rig.isKinematic = true;
        cap.isTrigger = true;
    }
}
