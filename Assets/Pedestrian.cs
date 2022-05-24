using UnityEngine;
using UnityEngine.AI;

public class Pedestrian : MonoBehaviour
{
    NavMeshAgent Ag;
    public Transform[] NPCPoints;
    int r;
    private void Start()
    {
        Ag = GetComponent<NavMeshAgent>();
        r = Random.Range(0, 7);
        Ag.SetDestination(NPCPoints[r].position);
    }

    void ReLoad()
    {
       r = Random.Range(0, 7);
        Ag.SetDestination(NPCPoints[r].position);
    }
    private void Update()
    {
        Debug.Log(Ag.destination);
        if (Vector3.Distance(NPCPoints[r].position, Ag.transform.position) <= Ag.stoppingDistance)
        {
            if (!Ag.hasPath || Ag.velocity.sqrMagnitude == 0f)
            {
                ReLoad();
            }
        }
    }

     
}
