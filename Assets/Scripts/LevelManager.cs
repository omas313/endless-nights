using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    readonly string[] END_LINES = new string[]
    {
        "devoured by demons",
        "lost to the void",
        "will this nightmare ever end?",
        "the glitter of hope fades..."
    };

    [SerializeField] Animation _endPanelAnimation;
    [SerializeField] Animation _fadeInAnimation;
    [SerializeField] Animation _bgLightAnimation;
    [SerializeField] int _maxFear = 100;
    [SerializeField] float _endPanelTimeout = 5f;
    [SerializeField] TextMeshProUGUI _scoreText;

    int _score;
    int _currentFear;
    FearPanel _fearPanel;
    bool _isEnding;
    Player _player;
    private TextLinesPlayer _textLinesPlayer;

    IEnumerator EndLevel()
    {
        _isEnding = true;

        _bgLightAnimation.Play();
        _player.Deactivate();
        FindObjectOfType<EnemySpawner>().SpawnEnemies(80, 5f);
        yield return new WaitForSeconds(1f);

        _endPanelAnimation.GetComponentInChildren<TextMeshProUGUI>().SetText(END_LINES[UnityEngine.Random.Range(0, END_LINES.Length)]);
        _endPanelAnimation.Play();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_endPanelAnimation.isPlaying);

        var timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (Input.GetButtonDown("Fire1") || timer > _endPanelTimeout)
                break;

            yield return null;
        }

        _fadeInAnimation.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_fadeInAnimation.isPlaying);

        var highscore = PlayerPrefs.GetInt("hs");
        if (_score > highscore)
            PlayerPrefs.SetInt("hs", _score);

        SceneManager.LoadScene("MainMenu");
    }

    void EnemyCrossedLine(Enemy enemy)
    {
        _currentFear = Math.Min(_maxFear, _currentFear + enemy.Damage);
        _fearPanel.SetFearLevel((float)_currentFear / (float)_maxFear);

        Destroy(enemy.gameObject, 1f);

        if (_currentFear >= _maxFear && !_isEnding)
            StartCoroutine(EndLevel());
    }

    void SetScore(int score)
    {
        _score = score;
        _scoreText.SetText(_score.ToString());
    }

    void AddScore(int amount)
    {
        _score += amount;
        _scoreText.SetText(_score.ToString());
    }

    void OnTextLinesFinished()
    {
        FindObjectOfType<Canon>().Activate();
        // start spawner
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyCrossedLine(other.GetComponent<Enemy>());
    }

    void OnDestroy()
    {
        Enemy.Died -= AddScore;
        BombProjectile.Chain -= AddScore;
        _textLinesPlayer.Finished -= OnTextLinesFinished;
    }
    
    void Awake()
    {
        _fearPanel = FindObjectOfType<FearPanel>();
        _fearPanel.SetFearLevel(0f);
        _player = FindObjectOfType<Player>();
        _textLinesPlayer = FindObjectOfType<TextLinesPlayer>();
        
        SetScore(_score);

        var isFirstTime = PlayerPrefs.GetInt("ft") == 0;
        if (isFirstTime)
        {
            FindObjectOfType<Canon>().Deactivate();
            _textLinesPlayer.Finished += OnTextLinesFinished;
            StartCoroutine(_textLinesPlayer.PlayLines());
            PlayerPrefs.SetInt("ft", 1);
        }

        Enemy.Died += AddScore;
        Enemy.Died += AddScore;
        BombProjectile.Chain += AddScore;

        // start spawner
    }

    [ContextMenu("End Level")]
    public void EndLevelTest() 
    {
        StartCoroutine(EndLevel());
    }
}
