using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IsDoor : MonoBehaviour
{
	float direction;
	float zrx;
	float zry;
	float zrz;
	float cr;
	private AudioManager _audioManager;

	private void Start()
	{
		direction = 0f;
		zrx = transform.rotation.eulerAngles.x;
		zry = transform.rotation.eulerAngles.y;
		zrz = transform.rotation.eulerAngles.z;
	}

	public void Move()
	{
		if (_audioManager == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
			_audioManager = go.GetComponent<AudioManager>();
		}
		_audioManager.Play("toiletDoor", 1);

		Quaternion rotation = transform.rotation;

		if (cr < -45)
			direction = 1.5f;
		else
			direction = -1.5f;
	}

	public void Close()
	{
		direction = 1.5f;
	}

	public bool Closed
	{
		get
		{
			return cr >= 0;
		}
	}

	public void Update()
	{
		if (direction != 0)
		{
			if (direction > 0 && cr >= 0)
				direction = 0f;
			if (direction < 0 && cr <= -90)
				direction = 0f;

			cr += direction * Time.deltaTime * 60;
			transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
		}
	}
}
