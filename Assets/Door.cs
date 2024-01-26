using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class Door : MonoBehaviour
{
	public Collider col;
	public string[] loadScenesNames;
	public string[] unloadScenesNames;
	public string nextSceneName;
	public Vector3 position;
	private AudioManager audioManager;
	public string audioName;

	private void OnTriggerEnter(Collider collider)
	{
		Go(collider.gameObject);
	}

	public void Go(GameObject playerObject)
	{
		if (playerObject.tag == "Player")
		{
			if (audioManager == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
				audioManager = go.GetComponent<AudioManager>();
			}

			audioManager.Play(audioName, 1);

			foreach (string name in loadScenesNames)
				SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

			foreach (string name in unloadScenesNames)
				SceneManager.UnloadSceneAsync(name);

			StartCoroutine(WaitLoad(playerObject));
		}
	}

	IEnumerator WaitLoad(GameObject playerObject)
	{
		while (!SceneCurrentlyLoaded(nextSceneName))
			yield return new WaitForSeconds(0.2f);

		GameObject playerHub = playerObject.transform.parent.gameObject;
		PlayerMovement pm = playerHub.GetComponent<PlayerMovement>();

		Vector3 v = new Vector3(0, 0, 0);
		if (pm.isCrouching)
			v = new Vector3(0, -2.2f, 0);

		playerHub.transform.position = position + v;

		yield return null;
	}

	bool SceneCurrentlyLoaded(string sceneName_no_extention)
	{
		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			if (scene.name == sceneName_no_extention)
			{
				if (scene.isLoaded)
					return true;
				else
					return false;
			}
		}

		return false;//scene not currently loaded in the $$anonymous$$erarchy
	}
}