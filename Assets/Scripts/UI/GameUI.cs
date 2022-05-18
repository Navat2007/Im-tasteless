using System;
using System.Collections;
using System.Collections.Generic;
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

[Serializable]
public struct PanelUIStruct
{
    public Transform panel;
    public Action callback;
}

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    
    [SerializeField] private Image fadePanel;

    [Header("Timer")] 
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
    
    [Header("Wave banner")] 
    [SerializeField] private Spawner spawner;
    [SerializeField] private Transform waveBannerPanel;
    [SerializeField] private RectTransform waveBannerPanelRectTransform;
    [SerializeField] private TMP_Text waveBannerIndexText;
    [SerializeField] private TMP_Text waveBannerEnemyCountText;

    [Header("Player")] 
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text levelText;

    [Header("Buff")] 
    [SerializeField] private BuffIconStruct damageBuffIconStruct;
    [SerializeField] private BuffIconStruct attackSpeedBuffIconStruct;
    [SerializeField] private BuffIconStruct moveSpeedBuffIconStruct;
    [SerializeField] private BuffIconStruct armorBuffIconStruct;

    [Header("Weapon")] 
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text pistolAmmoText;
    [SerializeField] private TMP_Text shotgunAmmoText;
    [SerializeField] private TMP_Text rifleAmmoText;
    [SerializeField] private TMP_Text grenadeAmmoText;

    [Header("Reload")] 
    [SerializeField] private Transform reloadPanel;
    [SerializeField] private Slider reloadSlider;

    [Header("Inventory")] 
    [SerializeField] private SlotStruct slot1Struct;
    [SerializeField] private SlotStruct slot2Struct;
    [SerializeField] private SlotStruct slot3Struct;
    [SerializeField] private SlotStruct slot4Struct;

    [Header("GAME OVER SECTION")] [SerializeField]
    private GameObject gameOverUI;

    [SerializeField] private Button gameOverRestartButton;

    private float _maximumFade = 0.75f;

    private GameObject _player;
    private PlayerInput _playerInput;
    private HealthSystem _healthSystem;
    private ExperienceSystem _experienceSystem;
    private WeaponController _weaponController;
    private BusterController _busterController;
    
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
                StartPause();
                OpenPanel(Panels.PAUSE);
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
                timerText.text = $"{timeSpan:m\\:ss}";
            };
        }

        if (spawner != null)
        {
            spawner.OnNewWave += (waveIndex, enemyCount) =>
            {
                IEnumerator AnimateBanner()
                {
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
                }
                
                if(waveBannerPanel != null) waveBannerPanel.gameObject.SetActive(true);
                if(waveBannerIndexText != null) waveBannerIndexText.SetText($"- Волна {waveIndex + 1} -");
                if(waveBannerEnemyCountText != null) waveBannerEnemyCountText.SetText($"Количество зомби: {enemyCount}");

                StartCoroutine(AnimateBanner());
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
        Subscribe();
    }

    private void Restart()
    {
        GameManager.LevelRestart();
        StopPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Subscribe()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().gameObject;
        
        _playerInput = _player.GetComponent<PlayerInput>();
        _healthSystem = _player.GetComponent<HealthSystem>();
        _experienceSystem = _player.GetComponent<ExperienceSystem>();
        _weaponController = _player.GetComponent<WeaponController>();
        _busterController = _player.GetComponent<BusterController>();

        _healthSystem.OnDeath += OnGameOver;
        _healthSystem.OnHealthChange += OnHealthChange;
        _healthSystem.OnArmorChange += OnArmorChange;
        
        _experienceSystem.OnXpChange += OnXpChange;
        _experienceSystem.OnLevelChange += OnLevelChange;

        _weaponController.OnReloadStart += OnWeaponReloadStart;
        _weaponController.OnReloadEnd += OnWeaponReloadEnd;
        _weaponController.OnReloadPercent += OnWeaponReloadPercent;
        _weaponController.OnAmmoChange += OnWeaponAmmoChange;
        _weaponController.OnEquipWeapon += OnEquipWeapon;
        
        _busterController.OnMoveSpeedChange += OnMoveSpeedBusterChange;
        _busterController.OnDamageChange += OnDamageBusterChange;
        _busterController.OnAttackSpeedChange += OnAttackSpeedBusterChange;
    }

    private void Unsubscribe()
    {
        _healthSystem.OnDeath -= OnGameOver;
        _healthSystem.OnHealthChange -= OnHealthChange;
        _healthSystem.OnArmorChange -= OnArmorChange;
        
        _experienceSystem.OnXpChange -= OnXpChange;
        _experienceSystem.OnLevelChange -= OnLevelChange;
        
        _weaponController.OnReloadStart -= OnWeaponReloadStart;
        _weaponController.OnReloadEnd -= OnWeaponReloadEnd;
        _weaponController.OnReloadPercent -= OnWeaponReloadPercent;
        _weaponController.OnAmmoChange -= OnWeaponAmmoChange;
        _weaponController.OnEquipWeapon -= OnEquipWeapon;
        
        _busterController.OnMoveSpeedChange -= OnMoveSpeedBusterChange;
        _busterController.OnDamageChange -= OnDamageBusterChange;
        _busterController.OnAttackSpeedChange -= OnAttackSpeedBusterChange;
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

    private void OnHealthChange(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            healthText.SetText($"{currentHealth} / {maxHealth}");
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
    
    private void OnArmorChange(int currentArmor)
    {
        armorBuffIconStruct.icon.gameObject.SetActive(currentArmor > 0);
    }

    private void OnXpChange(int currentXp, int nextLevelXp)
    {
        if (xpText != null)
        {
            xpText.SetText($"{currentXp} / {nextLevelXp}");
        }

        if (xpSlider != null)
        {
            xpSlider.maxValue = nextLevelXp;
            xpSlider.value = currentXp;
        }
    }

    private void OnLevelChange(int level)
    {
        if (levelText != null)
        {
            levelText.SetText($"{level}");
        }
    }

    private void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
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
            
        if (ammoText != null && _weaponController.GetEquippedWeapon.CurrentWeaponType == weapon) 
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
    
    public void OpenPanel(Panels panel)
    {
        switch (panel)
        {
            case Panels.RESULT:
                _panelStack.Push(new PanelUIStruct
                {
                    panel = resultPanel,
                    callback = GameManager.FinishLevel
                });
                resultPanel.gameObject.SetActive(true);
                break;
            case Panels.PAUSE:
                _panelStack.Push(new PanelUIStruct
                {
                    panel = pausePanel,
                    callback = () => {}
                });
                pausePanel.gameObject.SetActive(true);
                break;
            case Panels.SETTINGS:
                _panelStack.Push(new PanelUIStruct
                {
                    panel = settingsPanel,
                    callback = () => {}
                });
                settingsPanel.gameObject.SetActive(true);
                break;
        }
    }
    
    public void ClosePanel()
    {
        if (_panelStack.Count == 0)
        {
            OpenPanel(Panels.PAUSE);
            StartPause();
        }
        else
        {
            var panel = _panelStack.Pop();
            panel.panel.gameObject.SetActive(false);
            panel.callback();
            
            if (_panelStack.Count == 0)
            {
                StopPause();
            }
        }
    }

    public void StartPause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        GameManager.SetGameState(GameManager.GameState.PAUSE);
    }
    
    public void StopPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        GameManager.SetGameState(GameManager.GameState.PLAY); 
    }

}

public enum Panels
{
    GAME,
    RESULT,
    PAUSE,
    SETTINGS
}