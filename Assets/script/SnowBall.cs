using UnityEngine;

public enum SnowBallState
{
    UnFormed1,
    UnFormed2,
    Ball,
    Rolling,
    TurnToSon,
}
public class SnowBall : MonoBehaviour
{
    [HideInInspector]
    public int playerNum;
    SpriteRenderer Renderer;
    int itemNum;
    bool isGround;
    //ѩ���ƶ��ķ���
    int dir1;
    Transform checkGround;
    public Transform prefabSon;
    public Transform prefabRedPotion;
    public Transform prefabBluePotion;
    public Transform prefabYellowPotion;
    public Transform prefabGreenPotion;
    int hp = 0;
    Animator anim;
    float freezeTime = 5;
    float beFrozonTime;
    Rigidbody2D rigid;
    public float rollSpeed = 6;
    public SnowBallState state = SnowBallState.UnFormed1;

    void Start()
    {
        
        Renderer = GetComponent<SpriteRenderer>();
        itemNum = UnityEngine.Random.Range(1, 100);
        hp = 0;
        beFrozonTime = Time.time;
        checkGround = transform.Find("CheckGround");
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

       // control = GetComponent<P1Control>();

    }
    void Update()
    {
        switch (state)
        {
            case SnowBallState.UnFormed1:
                {
                    hp = 0;
                    if (gameObject.tag != "RollingSnowBall")
                    {
                        gameObject.tag = "UnformedBall";
                        gameObject.layer = LayerMask.NameToLayer("UnformedBall");
                    }
                    if (Time.time >= beFrozonTime + freezeTime)
                    {

                        state = SnowBallState.TurnToSon;
                    }
                }
                break;
            case SnowBallState.UnFormed2:
                {
                    hp = 1;
                    if (gameObject.tag != "RollingSnowBall")
                    {
                        gameObject.tag = "UnformedBall";
                        gameObject.layer = LayerMask.NameToLayer("UnformedBall");
                    }
                    if (Time.time >= beFrozonTime + freezeTime)
                    {
                        beFrozonTime = Time.time;
                        state = SnowBallState.UnFormed1;
                    }
                }
                break;
            case SnowBallState.Ball:
                {
                    hp = 2;
                    if (gameObject.tag != "RollingSnowBall")
                    {
                        gameObject.tag = "SnowBall";
                        gameObject.layer = LayerMask.NameToLayer("SnowBall");
                    }
                    if (Time.time >= beFrozonTime + freezeTime)
                    {
                        beFrozonTime = Time.time;
                        state = SnowBallState.UnFormed2;

                    }
                }
                break;
            case SnowBallState.Rolling:
                {
                    anim.SetTrigger("Rolling");
                    if (isGround)
                    {
                        Vector2 v = new Vector2(dir1 * rollSpeed, rigid.velocity.y);
                        rigid.velocity = v;
                    }
                    else
                    {
                        Vector2 v = new Vector2(dir1 * 7, rigid.velocity.y);

                        rigid.velocity = v;
                    }

                }
                break;
            case SnowBallState.TurnToSon:
                {
                    Transform son = Instantiate(prefabSon, transform.position, Quaternion.identity);
                    Son son1 = son.GetComponent<Son>();
                    son1.reTurn = true;
                    Destroy(gameObject);
                }
                break;
        }
    }
    private void FixedUpdate()
    {
        CheckGround();
        anim.SetInteger("hp", hp);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.tag == "PlayerHit")
        {
            ChangeState();
            beFrozonTime = Time.time;
        }
        
