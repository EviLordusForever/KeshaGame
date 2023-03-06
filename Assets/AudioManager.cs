using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource gong;
	public AudioSource arfa;

	public void Play(string name)
	{
		switch (name)
		{
			case "gong":
				gong.Play();
				break;
			case "arfa":
				arfa.Play();
				break;
			default:
				break;
		}
	}
}
