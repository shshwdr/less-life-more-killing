using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Wander,
    Follow,
    Die,
    Attack
};


public enum EnemyType
{
    Melee,
    Ranged,
    RangedNoTarge,
};

public class EnemyController : MonoBehaviour
{

    GameObject player;
    public EnemyState currState = EnemyState.Idle;
    public EnemyType enemyType;
    public float range;
    public float speed;
    public float attackRange;
    public float bulletSpeed;
    public float coolDown;
    private bool chooseDir = false;
    private bool dead = false;
    private bool coolDownAttack = false;
    public bool notInRoom = false;
    private Vector3 randomDir;
    public GameObject bulletPrefab;
    public Rigidbody2D rigidbody;
    NavMeshAgent agent;
    Vector3 startPositon;
    Animator animator;
    public bool ignoreCollider;
    public float wanderInterval = 1;


    public int maxHP = 1;
    int currentHP;
    bool isDead = false;
    public GameObject blood;

    public AudioClip attackAudio;
    public AudioClip dieAudio;
    AudioSource audioSource;
    bool isHitBack;

    public float summonTime = 0;
    public GameObject summonMonster;
    float summonPastTime;

    public bool isAvoidingPlayer;
    Vector3 targetPosition;


    public void init(Vector3 p)
    {
        startPositon = p;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponent<Animator>();
        blood.SetActive(false);
        currentHP = maxHP;
        if (ignoreCollider)
        {
            agent.enabled = false;
        }
        //agent.updatePosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gameStarted)
        {
            if (agent.enabled)
                agent.Stop();
            return;
        }
        if (isHitBack)
        {
            if (agent.enabled)
                agent.Stop();
            return;
        }
        switch (currState)
        {

            //case(EnemyState.Idle):
            //    Idle();
            //break;
            case (EnemyState.Wander):

                Wander();
                break;
            case (EnemyState.Follow):
                Follow();
                break;
            case (EnemyState.Die):
                break;
            case (EnemyState.Attack):
                Attack();
                break;
            case (EnemyState.Idle):

                GetComponent<NavMeshAgent>().Warp(startPositon);
                break;
        }

        if (!notInRoom)
        {
            if (IsPlayerInRange(range) && currState != EnemyState.Die)
            {
                currState = EnemyState.Follow;
            }
            else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
            {
                currState = EnemyState.Wander;
            }
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                currState = EnemyState.Attack;
            }
        }
        else
        {
            currState = EnemyState.Idle;
        }
        summonPastTime += Time.deltaTime;

        if (summonMonster&&summonPastTime >= summonTime)
        {
            summonPastTime = 0;

            GameObject summon = Instantiate(summonMonster, transform.position, transform.rotation,transform.parent) as GameObject;

            summon.GetComponent<EnemyController>().init(transform.position);
            GameManager.enemyCount += 1;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDir = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        chooseDir = false;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance = 10)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }

    void Wander()
    {
        agent.isStopped = false;
        float dist = agent.remainingDistance; 
        if ((dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) || agent.velocity.sqrMagnitude<=0.01f)
        {
            var newDestination = RandomNavSphere(transform.position);


            if (isAvoidingPlayer)
            {
                float currentDistance = (transform.position - player.transform.position).magnitude;
                bool findFurthest = false;
                Vector3 findTarget = Vector3.zero;
                for (int i = 0; i < 10; i++)
                {
                    var tempDestination = RandomNavSphere(transform.position);
                    float newDistance = (tempDestination - player.transform.position).magnitude;
                    if (newDistance > currentDistance)
                    {
                        currentDistance = newDistance;
                        findTarget = newDestination;
                        findFurthest = true;
                    }
                }
                if (findFurthest)
                {
                    newDestination = findTarget;
                }

            }

            agent.SetDestination(newDestination);
        }
            //if (!chooseDir)
            //{
            //    StartCoroutine(ChooseDirection());
            //}

            //transform.position += -transform.right * speed * Time.deltaTime;
            //if (IsPlayerInRange(range))
            //{
            //    currState = EnemyState.Follow;
            //}


            animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetFloat("Horizontal", agent.velocity.x);

        animator.SetFloat("Vertical", agent.velocity.y);
    }

    void Follow()
    {
        //agent.updatePosition = true;
        if (!ignoreCollider)
        {
            //else
            {

                agent.isStopped = false;
                agent.SetDestination(player.transform.position);
                //Debug.Log("agent speed"+agent.velocity);
                animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
                animator.SetFloat("Horizontal", agent.velocity.x);

                animator.SetFloat("Vertical", agent.velocity.y);
            }
        }
        else
        {

            var moveToPosition = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            rigidbody.MovePosition(moveToPosition);
            //ri.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (agent.enabled)
        {

            agent.SetDestination(transform.position);
        }
        if (!coolDownAttack)
        {
            switch (enemyType)
            {
                case (EnemyType.Melee):
                    player.GetComponent<Completed.Player>().getAttacked();
                    //GameController.DamagePlayer(1);
                    StartCoroutine(CoolDown());
                    break;
                case (EnemyType.Ranged):
                    {
                        var dir = player.transform.position - transform.position;
                        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        var quat = Quaternion.AngleAxis(angle, Vector3.forward);

                        GameObject bullet = Instantiate(bulletPrefab, transform.position, quat) as GameObject;
                        bullet.GetComponent<BulletController>().GetPlayer(player.transform);
                        bullet.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * bulletSpeed;
                        //bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
                        bullet.GetComponent<BulletController>().isEnemyBullet = true;
                        StartCoroutine(CoolDown());
                    }
                    break;
                case (EnemyType.RangedNoTarge):
                    {
                        Vector2[] dirs = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
                        foreach (Vector2 dir in dirs)
                        {
                            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                            var quat = Quaternion.AngleAxis(angle, Vector3.forward);

                            GameObject bullet = Instantiate(bulletPrefab, transform.position, quat) as GameObject;
                            bullet.GetComponent<BulletController>().GetPlayer(player.transform);
                            bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
                            bullet.GetComponent<BulletController>().isEnemyBullet = true;
                            StartCoroutine(CoolDown());
                        }
                    }
                    break;


            }
            animator.SetTrigger("Attack");
            audioSource.PlayOneShot(attackAudio);
        }
    }

    private IEnumerator CoolDown()
    {
        coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        coolDownAttack = false;
    }
    public void Damage(int damage = 1)
    {
        currentHP -= damage;
        if (currentHP <= 0 && !isDead)
        {
            Death();
            isDead = true;
        }
        else
        {

            animator.SetTrigger("Hit");
            animator.SetBool("GetHit", true);
            isHitBack = true;
        }
    }
    public void Recover()
    {
        isHitBack = false;

        animator.SetBool("GetHit", false);
    }
    public void Death()
    {
        //Completed.SoundManager.instance.PlayOneShot(dieAudio);
        blood.SetActive(true);
        blood.transform.parent = transform.parent;
        if (dieAudio)
        {

            blood.GetComponent<AudioSource>().PlayOneShot(dieAudio);
        }
        //RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        Destroy(gameObject);

        GameManager.enemyCount -= 1;
    }
}
