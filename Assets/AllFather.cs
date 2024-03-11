using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllFather : MonoBehaviour
{
	Dictionary<string, object> _theSave;
	public GameObject _spot;
	public GameObject _sparkle;
	public GameObject _redSparkle;
	public List<GameObject> _spots;
	public AudioSource _caboom;
	public AudioSource _shot;
	public Camera _camera;
	public Canvas _canvas;
	public Inventory _inventory;
	public IsItem _redCrystal;
	public AudioManager _audioManager;

	public int _enemyBulletSparklesCount;

	void Start()
	{
		_theSave = new Dictionary<string, object>();
		_spots = new List<GameObject>();
		_spot.transform.SetParent(gameObject.transform);
		_sparkle.transform.SetParent(gameObject.transform);
	}

	public void Save(string key, object save)
	{
		_theSave[key] = save;
	}

	public object Load(string key)
	{
		return _theSave[key];
	}

	public bool Contains(string key)
	{
		return _theSave.ContainsKey(key);
	}

	public EnemyParams GetEnemyParams(string name)
	{
		EnemyParams ep = new EnemyParams();

		if (name == "zombie")
		{
			ep._screamerX = 0;
			ep._screamerY = -3.9f;
			ep._screamerZ = 0.60f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "professor")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.2f;
			ep._screamerZ = 0.65f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "musculus")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.6f;
			ep._screamerZ = 2.7f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "ghost")
		{
			ep._screamerX = 0;
			ep._screamerY = -3;
			ep._screamerZ = 1.5f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}

		return ep;
	}
}