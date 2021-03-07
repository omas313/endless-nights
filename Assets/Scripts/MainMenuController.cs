using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Animation _fadeInAnimation;
    [SerializeField] Animation _cameraAnimation;
    [SerializeField] Animation _clickToStartCanvasAnimation;
    [SerializeField] CanvasGroup _highscorePanel;
    [SerializeField] TextMeshProUGUI _highscoreText;
    
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

    void Awake()
    {
        var highscore = PlayerPrefs.GetInt("hs");
        if (highscore > 0)
        {
            _highscorePanel.alpha = 1f;
            _highscoreText.SetText(highscore.ToString());
        }
    }
}
