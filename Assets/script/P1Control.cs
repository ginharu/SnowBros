using UnityEngine;
using UnityEngine.Serialization;

namespace script
{
    public class P1Control : MonoBehaviour
    {
        private P1Player _player;
        private Rigidbody2D _rigBody;
        private Animator _ani;

        [FormerlySerializedAs("H")] [HideInInspector]
        public float h;

        [HideInInspector]
        public float v;

        public bool jump;
        public bool fire;
        public bool move;
        public float speed;
        public float jumpSpeed;
        private int _jumpCount;
        private GameObject _gameObject;
        private P1Player _p1Player;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Walk1 = Animator.StringToHash("Walk");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int IsGround = Animator.StringToHash("IsGround");

        private void Start()
        {
            //_gameObject = GameObject.Find("Nick");
            _player = GetComponent<P1Player>();
            _rigBody = GetComponent<Rigidbody2D>();
            _ani = GetComponent<Animator>();
        }


        private void Update()
        {
            if (_player.isDead) {
                return;
            }

            h = Input.GetAxis("P1Horizontal");

            jump = Input.GetButtonDown("P1Jump");
            fire = Input.GetButtonDown("P1Fire");
        
            if (h != 0)
            {
                _ani.SetFloat(Speed, Mathf.Abs(h));

                if (h > 0) {
                    _player.direction = 1;
                }
                else
                {
                    _player.direction = -1;
                }
                _ani.SetBool(Walk1, true);
                // 转身
                transform.localScale = new Vector3(h > 0 ? -1f : 1f, 1f, 1f);
                // 移动
                transform.Translate(Vector2.right * (h * speed * Time.deltaTime));
            }
            else
            {
                _ani.SetBool(Walk1, false);
            }
            if (fire)
            {
                _player.Fire();
            }
            Move(h);


        }

        // 重生时复活位置
        public void InitPosition(){
            var transform1 = transform;
            Vector3 pos = transform1.position;
            pos.x = -1f;
            pos.y = -3.59f;
            transform1.position = pos;
        }
        // 移动
        private void Move(float height)
        {
            var vy = _rigBody.velocity.y;
            if (jump && _jumpCount < 1)
            {
                _ani.SetTrigger(Jump);
                vy = jumpSpeed;
                _jumpCount++;
                // isGround = false;
            }

            _rigBody.velocity = new Vector2(height * speed, vy);
        }

        // 发生碰撞
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 只能一次跳跃
            if (collision.collider.CompareTag("Floor"))
            {
                _ani.SetBool(IsGround, true);
                _jumpCount = 0;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            // 判断如果是地面
            if (collision.collider.CompareTag("Floor"))
            {
                _ani.SetBool(IsGround, false);
            }
        }
    }
}
