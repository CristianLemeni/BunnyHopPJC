using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoderScript : MonoBehaviour
{
    public Animator animator;

    public float TransitionTime = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            LoadPreviousLevel();
        }
    }

    private void LoadPreviousLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1));
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));

    }

    IEnumerator LoadLevel(int LevelIndex)
    {
        if (LevelIndex > SceneManager.sceneCount || LevelIndex < 0 ) yield break ;
        animator.SetTrigger("start");

        yield return new WaitForSeconds(TransitionTime);

        SceneManager.LoadScene(LevelIndex);
    }
}
