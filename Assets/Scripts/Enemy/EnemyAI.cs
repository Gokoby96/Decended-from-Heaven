using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    public enum State { WANDER, CHASE, INVESTIGATE, ATTACK }
    public State currentState;

    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;
    public PlayerHealth ph;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    private float wanderCooldown;

    [Header("Movement Speeds")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("FOV Settings")]
    public float viewAngle = 60f;
    public float viewDistance = 15f;
    public LayerMask obstacleMask;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;
    private float attackTimer;

    private Vector3 lastSeenPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.WANDER;
        wanderCooldown = wanderTimer;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.WANDER: WanderState(); break;
            case State.CHASE: ChaseState(); break;
            case State.INVESTIGATE: InvestigateState(); break;
            case State.ATTACK: AttackState(); break;
        }

        FacePlayerBillboard();
    }

    // -------------------- STATE : WANDER ----------------------------
    void WanderState()
    {
        agent.speed = wanderSpeed;

        wanderCooldown -= Time.deltaTime;
        if (wanderCooldown <= 0f)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            wanderCooldown = wanderTimer;
        }

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
        }
    }

    // -------------------- STATE : CHASE ----------------------------
    void ChaseState()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        if (CanSeePlayer())
        {
            lastSeenPosition = player.position;

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= attackRange)
                currentState = State.ATTACK;
        }
        else
        {
            currentState = State.INVESTIGATE;
        }
    }

    // -------------------- STATE : INVESTIGATE ----------------------------
    void InvestigateState()
    {
        agent.speed = wanderSpeed;
        agent.SetDestination(lastSeenPosition);

        float dist = Vector3.Distance(transform.position, lastSeenPosition);

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
        }
        else if (dist <= 1f)
        {
            currentState = State.WANDER;
        }
    }

    // -------------------- STATE : ATTACK ----------------------------
    void AttackState()
    {
        agent.isStopped = true;
        Debug.Log("AttackState çalıştı");

        float dist = Vector3.Distance(transform.position, player.position);

        // Eğer oyuncuyu göremezse
        if (!CanSeePlayer())
        {
            agent.isStopped = false;
            currentState = State.INVESTIGATE;
            return;
        }

        // Oyuncu attack range dışında
        if (dist > attackRange)
        {
            agent.isStopped = false;
            currentState = State.CHASE;
            return;
        }

        
        if (attackTimer <= 0f)
        {
            Debug.Log("Enemy Attacked!");

            
           
            if (ph != null)
            {
                ph.TakeDamage(damage); 
            }

            attackTimer = attackCooldown;
        }
    }

    // -------------------- FOV CHECK ----------------------------
    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        
        Vector3 forwardForFOV = transform.forward;

        // 1) Açıyı kontrol et
        float angle = Vector3.Angle(forwardForFOV, dirToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

        // 2) Raycast
        if (Physics.Raycast(transform.position , dirToPlayer, out RaycastHit hit, viewDistance))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
        }

        return false;
    }

    // -------------------- BILLBOARDING ----------------------------
    void FacePlayerBillboard()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos); // DOOM tarzı sprite
    }

    // -------------------- RANDOM NAV SPHERE ----------------------------
    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }
    void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        // Görüş açısı ve mesafe
        float halfFOV = viewAngle * 0.5f;
        float radius = viewDistance;

        // Düşmanın position
        Vector3 pos = transform.position ; // biraz yukarıdan çizmek için

        // Gizmo rengi
        Gizmos.color = Color.yellow;

        // Center direction
        Vector3 forward = transform.forward;

        // Koni çizimi
        Vector3 leftDir = Quaternion.Euler(0, -halfFOV, 0) * forward * radius;
        Vector3 rightDir = Quaternion.Euler(0, halfFOV, 0) * forward * radius;

        Gizmos.DrawRay(pos, leftDir);
        Gizmos.DrawRay(pos, rightDir);
        Gizmos.DrawLine(pos + leftDir, pos + rightDir);

        // İsteğe bağlı: raycast görselleştirme
        if (CanSeePlayer())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawLine(pos, player.position + Vector3.up);
    }
}

