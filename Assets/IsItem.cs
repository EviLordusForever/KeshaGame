using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IsItem : MonoBehaviour
{
    public string name;
    public int count;
    public bool throwable;
    public Sprite image;

    [Header("Throwing:")]

    public int rotationX;
    public int rotationY;
    public int rotationZ;

    [Header("Showing:")]

    public float showingOffset;
    public Vector3 startShowingRotation;
    public Vector3 showingRotation;
    public string showingText;
    public string showingAudioName;
    public float showingDelay;
    public string pickUpAudioName;

    [Header("Do not set:")]

    public Rigidbody rb;
    public Collider collider;
    public Renderer renderer;
    public Transform transform;
    public GameObject obj;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        obj = gameObject;
    }

    public void Hide()
    {
        if (rb != null)
            rb.isKinematic = true;

        renderer.enabled = false;
        collider.enabled = false;
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
