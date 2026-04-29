using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public GameScene nextScene;
    private int playersAtExit = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersAtExit++;

            if (playersAtExit >= 2)
            {
                SceneManagerX.Instance.LoadScene(nextScene);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersAtExit--;
        }
    }
}
