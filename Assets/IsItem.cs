using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IsItem : MonoBehaviour
{
    public string name;
    public int count;
    public bool throwable;
    public Sprite image;
    public string pickUpAudioName;
    public bool _protected;

    [Header("Throwing:")]

    public int rotationX;
    public int rotationY;
    public int rotationZ;    

    [Header("Do not set:")]

    public Rigidbody rb;
    public Collider collider;
    public Renderer renderer;
    public Transform _transform;
    public GameObject obj;

    AllFather _allFather;
    string _sceneName;
    string _key;

    public void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _key = _sceneName + transform.position.x + transform.position.y + transform.position.z;

        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
        _transform = GetComponent<Transform>();
        obj = gameObject;

        if (_allFather.Contains(_key))
        {
            EnemySave es = _allFather.Load(_key) as EnemySave;

            if (es._hidden)
                Hide();
            if (es._destroyed)
                Destroy();
        }
    }

    public void Hide()
    {
        if (!_protected)
        {
            if (rb != null)
                rb.isKinematic = true;

            renderer.enabled = false;
            collider.enabled = false;

            EnemySave es = new EnemySave();
            es._hidden = true;            
            _allFather.Save(_key, es);
        }
    }

    public void Destroy()
    {
        EnemySave es = new EnemySave();
        es._destroyed = true;
        _allFather.Save(_key, es);

        Destroy(gameObject);
    }

    public void Throw(Vector3 position, Vector3 direction, float power, Vector3 playerVelocity, Quaternion rotation)
    {
        transform.position = position + direction;
        transform.rotation = rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
        renderer.enabled = true;
        collider.enabled = true;

        if (rb != null)
            rb.isKinematic = false;

        rb.velocity = playerVelocity;
        rb.AddForce(direction * power * rb.mass);
    }
}
