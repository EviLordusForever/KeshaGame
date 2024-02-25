using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStorage : MonoBehaviour
{
    public string _currentSceneName;
    public float _health;
    public Image _healthImage;
    public GameObject _onDiePanel;

    void Start()
    {
        _health = 100f;
    }

    public void Damage(float amount)
    {
        _health -= amount;
        _healthImage.fillAmount = _health / 100f;

        if (_health <= 0f)
        {
            StartCoroutine(WaitForThreeSeconds());

            IEnumerator WaitForThreeSeconds()
            {
                _onDiePanel.SetActive(true);
                yield return new WaitForSeconds(4f);

                // Unload all scenes
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    SceneManager.UnloadSceneAsync(scene);
                }

                // Load the "start" scene
                SceneManager.LoadScene("start");
            }
        }       
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

        return false;
    }

    public void Heal(float amount)
    {
        _health += amount;
        if (_health > 100f)
            _health = 100f;
        _healthImage.fillAmount = _health / 100f;
    }
}