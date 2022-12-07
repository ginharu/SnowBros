
using UnityEngine;
using TMPro;

public class P1Player : MonoBehaviour
{

    // 红药水加成，速度增加
    private bool SpeedUp;
    // 蓝药水加成，子弹变大
    private bool PowerUp;
    // 黄药水加成，子弹飞行时间边长
    private bool BulletUp;
    // 绿药水加成，进入无敌
    private bool InvincibleUp;
    public float potionsRedTime;
    public float potionsBlueTime;
    public float potionsYellowTime;
    public float potionsGreenTime;
    // 子弹的方向
    public int direction;
    // SnowBall ball;
    // 普通子弹
    public GameObject NormalBullet;
    //  强化子弹
    public GameObject EnhancedBullets;    
    private Collider2D Collider;
    private SpriteRenderer Renderer;
    private Rigidbody2D rigBody;
    public Animator ani;
    private SnowBall snowBall;
    public int Life;
    public TextMeshProUGUI LifeText;
    public bool IsDead;
    Transform firePoint;
    [HideInInspector]
    public float startInvinsibleTime;
    public float InvincibleTime;
    public bool isInvinsible;
    private Color CurrentColor;

    // Start is called before the first frame update
    void Start()
    {
        IsDead = false;
        isInvinsible = true;
        SpeedUp = false;
        PowerUp = false;
        BulletUp = false;
        InvincibleUp = false;
        firePoint = transform.Find("FirePoint");

        Collider = GetComponent<Collider2D>();
        rigBody = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        Renderer = GetComponent<SpriteRenderer>();
        snowBall = transform.GetComponent<SnowBall>();
        //Renderer.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {
        LifeText.text = "Life: " + Life.ToString();
        if (Time.time > startInvinsibleTime + InvincibleTime){
            changeNormal(false);
        }
        changeInvincible();
    }
    
    private void FixedUpdate() {
        if (Time.time > potionsRedTime + 20 && SpeedUp == true){
            SpeedUp = false;
            updateSpeed(false);
        }

        if (Time.time > potionsBlueTime + 20 && PowerUp == true){
            PowerUp = false;
        }

        if (Time.time > potionsYellowTime + 20 && BulletUp == true){
            BulletUp = false;
        }
        ani.SetBool("SpeedUp", SpeedUp);
    }


    public void Fire()
    {
        if (PowerUp == true)
        {
            GameObject bullet = Instantiate(EnhancedBullets, firePoint.position, Quaternion.identity);
            Bullet bulletNew = bullet.GetComponent<Bullet>();
            bulletNew.dir = direction;
            if (BulletUp == true) {
                bulletNew.updateTime(true);
            }
        }else{
            GameObject bullet = Instantiate(NormalBullet, firePoint.position, Quaternion.identity);
            Bullet bulletNew = bullet.GetComponent<Bullet>();
            bulletNew.dir = direction;
            if (BulletUp == true) {
                bulletNew.updateTime(true);
            }
        }
        ani.SetTrigger("Fire");
    }

    public void Pull(int dir)
    {
        snowBall.Roll(dir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "RollingSnowBall")
        {
            isInvinsible = true;
            startInvinsibleTime = Time.time;
        }
        else if (gameObject.tag == "Player" && !IsDead) 
        {
            if (collision.collider.tag == "Son" || collision.collider.tag == "Boss" || collision.collider.tag == "BossOnStage")
            {
                gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
                IsDead = true;
                // Collider.enabled = false;
                ani.SetTrigger("Dead");
            }
        }
    }
 
    private void updateSpeed(bool up) {
        P1Control P1Control =  GameObject.Find("Nick").GetComponent<P1Control>();
        if(up) {
            P1Control.speed = P1Control.speed * 2;
            SpeedUp = true;
        }else{
            P1Control.speed = P1Control.speed / 2;
            SpeedUp = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.tag == "Red" )
        {
            if (SpeedUp != true)
            {
                updateSpeed(true);
            }
            potionsRedTime = Time.time;
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Blue")
        {
            Destroy(collision.gameObject);
            potionsBlueTime = Time.time;
            PowerUp = true;
        }
        else if (collision.tag == "Yellow")
        {
            BulletUp = true;
            Destroy(collision.gameObject);
            potionsYellowTime = Time.time;
        }
        else if (collision.tag == "Green")
        {
            isInvinsible = true;
            startInvinsibleTime = Time.time;
            Destroy(collision.gameObject);
        }
        
    }

    private void score(Transform original){
        Transform add100 = Instantiate(original, transform.position, Quaternion.identity);
    }

    
    private void OnCollisionStay2D(Collision2D other) {

    }

    public void changeNormal(bool invincible){
        isInvinsible = invincible;
        Renderer.color = Color.white;
        gameObject.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void changeInvincible(){
        if (isInvinsible) {
            gameObject.tag = "Invincibility";
            gameObject.layer = LayerMask.NameToLayer("Invincibility");
            ChangeBodyColor();
        }
    }

    private void ChangeBodyColor() {
        Color[] colors = {Color.cyan, Color.blue, Color.red, Color.yellow, Color.green};
        int range = UnityEngine.Random.Range(0, colors.Length);
        if (CurrentColor != colors[range]) {
            CurrentColor = colors[range];
            Renderer.color = CurrentColor;
        }
    }

    private void Resurrection(){
        ani.SetTrigger("Born");
        P1Control P1Control =  GameObject.Find("Nick").GetComponent<P1Control>();
        P1Control.initPosition();
        startInvinsibleTime = Time.time;   
        isInvinsible = true;
        IsDead = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        //gameObject.tag = "Player";

    }

    void ChangeLife1() 
    {
        if (IsDead && Life > 0) {
            Life-=1;
            Resurrection();
        }else if (Life < 0){
            gameObject.SetActive(false);
        }

    }
}
