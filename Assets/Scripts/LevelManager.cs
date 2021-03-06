using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] int _maxFear = 100;
    [SerializeField] float _endPanelTimeout = 5f;

    int _currentFear;
    FearPanel _fearPanel;
    bool _isEnding;

    IEnumerator EndLevel()
    {
        _isEnding = true;

        FindObjectOfType<Player>().Deactivate();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyCrossedLine(other.GetComponent<Enemy>());
    }
    
    void Awake()
    {
        _fearPanel = FindObjectOfType<FearPanel>();
        _fearPanel.SetFearLevel(0f);
    }

    [ContextMenu("End Level")]
    public void EndLevelTest() 
    {
        StartCoroutine(EndLevel());
    }
}
