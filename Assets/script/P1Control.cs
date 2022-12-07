using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Control : MonoBehaviour
{

    P1Player player;
    Rigidbody2D rigBody;
    private Animator ani;

    [HideInInspector]
    public float H, v;
    public bool jump;
    public bool fire;
    public bool move;
    public float speed;
    public float jumpSpeed;
    private int jumpCount;

    void Start()
    {
        player = GetComponent<P1Player>();
        rigBody = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }


    void Update()
    {
        P1Player P1Player =  GameObject.Find("Nick").GetComponent<P1Player>();
        if (P1Player.IsDead == true) {
            return;
        }

        H = Input.GetAxis("P1Horizontal");

        jump = Input.GetButtonDown("P1Jump");
        fire = Input.GetButtonDown("P1Fire");
        Debug.Log(transform.forward);
        
        if (H != 0)
        {
            ani.SetFloat("Speed", Mathf.Abs(H));

            if (H > 0) {
                player.direction = 1;
            }
            else
            {
                player.direction = -1;
            }
            ani.SetBool("Walk", true);
            // 转身
            transform.localScale = new Vector3(H > 0 ? -1f : 1f, 1f, 1f);
            // 移动
            transform.Translate(Vector2.right * H * speed * Time.deltaTime);
        }
        else
        {
            ani.SetBool("Walk", false);
        }
        if (fire)
        {
            player.Fire();
        }
        Move(H);


    }

    // 重生时复活位置
    public void initPosition(){
        Vector3 pos = transform.position;
        pos.x = -1f;
        pos.y = -3.59f;
        transform.position = pos;
    }
    // 移动
    public void Move(float h)
    {
        float vy = rigBody.velocity.y;
        if (jump && jumpCount < 1)
        {
            ani.SetTrigger("Jump");
            vy = jumpSpeed;
            jumpCount++;
            // isGround = false;
        }

        rigBody.velocity = new Vector2(h * speed, vy);
    }

    // 发生碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 只能一次跳跃
        if (collision.collider.tag == "Floor")
        {
            ani.SetBool("IsGround", true);
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 判断如果是地面
        if (collision.collider.tag == "Floor")
        {
            ani.SetBool("IsGround", false);
        }
    }
}
