using UnityEngine;

public class AI_State : MonoBehaviour
{
    private AI_Move ai_Move;
    public Animator animator;

    private bool isDie = false;     // 죽었는지

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
            OnDie();    // 플레이어에게 맞았을 때 OnDie 메소드 호출
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
