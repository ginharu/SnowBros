using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dir;
    public float speed = 7;
    float destroyTime = 1;
    float shootingTime;
    float flyTime=0.5f;
    Rigidbody2D rigidbody;

    public void Start()
    {
        
        if (dir == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        rigidbody = GetComponent<Rigidbody2D>();
        shootingTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= flyTime + shootingTime)
        {
            rigidbody.isKinematic = false;
            transform.position += transform.right * dir *0.9f * Time.deltaTime;
        }else {
            transform.position += transform.right * dir * speed * Time.deltaTime;
        }

        if (Time.time >= shootingTime + destroyTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void updateTime(bool up) {
        if (up) {
            flyTime = flyTime * 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boss" || collision.tag == "UnformedBall")
        {
            
            //if (player == 1)
            //{
            //    Score1.score1p += 10;
            //}
            //else
            //{
            //    Score2.score2p += 10;
            //}

        }
        Destroy(gameObject);
    }

}
