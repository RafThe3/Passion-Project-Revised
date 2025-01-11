using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
using Unity.XR.Oculus.Input;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private bool testControls = false;

    [Header("Mechanics")]
    [SerializeField] private StarterAssetsInputs playerInput;

    [Header("Movement")]
    [SerializeField] private bool infiniteStamina = false;
    [Min(0), SerializeField] private float sprintTime = 1;
    [Min(0), SerializeField] private float sprintCooldownTime = 1;
    [Min(0), SerializeField] private float staminaRecoverySpeed = 1;

    [Header("Health")]
    [SerializeField] private bool isInvincible = false;
    [Min(0), SerializeField] private int startingHealth = 1;
    [Min(0), SerializeField] private int maxHealth = 100;
    [Min(0), SerializeField] private int startingHealthPacks = 1;
    [Min(0), SerializeField] private int maxHealthPacks = 1;
    [Min(0), SerializeField] private float healInterval = 1;

    [Header("Leveling")]
    [Min(0), SerializeField] private int startingLevel = 1;
    [Min(0), SerializeField] private int startingXP = 0;
    [Min(0), SerializeField] private int startingLevelUpXP = 1;
    [Min(0), SerializeField] private int levelCap = 1;
    [Min(0), SerializeField] private int levelUpXPMultiplier = 1;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI healthPacksText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Image staminaBarFillArea;

    [Header("SFX")]
    [SerializeField] private AudioClip healSFX;

    //Internal Variables
    private int currentHealth;
    private int healthPacks;
    private float healTimer;
    private int currentLevel, currentXP, currentLevelUpXP, xpCap;
    private bool isOnSprintCooldown;
    private bool isMoving;
    //private Animator animator;
    private CharacterController character;
    private FirstPersonController controller;
    private AudioSource audioSource;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        character = GetComponent<CharacterController>();
        controller = GetComponent<FirstPersonController>();
        audioSource = Camera.main.GetComponent<AudioSource>();
        playerInput = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        if (startingHealth > maxHealth)
        {
            startingHealth = maxHealth;
        }
        currentHealth = startingHealth;
        healthPacks = startingHealthPacks;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        xpBar.value = startingXP;
        xpBar.maxValue = startingLevelUpXP;
        currentLevelUpXP = startingLevelUpXP;
        currentXP = startingXP;
        currentLevel = startingLevel;
        xpCap = levelCap * startingLevelUpXP;
        staminaBar.maxValue = sprintTime;
        staminaBar.value = sprintTime;
    }

    private void Update()
    {
        healTimer += Time.deltaTime;
        UpdateUI();

        isMoving = Mathf.Abs(character.velocity.z) > Mathf.Epsilon;
        //animator.SetBool("isMoving", isMoving);

        //test
        if (testControls)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                TakeDamage(10);
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                AddXP();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Heal(10);
            }
        }
        //

        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<SaveSystem>().Load();
        }

        if (Input.GetButtonDown("Heal"))
        {
            Heal(10);
        }

        if (currentXP >= currentLevelUpXP)
        {
            LevelUp();
        }

        if (!infiniteStamina)
        {
            Sprint();
        }
    }

    private void Sprint()
    {
        if (staminaBar.value <= 0)
        {
            isOnSprintCooldown = true;
            controller.SprintSpeed = controller.MoveSpeed;
            staminaBarFillArea.color = Color.red;
        }

        if (isOnSprintCooldown && staminaBar.value >= sprintCooldownTime)
        {
            isOnSprintCooldown = false;
            controller.SprintSpeed = controller._tempSprintSpeed;
            staminaBarFillArea.color = Color.yellow;
        }

        if (controller.isSprinting && !isOnSprintCooldown && isMoving)
        {
            staminaBar.value -= Time.deltaTime;
        }
        else if (staminaBar.value < staminaBar.maxValue)
        {
            staminaBar.value += Time.deltaTime * staminaRecoverySpeed;
        }
    }

    private void LevelUp()
    {
        currentXP = 0;
        currentLevel++;
        currentLevelUpXP += startingLevelUpXP * levelUpXPMultiplier;
    }

    private void UpdateUI()
    {
        healthBar.value = currentHealth;
        healthText.text = $"Health: {currentHealth} / {maxHealth}";
        levelText.text = $"Level: {currentLevel}";
        if (currentLevelUpXP < xpCap)
        {
            xpBar.value = currentXP;
            xpBar.maxValue = currentLevelUpXP;
        }
        else
        {
            xpBar.value = xpBar.maxValue;
        }
        //xpText.text = $"XP: {xpBar.value} / {xpBar.maxValue}";
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int health)
    {
        if (currentHealth < maxHealth && healTimer >= healInterval && healthPacks > 0)
        {
            currentHealth += health;
            healthPacks--;
            healTimer = 0;
            audioSource.PlayOneShot(healSFX);
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GetComponent<SaveSystem>().Load();
    }

    public void AddHealthPack()
    {
        if (healthPacks < maxHealthPacks)
        {
            healthPacks++;
        }

        if (healthPacks > maxHealthPacks)
        {
            healthPacks = maxHealthPacks;
        }
    }

    public void AddXP()
    {
        if (currentLevelUpXP < xpCap)
        {
            currentXP++;
        }
    }

    public void GiveXP(int amount)
    {
        if (currentLevelUpXP < xpCap)
        {
            currentXP += amount;
        }
    }
    public void SetMaxHealth(int health)
    {
        maxHealth = health;
    }

    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
    }

    public void SetCurrentXP(int xp)
    {
        currentXP = xp;
    }

    public void SetCurrentLevelUpXP(int xp)
    {
        currentLevelUpXP = xp;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    public int GetMaxHealth => maxHealth;

    public int GetCurrentHealth => currentHealth;

    public int GetCurrentLevel => currentLevel;

    public int GetLevelUpXP => currentLevelUpXP;

    public int GetCurrentXP => currentXP;
}
