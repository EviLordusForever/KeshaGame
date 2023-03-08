using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class OnStartPanel : MonoBehaviour
{
	public TextMeshProUGUI text;

	void Start()
	{
		Image img = GetComponent<Image>();

		StartCoroutine(WaitLoad());

		IEnumerator WaitLoad()
		{
			gameObject.SetActive(true);

			while (!SceneCurrentlyLoaded("Hall"))
				yield return new WaitForSeconds(0.2f);

			Color clr = img.color;
			Color tclr = text.color;
			float alfa = 1;

			while (alfa > 0)
			{
				alfa -= 0.02f;

				text.color = new Color(tclr.r, tclr.g, tclr.b, alfa);

				yield return new WaitForSeconds(0.02f);
			}

			alfa = 1;

			while (alfa > 0)
			{
				alfa -= 0.005f;

				img.color = new Color(clr.r, clr.g, clr.b, alfa);

				yield return new WaitForSeconds(0.02f);
			}

			gameObject.SetActive(false);

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
}
