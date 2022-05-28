using UnityEngine.InputSystem;

public static class ControllerManager
{
    public static Player player;
    public static PlayerController playerController;
    public static PlayerAbilityController playerAbilityController;
    public static PlayerInput playerInput;
    public static SkillController skillController;
    public static WeaponController weaponController;
    public static BusterController busterController;
    public static ExperienceSystem experienceSystem;
    public static HealthSystem healthSystem;

    public static EnemySpawner enemySpawner;
    public static CrateSpawner crateSpawner;

    public static PauseManager pauseManager;
    public static AirplaneManager airplaneManager;
    public static MusicManager musicManager;
    public static AudioManager audioManager;
}