using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rg;

    [Range(1, 10)]
    public int hitsToDie;
    [Range(1,1000)]
    public float speed;
    [Range(1, 1000)]
    public float damage;
    [Range(1,10000)]
    public float knockbackForce;
    private bool canMove = true;
    private bool canMoveL = true;
    private bool canMoveR = true;
    private string moveDir = "Left";

    public LayerMask mask;

    float playerDir;

    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        rg = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GroundCheck();
        if (canMove)
        {
            if( (transform.position.x - player.transform.position.x < 4 && transform.position.x - player.transform.position.x > -4) && 
                (transform.position.y - player.transform.position.y < 0.5f && transform.position.y - player.transform.position.y > -0.5f))
            {
                ChasePlayer();
            }
            else
            {
                MoveTo(moveDir);
                if (canMoveL && !canMoveR)
                {
                    moveDir = "Left";
                }
                else
                {
                    if (canMoveR && !canMoveL)
                    {
                        moveDir = "Right";
                    }
                }
            }
        }
    }

    void ChasePlayer()
    {
        playerDir = Mathf.Clamp(player.transform.position.x - transform.position.x, -1f, 1f);
        if (playerDir > 0 && canMoveR)
        {
            rg.velocity = new Vector2(playerDir * Time.deltaTime * speed, rg.velocity.y);
        }
        else
        {
            if (playerDir < 0 && canMoveL)
            {
                rg.velocity = new Vector2(playerDir * Time.deltaTime * speed, rg.velocity.y);
            }
            else
            {
                rg.velocity = Vector2.zero;
            }
        }
    }
    
    void MoveTo(string dir)
    {
        switch (dir) {
            case "Left":
                rg.velocity = new Vector2(-1 * speed * Time.deltaTime, rg.velocity.y);
                break;
            case "Right":
                rg.velocity = new Vector2(1 * speed * Time.deltaTime, rg.velocity.y);
                break;
        }
    }


    void GroundCheck()
    {
        bool rayRight = Physics2D.Raycast((Vector2)transform.position + Vector2.right * 0.75f, Vector2.down, 2.5f, mask);
        bool rayLeft = Physics2D.Raycast((Vector2)transform.position + Vector2.left * 0.75f, Vector2.down, 2.5f, mask);

        if (rayRight && rayLeft)
        {
            canMoveL = true;
            canMoveR = true;
        }
        else
        {
            if (!rayRight)
            {
                canMoveR = false;
            }
            else
            {
                if (!rayLeft)
                {
                    canMoveL = false;
                }
            }
        }
    }

    public void TakeDamage()
    {
        if(hitsToDie <= 1)
        {
            Destroy(gameObject);
        }
        else
        {
            hitsToDie--;
            rg.AddForce(new Vector2(transform.position.x - player.transform.position.x, 0f) * knockbackForce, ForceMode2D.Force);
            StartCoroutine(Daze(0.5f));
        }
    }

    IEnumerator Daze(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("DamageArea"))
        {
            TakeDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            canMove = false;
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            canMove = true;
        }
    }
}
