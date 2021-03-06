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
    [SerializeField] int _maxHealth = 100;
    [SerializeField] float _endPanelTimeout = 5f;

    int _currentHealth;

    IEnumerator EndLevel()
    {
        FindObjectOfType<Player>().Deactivate();
        FindObjectOfType<EnemySpawner>().SpawnEnemies(80);
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
        _currentHealth = Math.Max(0, _currentHealth - enemy.Damage);
        Destroy(enemy.gameObject, 1f);
        StartCoroutine(EndLevel());
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyCrossedLine(other.GetComponent<Enemy>());
    }
    
    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    [ContextMenu("End Level")]
    public void EndLevelTest() 
    {
        StartCoroutine(EndLevel());
    }
}
