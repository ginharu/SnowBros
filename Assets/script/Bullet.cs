using UnityEngine;

namespace script
{
    public class Bullet : MonoBehaviour
    {
        public int dir;
        public float speed = 7;
        private const float DestroyTime = 1;
        private float _shootingTime;
        private float _flyTime=0.5f;
        private Rigidbody2D _rigidbody;

        public void Start()
        {
        
            transform.localScale = dir == 1 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            _rigidbody = GetComponent<Rigidbody2D>();
            _shootingTime = Time.time;
        }

        private void Update()
        {
            if (Time.time >= _flyTime + _shootingTime)
            {
                _rigidbody.isKinematic = false;
                var transform1 = transform;
                transform1.position += transform1.right * (dir * 0.9f * Time.deltaTime);
            }else
            {
                var transform1 = transform;
                transform1.position += transform1.right * (dir * speed * Time.deltaTime);
            }

            if (Time.time >= _shootingTime + DestroyTime)
            {
                Destroy(this.gameObject);
            }
        }

        public void UpdateTime(bool up) {
            if (up) {
                _flyTime = _flyTime * 2;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(gameObject);
        }

    }
}
