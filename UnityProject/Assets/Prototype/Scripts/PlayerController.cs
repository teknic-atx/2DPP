using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Settings
    public float speed = 15;
    public float jumpSpeed = 25;
    public float gravityScale = 8;
    public float airControl = 3;
    public float groundControl = 10;
    public float maxSpeed = 40;
    public float hitRadius = 0.6f;
    public float groundCheckOffset = 0.05f;
    LayerMask collisionMask = 1;

    // Vars
    bool bInputJump = true;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    float velocityX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        rb.gravityScale = gravityScale;
        Time.fixedDeltaTime = 1 / 125f; // Set physics tick rate
        var pm = new PhysicsMaterial2D();
        pm.friction = 0;
        rb.sharedMaterial = pm;
    }


    void FixedUpdate()
    {
        // Ground check
        RaycastHit2D groundHit = Physics2D.CircleCast(rb.position - Vector2.up * groundCheckOffset, hitRadius, Vector2.zero, 0, collisionMask.value);

        // Input
        var horizontalInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed) { horizontalInput = -1; }
        if (Keyboard.current.rightArrowKey.isPressed) { horizontalInput = 1; }

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (bInputJump && groundHit)
            {
                rb.velocity += Vector2.up * jumpSpeed;
            }
            bInputJump = false;
        }
        else
        {
            bInputJump = true;
        }

        // Sprite flip
        if (Mathf.Abs(horizontalInput) > 0.01f) { spriteRenderer.flipX = horizontalInput < 0f; }

        // Velocity
        float lerp = groundHit ? groundControl : airControl;
        velocityX = Mathf.Lerp(velocityX, horizontalInput, Time.fixedDeltaTime * lerp);
        rb.velocity = new Vector2(velocityX * speed, rb.velocity.y); ;

        // Speed limit
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * groundCheckOffset, hitRadius);
    }
}
