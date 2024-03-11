using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Spider : MonoBehaviour
{
    Vector3 _startPosition;
    string _key;

    AllFather _allFather;
    GameObject _playerObject;
    GameObject _playerHub;
    PlayerStorage _ps;
    NavMeshAgent _agent;
    public float _speed;

    string _sceneName;

    private float _laserCooldown;
    private float _fireCooldown;
    public float _nextLaserTime;
    public float _nextFireTime;

    GameObject _theBullet;
    Camera _camera;
    public bool _followPlayer;
    private Animator _ani;
    private Vector3 _pos;
    private float _realSpeed;
    private bool _run;
    public bool _dead;

    public float _health;

    public GameObject _ray;
    private GameObject _sparkle;

    public Vector3[] _directions;
    public GameObject[] _rays;

    void Start()
    {
        _startPosition = transform.position;
        _health = 300f;
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _key = _sceneName + transform.position.x + transform.position.y + transform.position.z;

        _directions = new Vector3[4];
        for (int i = 0; i < 4; i++)
            _directions[i] = new Vector3();

        _rays = new GameObject[4];

        _ray = GameObject.Find("TheRedRay");

        
        _sparkle = _allFather._redSparkle;

        _laserCooldown = 0.3f;
        _fireCooldown = 0.7f;

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = 13f;

        _ani = GetComponent<Animator>();        

        _followPlayer = false;              

        _theBullet = GameObject.Find("EnemyBullet");
        _camera = Camera.main;

        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerHub = _playerObject.transform.parent.gameObject;
        PlayerMovement pm = _playerHub.GetComponent<PlayerMovement>();
        _ps = _playerHub.GetComponent<PlayerStorage>();

        for (int i = 0; i < 4; i++)
        {
            _directions[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-1f, 1f));
        }

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
                for (int i = 0; i < 4; i++)
                    Destroy(_rays[i]);

                _dead = true;

                _ani.SetTrigger("TrDie");

                _agent.speed = 0f;

                _followPlayer = false;

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
            Vector3 toPlayer = _camera.transform.position - transform.position;
            Ray ray = new Ray(transform.position, toPlayer);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            _nextLaserTime -= Time.deltaTime;

            if (hit.collider.gameObject.tag == "Player")
            {
                float angle = Vector3.Angle(toPlayer, transform.forward);
                if (angle > -90f && angle < 90f)
                {
                    _followPlayer = true;
                }
            }

            if (_followPlayer && !_dead)
            {
                Laser();
                Fire();
                _agent.destination = _playerHub.transform.position;
            }                
        }
        else
        {
            _followPlayer = false;
            _agent.destination = _startPosition;

            for (int i = 0; i < 4; i++)
                if (_rays[i] != null)
                {
                    Destroy(_rays[i]);
                    _rays[i] = null;
                }
        }

        Vector3 delta = transform.position - _pos;
        _pos = transform.position;
        _realSpeed = _realSpeed * 0.5f + delta.magnitude * 0.5f;

        if (_realSpeed < 0.03f)
        {
            if (_run)
            {
                _ani.SetTrigger("TrIdle");
                _run = false;
            }
        }
        if (_realSpeed > 0.03f)
        {
            if (!_run)
            {
                _ani.SetTrigger("TrRun");
                _run = true;
            }
            _ani.SetFloat("speed", _realSpeed * 8f);
        }
    }

    void Laser()
    {
        if (_nextLaserTime <= 0)
        {
            _nextLaserTime = _laserCooldown;

            _directions[Convert.ToInt32(UnityEngine.Random.Range(0, 3))] = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-1f, 1f));
        }

        Vector3 from = transform.position + new Vector3(0, 2, 0);

        for (int i = 0; i < 4; i++)
        {
            float angle = transform.rotation.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);

            Ray ray = new Ray(from, /*rotation * */ _directions[i]);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (_rays[i] == null)
                    _rays[i] = Instantiate(_ray);

                float rx = (hit.point.x + from.x) / 2;
                float ry = (hit.point.y + from.y) / 2;
                float rz = (hit.point.z + from.z) / 2;
                _rays[i].transform.position = new Vector3(rx, ry, rz);
                _rays[i].transform.rotation = Quaternion.LookRotation(hit.point - from);

                float desiredLength = (hit.point - from).magnitude;

                var scale = _rays[i].transform.localScale;
                float factor = 100;
                scale.z = desiredLength * factor;
                _rays[i].transform.localScale = scale;

/*                if (UnityEngine.Random.Range(0, 4) < 1)
                {
                    GameObject sparkle = Instantiate(_sparkle);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                    sparkle.GetComponent<IsSparkle>()._active = true;
                }*/

                GameObject go = hit.collider.gameObject;
                if (go.CompareTag("Player"))
                {
                    PlayerStorage ps = go.transform.parent.gameObject.GetComponent<PlayerStorage>();
                    ps.Damage(0.75f);
                }
            }
        }
    }

    void Fire()
    {
        _nextFireTime -= Time.deltaTime;

        if (_nextFireTime <= 0)
        {
            _nextFireTime = _fireCooldown;
            GameObject bullet = Instantiate(_theBullet);
            bullet.transform.position = gameObject.transform.position + new Vector3(0, 2, 0);
            //bullet.transform.LookAt(_camera.transform.position);
            bullet.transform.Rotate(0, UnityEngine.Random.Range(0f, 360f), 0);
            EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
            eb._active = true;
            eb._speed = 30;
            Destroy(bullet, 15);

            AudioSource shot = Instantiate(_allFather._shot);
            shot.transform.position = transform.position;
            shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            float distance = (transform.position - _allFather._camera.transform.position).magnitude;
            shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
            shot.Play();
            Destroy(shot, 5);
        }
    }
}
