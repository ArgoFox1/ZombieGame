using UnityEngine;
using UnityEngine.AI;
public class Liner : MonoBehaviour // gamae managerdan buytime olup olmadýðýný bilgisini al
{
    public GameManager gameManager;
    public Transform startPoint;
    public NavMeshAgent agent;
    public GameObject player;
    private float time;
    private void Update()
    {
        time = gameManager.GetBuyTime(time);
        if(time > 0f)
        {
            agent.destination = player.transform.position;
        }
        else
        {
            agent.destination = startPoint.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            agent.nextPosition = startPoint.position;
        }
    }
}
