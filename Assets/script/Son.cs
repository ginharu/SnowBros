using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SonState
{
    Born,
    Awake,
    WalkToLeft,
    WalkToRight,
    Stay,
}

public class Son : MonoBehaviour
{
    public Transform preFabSnowBall;
    public Transform prefabBluePotion;
    public Transform prefabYellowPotion;
    public Transform prefabGreenPotion;
    public Transform prefabRedPotion;

    Transform checkGround;
    
    public bool reTurn;
    Animator anim;

    public float speed = 8;
    public float y;
    Rigidbody2D Renderer;
    public SonState state = SonState.Born;
    int i;
    bool move = false;
    bool awake = false;
    bool isGround;
    float bornTime;

    [HideInInspector]
    public Vector3 scaleRight = new Vector3(-1, 1, 1);
    [HideInInspector]
    public Vector3 scaleLeft = new Vector3(1, 1, 1);
    
    int itemNum;

    void Start()
    {
        itemNum = Random.Range(1, 100);
        y = Random.Range(-4, -1);
        bornTime = Time.time;
        Renderer = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        checkGround = transform.Find("CheckGround");
    }

    // Update is called once per frame
    void Update()
    {

        anim.SetBool("Awake", awake);
       // anim.SetBool("IsGround", isGround);
        float sp = Mathf.Abs(Renderer.velocity.magnitude);
        if (sp > 0) { move = true; }
        anim.SetBool("Move", move);
        anim.SetFloat("Speed", sp);
    }

    private void FixedUpdate()
    {
        awake = false;
        switch (state)
        {
            case SonState.Stay:
                {
                    Renderer.isKinematic = false;
                    if (Time.time - bornTime > 1f)
                    {
                        i = Random.Range(0, 2);
                        if (i == 0) { state = SonState.WalkToLeft; }
                        else if (i == 1) { state = SonState.WalkToRight; }
                    }
                }
                break;
            case SonState.Born:
                {
                    if (reTurn == false)
                    {
                        Born();
                        if (Time.time - bornTime > 1f) { state = SonState.Awake; }
                    }
                    else
                    {
                        state = SonState.Stay;
                    }

                }
                break;
            case SonState.Awake:
                {
                    Renderer.isKinematic = false;
                    i = Random.Range(0, 2);
                    awake = true;
                    if (i == 0) { state = SonState.WalkToLeft; }
                    else if (i == 1) { state = SonState.WalkToRight; }

                }
                break;
            case SonState.WalkToLeft:
                {
                    Walk(-1);
                }
                break;
            case SonState.WalkToRight:
                {
                    Walk(1);
                }
                break;
        }
    }

    void Flip(float h)
    {
        if (h > 0)
        {
            transform.localScale = scaleRight;
        }
        else if (h < 0)
        {
            transform.localScale = scaleLeft;
        }
    }
    
    void Walk(int dir)
    {
        Flip(dir);


        if (isGround)
        {
            Vector2 v = new Vector2(dir * speed / 5, Renderer.velocity.y);
            Renderer.velocity = v;
        }
        else
        {
            Vector2 v = new Vector2(dir * 0.7f, Renderer.velocity.y);
            Renderer.velocity = v;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHit")
        {
            Bullet bullet = collision.transform.GetComponent<Bullet>();
            //if (bullet.player == 1)
            //{
            //    Score1.score1p += 10;
            //}
            //else
            //{
            //    Score2.score2p += 10;
            //}
            Destroy(gameObject);
            Transform snowBall = Instantiate(preFabSnowBall, transform.position, Quaternion.identity);

        }

    }
    
    void Born()
    {
        Renderer.isKinematic = true;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(y, transform.position.y, transform.position.x), speed * Time.deltaTime);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Floor")
        {
            anim.SetBool("IsGround", false);
            isGround = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.tag == "RollingSnowBall")
        {
            Destroy(gameObject);
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
                Transform greenPotin = Instantiate(prefabGreenPotion, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Floor")
        {
            anim.SetBool("IsGround", true);
            isGround = true;
        }

        if (collision.collider.tag == "Wall")
        {
            if (state == SonState.WalkToLeft)
            {
                state = SonState.WalkToRight;
            }
            else if (state == SonState.WalkToRight)
            {
                state = SonState.WalkToLeft;
            }
        }

 
        if (collision.collider.tag == "DeadZone")
        {
            Destroy(gameObject);
        }
    }
}
