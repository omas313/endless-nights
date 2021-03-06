using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Animation _fadeInAnimation;
    [SerializeField] Animation _cameraAnimation;
    [SerializeField] Animation _clickToStartCanvasAnimation;
    
    bool _isLoading;

    IEnumerator LoadLevel()
    {
        _isLoading = true;

        _clickToStartCanvasAnimation.Play();
        _cameraAnimation.Play();
        _fadeInAnimation.Play();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_fadeInAnimation.isPlaying);

        SceneManager.LoadScene("Level");
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !_isLoading)
            StartCoroutine(LoadLevel());
    }
}
