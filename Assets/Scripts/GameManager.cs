using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState
{
    Normal,
    Double,
    Triple,
    Paused
}

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    
    [Min(3)] public int maxMat1 = 5;
    [HideInInspector] public int mat1 = 0;
    [Min(3)] public int maxMat2 = 5;
    [HideInInspector] public int mat2 = 0;
    [Min(3)] public int maxMat3 = 5;
    [HideInInspector] public int mat3 = 0;

    public float energyBar;
    public float energyLimit;
    [SerializeField] private float recoveryRate;
    private bool needsEnergy = false;

    [Header("Object Crafting")]
    //array used for reference for possible craftable object prefabs
    [SerializeField] private GameObject[] objects;
    public ObjectCraftingScriptable[] orders;
    public int randomOrder;

    public static event Action<float, float> onEnergyChanged;
    public static event Action<ObjectCraftingScriptable, int> onOrderGenerated;
    public static event Action<int, int> onMaterial1Changed, onMaterial2Changed, onMaterial3Changed;
    public static event Action<int> onSpeedChanged;

    private void OnEnable()
    {
        AIController.onOrderRequest += GenerateRandomOrder;
        AIController.onMatsChange1 += UpdateMaterials1;
        AIController.onMatsChange2 += UpdateMaterials2;
        AIController.onMatsChange3 += UpdateMaterials3;
        AIController.onObjectCrafting += SpawnObject;
        AIController.onCraftingFinished += HideObject;
        AIController.onRestingStart += StartResting;
        AIController.onRestingEnd += StopResting;
    }

    private void OnDisable()
    {
        AIController.onOrderRequest -= GenerateRandomOrder;
        AIController.onMatsChange1 -= UpdateMaterials1;
        AIController.onMatsChange2 -= UpdateMaterials2;
        AIController.onMatsChange3 -= UpdateMaterials3;
        AIController.onObjectCrafting -= SpawnObject;
        AIController.onCraftingFinished -= HideObject;
        AIController.onRestingStart -= StartResting;
        AIController.onRestingEnd -= StopResting;
    }

    private void Start()
    {
        gameState = GameState.Normal;
        UpdateMaterials1(maxMat1);
        UpdateMaterials2(maxMat2);
        UpdateMaterials3(maxMat3);
        energyBar = energyLimit;
    }

    private void Update()
    {
        energyBar = Mathf.Clamp(energyBar, 0f, energyLimit);
        
        if (!needsEnergy)
        {
            energyBar -= Time.deltaTime;
        }
        else
        {
            energyBar += recoveryRate * Time.deltaTime;
        }
        
        onEnergyChanged?.Invoke(energyBar, energyLimit);
    }

    private void GenerateRandomOrder()
    {
        randomOrder = Random.Range(0, orders.Length);
        onOrderGenerated?.Invoke(orders[randomOrder], randomOrder);
    }

    private void UpdateMaterials1(int changeAmount)
    {
        mat1 += changeAmount;
        onMaterial1Changed?.Invoke(mat1, maxMat1);
    } 

    private void UpdateMaterials2(int changeAmount)
    {
        mat2 += changeAmount;
        onMaterial2Changed?.Invoke(mat2, maxMat2);
    } 
    
    private void UpdateMaterials3(int changeAmount)
    {
        mat3 += changeAmount;
        onMaterial3Changed?.Invoke(mat3, maxMat3);
    }

    private void SpawnObject() => objects[randomOrder].SetActive(true);
    
    private void HideObject() => objects[randomOrder].SetActive(false);

    private void StartResting() => needsEnergy = true;
    
    private void StopResting() => needsEnergy = false;
    
    #region Buttons
    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        if (gameState != GameState.Paused)
        {
            gameState = GameState.Paused;
            Time.timeScale = 0f;
            onSpeedChanged?.Invoke(0);
        }
        else
        {
            gameState = GameState.Normal;
            Time.timeScale = 1f;
            onSpeedChanged?.Invoke(1);
        }
    }

    public void ChangeSpeed()
    {
        if (gameState == GameState.Paused)
            return;
        
        switch (gameState)
        {
            case GameState.Normal:
                gameState = GameState.Double;
                Time.timeScale = 2f;
                onSpeedChanged?.Invoke(2);
                break;
            
            case GameState.Double:
                gameState = GameState.Triple;
                Time.timeScale = 3f;
                onSpeedChanged?.Invoke(3);
                break;
            
            case GameState.Triple:
                gameState = GameState.Normal;
                Time.timeScale = 1f;
                onSpeedChanged?.Invoke(1);
                break;
        }
    }
    #endregion
}
