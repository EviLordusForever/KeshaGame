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
	public GameObject everything;

	private void OnTriggerEnter(Collider col2)
	{
		if (col2.gameObject.tag == "Player")
		{
			//DontDestroyOnLoad(player);

			foreach (string name in loadScenesNames)
				SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

			foreach (string name in unloadScenesNames)
				SceneManager.UnloadSceneAsync(name);

			StartCoroutine(WaitLoad(col2));
		}
	}

	/*#if UNITY_EDITOR
		bool SceneCurrentlyLoaded(string sceneName_no_extention)
		{
			 for(int i=0; i< UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i) {
				 var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);

				 if(scene.name == sceneName_no_extention) 
				 {
					if (scene.isLoaded)
						return true;
					else
						return false;
				 }
			 }
			 //scene not currently loaded in the $$anonymous$$erarchy:
			 return false;
		 }
	#endif*/

	IEnumerator WaitLoad(Collider col2)
	{
		while (!SceneCurrentlyLoaded(nextSceneName))
		{
			//Debug.Log("no");
			yield return new WaitForSeconds(0.2f);
		}

		GameObject player = col2.gameObject.transform.parent.gameObject;
		player.transform.position = position;

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
