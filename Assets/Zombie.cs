using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    public string _type;
    public float _fireCooldown;
    public float _health;
    public float _speed;
    public float _heigh;
    public float _stopSpeed;
    public float _animationSpeed;

    Vector3 _startPosition;
    string _key;

    AllFather _allFather;
    GameObject _playerObject;
    GameObject _playerHub;
    PlayerStorage _ps;
    string _sceneName;
    NavMeshAgent _agent;    
    
    public float _nextFireTime;
    GameObject _theBullet;
    public bool _followPlayer;
    private Animator _ani;
    private Vector3 _pos;
    private float _realSpeed;
    private bool _run;
    public bool _dead;
    bool _screamerStarted;
    Collider _collider;
    EnemyParams _ep;

    void Start()
    {      
        if (_health == 0)
            _health = 100;
        if (_fireCooldown == 0)
            _fireCooldown = 1.3f;
        if (_speed == 0)
            _speed = 13f;
        if (_heigh == 0)
            _heigh = 4f;
        if (_stopSpeed == 0)
            _stopSpeed = 0.06f;
        if (_animationSpeed == 0)
            _animationSpeed = 13f;

        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        _ep = _allFather.GetEnemyParams(_type);
        //Debug.Log(_ep);

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _key = _sceneName + transform.position.x + transform.position.y + transform.position.z;
        
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;

        _ani = GetComponent<Animator>();

        _collider = gameObject.GetComponent<Collider>();

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

            gameObject.SetActive(false);
            transform.position = es._position;
            transform.rotation = es._rotation;
            _health = es._health;
            gameObject.SetActive(true);

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
                Destroy(_collider);
            }
        }
    }

    void Update()
    {
        if (!_dead)
        {
            if (!_screamerStarted)
            {
                if (_ps._currentSceneName == _sceneName)
                {
                    Vector3 from = transform.position + new Vector3(0, _heigh, 0);
                    Vector3 toPlayer = Camera.main.transform.position - from;
                    Ray ray = new Ray(from, toPlayer);
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
                    {
                        _agent.destination = _playerHub.transform.position;
                        Screamer();
                    }
                }
                else
                {
                    _followPlayer = false;
                    _agent.destination = _startPosition;
                }

                Vector3 delta = transform.position - _pos;
                _pos = transform.position;
                _realSpeed = _realSpeed * 0.9f + delta.magnitude * 0.1f;

                if (_realSpeed < _stopSpeed)
                {
                    if (_run)
                    {
                        _ani.SetTrigger("TrIdle");
                        _run = false;
                    }

                    if (_followPlayer)
                        LookToPlayer();
                }
                if (_realSpeed > _stopSpeed)
                {
                    if (!_run)
                    {
                        _ani.SetTrigger("TrRun");
                        _run = true;
                    }

                    _ani.SetFloat("speed", _realSpeed * _animationSpeed);
                }                
            }
            else
            {
                _realSpeed = 0f;
                _ani.SetFloat("speed", 2f * _animationSpeed);

                transform.position = Camera.main.transform.position;
                transform.rotation = Camera.main.transform.rotation;

                transform.position += transform.right * _ep._screamerX;
                transform.position += transform.up * _ep._screamerY;
                transform.position += transform.forward * _ep._screamerZ;
                transform.Rotate(0, 180, 0);
            }
        }
    }

    void LookToPlayer()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Screamer()
    {
        if (!_screamerStarted)
            if (UnityEngine.Random.Range(0, 1200) < 1)
                StartCoroutine(ScreamerC());

        IEnumerator ScreamerC()
        {
            _screamerStarted = true;
            _collider.enabled = false;
            _agent.enabled = false;

            Vector3 normalPosition = transform.position;
            Quaternion normalRotation = transform.rotation;

            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
            
            transform.position += transform.right * _ep._screamerX;
            transform.position += transform.up * _ep._screamerY;
            transform.position += transform.forward * _ep._screamerZ;
            transform.Rotate(0, 180, 0);            

            int number = UnityEngine.Random.Range(0, _ep._screamerSounds.Length);
            string audio = _ep._screamerSounds[number];
            _allFather._audioManager.Play(audio, 1);

            yield return new WaitForSeconds(0.7f);

            _screamerStarted = false;

            yield return new WaitForSeconds(0.1f);

            transform.position = normalPosition;
            transform.rotation = normalRotation;

            _agent.enabled = true;
            _collider.enabled = true;
        }
    }

    void Fire()
    {
        _nextFireTime = _fireCooldown;
        GameObject bullet = Instantiate(_theBullet);
        bullet.transform.position = gameObject.transform.position + new Vector3(0, _heigh, 0);
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
