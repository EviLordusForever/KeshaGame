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
}