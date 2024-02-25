using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    Vector3 _startPosition;
    string _key;

    AllFather _allFather;
    GameObject _playerObject;
    GameObject _playerHub;
    PlayerStorage _ps;
    string _sceneName;
    NavMeshAgent _agent;
    public float _speed;
    private float _fireCooldown;
    public float _nextFireTime;
    GameObject _theBullet;
    public bool _followPlayer;
    private Animator _ani;
    private Vector3 _pos;
    private float _realSpeed;
    private bool _run;
    public bool _dead;

    public float _maxHealth;
    public float _health;

    void Start()
    {
        _health = 100;

        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _key = _sceneName + transform.position.x + transform.position.y + transform.position.z;

        _fireCooldown = 1.3f;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = 13f;

        _ani = GetComponent<Animator>();

        _followPlayer = false;   

        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerHub = _playerObject.transform.parent.gameObject;
        PlayerMovement pm = _playerHub.GetComponent<PlayerMovement>();
        _ps = _playerHub.GetComponent<PlayerStorage>();      

        _theBullet = GameObject.Find("EnemyBullet");

        _startPosition = transform.position;

        if (_allFather.Contains(_key))
        {
            EnemySave es = _allFather.Load(_key) as EnemySave;
            transform.position = es._position;
            transform.rotation = es._rotation;
            _health = es._health;

            if (_health <= 0)
            {
                _ani.SetFloat("deathSpeed", 100f);
                Damage(1);
            }
        }

        InvokeRepeating("SavingMethod", 0f, 3f);
    }

    void SavingMethod()
    {
        EnemySave es = new EnemySave();
        es._position = transform.position;
        es._rotation = transform.rotation;
        es._health = _health;
        _allFather.Save(_key, es);
    }

    public void Damage(float amount)
    {
        _followPlayer = true;

        _health -= amount;

        if (_health <= 0)
        {
            if (!_dead)
            {
                _dead = true;
                _ani.SetTrigger("TrDie");
                _agent.speed = 0f;
                _followPlayer = false;
                _dead = true;
                Collider collider = gameObject.GetComponent<Collider>();
                Destroy(collider);
                //Destroy(gameObject, 300);
            }
        }
    }

    void Update()
    {
        if (_ps._currentSceneName == _sceneName)
        {
            Vector3 toPlayer = Camera.main.transform.position - transform.position;
            Ray ray = new Ray(transform.position, toPlayer);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            _nextFireTime -= Time.deltaTime;

            if (hit.collider.gameObject.tag == "Player")
            {
                float angle = Vector3.Angle(toPlayer, transform.forward);
                if (angle > -90f && angle < 90f)
                {
                    _followPlayer = true;
                }

                if (_followPlayer && !_dead)
                    if (_nextFireTime <= 0)
                        Fire();
            }

            if (_followPlayer)
                _agent.destination = _playerHub.transform.position;
        }
        else
        {
            _followPlayer = false;
            _agent.destination = _startPosition;
        }

        Vector3 delta = transform.position - _pos;
        _pos = transform.position;
        _realSpeed = _realSpeed * 0.9f + delta.magnitude * 0.1f;

        if (_realSpeed < 0.06f)
        {
            if (_run)
            {
                _ani.SetTrigger("TrIdle");
                _run = false;
            }
        }
        if (_realSpeed > 0.06f)
        {
            if (!_run)
            {
                _ani.SetTrigger("TrRun");
                _run = true;
            }
            _ani.SetFloat("speed", _realSpeed * 13f);
        }
    }

    void Fire()
    {
        _nextFireTime = _fireCooldown;
        GameObject bullet = Instantiate(_theBullet);
        bullet.transform.position = gameObject.transform.position + new Vector3(0, 4, 0);
        bullet.transform.LookAt(Camera.main.transform.position);
        EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
        eb._active = true;
        eb._speed = 30;
        Destroy(bullet, 15);

        AudioSource shot = Instantiate(_allFather._shot);
        shot.transform.position =transform.position;
        shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        float distance = (transform.position - _allFather._camera.transform.position).magnitude;
        shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
        shot.Play();
        Destroy(shot, 5);
    }
}
