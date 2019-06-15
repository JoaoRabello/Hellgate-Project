using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D rg;
    private Animator an;
    private SpriteRenderer sp;
    public Slider slider;
    public Camera cam;
    public LayerMask jumpMask;

    [Range(1,1000)]
    public float life;

    [Range(1,1000)]
    public float moveSpeed;
    float horizontalInput;
    Vector2 lastPosition;

    //Pulo
    private bool grounded;
    private int jumpCount = 0;
    Vector2 bodySize;
    Vector2 boxSize;
    float groundedSkin = 0.5f;
    [Range(1, 1000)]
    public float velocidadePulo;
    [Range(1, 1000)]
    public float forcaPuloAlto = 2.5f;
    [Range(1, 1000)]
    public float forcaPuloBaixo = 2f;


    void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();

        lastPosition = transform.position;
        bodySize = GetComponent<CapsuleCollider2D>().size;
        boxSize = new Vector2(bodySize.x, groundedSkin);
    }

    void Update()
    {
        cam.gameObject.transform.position = new Vector3(transform.position.x,transform.position.y, cam.gameObject.transform.position.z);
        horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput != 0)
        {
            Walk();
        }
        else
        {
            an.SetBool("walking", false);
            an.SetFloat("x", 0);
        }
        Jump();

        lastPosition = transform.position;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        slider.value = life;
    }

    void Walk()
    {
        an.SetBool("walking", true);
        an.SetFloat("x", 1f);
        rg.velocity = new Vector2(horizontalInput * Time.deltaTime * (moveSpeed * 100), rg.velocity.y);

        //Visuals
        if (horizontalInput < 0)
        {
            sp.flipX = true;
        }
        else
        {
            sp.flipX = false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && (grounded || jumpCount < 1))
        {
            an.SetBool("jumping", true);
            
            rg.velocity = Vector2.up * velocidadePulo;
            grounded = false;
            jumpCount++;
        }
        else
        {
            Vector2 capsuleCenter = (Vector2)transform.position + Vector2.down * (bodySize.y + boxSize.y) * 0.5f;
            grounded = (Physics2D.OverlapBox(capsuleCenter, boxSize, 0, jumpMask) != null);
            if (grounded)
            {
                jumpCount = 0;
            }
        }

        //Better Jumping
        if (rg.velocity.y >= 0)
        {
            an.SetBool("falling", false);
        }
        else
        {
            if (rg.velocity.y < 0)
            {
                an.SetBool("jumping", false);
                an.SetBool("falling", true);
                rg.velocity += Vector2.up * Physics2D.gravity.y * (forcaPuloAlto - 1) * Time.deltaTime;
            }
            else
            {
                if (rg.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    an.SetBool("jumping", true);
                    rg.velocity += Vector2.up * Physics2D.gravity.y * (forcaPuloBaixo - 1) * Time.deltaTime;
                }
            }
        }
    }

    void Attack()
    {
        an.SetBool("attacking", true);
    }

    void EndAttack()
    {
        an.SetBool("attacking", false);
    }

    void TakeDamage(float dmg)
    {
        life -= dmg;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(dmg: c.gameObject.GetComponent<Enemy>().damage);
        }
    }
}
