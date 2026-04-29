using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private float restartDelay = 2f;
    private bool isRestarting = false;

    void OnEnable()
    {
        PlayerController.OnPlayerDied += HandlePlayerDeath;     
    }

    void OnDisable()
    {
        PlayerController.OnPlayerDied -= HandlePlayerDeath;
    }

    void HandlePlayerDeath()
    {
        if (!isRestarting)
        {
            isRestarting = true;
            StartCoroutine(RestartSequence());
        }
    }

    IEnumerator RestartSequence()
    {
        // TODO: trigger fade out here
        yield return new WaitForSeconds(restartDelay);
        SceneManagerX.Instance.RestartScene();
    }
}
