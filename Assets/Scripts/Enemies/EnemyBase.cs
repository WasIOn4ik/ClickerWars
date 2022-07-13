using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyBase : MonoBehaviour
{
    #region Varaibles

    [Header("Movement")]
    [SerializeField] protected float targetThreshold = 0.25f;
    [SerializeField] protected float movementSpeed = 1f;
    [SerializeField] protected float rotationBeforeMovementThreshold = 0.4f;
    [Range(0, 1)]
    [SerializeField] protected float rotationSpeed = 0.4f;

    [Header("Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] protected Renderer rendererComp;
    [SerializeField] protected ParticleHandler deathParticles;
    [SerializeField] protected AudioClip deathSound;

    private int hp;
    private int maxHP;

    protected MarkerHandler marker;
    protected bool bFreezed;
    protected Vector3 target;
    protected List<Color> startColors = new List<Color>();

    protected bool isBoss = false;

    #endregion

    #region UnityCallbacks

    public void Awake()
    {
        canvas.worldCamera = Camera.main;
        GameBase.instance.activeEnemies.Add(this);
        marker = GameBase.instance.cameraHandler.FindAvailableMarker();
        target = GameBase.instance.RandomVectorInArea();

        List<Material> mat = new List<Material>();
        rendererComp.GetMaterials(mat);
        foreach (var m in mat)
        {
            startColors.Add(m.color);
        }
    }

    public void Update()
    {
        HandleTarget();
        if (Application.platform == RuntimePlatform.Android)
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(raycast, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                        HandleTap();
                }
            }
    }

#if UNITY_EDITOR

    private void OnMouseDown()
    {
        HandleTap();
    }

#endif

    void LateUpdate()
    {
        canvas.transform.LookAt(canvas.transform.position + Camera.main.transform.forward);
    }

    private void OnDestroy()
    {
        if (GameBase.instance)
            GameBase.instance.activeEnemies.Remove(this);

        if (marker)
            marker.Unlock();
    }

    #endregion

    #region Functions

    public bool GetBoss()
    {
        return isBoss;
    }

    public void SetBoss(bool boss)
    {
        isBoss = boss;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public void ReceiveDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            HandleDeath();
        }
        else
            UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.SetCurrentValue(hp);
        healthBar.SetMaxValue(maxHP);
        healthBar.UpdateDisplay();
    }

    public void SetMaxHP(int newMaxHP, bool bUpdateCurrent = true)
    {
        maxHP = newMaxHP;

        if (bUpdateCurrent)
            hp = maxHP;

        UpdateHealthBar();
    }

    public MarkerHandler GetMarker()
    {
        return marker;
    }

    public virtual void MoveToTarget()
    {
        Vector3 movementDirection = transform.position - target;
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);

        if ((transform.rotation.eulerAngles - targetRotation.eulerAngles).magnitude < rotationBeforeMovementThreshold)
        {
            transform.position -= movementDirection.normalized * movementSpeed * (99 + GameBase.instance.spawnedEnemyNumber) / 1000;//Vector3.Lerp(transform.position, target, movementSpeed / 500);
        }
    }

    public void HandleTarget()
    {
        if (bFreezed)
            return;

        if ((transform.position - target).magnitude < targetThreshold)
        {
            target = GameBase.instance.RandomVectorInArea();
        }
        else
        {
            MoveToTarget();
        }
    }

    public void Freeze(float time)
    {
        StartCoroutine(HandleFreeze(time));
    }

    public virtual IEnumerator HandleFreeze(float time)
    {
        bFreezed = true;
        List<Material> mat = new List<Material>();
        rendererComp.GetMaterials(mat);
        foreach (var m in mat)
        {
            m.color = Color.cyan;
        }
        yield return new WaitForSeconds(time);

        for (int i = 0; i < mat.Count; i++)
        {
            mat[i].color = startColors[i];
        }
        bFreezed = false;
    }

    public void HandleDeath()
    {
        Leadership.currentSessionScore += isBoss ? 10 : 1;
        GameBase.instance.UpdateScores();
        Instantiate(deathParticles, transform.position, Quaternion.LookRotation(Camera.main.transform.forward));
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        Destroy(gameObject);
    }
    public void HandleTap()
    {
        if (!GameBase.instance.gameActive)
            return;

        ReceiveDamage(1);
    }

    #endregion
}
