using UnityEngine;

namespace CrystalCaveBackgroundsPixelArt
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCol;

        [Header("Movement")]
        [SerializeField, Range(2f, 8f)]
        private float speed = 4f;

        [SerializeField, Range(10f, 20f)]
        private float jumpForce = 16f;

        [Header("Crouch")]
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField, Range(0.2f, 1f)]
        private float crouchSpeedMultiplier = 0.4f;

        [SerializeField] private Vector2 crouchColliderSize = new Vector2(0.6f, 0.9f);
        [SerializeField] private Vector2 crouchColliderOffset = new Vector2(0f, 0f);

        private Vector2 originalColliderSize;
        private Vector2 originalColliderOffset;

        [Header("Scale")]
        [SerializeField]
        private Vector3 lockedScale = new Vector3(5f, 5f, 5f);

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCol = GetComponent<BoxCollider2D>();

            // Save original collider values
            originalColliderSize = boxCol.size;
            originalColliderOffset = boxCol.offset;

            // Lock scale once — NEVER touch it again
            transform.localScale = lockedScale;
        }

        private void Update()
        {
            // --- CROUCH ---
            bool isCrouching = Input.GetKey(crouchKey);
            animator.SetBool("IsCrouching", isCrouching);

            if (isCrouching)
            {
                boxCol.size = crouchColliderSize;
                boxCol.offset = crouchColliderOffset;
            }
            else
            {
                boxCol.size = originalColliderSize;
                boxCol.offset = originalColliderOffset;
            }

            // --- MOVEMENT ---
            float horizontal = Input.GetAxisRaw("Horizontal");

            float currentSpeed = speed;
            if (isCrouching)
                currentSpeed *= crouchSpeedMultiplier;

            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);

            // --- FLIP SPRITE (NO NEGATIVE SCALE) ---
            if (horizontal < 0)
                spriteRenderer.flipX = true;
            else if (horizontal > 0)
                spriteRenderer.flipX = false;

            // --- JUMP (disabled while crouching) ---
            if (!isCrouching && Input.GetButtonDown("Jump") && IsGrounded())
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
