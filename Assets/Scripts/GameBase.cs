using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    #region Variables

    [Header("SpawnArea")]

    [Tooltip("Левый верхний ограничитель спавна врагов")]
    [SerializeField] private Transform LUMarker;
    [Tooltip("Правый нижний ограничитель спавна врагов")]
    [SerializeField] private Transform RDMarker;

    [Header("Enemies")]

    [Tooltip("Родитель для привязки врагов")]
    [SerializeField] private Transform enemiesHolder;
    [Tooltip("Список врагов, выбираются случайно")]
    [SerializeField] private List<EnemyBase> enemyPrefabs;
    [Tooltip("Каждый 50-ый враг - босс, выбирается случайно")]
    [SerializeField] private List<EnemyBase> bossPrefabs;

    [Header("Difficulty")]

    [Tooltip("Начальное время спавна в секундах")]
    [SerializeField] private float initialSpawnDelay = 5f;
    [Tooltip("минимальное время до спавна в секундах")]
    [SerializeField] private float minimalSpawnDelay = 2f;
    [Tooltip("Разброс таймера спавна в долях")]
    [SerializeField] private float spawnDelayRange = 0.4f;

    [Tooltip("Стартовое здоровье врагов")]
    [SerializeField] private int startEnemyHP = 2;
    [Tooltip("Множитель роста здоровья")]
    [SerializeField] private float timeToDifficultyFactor = 1f;
    [Tooltip("Разброс значений здоровья врагов в долях")]
    [SerializeField] private float enemyHPRange = 0.5f;

    [Header("System")]

    [Tooltip("Номер заспавненного врага")]
    [SerializeField] public int spawnedEnemyNumber = 0;
    [Tooltip("Время спавна для следующего врага до учета разброса")]
    [SerializeField] private float currentSpawnDelay;
    [Tooltip("Текущее количество здоровья у врагов до учета разброса и сложности")]
    [SerializeField] private float currentEnemyHP;

    [Header("References")]

    [SerializeField] private Transform result;
    [SerializeField] private TMP_Text scoresText;
    [SerializeField] private TMP_Text scoresTextInGame;
    public CameraHandler cameraHandler;
    [SerializeField] protected AudioSource awakeSound;

    [Header("Busters")]
    [SerializeField] private BusterHandler freezeBuster;
    [SerializeField] private BusterHandler killBuster;
    [SerializeField] private BusterHandler portalBuster;

    public bool gameActive;
    public bool Freezed;
    public float freezeTime;
    public float freezeDuration = 5f;

    public List<EnemyBase> activeEnemies = new List<EnemyBase>();
    public static GameBase instance;

    #endregion

    #region UnityCallbacks

    public void Awake()
    {
        if (GameBase.instance)
            Destroy(this);

        instance = this;
        Leadership.currentSessionScore = 0;
        gameActive = true;
    }

    public void Start()
    {
        currentEnemyHP = startEnemyHP;
        StartCoroutine(HandleSpawn());
    }

    public void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    #endregion

    #region Functions

    public void UpdateScores()
    {
        scoresTextInGame.text = "Очки: " + Leadership.currentSessionScore.ToString();
    }

    public void SpawnEnemy(EnemyBase prefab, float hp, Vector3 position, bool boss = false)
    {
        var en = Instantiate(prefab, position, Quaternion.identity, enemiesHolder);
        en.SetMaxHP((int)hp);
        en.SetBoss(boss);
        en.name = "Enemy_" + spawnedEnemyNumber;
        awakeSound.Play();
    }
    public Vector3 RandomVectorInArea()
    {
        return new Vector3(Random.Range(LUMarker.position.x, RDMarker.position.x), 0f, Random.Range(LUMarker.position.z, RDMarker.position.z));
    }

    public IEnumerator HandleSpawn()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while (true)
        {
            if (Freezed)
            {
                yield return new WaitForSeconds(freezeDuration - Mathf.Max(Time.time - freezeTime, 0));
                Freezed = false;
            }

            spawnedEnemyNumber++;

            currentSpawnDelay = Mathf.Clamp(initialSpawnDelay * 50 / (50 + spawnedEnemyNumber), minimalSpawnDelay, initialSpawnDelay);
            Vector3 SpawnPosition = RandomVectorInArea();

            currentEnemyHP = Mathf.FloorToInt(startEnemyHP * (50 + spawnedEnemyNumber) / 50);

            if (spawnedEnemyNumber % 50 == 0)
            {
                EnemyBase enemyToSpawn = bossPrefabs[Random.Range(0, bossPrefabs.Count)];
                int hpToSpawn = Mathf.FloorToInt(Random.Range(currentEnemyHP * spawnedEnemyNumber * timeToDifficultyFactor * (1 - enemyHPRange), currentEnemyHP * spawnedEnemyNumber * timeToDifficultyFactor * (1 + enemyHPRange)));
                SpawnEnemy(enemyToSpawn, hpToSpawn, SpawnPosition, true);
            }
            else
            {
                EnemyBase enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                int hpToSpawn = Mathf.FloorToInt(Random.Range(currentEnemyHP * timeToDifficultyFactor * (1 - enemyHPRange), currentEnemyHP * timeToDifficultyFactor * (1 + enemyHPRange)));
                SpawnEnemy(enemyToSpawn, hpToSpawn, SpawnPosition);
            }

            Debug.Log("Current time now: " + currentSpawnDelay + " | hp: " + currentEnemyHP + " | enemy number: " + spawnedEnemyNumber);

            if (CheckLoseCondition())
            {
                result.gameObject.SetActive(true);
                scoresText.text = "Очки: " + Leadership.currentSessionScore.ToString();
                scoresTextInGame.gameObject.SetActive(false);
                gameActive = false;
                break;
            }

            yield return new WaitForSeconds(Random.Range(currentSpawnDelay * (1 - spawnDelayRange), currentSpawnDelay * (1 + spawnDelayRange)));
        }
        yield return null;
    }

    protected bool CheckLoseCondition()
    {
        return activeEnemies.Count > 9;
    }

    #endregion

    #region UIFunctions

    public void OnReturnToMenuClicked()
    {
        Leadership.AddScores(Leadership.currentSessionScore);
        Leadership.Save();
        SceneManager.LoadScene("StartScene");
    }
    #endregion
}
