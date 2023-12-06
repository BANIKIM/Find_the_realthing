using UnityEngine;
using UnityEngine.AI;

public class AI_Move : MonoBehaviour
{
    [SerializeField] private Transform[] TargetPos;
    private Transform Target = null;

    private Animator ani;
    private NavMeshAgent navMesh;

    private bool isMove = false;
    private float Distance = 3f;          // Å¸°Ù°úÀÇ °Å¸®

    private void Start()
    {
        TryGetComponent<Animator>(out ani);
        TryGetComponent<NavMeshAgent>(out navMesh);
    }

    public void Move()
    {
        if (Target != null && isMove)
        {
            navMesh.SetDestination(Target.position);

            if (isArrive() && isMove)
            {
                ani.SetBool("isRun", false);
                isMove = false;

                int Temp = Random.Range(0, 5);
                if (Temp.Equals(4)) Invoke("ResetTarget", Temp + 2);
                else ResetTarget();
            }
        }
        else if (Target == null && !isMove) TargetSetting();
    }

    private void ResetTarget()
    {
        Target = null;
    }
    private void TargetSetting()
    {
        Target = TargetPos[Random.Range(0, TargetPos.Length)];           // ·£´ý Å¸°Ù¼³Á¤

        ani.SetBool("isRun", true);
        isMove = true;
    }

    private bool isArrive()        // Å¸°Ù ÁÖº¯¿¡ ÀÖ´ÂÁö
    {
        return (Vector3.Magnitude(transform.position - Target.position) < Distance) ? true : false;
    }

}
