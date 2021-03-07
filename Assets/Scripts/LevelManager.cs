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
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioClip[] _lineCrossAudioClips;
    [SerializeField] float _enemySpawnMultiplierPerLevel = 2.5f;

    TextLinesPlayer _textLinesPlayer;
    FearPanel _fearPanel;
    Player _player;
    EnemySpawner _enemySpawner;
    AudioSource _audioSource;
    
    int _score;
    int _currentFear;
    bool _isEnding;
    int _currentLevel;
    int _currentEnemyCount;
    bool _isAdvancing;

    IEnumerator EndLevel()
    {
        _isEnding = true;

        _bgLightAnimation.Play();
        _player.Deactivate();
        FindObjectOfType<EnemySpawner>().SpawnEnemies(20, 20, 5);
        yield return new WaitForSeconds(1f);

        var bomb = FindObjectOfType<BombProjectile>();
        if (bomb != null)
            Destroy(bomb.gameObject);

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

        while (_musicSource.volume > 0.05f)
        {
            _musicSource.volume -= Time.deltaTime;
            yield return null;
        }

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

        enemy.CrossTheLine();
        _currentEnemyCount--;
        
        if (!_isEnding)
            _audioSource.PlayOneShot(_lineCrossAudioClips[UnityEngine.Random.Range(0, _lineCrossAudioClips.Length)]);

        if (_currentFear >= _maxFear && !_isEnding)
            StartCoroutine(EndLevel());
        else if (_currentEnemyCount <= 0)
            AdvanceLevel();
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
        _textLinesPlayer.Finished -= OnTextLinesFinished;

        _currentLevel = 0;
        AdvanceLevel();
    }

    void OnEnemyDied(int scoreAmount)
    {
        AddScore(scoreAmount);
        
        _currentEnemyCount--;

        if (_currentEnemyCount <= 0)
            AdvanceLevel();
    }

    void AdvanceLevel()
    {
        if (_isAdvancing)
            return;
        
        // Debug.Log($"advancing level, {_currentLevel} -> {_currentLevel + 1}");

        _isAdvancing = true;
        _currentLevel++;

        var newCount = GetEnemyCountForLevel(_currentLevel);
        _currentEnemyCount = newCount;

        _enemySpawner.SpawnEnemies(_currentLevel, newCount);

        _isAdvancing = false;
    }

    int GetEnemyCountForLevel(int level)
    {
        return (int)Math.Ceiling(level * _enemySpawnMultiplierPerLevel);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyCrossedLine(other.GetComponent<Enemy>());
    }

    void OnDestroy()
    {
        Enemy.Died -= OnEnemyDied;
        BombProjectile.Chain -= AddScore;
    }
    
    void Awake()
    {
        _fearPanel = FindObjectOfType<FearPanel>();
        _fearPanel.SetFearLevel(0f);
        _player = FindObjectOfType<Player>();
        _textLinesPlayer = FindObjectOfType<TextLinesPlayer>();
        _audioSource = GetComponent<AudioSource>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        
        SetScore(_score);
        Enemy.Died += OnEnemyDied;
        BombProjectile.Chain += AddScore;

        var isFirstTime = PlayerPrefs.GetInt("ft") == 0;
        if (isFirstTime)
        {
            FindObjectOfType<Canon>().Deactivate();
            _textLinesPlayer.Finished += OnTextLinesFinished;
            StartCoroutine(_textLinesPlayer.PlayLines());
            PlayerPrefs.SetInt("ft", 1);
        }
        else
        {
            _currentLevel = 0;
            AdvanceLevel();
        }
    }

    [ContextMenu("Spawn enemies")]
    public void SpawnEnemiesTest() 
    {
        AdvanceLevel();
    }

    [ContextMenu("End Level")]
    public void EndLevelTest() 
    {
        StartCoroutine(EndLevel());
    }
}