        if (gameObject.tag == "RollingSnowBall")
        {

            if (collision.tag == "Wall" || collision.tag == "Stair")
            {
                dir1 = -dir1;
            }
        }
    }


    
    void ChangeState()
    {
        if (state == SnowBallState.UnFormed1)
        {
            state = SnowBallState.UnFormed2;
        }
        else if (state == SnowBallState.UnFormed2)
        {
            state = SnowBallState.Ball;
        }
    }
    
    public void Roll(int dir)
    {
        dir1 = dir;
        gameObject.tag = "RollingSnowBall";
        gameObject.layer = LayerMask.NameToLayer("RollingSnowBall");
        Vector2 v = new Vector2(dir * rollSpeed, rigid.velocity.y);
        rigid.velocity = v;
        state = SnowBallState.Rolling;
    }


    void CheckGround()
    {
        isGround = Physics2D.OverlapCircle(checkGround.position, 0.1f, ~LayerMask.GetMask("RollingSnowBall"));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "RollingSnowBall" && gameObject.tag == "UnformedBall")
        {
            Destroy(gameObject);
          
            if (itemNum >= 90)
            {
                Transform redPotion = Instantiate(prefabRedPotion, transform.position, Quaternion.identity);
            }
            else if (itemNum >= 80 && itemNum < 90)
            {
                Transform bluePotion = Instantiate(prefabBluePotion, transform.position, Quaternion.identity);
            }
            else if (itemNum >= 70 && itemNum < 80)
            {
                Transform yellowPotion = Instantiate(prefabYellowPotion, transform.position, Quaternion.identity);
            }
            else
            {
                Transform greenPotion = Instantiate(prefabGreenPotion, transform.position, Quaternion.identity);
            }
        }
        if (gameObject.tag == "RollingSnowBall")
        {
            if (collision.collider.tag == "DeadZone" || collision.collider.tag == "Boss")
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        P1Player P1player =  GameObject.Find("Nick").GetComponent<P1Player>();
        if (gameObject.tag == "SnowBall" || gameObject.tag == "RollingSnowBall")
        {
            P1player.ani.SetBool("Push", false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        P1Control P1Control =  GameObject.Find("Nick").GetComponent<P1Control>();
        P1Player P1player =  GameObject.Find("Nick").GetComponent<P1Player>();
        if (gameObject.tag == "SnowBall")
        {
            if (collision.collider.tag == "Player" || collision.collider.tag == "Invincibility")
            {
                P1player.ani.SetBool("Push", true);
                if (P1Control.fire){
                   Roll(P1player.direction);
                }
            }
        }

        if (gameObject.tag == "RollingSnowBall")
        {
            if (collision.collider.tag == "Wall")
            {
                if (transform.position.x > 0)
                {
                    Roll(-1);
                }
                else
                {
                    Roll(1);
                }
            }
            if (collision.collider.tag == "Stair")
            {
                if (transform.position.x > 0)
                {
                    Roll(1);
                }
                else
                {
                    Roll(-1);
                }
            }
        }

        if (collision.collider.tag == "RollingSnowBall" && gameObject.tag == "UnformedBall")
        {
            Destroy(gameObject);
            //���ߵ���
            if (itemNum >= 75)
            {
                Transform redPotion = Instantiate(prefabRedPotion, transform.position, Quaternion.identity);
            }
            else if (itemNum >= 50 && itemNum < 75)
            {
                Transform bluePotion = Instantiate(prefabBluePotion, transform.position, Quaternion.identity);
            }
            else if (itemNum >= 25 && itemNum < 50)
            {
                Transform yellowPotion = Instantiate(prefabYellowPotion, transform.position, Quaternion.identity);
            }
            else
            {
                Transform greenPotion = Instantiate(prefabGreenPotion, transform.position, Quaternion.identity);
            }
        }

        if (gameObject.tag == "RollingSnowBall")
        {
            //ѩ������Boss��DeadZone������
            if (collision.collider.tag == "DeadZone" || collision.collider.tag == "Boss")
            {
                Destroy(gameObject);
            }

            //�����е�ѩ������������ѩ����򷴷������
            if (collision.collider.tag == "SnowBall")
            {
                SnowBall ball = collision.transform.GetComponent<SnowBall>();
                if (playerNum == 1)
                {
                    ball.Renderer.color = Color.blue;
                }
                else
                {
                    ball.Renderer.color = Color.red;
                }

                ball.Roll(dir1);
                dir1 = -dir1;
            }

        }
    }

}

