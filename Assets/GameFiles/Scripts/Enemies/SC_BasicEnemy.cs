using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SC_BasicEnemy : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float detectionRange = 10f; // Distancia para detectar al jugador
    public float patrolRange = 5f; // Área alrededor del punto de origen donde patrulla
    public float returnSpeed = 3.5f; // Velocidad al regresar al origen

    private NavMeshAgent agent;
    private Vector3 originPoint;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originPoint = transform.position; // Guardar el punto de origen
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            StartChasing();
        }
        else if (isChasing && distanceToPlayer > detectionRange)
        {
            StopChasing();
        }
        else if (!isChasing)
        {
            Patrol();
        }
    }

    void StartChasing()
    {
        isChasing = true;
        agent.speed = 5f; // Aumentar velocidad si está persiguiendo
        agent.SetDestination(player.position); // Perseguir al jugador
    }

    void StopChasing()
    {
        isChasing = false;
        agent.speed = returnSpeed; // Velocidad al regresar
        agent.SetDestination(originPoint); // Regresa al punto de origen
    }

    void Patrol()
    {
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomPoint = originPoint + new Vector3(
                Random.Range(-patrolRange, patrolRange),
                0,
                Random.Range(-patrolRange, patrolRange)
            );
            agent.SetDestination(randomPoint); // Selecciona un punto aleatorio para patrullar
        }
    }
}
