using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerX : MonoBehaviour
{
   // Singleton
   public static SceneManagerX Instance;

   void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

     public void LoadScene(GameScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
