using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource _gong;
	public AudioSource _arfa;
	public AudioSource _door;
	public AudioSource _pickUp;
	public AudioSource _throw;
	public AudioSource _inventory;
	public AudioSource _plasma;
	public AudioSource _money;
	public AudioSource _notEnoughCash;
	public AudioSource _toiletDoor;
	public AudioSource _noAmmo;
	public AudioSource _reload;
	public AudioSource _toilet;
	public bool muted;

	public void Play(string name, float pitch)
	{
		if (muted)
			return;

		System.Random rnd = new System.Random();

		switch (name)
		{		
			case "gong":
				_gong.pitch = pitch;
				_gong.Play();
				break;
			case "arfa":
				_arfa.pitch = pitch;
				_arfa.Play();
				break;
			case "Door":
				pitch = 0.9f + (float)rnd.NextDouble() * 0.2f;
				_door.pitch = pitch;
				_door.Play();
				break;
			case "pickUp":
				pitch += -0.1f + (float)rnd.NextDouble() * 0.2f;
				_pickUp.pitch = pitch;
				_pickUp.Play();
				break;
			case "throw":
				pitch += 0.2f + (float)rnd.NextDouble() * 0.2f;
				_throw.pitch = pitch;
				_throw.Play();
				break;
			case "inventory":
				_inventory.pitch = pitch;
				_inventory.Play();
				break;
			case "plasma":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_plasma.pitch = pitch;
				_plasma.Play();
				break;
			case "money":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_money.pitch = pitch;
				_money.Play();
				break;
			case "notEnoughCash":
				pitch = 0.9f;
				_notEnoughCash.pitch = pitch;
				_notEnoughCash.Play();
				break;
			case "noAmmo":
				pitch = 0.9f;
				_noAmmo.pitch = pitch;
				_noAmmo.Play();
				break;
			case "toiletDoor":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_toiletDoor.pitch = pitch;
				_toiletDoor.Play();
				break;
			case "reload":
				pitch = 1f;
				_reload.pitch = pitch;
				_reload.Play();
				break;
			case "toilet":
				pitch = 1f;
				_toilet.pitch = pitch;
				_toilet.Play();
				break;
			default:
				Debug.Log($"No such audioSource {name}!");
				break;
		}
	}
}
