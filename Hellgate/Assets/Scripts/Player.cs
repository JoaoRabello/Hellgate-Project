﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private enum State { NORMAL, BASHING, DASHING };
    private State state;

    private enum Appearance { ASMODEUS, SLAVE, DOMINATRIX };
    private Appearance appearance = Appearance.ASMODEUS;

    private Rigidbody2D rg;
    private Animator an;
    private SpriteRenderer sp;
    
    public Slider lifeSlider;
    public Slider tankSlider;
    public LayerMask jumpMask;

    [Range(1,1000)]
    public float totalHP = 100f;
    private float actualHP;
    public float totalTankHP = 50f;
    private float actualTankHP;
    public Transform attackAreaR;
    public Transform attackAreaL;
    private float counter = 0f;
    private float timeToAttack = 0.75f;
    private bool canAttack = true;

    private float strength = 100f;
    private float weaponStrength = 10f;
    private float atk = 10f;
    public static float bloodlustDamage;

    [Range(1,1000)]
    public float moveSpeed;
    [SerializeField] private float dashSpeed;
    float horizontalInput;
    bool lookingRight = true;

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

    private bool _isBashArea = false;
    private bool _canBash = true;
    private bool _enemyGrab = false;
    private GameObject _enemyBashed;

    private bool _canDash = true;

    void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();

        bodySize = GetComponent<CapsuleCollider2D>().size;
        boxSize = new Vector2(bodySize.x, groundedSkin);

        actualHP = totalHP;
        actualTankHP = totalTankHP;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        switch (state)
        {
            case State.NORMAL:
                if (horizontalInput != 0)
                {
                    Walk();
                }
                else
                {
                    an.SetBool("walking", false);
                    an.SetFloat("x", 0);
                }

                Jump();

                AttackInputCheck();
                DashInputCheck();
                BashInputCheck();
                break;

            case State.BASHING:
                BashInputCheck();

                if(horizontalInput > 0)
                {
                    _enemyBashed.transform.Rotate(0f, 0f, -1f);
                }
                else
                {
                    if(horizontalInput < 0)
                    {
                        _enemyBashed.transform.Rotate(0f, 0f, 1f);
                    }
                }
                break;
            case State.DASHING:
                Dash();
                break;
            default:
                break;
        }

        switch (appearance)
        {
            case Appearance.ASMODEUS:

                break;
            case Appearance.SLAVE:

                break;
            case Appearance.DOMINATRIX:

                break;
            default:
                break;
        }

        lifeSlider.value = actualHP;
        tankSlider.value = actualTankHP;
    }

    #region Dash Behaviour
    void DashInputCheck()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _canDash)
        {
            state = State.DASHING;
            StartCoroutine(DashTime());
        }
    }

    void Dash()
    {
        if(horizontalInput > 0)
        {
            rg.velocity = new Vector2((dashSpeed * 100) * Time.deltaTime, 0);
        }
        else
        {
            if(horizontalInput < 0)
            {
                rg.velocity = new Vector2(-(dashSpeed * 100) * Time.deltaTime, 0);
            }
        }
    }

    IEnumerator DashTime()
    {
        _canDash = false;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
        yield return new WaitForSeconds(0.3f);
        state = State.NORMAL;
        rg.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        _canDash = true;
    }
    #endregion

    #region Bash Behaviour
    void BashInputCheck()
    {
        if (Input.GetKeyDown(KeyCode.E) && _canBash && _isBashArea && !_enemyGrab)
        {
            GrabEnemy(_enemyBashed);
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.E) && _canBash && _isBashArea && _enemyGrab)
            {
                Bash(_enemyBashed);
            }
        }
    }

    void GrabEnemy(GameObject enemy)
    {
        state = State.BASHING;
        transform.position = enemy.transform.Find("GrabPosition").transform.position;
        transform.SetParent(enemy.transform);
        rg.velocity = Vector2.zero;
        rg.isKinematic = true;
        _enemyGrab = true;
    }
    
    void Bash(GameObject enemy)
    {
        state = State.NORMAL;
        transform.SetParent(null);
        rg.isKinematic = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        Vector2 dir = (enemy.transform.position - transform.position).normalized;

        enemy.GetComponent<Rigidbody2D>().AddForce(dir * 5f, ForceMode2D.Impulse);
        rg.AddForce(-dir * 15f, ForceMode2D.Impulse);
    }
    #endregion

    #region Normal Behaviour
    void AttackInputCheck()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
        {
            Attack();
            canAttack = false;
            counter = 0;
        }

        if (counter >= timeToAttack)
        {
            canAttack = true;
        }
        else
        {
            counter += Time.deltaTime;
        }
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
            lookingRight = false;
        }
        else
        {
            sp.flipX = false;
            lookingRight = true;
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
    #endregion

    #region Combat Behaviour
    void Attack()
    {
        float damage = (strength + weaponStrength + atk + Random.Range(20f, 40f)) / 2;
        bloodlustDamage = damage * (((((actualHP / totalHP) * 100) - 100) * (-1) / (200)) + 1);
        print(bloodlustDamage);
        an.SetBool("attacking", true);
    }

    void EndAttack()
    {
        an.SetBool("attacking", false);
    }

    void SpawnAttack()
    {
        if (lookingRight)
        {
            attackAreaL.gameObject.SetActive(false);
            attackAreaR.gameObject.SetActive(true);
        }
        else
        {
            attackAreaR.gameObject.SetActive(false);
            attackAreaL.gameObject.SetActive(true);
        }
    }

    void DespawnAttack()
    {
        attackAreaR.gameObject.SetActive(false);
        attackAreaL.gameObject.SetActive(false);
    }

    void TakeDamage(float dmg)
    {
        switch (appearance)
        {
            case Appearance.ASMODEUS:

                if(actualHP > 0 && actualHP > dmg)
                {
                    print("Toma dano");
                    actualHP -= dmg;
                }
                else
                {
                    if((actualHP <= 0 || actualHP <= dmg) && actualTankHP > 0)
                    {
                        print("Ressuscita");
                        Ressurrect();
                    }
                    else
                    {
                        print("Morre");
                        //Die();
                    }
                }

                break;
            case Appearance.SLAVE:

                break;
            case Appearance.DOMINATRIX:

                break;
            default:
                break;
        }
    }

    void Ressurrect()
    {
        actualHP = 0f;
        lifeSlider.value = actualHP;

        actualHP += actualTankHP;

        actualTankHP = 0;
        tankSlider.value = 0f;
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(dmg: c.gameObject.GetComponent<Enemy>().damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("BashArea"))
        {
            _isBashArea = true;
            _enemyBashed = c.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("BashArea"))
        {
            _isBashArea = false;
            _enemyBashed = null;
        }
    }
}
