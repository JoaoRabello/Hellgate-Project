using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rg;

    [Range(1,1000)]
    public float speed;
    [Range(1, 1000)]
    public float damage;
    private bool canMove = true;
    private bool canMoveL = true;
    private bool canMoveR = true;
    public LayerMask mask;

    void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        rg = GetComponent<Rigidbody2D>();
    }

    void Update()
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

        if (canMove)
        {
            float dir = Mathf.Clamp(player.transform.position.x - transform.position.x, -1f, 1f);
            if(dir > 0 && canMoveR)
            {
                rg.velocity = new Vector2(dir * Time.deltaTime * speed, rg.velocity.y);
            }
            else
            {
                if (dir < 0 && canMoveL)
                {
                    rg.velocity = new Vector2(dir * Time.deltaTime * speed, rg.velocity.y);
                }
                else
                {
                    rg.velocity = Vector2.zero;
                }
            }
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
