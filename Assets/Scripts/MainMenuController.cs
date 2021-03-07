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
    [SerializeField] AudioSource _audioSource;
    
    bool _isLoading;

    IEnumerator LoadLevel()
    {
        _isLoading = true;

        _clickToStartCanvasAnimation.Play();
        _cameraAnimation.Play();
        _fadeInAnimation.Play();
        
        while (_audioSource.volume > 0.05f)
        {
            _audioSource.volume -= Time.deltaTime;
            yield return null;
        }

        GetComponent<AudioSource>().Play();
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
