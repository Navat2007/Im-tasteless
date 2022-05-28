using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Managers;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct SlotStruct
{
    public Transform mainButton;
    public Transform focusButton;
    public Transform focusTab;
}

[Serializable]
public struct BuffIconStruct
{
    public Transform icon;
    public Image image;
    public TMP_Text timerText;
}

// Структура для Stack очереди панелей
[Serializable]
public struct PanelUIStruct
{
    public Transform panel;
    public PanelType panelType;
    public Action callback;
}

[Serializable]
public struct ResultPanelStruct
{
    public TMP_Text rewardGoldText;
    public Button rewardClaimButton;
    public Button reward2XClaimButton;
}

[Serializable]
public struct SkillPanelStruct
{
    [Header("Первый скилл для выбора")]
    public Button skill1Button;
    public Image skill1FrameImage;
    public Image skill1Image;
    public TMP_Text skill1ImageText;
    public TMP_Text skill1Text;
    public TMP_Text skill1Description;
    public Action skill1Callback;
    
    [Header("Второй скилл для выбора")]
    public Button skill2Button;
    public Image skill2FrameImage;
    public Image skill2Image;
    public TMP_Text skill2ImageText;
    public TMP_Text skill2Text;
    public TMP_Text skill2Description;
    public Action skill2Callback;
    
    [Header("Третий скилл для выбора")]
    public Button skill3Button;
    public Image skill3FrameImage;
    public Image skill3Image;
    public TMP_Text skill3ImageText;
    public TMP_Text skill3Text;
    public TMP_Text skill3Description;
    public Action skill3Callback;
}

