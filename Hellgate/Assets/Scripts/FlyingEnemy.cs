using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    private bool _playerOnSight = false;
    private GameObject _player;

    private float _shootCounter = 1f;
    private float _timeToShoot = 2f;

    [SerializeField] private GameObject _bullet;

    private void Update()
    {
        if (_playerOnSight)
        {
            CountAimShoot();
        }
    }

    private void CountAimShoot()
    {
        if(_shootCounter >= _timeToShoot)
        {
            Vector2 dir = (_player.transform.position - transform.position).normalized;
            Shoot(dir);
            _shootCounter = 0;
        }
        else
        {
            _shootCounter += Time.deltaTime;
        }
    }

    private void Shoot(Vector2 direction)
    {
        Bullet bullet = Instantiate(_bullet, transform).GetComponent<Bullet>();
        bullet.Direction = direction;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _playerOnSight = true;
            _player = col.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _playerOnSight = false;
            _player = null;
        }    
    }
}
