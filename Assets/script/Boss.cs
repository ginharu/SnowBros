using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace script
{
    public class Boss : MonoBehaviour
    {
        [FormerlySerializedAs("Bar")] public Image bar;
        private bool _jump;
        private bool _bigJump;
        [FormerlySerializedAs("BossHP")] public TextMeshProUGUI bossHp;

        [FormerlySerializedAs("HP")] public float hp;
        [FormerlySerializedAs("maxHP")] public float maxHp;
        public int bigJumpHeight = 10;
        [FormerlySerializedAs("Son1")] public GameObject son1;
        private float _standTime = 3f;
        private float _lastStandTime;
        private int _jumpCount = 3;
        private string _bossState;
        private Animator _anim;
        private Rigidbody2D _rigidbody;
        private Transform _firePoint;
        [FormerlySerializedAs("fireCD")] public float fireCd = 0.1f;
        private float _lastFireTime;
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int BigJump1 = Animator.StringToHash("BigJump");
        private static readonly int IsGround = Animator.StringToHash("IsGround");
        private static readonly int Dead = Animator.StringToHash("Dead");

        // Start is called before the first frame update
        private void Start()
        {
            _bossState = "stand";
            _firePoint = transform.Find("FirePoint");
            _rigidbody = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            // ReSharper disable once HeapView.BoxingAllocation
            bossHp.text = $"Boss: {hp}";
            if (_bossState == "Down")
            {
                _jumpCount = 4;
            }
            BarFiller();
        }

        public void GetHit(int damage)
        {
            hp -= damage;
            if (hp < 0)
            {
                hp = 0;
            }
        }

        private void Attack()
        {
            if (Time.time < _lastFireTime + fireCd)
            {
                return;
            }
            var sons = Instantiate(son1, _firePoint.position, Quaternion.identity);
            _lastFireTime = Time.time;
        }
    
        private void BarFiller(){
            bar.fillAmount = hp/maxHp;
        }

        private void NormalJump()
        {
            _jumpCount--;
            if (_jumpCount < 0)
            {
                _jumpCount = 0;
            }

            _jump = true;
            if (_jump)
            {
                _anim.SetTrigger(Jump);
            }
        
            var vx = _rigidbody.velocity.x;
            _rigidbody.velocity = new Vector2(vx, 3);
            _bossState = "stand";
        }

        private void BigJump()
        {
            _bigJump = true;
            if (_bigJump)
            {
                _anim.SetTrigger(BigJump1);
                float vx = _rigidbody.velocity.x;
                _rigidbody.velocity = new Vector2(vx, bigJumpHeight);
            }
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private void Down()
        {
            _bigJump = true;
            if (!_bigJump) return;
            _anim.SetTrigger(BigJump1);
            var vx = _rigidbody.velocity.x;
            var px = _rigidbody.position.x;
            if (px > 0) {
                vx -= 5;
            }
            else
            {
                vx += 5f;
            }

            _rigidbody.velocity = new Vector2(vx, 8);

            //int range = UnityEngine.Random.Range(1, 3);
            //Debug.Log(range);
            transform.localScale = new Vector3(px > 0 ? -1f : 1f, 1f, 1f);

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerHit"))
            {
                hp--;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("RollingSnowBall"))
            {
                hp -= 200;
                if (hp < 0)
                {
                    hp = 0;
                }
            }

            _anim.SetBool(IsGround, collision.collider.CompareTag("Floor"));
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }
        private void FixedUpdate()
        {
            _bigJump = false;
            if (hp <= 0) 
            {
                _bossState = "dead";
            }
            switch (_bossState)
            {
                case "stand":
                {
                    if (Time.time - (_lastStandTime + _standTime) > 1.5f)
                    {
                        _standTime = 0;
                        if (transform.position.y > 1 && Time.time - _lastStandTime > 6)
                        {
                            _bossState = "down";
                        }
                        else if (transform.position.y < 1)
                        {
                            _bossState = "jump";
                        }
                    }
                    break;
                }
                case "jump":
                {
                    NormalJump();
                    _lastStandTime = Time.time;
                    if (_jumpCount == 0)
                    {
                        _bossState = "bigJump";
                    }
                    break;
                }
                case "bigJump":
                {
                    BigJump();
                    _lastStandTime = Time.time;
                    _bossState = "stand";
                    break;
                }
                case "down":
                {
                    Down();
                    _lastStandTime = Time.time;
                    _bossState = "stand";
                    break;
                }
                case "dead":
                {
                
                    _anim.SetBool(Dead,true);
                    break;
                }
            }
        }
    }
}
