using UnityEngine;

namespace CrystalCaveBackgroundsPixelArt
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Animator animator;

        [SerializeField]
        [Range(2.0f, 8.0f)]
        private float speed = 4.0f;

        [SerializeField]
        [Range(10.0f, 20.0f)]
        private float jumpForce = 16.0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // --- HORIZONTAL MOVEMENT ---
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

            // --- FLIP SPRITE ---
            if (horizontal != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontal), 1f, 1f);
            }

            // --- JUMP ---
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            // Short hop if jump released early
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }

            // --- ATTACK ---
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");
            }

            // --- ANIMATION PARAMETERS ---
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
            animator.SetBool("IsGrounded", IsGrounded());
        }

        // --- SIMPLE GROUND CHECK ---
        private bool IsGrounded()
        {
            return Mathf.Abs(rb.linearVelocity.y) < 0.1f;
        }
    }
}
