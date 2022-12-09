using System;
using UnityEngine;

namespace script
{
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

        private SpriteRenderer _renderer;
        private int _itemNum;

        private bool _isGround;
        private int _dir1;
        private Transform _checkGround;
        public Transform prefabSon;
        public Transform prefabRedPotion;
        public Transform prefabBluePotion;
        public Transform prefabYellowPotion;
        public Transform prefabGreenPotion;
        private int _hp = 0;
        private Animator _anim;
        private const float FreezeTime = 5;
        private float _beFrozenTime;
        private Rigidbody2D _rigid;
        public float rollSpeed = 6;
        public SnowBallState state = SnowBallState.UnFormed1;
        private static readonly int Rolling = Animator.StringToHash("Rolling");
        private static readonly int Hp = Animator.StringToHash("hp");
        private static readonly int Push = Animator.StringToHash("Push");

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _itemNum = UnityEngine.Random.Range(1, 100);
            _hp = 0;
            _beFrozenTime = Time.time;
            _checkGround = transform.Find("CheckGround");
            _anim = GetComponent<Animator>();
            _rigid = GetComponent<Rigidbody2D>();

            // control = GetComponent<P1Control>();

        }

        private void Update()
        {
            switch (state)
            {
                case SnowBallState.UnFormed1:
                {
                    _hp = 0;
                    if (!gameObject.CompareTag("RollingSnowBall"))
                    {
                        gameObject.tag = "UnformedBall";
                        gameObject.layer = LayerMask.NameToLayer("UnformedBall");
                    }
                    if (Time.time >= _beFrozenTime + FreezeTime)
                    {
                        state = SnowBallState.TurnToSon;
                    }
                }
                    break;
                case SnowBallState.UnFormed2:
                {
                    _hp = 1;
                    if (!gameObject.CompareTag("RollingSnowBall"))
                    {
                        gameObject.tag = "UnformedBall";
                        gameObject.layer = LayerMask.NameToLayer("UnformedBall");
                    }
                    if (Time.time >= _beFrozenTime + FreezeTime)
                    {
                        _beFrozenTime = Time.time;
                        state = SnowBallState.UnFormed1;
                    }
                }
                    break;
                case SnowBallState.Ball:
                {
                    _hp = 2;
                    if (!gameObject.CompareTag("RollingSnowBall"))
                    {
                        gameObject.tag = "SnowBall";
                        gameObject.layer = LayerMask.NameToLayer("SnowBall");
                    }
                    if (Time.time >= _beFrozenTime + FreezeTime)
                    {
                        _beFrozenTime = Time.time;
                        state = SnowBallState.UnFormed2;

                    }
                }
                    break;
                case SnowBallState.Rolling:
                {
                    _anim.SetTrigger(Rolling);
                    if (_isGround)
                    {
                        Vector2 v = new Vector2(_dir1 * rollSpeed, _rigid.velocity.y);
                        _rigid.velocity = v;
                    }
                    else
                    {
                        Vector2 v = new Vector2(_dir1 * 7, _rigid.velocity.y);

                        _rigid.velocity = v;
                    }

                }
                    break;
                case SnowBallState.TurnToSon:
                {
                    var son = Instantiate(prefabSon, transform.position, Quaternion.identity);
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    var son1 = son.GetComponent<Son>();
                    son1.reTurn = true;
                    Destroy(gameObject);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void FixedUpdate()
        {
            CheckGround();
            _anim.SetInteger(Hp, _hp);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
       
            if (collision.CompareTag("PlayerHit"))
            {
                ChangeState();
                _beFrozenTime = Time.time;
            }
        
            if (gameObject.CompareTag("RollingSnowBall"))
            {

                if (collision.CompareTag("Wall") || collision.CompareTag("Stair"))
                {
                    _dir1 = -_dir1;
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
            _dir1 = dir;
            gameObject.tag = "RollingSnowBall";
            gameObject.layer = LayerMask.NameToLayer("RollingSnowBall");
            Vector2 v = new Vector2(dir * rollSpeed, _rigid.velocity.y);
            _rigid.velocity = v;
            state = SnowBallState.Rolling;
        }


        private void CheckGround()
        {
            // ReSharper disable once HeapView.ObjectAllocation
            _isGround = Physics2D.OverlapCircle(_checkGround.position, 0.1f, ~LayerMask.GetMask("RollingSnowBall"));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("RollingSnowBall") && gameObject.CompareTag("UnformedBall"))
            {
                Destroy(gameObject);

                switch (_itemNum)
                {
                    case >= 90:
                    {
                        Instantiate(prefabRedPotion, transform.position, Quaternion.identity);
                        break;
                    }
                    case >= 80 and < 90:
                    {
                        Instantiate(prefabBluePotion, transform.position, Quaternion.identity);
                        break;
                    }
                    case >= 70 and < 80:
                    {
                        Instantiate(prefabYellowPotion, transform.position, Quaternion.identity);
                        break;
                    }
                    default:
                    {
                        Instantiate(prefabGreenPotion, transform.position, Quaternion.identity);
                        break;
                    }
                }
            }

            if (!gameObject.CompareTag("RollingSnowBall")) return;
            if (collision.collider.CompareTag("DeadZone") || collision.collider.CompareTag("Boss"))
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionExit2D() {
            var p1Player =  GameObject.Find("Nick").GetComponent<P1Player>();
            if (gameObject.CompareTag("SnowBall") || gameObject.CompareTag("RollingSnowBall"))
            {
                p1Player.ani.SetBool(Push, false);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            var p1Control =  GameObject.Find("Nick").GetComponent<P1Control>();
            var p1Player =  GameObject.Find("Nick").GetComponent<P1Player>();
            if (gameObject.CompareTag("SnowBall"))
            {
                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Invincibility"))
                {
                    p1Player.ani.SetBool(Push, true);
                    if (p1Control.fire){
                        Roll(p1Player.direction);
                    }
                }
            }

            if (gameObject.CompareTag("RollingSnowBall"))
            {
                if (collision.collider.CompareTag("Wall"))
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
                if (collision.collider.CompareTag("Stair"))
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

            if (collision.collider.CompareTag("RollingSnowBall") && gameObject.CompareTag("UnformedBall"))
            {
                Destroy(gameObject);
                switch (_itemNum)
                {
                    case >= 75:
                    {
                        Instantiate(prefabRedPotion, transform.position, Quaternion.identity);
                        break;
                    }
                    case >= 50 and < 75:
                    {
                        Instantiate(prefabBluePotion, transform.position, Quaternion.identity);
                        break;
                    }
                    case >= 25 and < 50:
                    {
                        Instantiate(prefabYellowPotion, transform.position, Quaternion.identity);
                        break;
                    }
                    default:
                    {
                        Instantiate(prefabGreenPotion, transform.position, Quaternion.identity);
                        break;
                    }
                }
            }

            if (!gameObject.CompareTag("RollingSnowBall")) return;
            if (collision.collider.CompareTag("DeadZone") || collision.collider.CompareTag("Boss"))
            {
                Destroy(gameObject);
            }

            if (collision.collider.CompareTag("SnowBall"))
            {
                var ball = collision.transform.GetComponent<SnowBall>();
                ball._renderer.color = playerNum == 1 ? Color.blue : Color.red;

                ball.Roll(_dir1);
                _dir1 = -_dir1;
            }
        }

    }
}