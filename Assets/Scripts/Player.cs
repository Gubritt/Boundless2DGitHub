using UnityEngine;

namespace CrystalCaveBackgroundsPixelArt
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        [Header("Movement")]
        [SerializeField, Range(2f, 8f)]
        private float speed = 4f;

        [SerializeField, Range(10f, 20f)]
        private float jumpForce = 16f;

        [Header("Scale")]
        [SerializeField]
        private Vector3 lockedScale = new Vector3(5f, 5f, 5f);

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Lock scale once — NEVER touch it again
            transform.localScale = lockedScale;
        }

        private void Update()
        {
            // --- MOVEMENT ---
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

            // --- FLIP SPRITE (NO NEGATIVE SCALE) ---
            if (horizontal < 0)
                spriteRenderer.flipX = true;
            else if (horizontal > 0)
                spriteRenderer.flipX = false;

            // --- JUMP ---
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            // Short hop
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }

            // --- ATTACK ---
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");
            }

            // --- ANIMATION ---
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
            animator.SetBool("IsGrounded", IsGrounded());
        }

        private bool IsGrounded()
        {
            return Mathf.Abs(rb.linearVelocity.y) < 0.1f;
        }
    }
}