public enum PanelType
{
    GAME,
    RESULT,
    PAUSE,
    SETTINGS,
    SKILLS
}

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    
    [SerializeField] private Image fadePanel;
    
    [Header("FPS")]
    [SerializeField] private Transform fpsPanel;
    [SerializeField] private TMP_Text fpsText;

    [Header("Timer")] 
    [SerializeField] private Transform timerPanel;
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text timerText;

    [Header("Pause")] 
    [SerializeField] private Transform pausePanel;
    [SerializeField] private Button openPausePanelButton;
    [SerializeField] private Button closePausePanelButton;
    [SerializeField] private Button restartPausePanelButton;
    [SerializeField] private Button settingsPausePanelButton;
    
    [Header("Settings")] 
    [SerializeField] private Transform settingsPanel;
    
    [Header("Result")] 
    [SerializeField] private Transform resultPanel;
    [SerializeField] private ResultPanelStruct resultPanelStruct;
    
    [Header("Wave banner")] 
    [SerializeField] private Transform enemyCounterPanel;
    [SerializeField] private TMP_Text enemyCounterText;
    [SerializeField] private Transform waveBannerPanel;
    [SerializeField] private RectTransform waveBannerPanelRectTransform;
    [SerializeField] private TMP_Text waveBannerIndexText;
    [SerializeField] private TMP_Text waveBannerEnemyCountText;

    [Header("Player")] 
    [SerializeField] private Transform playerPanel;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image healthDamagedBarImage;
    [SerializeField] private float healthDamagedShrinkTimer;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private Image xpBarImage;
    [SerializeField] private TMP_Text levelText;

    [Header("Buff")] 
    [SerializeField] private BuffIconStruct damageBuffIconStruct;
    [SerializeField] private BuffIconStruct attackSpeedBuffIconStruct;
    [SerializeField] private BuffIconStruct moveSpeedBuffIconStruct;
    [SerializeField] private BuffIconStruct armorBuffIconStruct;
    
    [Header("Ability")] 
    [SerializeField] private Transform abilityPanel;
    [SerializeField] private TMP_Text abilityTimer;
    [SerializeField] private Image abilityImage;
    [SerializeField] private Image abilityFadeImage;
    [Space(10)]
    [SerializeField] private Transform abilitySprintIcon;
    [SerializeField] private Transform abilitySprintText;

    [Header("Weapon")] 
    [SerializeField] private Transform bulletsPanel;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text pistolAmmoText;
    [SerializeField] private TMP_Text shotgunAmmoText;
    [SerializeField] private TMP_Text rifleAmmoText;
    [SerializeField] private TMP_Text grenadeAmmoText;

    [Header("Reload")] 
    [SerializeField] private Transform reloadPanel;
    [SerializeField] private Slider reloadSlider;

    [Header("Inventory")] 
    [SerializeField] private Transform slotPanel;
    [SerializeField] private SlotStruct slot1Struct;
    [SerializeField] private SlotStruct slot2Struct;
    [SerializeField] private SlotStruct slot3Struct;
    [SerializeField] private SlotStruct slot4Struct;

    [Header("Game over panel")] 
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Button gameOverRestartButton;
    
    [Header("Skill choice panel")] 
    [SerializeField] private Transform skillChoicePanel;
    [SerializeField] private TMP_Text skillChoiceLevelText;
    [SerializeField] private SkillPanelStruct skillPanelStruct;
    [Space(10)] 
    [SerializeField] private Sprite frameWhiteSprite;
    [SerializeField] private Sprite frameRedSprite;
    [SerializeField] private Sprite frameOrangeSprite;
    [SerializeField] private Sprite frameGraySprite;
    [SerializeField] private Sprite framePurpleSprite;
    [SerializeField] private Sprite frameBlueSprite;
    [SerializeField] private Sprite frameGreenSprite;

    private float _maximumFade = 0.75f;
    
    private Stack<PanelUIStruct> _panelStack = new ();

    private void Awake()
    {
        instance = this;
        
        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(Restart);
        }

        if (openPausePanelButton != null)
        {
            openPausePanelButton.onClick.AddListener(() =>
            {
                OpenPanel(PanelType.PAUSE);
            });
        }

        if (closePausePanelButton != null)
        {
            closePausePanelButton.onClick.AddListener(() =>
            {
                ClosePanel();
            });
        }
        
        if (restartPausePanelButton != null)
        {
            restartPausePanelButton.onClick.AddListener(Restart);
        }

        if (timer != null && timerText != null)
        {
            timer.OnTimerChange += (sender, time) =>
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                string formattedTimeSpan = string.Format("{0:D2} : {1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                timerText.text = formattedTimeSpan;
            };
        }
    }

    private void OnEnable()
    {
        GameManager.OnLevelRestart += Unsubscribe;
    }

    private void OnDisable()
    {
        GameManager.OnLevelRestart -= Unsubscribe;
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetFPS), 1, 1);
        Subscribe();
    }
    
    private void Update() {
        
        healthDamagedShrinkTimer -= Time.deltaTime;
        
        if (healthDamagedShrinkTimer < 0) {
            if (healthBarImage.fillAmount < healthDamagedBarImage.fillAmount) {
                float shrinkSpeed = 1f;
                healthDamagedBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }

    private void Restart()
    {
        GameManager.LevelRestart();
        ControllerManager.pauseManager.StopPause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Subscribe()
    {
        if (ControllerManager.enemySpawner != null)
        {
            ControllerManager.enemySpawner.OnNewWave += OnNewWave;
            ControllerManager.enemySpawner.OnEnemyCountChange += EnemySpawnerOnEnemyCountChange;
        }

        if (ControllerManager.healthSystem != null)
        {
            ControllerManager.healthSystem.OnDeath += OnGameOver;
            ControllerManager.healthSystem.OnTakeDamage += OnDamaged;
            ControllerManager.healthSystem.OnHealed += OnHealed;
            ControllerManager.healthSystem.OnMaxHealthChange += OnMaxHealthChange;
            ControllerManager.healthSystem.OnArmorChange += OnArmorChange;
        }
        
        if (ControllerManager.experienceSystem != null)
        {
            ControllerManager.experienceSystem.OnXpChange += OnXpChange;
            ControllerManager.experienceSystem.OnNextLevelXpChange += OnNextLevelXpChange;
            ControllerManager.experienceSystem.OnLevelChange += OnLevelChange;
        }
        
        if (ControllerManager.weaponController != null)
        {
            ControllerManager.weaponController.OnReloadStart += OnWeaponReloadStart;
            ControllerManager.weaponController.OnReloadEnd += OnWeaponReloadEnd;
            ControllerManager.weaponController.OnReloadPercent += OnWeaponReloadPercent;
            ControllerManager.weaponController.OnAmmoChange += OnWeaponAmmoChange;
            ControllerManager.weaponController.OnEquipWeapon += OnEquipWeapon;
        }
        
        if (ControllerManager.busterController != null)
        {
            ControllerManager.busterController.OnMoveSpeedChange += OnMoveSpeedBusterChange;
            ControllerManager.busterController.OnDamageChange += OnDamageBusterChange;
            ControllerManager.busterController.OnAttackSpeedChange += OnAttackSpeedBusterChange;
        }

        if (ControllerManager.playerInput != null)
        {
            ControllerManager.playerInput.actions["FPS"].performed += ShowFpsPanel;
        }
        
        if (ControllerManager.playerAbilityController != null)
        {
            ControllerManager.playerAbilityController.OnAbilitySet += OnAbilitySet;
            ControllerManager.playerAbilityController.OnAbilityUse += OnAbilityUse;
        }
    }

    private void Unsubscribe()
    {
        if (ControllerManager.enemySpawner != null)
        {
            ControllerManager.enemySpawner.OnNewWave -= OnNewWave;
            ControllerManager.enemySpawner.OnEnemyCountChange -= EnemySpawnerOnEnemyCountChange;
        }

        if (ControllerManager.healthSystem != null)
        {
            ControllerManager.healthSystem.OnDeath -= OnGameOver;
            ControllerManager.healthSystem.OnTakeDamage -= OnDamaged;
            ControllerManager.healthSystem.OnHealed -= OnHealed;
            ControllerManager.healthSystem.OnMaxHealthChange -= OnMaxHealthChange;
            ControllerManager.healthSystem.OnArmorChange -= OnArmorChange;
        }

        if (ControllerManager.experienceSystem != null)
        {
            ControllerManager.experienceSystem.OnXpChange -= OnXpChange;
            ControllerManager.experienceSystem.OnNextLevelXpChange -= OnNextLevelXpChange;
            ControllerManager.experienceSystem.OnLevelChange -= OnLevelChange;
        }

        if (ControllerManager.weaponController != null)
        {
            ControllerManager.weaponController.OnReloadStart -= OnWeaponReloadStart;
            ControllerManager.weaponController.OnReloadEnd -= OnWeaponReloadEnd;
            ControllerManager.weaponController.OnReloadPercent -= OnWeaponReloadPercent;
            ControllerManager.weaponController.OnAmmoChange -= OnWeaponAmmoChange;
            ControllerManager.weaponController.OnEquipWeapon -= OnEquipWeapon;
        }

        if (ControllerManager.busterController != null)
        {
            ControllerManager.busterController.OnMoveSpeedChange -= OnMoveSpeedBusterChange;
            ControllerManager.busterController.OnDamageChange -= OnDamageBusterChange;
            ControllerManager.busterController.OnAttackSpeedChange -= OnAttackSpeedBusterChange;
        }
        
        if (ControllerManager.playerInput != null)
        {
            ControllerManager.playerInput.actions["FPS"].performed -= ShowFpsPanel;
        }
        
        if (ControllerManager.playerAbilityController != null)
        {
            ControllerManager.playerAbilityController.OnAbilitySet -= OnAbilitySet;
            ControllerManager.playerAbilityController.OnAbilityUse -= OnAbilityUse;
        }
    }

    private void GetFPS()
    {
        var fps = (int)(1f / Time.unscaledDeltaTime);
        if(fpsText != null) fpsText.SetText($"FPS: {fps}");
    }
    
    private void ShowFpsPanel(InputAction.CallbackContext obj)
    {
        if (fpsPanel != null)
        {
            fpsPanel.gameObject.SetActive(!fpsPanel.gameObject.activeSelf);
        }
    }
    
    private void OnAbilitySet(Ability ability)
    {
        abilityPanel.gameObject.SetActive(true);
        abilityImage.sprite = ability.GetImage;
    }
    
    private void OnAbilityUse(float reuseDuration, Ability ability)
    {
        IEnumerator AbilityDurationTimer()
        {
            float _timer = 0;
            
            abilitySprintIcon.gameObject.SetActive(true);

            while (_timer <= reuseDuration)
            {
                _timer += Time.deltaTime;
                TimeSpan timeSpan = TimeSpan.FromSeconds(reuseDuration - _timer);
                string formattedTimeSpan = timeSpan.Seconds.ToString();
                abilityTimer.text = formattedTimeSpan;
                abilityFadeImage.fillAmount = ((reuseDuration - _timer) * 100 / reuseDuration) / 100;
                
                yield return null;
            }
        }
        
        IEnumerator AbilityReuseTimer()
        {
            float _timer = 0;
            
            abilityFadeImage.gameObject.SetActive(true);
            abilityFadeImage.fillAmount = 1;

            while (_timer <= reuseDuration)
            {
                _timer += Time.deltaTime;
                TimeSpan timeSpan = TimeSpan.FromSeconds(reuseDuration - _timer);
                string formattedTimeSpan = timeSpan.Seconds.ToString();
                abilityTimer.text = formattedTimeSpan;
                abilityFadeImage.fillAmount = ((reuseDuration - _timer) * 100 / reuseDuration) / 100;
                
                yield return null;
            }
            
            abilityFadeImage.gameObject.SetActive(false);
        }

        if (ability is id_2_SprintAbility)
        {
            Debug.Log("Sprint");
        }

        StartCoroutine(AbilityReuseTimer());
    }

    private void OnEquipWeapon(WeaponType weaponType)
    {
        UnSelectSlots();
        
        switch (weaponType)
        {
            case WeaponType.PISTOL:
                slot1Struct.focusButton.gameObject.SetActive(true);
                slot1Struct.focusTab.gameObject.SetActive(true);
                break;
            case WeaponType.SHOTGUN:
                slot2Struct.focusButton.gameObject.SetActive(true);
                slot2Struct.focusTab.gameObject.SetActive(true);
                break;
            case WeaponType.RIFLE:
                slot3Struct.focusButton.gameObject.SetActive(true);
                slot3Struct.focusTab.gameObject.SetActive(true);
                break;
        }
    }
    
    private void OnNewWave(int waveIndex, int enemyCount)
    {
        IEnumerator AnimateBanner()
        {
            enemyCounterPanel.gameObject.SetActive(false);
                    
            float delayTime = 2f;
            float speed = 0.7f;
            float animatePercent = 0;
            int direction = 1;
            float endDelayTime = Time.time + 1 / speed + delayTime;

            while (animatePercent >= 0)
            {
                animatePercent += Time.deltaTime * speed * direction;

                if (animatePercent >= 1)
                {
                    animatePercent = 1;
                    if (Time.time > endDelayTime)
                    {
                        direction = -1;
                    }
                }
                        
                waveBannerPanelRectTransform.anchoredPosition = Vector2.up * Mathf.Lerp(-200, 180, animatePercent);
                yield return null;
            }
                    
            waveBannerPanel.gameObject.SetActive(false);
            enemyCounterPanel.gameObject.SetActive(true);
        }

        if (enemyCounterPanel == null) throw new NotImplementedException("GameUI enemyCounterPanel: Не назначен счетчик врагов");
        if (waveBannerPanel == null) throw new NotImplementedException("GameUI waveBannerPanel: Не назначен баннер волны");
                
        if(waveBannerPanel != null) waveBannerPanel.gameObject.SetActive(true);
        if(waveBannerIndexText != null) waveBannerIndexText.SetText($"- Волна {waveIndex + 1} -");
        if(waveBannerEnemyCountText != null) waveBannerEnemyCountText.SetText($"Количество зомби: {enemyCount}");

        StartCoroutine(AnimateBanner());
    }
    
    private void EnemySpawnerOnEnemyCountChange(int enemyCount)
    {
        if (enemyCounterText == null) throw new NotImplementedException("GameUI enemyCounterText: Не назначен текст счетчика врагов");
        
        enemyCounterText.SetText($"Зомби: {enemyCount}");
    }

    private void OnHealed(float currentHealth)
    {
        if (healthText != null)
        {
            healthText.SetText($"{Math.Round(currentHealth)} / {Math.Round(ControllerManager.healthSystem.MaxHealth)}");
        }

        healthBarImage.fillAmount = currentHealth / ControllerManager.healthSystem.MaxHealth;
        healthDamagedBarImage.fillAmount = healthBarImage.fillAmount;
    }
    
    private void OnDamaged(float damage, float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            healthText.SetText($"{Math.Round(currentHealth)} / {Math.Round(maxHealth)}");
        }

        healthDamagedShrinkTimer = 0.6f;
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
    
    private void OnMaxHealthChange(float maxHealth)
    {
        if (healthText != null)
        {
            healthText.SetText($"{Math.Round(ControllerManager.healthSystem.CurrentHealth)} / {Math.Round(maxHealth)}");
        }

        healthBarImage.fillAmount = (float)ControllerManager.healthSystem.CurrentHealth / ControllerManager.healthSystem.MaxHealth;
        healthDamagedBarImage.fillAmount = healthBarImage.fillAmount;
    }
    
    private void OnArmorChange(int currentArmor)
    {
        armorBuffIconStruct.icon.gameObject.SetActive(currentArmor > 0);
    }

    private void OnXpChange(float currentXp)
    {
        if (xpText != null)
        {
            xpText.SetText($"{Math.Round(currentXp)} / {Math.Round(ControllerManager.experienceSystem.NextLevelXp)}");
        }
        
        xpBarImage.fillAmount = currentXp / ControllerManager.experienceSystem.NextLevelXp;
    }
    
    private void OnNextLevelXpChange(float nextLevelXp)
    {
        if (xpText != null)
        {
            xpText.SetText($"{Math.Round(ControllerManager.experienceSystem.Xp)} / {Math.Round(nextLevelXp)}");
        }

        xpBarImage.fillAmount = ControllerManager.experienceSystem.Xp / nextLevelXp;
    }

    private void OnLevelChange(int level)
    {
        if (levelText != null)
        {
            levelText.SetText($"{level}");
        }
        else
        {
            throw new NotImplementedException("UI: текст уровня на панели персонажа не назначен");
        }

        if (skillChoicePanel != null)
        {
            OpenPanel(PanelType.SKILLS);
        }
        else
        {
            throw new NotImplementedException("UI: панель выбора скила не назначена");
        }
    }

    private void OnGameOver(GameObject owner, ProjectileHitInfo projectileHitInfo)
    {
        IEnumerator MoveEnemyCounterPanel()
        {
            float newY = enemyCounterPanel.position.y;
            float speed = 25;
            
            while (newY > 45)
            {
                newY -= Time.time / speed;
                enemyCounterPanel.position = new Vector2(enemyCounterPanel.position.x, newY);
                yield return null;
            }
        }

        StartCoroutine(MoveEnemyCounterPanel());
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        
        gameOverUI.SetActive(true);
        playerPanel.gameObject.SetActive(false);
        slotPanel.gameObject.SetActive(false);
        bulletsPanel.gameObject.SetActive(false);
        timerPanel.gameObject.SetActive(false);

        Cursor.visible = true;
    }

    private void OnWeaponReloadStart()
    {
        if (reloadPanel != null) reloadPanel.gameObject.SetActive(true);
    }
    
    private void OnWeaponReloadEnd()
    {
        if (reloadPanel != null) reloadPanel.gameObject.SetActive(false);
    }
    
    private void OnWeaponReloadPercent(float value)
    {
        if (reloadSlider != null) reloadSlider.value = value;
    }
    
    private void OnWeaponAmmoChange(int currentAmmo, int totalAmmo, bool infinite, WeaponType weapon)
    {
        string infiniteSymbol = "∞";
        string totalString = infinite ? infiniteSymbol : totalAmmo.ToString();
            
        if (ammoText != null && ControllerManager.weaponController.GetEquippedWeapon.CurrentWeaponType == weapon) 
            ammoText.SetText($"{currentAmmo} / {totalString}");

        switch (weapon)
        {
            case WeaponType.PISTOL:
                if (pistolAmmoText != null)
                {
                    pistolAmmoText.SetText($"{currentAmmo} / {totalString}");
                }
                else
                    throw new NotImplementedException("Текст для кол-ва патронов пистолета не назначен");
                break;
            case WeaponType.SHOTGUN:
                if (shotgunAmmoText != null)
                {
                    shotgunAmmoText.SetText($"{currentAmmo} / {totalString}");
                }
                else
                    throw new NotImplementedException("Текст для кол-ва патронов дробовика не назначен");
                break;
            case WeaponType.RIFLE:
                if (rifleAmmoText != null)
                {
                    rifleAmmoText.SetText($"{currentAmmo} / {totalString}");
                }
                else
                    throw new NotImplementedException("Текст для кол-ва патронов автомата не назначен");
                break;
            case WeaponType.GRENADE:
                if (grenadeAmmoText != null)
                {
                    grenadeAmmoText.SetText($"{currentAmmo} / {totalString}");
                }
                else
                    throw new NotImplementedException("Текст для кол-ва гранат не назначен");
                break;
        }
    }
    
    private void OnMoveSpeedBusterChange(float time)
    {
        moveSpeedBuffIconStruct.icon.gameObject.SetActive(time > 0);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        moveSpeedBuffIconStruct.timerText.text = $"{timeSpan:m\\:ss}";
    }
    
    private void OnDamageBusterChange(float time)
    {
        damageBuffIconStruct.icon.gameObject.SetActive(time > 0);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        damageBuffIconStruct.timerText.text = $"{timeSpan:m\\:ss}";
    }
    
    private void OnAttackSpeedBusterChange(float time)
    {
        attackSpeedBuffIconStruct.icon.gameObject.SetActive(time > 0);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        attackSpeedBuffIconStruct.timerText.text = $"{timeSpan:m\\:ss}";
    }

    private IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < _maximumFade)
        {
            percent += Time.deltaTime * speed;
            fadePanel.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    private void UnSelectSlots()
    {
        slot1Struct.focusButton.gameObject.SetActive(false);
        slot1Struct.focusTab.gameObject.SetActive(false);
        
        slot2Struct.focusButton.gameObject.SetActive(false);
        slot2Struct.focusTab.gameObject.SetActive(false);
        
        slot3Struct.focusButton.gameObject.SetActive(false);
        slot3Struct.focusTab.gameObject.SetActive(false);
    }

    public void SetSlot(int index, bool active)
    {
        switch (index)
        {
            case 1:
                slot1Struct.mainButton.gameObject.SetActive(active);
                break;
            case 2:
                slot2Struct.mainButton.gameObject.SetActive(active);
                break;
            case 3:
                slot3Struct.mainButton.gameObject.SetActive(active);
                break;
            case 4:
                slot4Struct.mainButton.gameObject.SetActive(active);
                break;
        }
    }
    
    public void OpenPanel(PanelType panelType)
    {
        IEnumerator MakeButtonsInteractable()
        {
            yield return new WaitForSecondsRealtime(2);
            
            skillPanelStruct.skill1Button.interactable = true;
            skillPanelStruct.skill2Button.interactable = true;
            skillPanelStruct.skill3Button.interactable = true;
        }

        void MakeButtonsNotInteractable()
        {
            skillPanelStruct.skill1Button.interactable = false;
            skillPanelStruct.skill2Button.interactable = false;
            skillPanelStruct.skill3Button.interactable = false;
        }

        switch (panelType)
        {
            case PanelType.RESULT:
                if (resultPanel == null) throw new NotImplementedException("Панель результата не назначена");

                ControllerManager.pauseManager.StartPause();
                
                resultPanelStruct.rewardClaimButton.onClick.RemoveAllListeners();
                resultPanelStruct.reward2XClaimButton.onClick.RemoveAllListeners();
                
                resultPanelStruct.rewardGoldText.SetText($"{CurrencyManager.GetCurrency(CurrencyType.LEVEL_GOLD)}");
                
                resultPanelStruct.rewardClaimButton.onClick.AddListener(() =>
                {
                    CurrencyManager.AddCurrency(CurrencyType.GOLD, CurrencyManager.GetCurrency(CurrencyType.LEVEL_GOLD)); 
                    CurrencyManager.SetCurrency(CurrencyType.LEVEL_GOLD, 0); 
                    
                    //TODO выход назад на глобальную сцену
                    Restart();
                });
                
                resultPanelStruct.reward2XClaimButton.onClick.AddListener(() =>
                {
                    CurrencyManager.AddCurrency(CurrencyType.GOLD, CurrencyManager.GetCurrency(CurrencyType.LEVEL_GOLD) * 2); 
                    CurrencyManager.SetCurrency(CurrencyType.LEVEL_GOLD, 0); 
                    
                    //TODO выход назад на глобальную сцену
                    Restart();
                });
                
                _panelStack.Push(new PanelUIStruct
                {
                    panel = resultPanel,
                    panelType = PanelType.RESULT,
                    callback = GameManager.FinishLevel
                });
                resultPanel.gameObject.SetActive(true);
                break;
            case PanelType.PAUSE:
                if (pausePanel == null) throw new NotImplementedException("Панель паузы не назначена");
                
                ControllerManager.pauseManager.StartPause();
                _panelStack.Push(new PanelUIStruct
                {
                    panel = pausePanel,
                    panelType = PanelType.PAUSE,
                    callback = () => {}
                });
                pausePanel.gameObject.SetActive(true);
                break;
            case PanelType.SETTINGS:
                if (settingsPanel == null) throw new NotImplementedException("Панель настроек не назначена");
                
                _panelStack.Push(new PanelUIStruct
                {
                    panel = settingsPanel,
                    panelType = PanelType.SETTINGS,
                    callback = () => {}
                });
                settingsPanel.gameObject.SetActive(true);
                break;
            case PanelType.SKILLS:
                if (skillChoicePanel == null) throw new NotImplementedException("Панель выбора навыков не назначена");
                
                ControllerManager.pauseManager.StartPause();
                
                skillChoiceLevelText.SetText($"Вы получили {ControllerManager.experienceSystem.Level} уровень");

                void SetPanelStruct(int index, Skill skill)
                {
                    var skillStruct = ControllerManager.skillController.GetSkillByID(skill.GetID);
                    
                    switch (index)
                    {
                        case 1:
                            skillPanelStruct.skill1Image.sprite = skill.GetImage;
                            skillPanelStruct.skill1ImageText.SetText($"{skillStruct.level + 1} уровень");
                            skillPanelStruct.skill1Text.SetText(skill.GetName);
                            skillPanelStruct.skill1Description.SetText(skill.GetDescription);
                            skillPanelStruct.skill1Button.onClick.RemoveAllListeners();
                            skillPanelStruct.skill1Button.onClick.AddListener(() =>
                            {
                                ClosePanel();
                                ControllerManager.skillController.AddToSkillsList(skill);
                                MakeButtonsNotInteractable();
                            });
                            break;
                        case 2:
                            skillPanelStruct.skill2Image.sprite = skill.GetImage;
                            skillPanelStruct.skill2ImageText.SetText($"{skillStruct.level + 1} уровень");
                            skillPanelStruct.skill2Text.SetText(skill.GetName);
                            skillPanelStruct.skill2Description.SetText(skill.GetDescription);
                            skillPanelStruct.skill2Button.onClick.RemoveAllListeners();
                            skillPanelStruct.skill2Button.onClick.AddListener(() =>
                            {
                                ClosePanel();
                                ControllerManager.skillController.AddToSkillsList(skill);
                                MakeButtonsNotInteractable();
                            });
                            break;
                        case 3:
                            skillPanelStruct.skill3Image.sprite = skill.GetImage;
                            skillPanelStruct.skill3ImageText.SetText($"{skillStruct.level + 1} уровень");
                            skillPanelStruct.skill3Text.SetText(skill.GetName);
                            skillPanelStruct.skill3Description.SetText(skill.GetDescription);
                            skillPanelStruct.skill3Button.onClick.RemoveAllListeners();
                            skillPanelStruct.skill3Button.onClick.AddListener(() =>
                            {
                                ClosePanel();
                                ControllerManager.skillController.AddToSkillsList(skill);
                                MakeButtonsNotInteractable();
                            });
                            break;
                    }
                }

                void SetFrameColor(Image image, Skill skill)
                {
                    switch (skill.GetSkillRarity)
                    {
                        case SkillRarity.COMMON:
                            image.sprite = frameWhiteSprite;
                            break;
                        case SkillRarity.UNCOMMON:
                            image.sprite = frameGreenSprite;
                            break;
                        case SkillRarity.RARE:
                            image.sprite = frameBlueSprite;
                            break;
                        case SkillRarity.UNIQUE:
                            image.sprite = frameOrangeSprite;
                            break;
                    }
                }

                var skill1 = ControllerManager.skillController.GetRandomSkill();
                var skill2 = ControllerManager.skillController.GetRandomSkill();
                var skill3 = ControllerManager.skillController.GetRandomSkill();
                
                SetPanelStruct(1, skill1);
                SetPanelStruct(2, skill2);
                SetPanelStruct(3, skill3);
                
                SetFrameColor(skillPanelStruct.skill1FrameImage, skill1);
                SetFrameColor(skillPanelStruct.skill2FrameImage, skill2);
                SetFrameColor(skillPanelStruct.skill3FrameImage, skill3);

                _panelStack.Push(new PanelUIStruct
                {
                    panel = skillChoicePanel,
                    panelType = PanelType.SKILLS,
                    callback = () => {}
                });
                skillChoicePanel.gameObject.SetActive(true);
                
                StartCoroutine(MakeButtonsInteractable());
                
                break;
        }
    }
    
    public void ClosePanel(bool isPlayerInput = false)
    {
        if(isPlayerInput && _panelStack.Count > 0 && _panelStack.Peek().panelType == PanelType.SKILLS)
            return;
        
        if (_panelStack.Count == 0)
        {
            OpenPanel(PanelType.PAUSE);
            ControllerManager.pauseManager.StartPause();
        }
        else
        {
            var panel = _panelStack.Pop();
            panel.panel.gameObject.SetActive(false);
            panel.callback();
            
            if (_panelStack.Count == 0)
            {
                ControllerManager.pauseManager.StopPause();
            }
        }
    }
}