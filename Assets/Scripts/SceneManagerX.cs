using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerX : MonoBehaviour
{
   // Singleton
   public static SceneManagerX Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("_SceneManager");
                _instance = go.AddComponent<SceneManagerX>();
            }
            return _instance;
        }
    }

    private static SceneManagerX _instance;

   void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

     public void LoadScene(GameScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
