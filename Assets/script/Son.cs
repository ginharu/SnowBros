using UnityEngine;

namespace script
{
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
        
        public bool reTurn;
        private Animator _anim;

        public float speed = 8;
        public float y;
        private Rigidbody2D _renderer;
        public SonState state = SonState.Born;
        private int _i;
        private bool _move;
        private bool _awake;
        private bool _isGround;
        private float _bornTime;

        [HideInInspector]
        public Vector3 scaleRight = new Vector3(-1, 1, 1);
        [HideInInspector]
        public Vector3 scaleLeft = new Vector3(1, 1, 1);

        private int _itemNum;
        private static readonly int Awake = Animator.StringToHash("Awake");
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGround = Animator.StringToHash("IsGround");

        private void Start()
        {
            _itemNum = Random.Range(1, 100);
            y = Random.Range(-4, -1);
            _bornTime = Time.time;
            _renderer = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

            _anim.SetBool(Awake, _awake);
            // anim.SetBool("IsGround", isGround);
            float sp = Mathf.Abs(_renderer.velocity.magnitude);
            if (sp > 0) { _move = true; }
            _anim.SetBool(Move, _move);
            _anim.SetFloat(Speed, sp);
        }

        private void FixedUpdate()
        {
            _awake = false;
            switch (state)
            {
                case SonState.Stay:
                {
                    _renderer.isKinematic = false;
                    if (Time.time - _bornTime > 1f)
                    {
                        _i = Random.Range(0, 2);
                        if (_i == 0) { state = SonState.WalkToLeft; }
                        else if (_i == 1) { state = SonState.WalkToRight; }
                    }
                }
                    break;
                case SonState.Born:
                {
                    if (reTurn == false)
                    {
                        Born();
                        if (Time.time - _bornTime > 1f) { state = SonState.Awake; }
                    }
                    else
                    {
                        state = SonState.Stay;
                    }

                }
                    break;
                case SonState.Awake:
                {
                    _renderer.isKinematic = false;
                    _i = Random.Range(0, 2);
                    _awake = true;
                    if (_i == 0) { state = SonState.WalkToLeft; }
                    else if (_i == 1) { state = SonState.WalkToRight; }

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


            if (_isGround)
            {
                Vector2 v = new Vector2(dir * speed / 5, _renderer.velocity.y);
                _renderer.velocity = v;
            }
            else
            {
                Vector2 v = new Vector2(dir * 0.7f, _renderer.velocity.y);
                _renderer.velocity = v;
            }
        }
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerHit"))
            {
                Destroy(gameObject);
                Instantiate(preFabSnowBall, transform.position, Quaternion.identity);
            }

        }
    
        void Born()
        {
            _renderer.isKinematic = true;
            var position = transform.position;
            position = Vector3.MoveTowards(position, new Vector3(y, position.y, position.x), speed * Time.deltaTime);
            transform.position = position;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Floor"))
            {
                _anim.SetBool(IsGround, false);
                _isGround = false;
            }
        }

        private void OnCollisionStay2D(Collision2D collision) {
            if (collision.collider.CompareTag("RollingSnowBall"))
            {
                Destroy(gameObject);
                if (_itemNum >= 75)
                {
                    Instantiate(prefabRedPotion, transform.position, Quaternion.identity);
                }
                else if (_itemNum is >= 50 and < 75)
                {
                    Instantiate(prefabBluePotion, transform.position, Quaternion.identity);
                }
                else if (_itemNum is >= 25 and < 50)
                {
                    Instantiate(prefabYellowPotion, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(prefabGreenPotion, transform.position, Quaternion.identity);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Floor"))
            {
                _anim.SetBool(IsGround, true);
                _isGround = true;
            }

            if (collision.collider.CompareTag("Wall"))
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

 
            if (collision.collider.CompareTag("DeadZone"))
            {
                Destroy(gameObject);
            }
        }
    }
}