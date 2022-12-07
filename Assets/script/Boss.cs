using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    public Image Bar;
    private bool jump = false;
    private bool bigJump = false;
    // private bool stand = false;
    // private bool down = false;
    // private bool dead = true;
    public TextMeshProUGUI BossHP;

    public float HP;
    public float maxHP;
    public int bigJumpHeight = 10;
    public GameObject Son1;
    float standTime = 3f;
    float lastStandTime;
    int jumpCount = 3;
    private string bossState;
    Animator anim;
    Rigidbody2D Rigidbody;
    Transform firePoint;
    public float fireCD = 0.1f;
    float lastFireTime;

    // Start is called before the first frame update
    void Start()
    {
        bossState = "stand";
        firePoint = transform.Find("FirePoint");
        Rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        BossHP.text = "Boss: " + HP.ToString();
        if (bossState == "Down")
        {
            jumpCount = 4;
        }
        BarFiller();
    }

    public void GetHit(int damage)
    {
        HP -= damage;
        if (HP < 0)
        {
            HP = 0;
        }
    }
    void Attack()
    {
        if (Time.time < lastFireTime + fireCD)
        {
            return;
        }
        GameObject Sons = Instantiate(Son1, firePoint.position, Quaternion.identity);
        lastFireTime = Time.time;
    }
    
    private void BarFiller(){
        Bar.fillAmount = HP/maxHP;
    }

    public void normalJump()
    {
        jumpCount--;
        if (jumpCount < 0)
        {
            jumpCount = 0;
        }

        jump = true;
        if (jump)
        {
            anim.SetTrigger("Jump");
        }
        
        float vx = Rigidbody.velocity.x;
        Rigidbody.velocity = new Vector2(vx, 3);
        bossState = "stand";
    }

    public void BigJump()
    {
        bigJump = true;
        if (bigJump)
        {
            anim.SetTrigger("BigJump");
            float vx = Rigidbody.velocity.x;
            Rigidbody.velocity = new Vector2(vx, bigJumpHeight);
        }
    }
    public void Down()
    {
        bigJump = true;
        if (bigJump)
        {
            anim.SetTrigger("BigJump");
            float vx = Rigidbody.velocity.x;
            Debug.Log(Rigidbody.velocity.x);
            float px = Rigidbody.position.x;
            if (px > 0) {
                vx -= 5;
            }
            else
            {
                vx += 5f;
            }

            Rigidbody.velocity = new Vector2(vx, 8);

            //int range = UnityEngine.Random.Range(1, 3);
            //Debug.Log(range);
            transform.localScale = new Vector3(px > 0 ? -1f : 1f, 1f, 1f);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHit")
        {
            HP--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "RollingSnowBall")
        {
            HP -= 200;
            if (HP < 0)
            {
                HP = 0;
            }
        }
        if (collision.collider.tag == "Floor")
        {
            anim.SetBool("IsGround", true);
        }

        else
        {
            anim.SetBool("IsGround", false);
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        bigJump = false;
        if (HP <= 0) 
        {
            bossState = "dead";
        }
        switch (bossState)
        {
            case "stand":
            {
                if (Time.time - (lastStandTime + standTime) > 1.5f)
                {
                    standTime = 0;
                    if (transform.position.y > 1 && Time.time - lastStandTime > 6)
                    {
                        bossState = "down";
                    }
                    else if (transform.position.y < 1)
                    {
                        bossState = "jump";
                    }
                }
                break;
            }
            case "jump":
                {
                    normalJump();
                    lastStandTime = Time.time;
                    if (jumpCount == 0)
                    {
                        bossState = "bigJump";
                    }
                    break;
                }
            case "bigJump":
                {
                    BigJump();
                    Debug.Log("BIG");
                    lastStandTime = Time.time;
                    bossState = "stand";
                    break;
                }
            case "down":
                {
                    Down();
                    Debug.Log("Down");
                    lastStandTime = Time.time;
                    bossState = "stand";
                    break;
                }
            case "dead":
                {
                
                    anim.SetBool("Dead",true);
                    break;
                }
        }
    }
}
