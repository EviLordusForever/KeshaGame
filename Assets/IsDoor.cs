using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IsDoor : MonoBehaviour
{
	//public
	float direction = 0f;

	public void Move()
	{
		Quaternion rotation = transform.rotation;
		Debug.Log("Object rotation: " + rotation.eulerAngles.y);

		if (rotation.eulerAngles.y > 45f)
			direction = -1f;
		else
			direction = 1f;
	}

	public void Update()
	{
		if (direction != 0)
		{
			Quaternion rotation = transform.rotation;

			if (rotation.eulerAngles.y > 90f && direction == 1f)
				direction = 0f;
			if (rotation.eulerAngles.y < 1.5f && direction == -1f)
				direction = 0f;

			transform.Rotate(0f, direction, 0f);

			Debug.Log("Object rotation: " + rotation.eulerAngles.y);
		}
	}
}
