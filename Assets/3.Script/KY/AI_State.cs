using UnityEngine;

public class AI_State : MonoBehaviour
{
    private AI_Move ai_Move;
    private Animator animator;

    private bool isDie = false;     // 죽었는지

    private void Awake()
    {
        TryGetComponent<AI_Move>(out ai_Move);
        TryGetComponent<Animator>(out animator);
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
        OnDie();    // 플레이어에게 맞았을 때 OnDie 메소드 호출
    }

    private void OnDie()
    {
        isDie = true;
    }
}
