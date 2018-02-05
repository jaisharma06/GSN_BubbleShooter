using UnityEngine;
using System.Collections;

namespace BubbleShooter
{
    public class BubbleController : MonoBehaviour
    {
        public BubbleModel bubble;

        #region View
        public float leftBorder;
        public float rightBorder;
        public float topBorder;

        public float speed;
        public float radius;
        public float angle;
        public bool isMoving;
        #endregion

        private const float destroySpeed = 10.0f;

        MotionDetectionDelegate motionDelegate;
        public delegate bool MotionDetectionDelegate(Vector3 position);

        CollisionDetectionDelegate collisionDelegate;
        public delegate void CollisionDetectionDelegate(GameObject bubble);

        public CollisionDetectionDelegate CollisionDelegate
        {
            set
            {
                collisionDelegate = value;
            }
        }
        public MotionDetectionDelegate MotionDelegate
        {
            set
            {
                motionDelegate = value;
            }
        }

        void Awake()
        {
            bubble = new BubbleModel(Utils.GetRandomEnum<BubbleColor>());
        }

        void Start()
        {
            GetComponent<SpriteRenderer>().color = Utils.BubbleColorToColor(bubble.color);
        }

        void Update()
        {
            if (isMoving)
            {
                this.transform.Translate(Vector3.right * speed * Mathf.Cos(Mathf.Deg2Rad * angle) * Time.deltaTime);
                this.transform.Translate(Vector3.up * speed * Mathf.Sin(Mathf.Deg2Rad * angle) * Time.deltaTime);
                if (this.motionDelegate != null)
                {
                    if (!motionDelegate(transform.position))
                    {
                        transform.Translate(Vector3.left * speed * Mathf.Cos(Mathf.Deg2Rad * angle) * Time.deltaTime);
                        transform.Translate(Vector3.down * speed * Mathf.Sin(Mathf.Deg2Rad * angle) * Time.deltaTime);
                        isMoving = false;
                        if (collisionDelegate != null)
                        {
                            collisionDelegate(this.gameObject);
                        }
                    }
                    else
                    {
                        UpdateDirection();
                    }
                }
            }
        }

        public void DestroyBubble(bool explodes)
        {
            StopAllCoroutines();
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
            if (explodes)
            {
               Bounce();
            }
            else
            {
                Vector3 killPosition = new Vector3(transform.position.x, 0f, 0f);
                float distance = Vector3.Distance(transform.position, killPosition);
                MoveTo(killPosition, distance / destroySpeed);
            }
        }

        public void MoveTo(Vector3 destination, float duration)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", destination, "time", duration, "oncomplete", "OnComplete", "oncompletetarget", gameObject));
        }

        private void Bounce()
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + new Vector3(0, 0.5f, 0), "time", 0.1f, "oncomplete", "OnBounce"));
        }

        private void OnBounce()
        {
            MoveTo(transform.position + new Vector3(0, -10, 0), 4f);
        }

        private void OnComplete()
        {
            if (GetComponent<Rigidbody2D>() == null)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (this.isMoving)
            {
                this.isMoving = false;
                if (collisionDelegate != null)
                {
                    collisionDelegate(gameObject);
                }
            }
        }

        void UpdateDirection()
        {
            if (this.transform.position.x + radius >= rightBorder || transform.position.x - radius <= leftBorder)
            {
                angle = 180.0f - angle;
                if (transform.position.x + radius >= rightBorder)
                    transform.position = new Vector3(rightBorder - radius, transform.position.y, transform.position.z);
                if (transform.position.x - radius <= leftBorder)
                    transform.position = new Vector3(leftBorder + radius, transform.position.y, transform.position.z);
            }

            if (transform.position.y + radius >= topBorder)
            {
                isMoving = false;
                if (collisionDelegate != null)
                {
                    collisionDelegate(gameObject);
                }
            }
        }
    }
}