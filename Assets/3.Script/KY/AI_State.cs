using UnityEngine;
using UnityEngine.AI;

public class AI_State : MonoBehaviour
{
    public AI_Move ai_Move;
    public Animator animator;

    private bool isDie = false;     // 죽었는지
    //네비메쉬
    private NavMeshAgent nav;

    private void Awake()
    {
        TryGetComponent<AI_Move>(out ai_Move);
        //ai_Move = transform.GetChild(0).GetComponent<AI_Move>();//1207 mhkim 추가
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
            OnDie();    // 플레이어에게 맞았을 때 OnDie 메소드 호출
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Attack") && !isDie)
        {
            Debug.Log("스테이");
        }
    }

    private void OnDie()
    {
        isDie = true;
        animator.SetTrigger("isDie");
        //재윤님 코드
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // 죽을 때 y축이 바뀌면 땅에 묻혀서 y축 고정
        Rigidbody rig = GetComponent<Rigidbody>();
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        NavmeshStop();
        rig.isKinematic = true;
        cap.isTrigger = true;
    }

    private void NavmeshStop()//재윤님 메서드
    {
        // don't slide
        nav.isStopped = true;
        nav.updatePosition = false;
        nav.updateRotation = false;
        nav.velocity = Vector3.zero;
    }
}
