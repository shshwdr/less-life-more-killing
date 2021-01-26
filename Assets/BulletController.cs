using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime;
    public bool isEnemyBullet = false;

    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 playerPos;
    bool hitOnce;
    Rigidbody2D rigidbody;
    Animator animator;
    bool isBreaking;
    public AudioClip bounce;
    public AudioClip crash;
    public AudioClip shoot;
    AudioSource audioSource;
    float liveTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //StartCoroutine(DeathDelay());
        if (!isEnemyBullet)
        {
            //transform.localScale = new Vector2(GameController.BulletSize, GameController.BulletSize);
        }
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (shoot)
        {
            audioSource.PlayOneShot(shoot);
        }
    }

    void Update()
    {
        if (isBreaking)
        {
            return;
        }
        liveTime += Time.deltaTime;
        //if (isEnemyBullet)
        //{
        //    curPos = transform.position;
        //    transform.position = Vector2.MoveTowards(transform.position, playerPos, 5f * Time.deltaTime);
        //    //if (curPos == lastPos)
        //    //{
        //    //    Destroy(gameObject);
        //    //}
        //    lastPos = curPos;
        //}
    }

    public void GetPlayer(Transform player)
    {
        playerPos = player.position;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        DestorySelf();
    }
    void DestorySelf()
    {
        isBreaking = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = 0;
        animator.SetTrigger("Break");
        audioSource.Stop();
    }

    public void DestoryIt()
    {
        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D col)
    {

        if (isBreaking)
        {
            return;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isBreaking)
        {
            return;
        }
        if (col.tag == "Enemy" && !isEnemyBullet)
        {
            col.gameObject.GetComponent<EnemyController>().Damage(1);
            //Destroy(gameObject);
        }
        if (col.tag == "Player" && !isEnemyBullet&& hitOnce)
        {
            col.gameObject.GetComponent<Completed.Player>().getHealed();
            //GameController.DamagePlayer(1);
            Destroy(gameObject);
        }
        if (col.tag == "Player" && isEnemyBullet)
        {
            col.gameObject.GetComponent<Completed.Player>().getAttacked();
            //GameController.DamagePlayer(1);
            DestorySelf();
        }
        if (col.tag == "wall" && isEnemyBullet)
        {
            //col.gameObject.GetComponent<Completed.Player>().getAttacked();
            //GameController.DamagePlayer(1);
            DestorySelf();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (liveTime > lifeTime && !isBreaking)
        {
            DestorySelf();
        }
        if (bounce && collision.collider.tag != "Player"&& collision.collider.tag != "Enemy")
        {

            audioSource.PlayOneShot(bounce);
        }
        if (!hitOnce && collision.collider.tag != "Player")
        {
            hitOnce = true;
        }
    }
}
