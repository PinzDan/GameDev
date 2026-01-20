using System.Collections;
using System.Collections.Generic;
using gameSpaces.enemies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{


    [Header("Stats")]
    [SerializeField] private GameObject HealtBar;
    public Slider Hpslider;

    [SerializeField] private GameObject staminaBar;
    public Slider StaminaSlider;

    private float previousHealth;
    private float previousStamina;

    private float timer = 0.0f;
    private float runConsume = 0.3f;
    public event System.Action<EnemyType, float> OnPlayerHit;
    public GameObject gameOver;


    public PlayerState stato;
    public float baseDamage = 10f;




    private bool isStaminaInfinite = false;


    private Dictionary<string, StatusEffectData> statusEffects;
    private Dictionary<string, Coroutine> activeStatusEffectsCoroutines = new Dictionary<string, Coroutine>();

    public StatusEffectData GetStatusEffect(string key)
    {
        if (statusEffects.TryGetValue(key, out var effect))
        {
            return effect;
        }

        Debug.LogWarning($"Nessun status effect trovato con chiave: {key}");
        return null;
    }

    private bool checkStato()
    {
        return (stato & PlayerState.Attack) != 0;
    }

    public void riceviAttacco(Enemy enemy)
    {
        if (!checkStato())
        {
            Debug.Log($"ricevo l'attacco da {enemy}");
            EnemyType type = enemy.getEnemyType();
            OnPlayerHit?.Invoke(type, enemy.getCurrentAttack());
        }
    }

    void Awake()
    {
        GameObject baseUI = GameObject.FindWithTag("BaseUI");
        HealtBar = baseUI.transform.Find("HealtBar").gameObject;
        staminaBar = HealtBar.transform.Find("StaminaBar").gameObject;
        gameOver = baseUI.transform.Find("GameOver").gameObject;

        if (HealtBar != null)
        {
            Hpslider = HealtBar.GetComponent<Slider>();
            StaminaSlider = staminaBar.GetComponent<Slider>();
        }


        LoadStatusEffects();

    }



    public new void Start()
    {
        base.Start();

        GameManager.Instance.OnDataLoaded += ApplyGameData;

        if (GameManager.Instance.isDataLoaded)
            ApplyGameData();
    }

    void Update()
    {

        if (isDead) return;
        staminaController();
        rechargeStamina();
        checkDeath();
    }

    bool isDead = false;
    public void checkDeath()
    {
        if (currentHealth <= 0)
        {
            isDead = true;

            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("death");

            die();
        }
    }

    public void resetStats()
    {
        isDead = false;

        activeStatusEffectsCoroutines.Clear();
        ApplyGameData();
        changeHpColor();
        resetTimer();

    }

    override protected void die()
    {
        base.die();
        gameOver.SetActive(true);

        CanvasGroup group = gameOver.GetComponent<CanvasGroup>();

        StartCoroutine(FadeInCanvas(group, 2f));
    }

    private IEnumerator FadeInCanvas(CanvasGroup group, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        group.alpha = 1f;

        yield return new WaitForSeconds(3f);

        gameOver.SetActive(false);

        GameManager.Instance.CaricaGioco(GameManager.Instance.getSlot());

        yield return null;


        GameManager.Instance.isFirstLaunch = false;
        GameManager.Instance.gameOver = true;
        //resetStats();

        gameObject.GetComponent<Animator>().enabled = false;
        var scena = GameManager.Instance.checkPointEnabled ? SceneManager.GetActiveScene().name : "MappaTutorial";
        Debug.Log("Scena:" + scena + "CheckPoint: " + GameManager.Instance.checkPointEnabled);
        SceneManager.LoadScene(scena);
    }

    public void ApplyGameData()
    {
        maxHealth = GameManager.Instance.getMaxHealth();
        maxStamina = GameManager.Instance.getMaxStamina();
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        baseDamage = GameManager.Instance.getDannoBase();

        if (Hpslider != null && StaminaSlider != null)
        {
            Hpslider.maxValue = maxHealth;
            Hpslider.value = currentHealth;
            StaminaSlider.maxValue = maxStamina;
            StaminaSlider.value = currentStamina;
        }
    }

    private void LoadStatusEffects()
    {
        StatusEffectData[] loaded = Resources.LoadAll<StatusEffectData>("Data/StatusEffect");
        statusEffects = new Dictionary<string, StatusEffectData>();

        foreach (var effect in loaded)
        {
            if (!statusEffects.ContainsKey(effect.effectName))
            {
                statusEffects.Add(effect.effectName, effect);
            }
            else
            {
                Debug.LogWarning($"Effetto duplicato trovato: {effect.effectName}");
            }
        }
        Debug.Log($"Caricati {statusEffects.Count} status effect.");
    }



    public void staminaController()
    {
        if (isStaminaInfinite)
        {
            return;
        }

        if (itsRunning())
        {
            useStamina(runConsume);
            updateStaminaSlider();
            resetTimer();
        }
        else if (timer < 3f)
        {
            incrementTimer();
        }
    }

    public void callUseStamina(float danno)
    {
        useStamina(danno);
    }
    protected override void useStamina(float danno)
    {
        base.useStamina(danno);
        updateStaminaSlider();
    }

    public bool itsRunning()
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        return isShiftPressed && (horizontalInput != 0 || verticalInput != 0);
    }

    public float getStamina()
    {
        return currentStamina;
    }



    private void updateStaminaSlider()
    {
        if (StaminaSlider != null)
        {
            StaminaSlider.value = currentStamina;
            previousStamina = currentStamina;
        }
    }

    private void updateHpSlider()
    {
        if (Hpslider != null)
        {
            Hpslider.value = currentHealth;
            previousHealth = currentHealth;
            changeHpColor();
        }
    }

    private void rechargeStamina()
    {

        if ((timer >= 0.8f) && (currentStamina <= maxStamina))
            currentStamina += 0.8f;

        updateStaminaSlider();
    }

    public void resetTimer()
    {
        timer = 0f;
    }
    private void incrementTimer()
    {
        timer += Time.deltaTime;
    }

    private void changeHpColor()
    {
        if (currentHealth <= 25)
            Hpslider.fillRect.GetComponent<Image>().color = new Color32(214, 40, 40, 255);
        else if (currentHealth <= 50)
            Hpslider.fillRect.GetComponent<Image>().color = new Color32(255, 190, 11, 255);
        else
            Hpslider.fillRect.GetComponent<Image>().color = new Color32(84, 229, 51, 255);
    }


    public void getHit(float attackDamage = 10F)
    {
        this.takeDamage(attackDamage);
        updateHpSlider();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        updateHpSlider();
        Debug.Log($"Cura effettuata: +{amount} HP (HP attuali: {currentHealth})");
    }

    public void RestoreStamina(float amount, float duration)
    {
        currentStamina = maxStamina;
        updateStaminaSlider();
        StartCoroutine(InfiniteStaminaCoroutine(duration));
    }

    public void BoostMaxStamina(float amount)
    {
        maxStamina += amount;
        Debug.Log($"Stamina massima aumentata di {amount}. Nuova stamina max: {maxStamina}");
        currentStamina = maxStamina;
        StaminaSlider.maxValue = maxStamina;
        Debug.Log($"Stamina attuale aggiornata: {currentStamina}/{maxStamina}");
        updateStaminaSlider();
        GameManager.Instance.dati.maxStamina = maxStamina;
        Debug.Log($"Stamina massima aumentata permanentemente di {amount}! Nuova stamina max: {maxStamina}");
    }

    private IEnumerator InfiniteStaminaCoroutine(float duration)
    {
        isStaminaInfinite = true;
        yield return new WaitForSeconds(duration);
        isStaminaInfinite = false;
    }

    // NUOVO: Metodo per curare il veleno (Antidoto)
    public void CurePoisonEffect()
    {
        if (activeStatusEffectsCoroutines.ContainsKey("Poison"))
        {
            StopCoroutine(activeStatusEffectsCoroutines["Poison"]);
            activeStatusEffectsCoroutines.Remove("Poison");
            Debug.Log("Effetto 'Poison' rimosso!");
        }
    }

    // NUOVO: Metodo per curare la bruciatura (Acqua)
    public void CureBurningEffect()
    {
        if (activeStatusEffectsCoroutines.ContainsKey("Burning"))
        {
            StopCoroutine(activeStatusEffectsCoroutines["Burning"]);
            activeStatusEffectsCoroutines.Remove("Burning");
            Debug.Log("Effetto 'Burning' rimosso!");
        }
    }

    // Metodo per curare tutti gli effetti di stato (da usare solo se necessario)
    public void CureAllStatusEffects()
    {
        foreach (var coroutine in activeStatusEffectsCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        activeStatusEffectsCoroutines.Clear();
        Debug.Log("Tutti gli effetti di stato sono stati rimossi.");
    }


    public void ApplyStatusEffect(string statusEffectName)
    {
        Debug.Log($"Applying status effect: {statusEffectName}");

        if (activeStatusEffectsCoroutines.ContainsKey(statusEffectName))
        {
            Debug.LogWarning($"Effetto '{statusEffectName}' già attivo. Non lo riapplico.");
            return;
        }

        switch (statusEffectName)
        {
            case "Poison":
                Debug.Log("avvelenamento");
                StatusEffectData effect = GetStatusEffect("Poison");
                Coroutine poisonCoroutine = StartCoroutine(ApplyEffectOverTime("Poison", effect.durata, effect.DannoPerIntervallo));
                activeStatusEffectsCoroutines.Add("Poison", poisonCoroutine);
                break;
            case "Burning":
                effect = GetStatusEffect("Burning");
                Coroutine burningCoroutine = StartCoroutine(ApplyEffectOverTime("Burning", effect.durata, effect.DannoPerIntervallo, effect.intervallo));
                activeStatusEffectsCoroutines.Add("Burning", burningCoroutine);
                break;
        }
    }

    public IEnumerator ApplyEffectOverTime(string name, float duration, float damagePerSecond, float intervallo = 1f)
    {
        float timer = 0f;
        Debug.Log("name " + name + duration + damagePerSecond + intervallo + "" + activeStatusEffectsCoroutines.ContainsKey(name));

        while (timer < duration || activeStatusEffectsCoroutines.ContainsKey(name)) // Controllo se l'effetto è ancora attivo
        {
            Debug.Log("Applico");
            yield return new WaitForSeconds(intervallo);

            // Assicurati che l'oggetto delle particelle esista
            Transform particleTransform = transform.Find(name);
            if (particleTransform != null)
            {
                ParticleSystem particelle = particleTransform.GetComponent<ParticleSystem>();
                particelle.Play();
            }

            takeDamage(damagePerSecond);
            updateHpSlider();
            timer += intervallo;
        }

        // Rimuovi l'effetto dal dizionario quando finisce il timer
        if (activeStatusEffectsCoroutines.ContainsKey(name))
        {
            activeStatusEffectsCoroutines.Remove(name);
        }
    }
}