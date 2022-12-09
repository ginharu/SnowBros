using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace script
{
    public class P1Player : MonoBehaviour
    {

        // 红药水加成，速度增加
        private bool _speedUp;
        // 蓝药水加成，子弹变大
        private bool _powerUp;
        // 黄药水加成，子弹飞行时间边长
        private bool _bulletUp;
        // 绿药水加成，进入无敌
        public float potionsRedTime;
        public float potionsBlueTime;
        public float potionsYellowTime;
        public float potionsGreenTime;
        // 子弹的方向
        public int direction;
        // SnowBall ball;
        // 普通子弹
        public GameObject normalBullet;
        //  强化子弹
        [FormerlySerializedAs("EnhancedBullets")] public GameObject enhancedBullets;    
        private Collider2D _collider;
        private SpriteRenderer _renderer;
        public Animator ani;
        private SnowBall _snowBall;
        public int life;
        public TextMeshProUGUI lifeText;
        public bool isDead;
        private Transform _firePoint;
        [HideInInspector]
        public float startInvincibleTime;
        [FormerlySerializedAs("InvincibleTime")] public float invincibleTime;
        public bool isInvincible;
        private Color _currentColor;
        private static readonly int SpeedUp = Animator.StringToHash("SpeedUp");
        private static readonly int AniFire = Animator.StringToHash("Fire");
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Born = Animator.StringToHash("Born");

        // Start is called before the first frame update
        void Start()
        {
            isDead = false;
            isInvincible = true;
            _speedUp = false;
            _powerUp = false;
            _bulletUp = false;
            _firePoint = transform.Find("FirePoint");

            _collider = GetComponent<Collider2D>();
            ani = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
            _snowBall = transform.GetComponent<SnowBall>();
            //Renderer.color = Color.blue;
        }

        // Update is called once per frame
        void Update()
        {
            
            lifeText.text = $"Life:{life}";
            if (Time.time > startInvincibleTime + invincibleTime){
                ChangeNormal(false);
            }
            ChangeInvincible();
        }
    
        private void FixedUpdate() {
        
            if (Time.time > potionsRedTime + 20 && _speedUp){
                _speedUp = false;
                UpdateSpeed(false);
            }

            if (Time.time > potionsBlueTime + 20 && _powerUp){
                _powerUp = false;
            }

            if (Time.time > potionsYellowTime + 20 && _bulletUp){
                _bulletUp = false;
            }
            ani.SetBool(SpeedUp, _speedUp);
        }


        // ReSharper disable Unity.PerformanceAnalysis
        public void Fire()
        {
            if (_powerUp)
            {
                var bullet = Instantiate(enhancedBullets, _firePoint.position, Quaternion.identity);
                var bulletNew = bullet.GetComponent<Bullet>();
                bulletNew.dir = direction;
                if (_bulletUp) {
                    bulletNew.UpdateTime(true);
                }
            }else{
                var bullet = Instantiate(normalBullet, _firePoint.position, Quaternion.identity);
                var bulletNew = bullet.GetComponent<Bullet>();
                bulletNew.dir = direction;
                if (_bulletUp) {
                    bulletNew.UpdateTime(true);
                }
            }
            ani.SetTrigger(AniFire);
        }

        public void Pull(int dir)
        {
            _snowBall.Roll(dir);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("RollingSnowBall"))
            {
                isInvincible = true;
                startInvincibleTime = Time.time;
            }
            else if (gameObject.CompareTag("Player") && !isDead) 
            {
                if (collision.collider.CompareTag("Son") || collision.collider.CompareTag("Boss") || collision.collider.CompareTag($"BossOnStage"))
                {
                    gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
                    isDead = true;
                    ani.SetTrigger(Dead);
                }
            }
        }
 
        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateSpeed(bool up) {
            var p1Control =  GameObject.Find("Nick").GetComponent<P1Control>();
            if(up) {
                p1Control.speed *= 2;
                _speedUp = true;
            }else{
                p1Control.speed /= 2;
                _speedUp = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {

            if (collision.CompareTag("Red") )
            {
                if (_speedUp != true)
                {
                    UpdateSpeed(true);
                }
                potionsRedTime = Time.time;
                Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Blue"))
            {
                Destroy(collision.gameObject);
                potionsBlueTime = Time.time;
                _powerUp = true;
            }
            else if (collision.CompareTag("Yellow"))
            {
                _bulletUp = true;
                Destroy(collision.gameObject);
                potionsYellowTime = Time.time;
            }
            else if (collision.CompareTag("Green"))
            {
                isInvincible = true;
                startInvincibleTime = Time.time;
                Destroy(collision.gameObject);
            }
        
        }
        
        private void ChangeNormal(bool invincible){
            isInvincible = invincible;
            _renderer.color = Color.white;
            gameObject.tag = "Player";
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        private void ChangeInvincible()
        {
            if (!isInvincible) return;
            gameObject.tag = "Invincibility";
            gameObject.layer = LayerMask.NameToLayer("Invincibility");
            ChangeBodyColor();
        }

        private void ChangeBodyColor() {
            var colors = new[] {Color.cyan, Color.blue, Color.red, Color.yellow, Color.green};
            var range = Random.Range(0, colors.Length);
            if (_currentColor == colors[range]) return;
            _currentColor = colors[range];
            _renderer.color = _currentColor;
        }

        private void Resurrection(){
            ani.SetTrigger(Born);
            var p1Control =  GameObject.Find("Nick").GetComponent<P1Control>();
            p1Control.InitPosition();
            startInvincibleTime = Time.time;   
            isInvincible = true;
            isDead = false;
            GameObject o;
            (o = gameObject).layer = LayerMask.NameToLayer("Player");
            o.tag = "Player";
        }

        private void ChangeLife1() 
        {
            if (isDead && life > 0) {
                life-=1;
                Resurrection();
            }else if (life <= 0){
                if (life ==0) {
                    _collider.enabled = false;
                }
            }

        }
    }
}
