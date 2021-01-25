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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeathDelay());
        if (!isEnemyBullet)
        {
            //transform.localScale = new Vector2(GameController.BulletSize, GameController.BulletSize);
        }
    }

    void Update()
    {
        if (isEnemyBullet)
        {
            curPos = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, playerPos, 5f * Time.deltaTime);
            if (curPos == lastPos)
            {
                Destroy(gameObject);
            }
            lastPos = curPos;
        }
    }

    public void GetPlayer(Transform player)
    {
        playerPos = player.position;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
        
    //}
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Enemy" && !isEnemyBullet)
        {
            col.gameObject.GetComponent<EnemyController>().Death();
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
            Destroy(gameObject);
        }
        if (col.tag == "wall" && isEnemyBullet)
        {
            //col.gameObject.GetComponent<Completed.Player>().getAttacked();
            //GameController.DamagePlayer(1);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hitOnce && collision.collider.tag != "Player")
        {
            hitOnce = true;
        }
    }
}
