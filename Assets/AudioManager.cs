using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource gong;
	public AudioSource arfa;
	public AudioSource door;
	public AudioSource pickUp;
	public AudioSource _throw;
	public AudioSource inventory;
	public bool muted;

	public void Play(string name, float pitch)
	{
		if (muted)
			return;

		System.Random rnd = new System.Random();

		switch (name)
		{		
			case "gong":
				gong.pitch = pitch;
				gong.Play();
				break;
			case "arfa":
				arfa.pitch = pitch;
				arfa.Play();
				break;
			case "Door":
				pitch = 0.9f + (float)rnd.NextDouble() * 0.2f;
				door.pitch = pitch;
				door.Play();
				break;
			case "pickUp":
				pitch += -0.1f + (float)rnd.NextDouble() * 0.2f;
				pickUp.pitch = pitch;
				pickUp.Play();
				break;
			case "throw":
				pitch += 0.2f + (float)rnd.NextDouble() * 0.2f;
				_throw.pitch = pitch;
				_throw.Play();
				break;
			case "inventory":
				inventory.pitch = pitch;
				inventory.Play();
				break;
			default:
				break;
		}
	}
}
