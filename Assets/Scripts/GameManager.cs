using System;
using UnityEngine;
using Random = UnityEngine.Random;

//Enum for the different game speeds and pause state
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
    
    //Doesn't allow maxMat values to go below 3
    [Min(3)] public int maxMat1 = 5;
    [HideInInspector] public int mat1;
    [Min(3)] public int maxMat2 = 5;
    [HideInInspector] public int mat2;
    [Min(3)] public int maxMat3 = 5;
    [HideInInspector] public int mat3;

    [Header("Object Crafting")]
    //Array of objects to be crafted
    [SerializeField] private GameObject[] objects;
    //Array of Scriptable Objects to get amount of materials needed to craft the object
    public ObjectCraftingScriptable[] orders;
    public int randomOrder;

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
    }

    private void OnDisable()
    {
        AIController.onOrderRequest -= GenerateRandomOrder;
        AIController.onMatsChange1 -= UpdateMaterials1;
        AIController.onMatsChange2 -= UpdateMaterials2;
        AIController.onMatsChange3 -= UpdateMaterials3;
        AIController.onObjectCrafting -= SpawnObject;
        AIController.onCraftingFinished -= HideObject;
    }

    private void Start()
    {
        //Sets GameState to default value
        gameState = GameState.Normal;
        //Updates current materials to their max amount and updates UI at the same time
        UpdateMaterials1(maxMat1);
        UpdateMaterials2(maxMat2);
        UpdateMaterials3(maxMat3);
    }

    private void GenerateRandomOrder()
    {
        //Gets a random index number for the order
        randomOrder = Random.Range(0, orders.Length);
        //Then updates UI order information screens
        onOrderGenerated?.Invoke(orders[randomOrder], randomOrder);
    }

    //Updates first material count and updates amount shown from UIManager
    private void UpdateMaterials1(int changeAmount)
    {
        mat1 += changeAmount;
        onMaterial1Changed?.Invoke(mat1, maxMat1);
    } 

    //Updates second material count and updates amount shown from UIManager
    private void UpdateMaterials2(int changeAmount)
    {
        mat2 += changeAmount;
        onMaterial2Changed?.Invoke(mat2, maxMat2);
    } 
    
    //Updates third material count and updates amount shown from UIManager
    private void UpdateMaterials3(int changeAmount)
    {
        mat3 += changeAmount;
        onMaterial3Changed?.Invoke(mat3, maxMat3);
    }

    //Shows the object that is being crafted at crafting station during crafting Process
    private void SpawnObject() => objects[randomOrder].SetActive(true);
    
    //Hides the object crafted when finishing crafting Process
    private void HideObject() => objects[randomOrder].SetActive(false);
    
    #region Buttons
    //Closes application from build
    public void QuitGame() => Application.Quit();

    public void PauseGame()
    {
        //If game isn't paused, then pauses game and updates UI buttons from UIManager
        if (gameState != GameState.Paused)
        {
            gameState = GameState.Paused;
            Time.timeScale = 0f;
            onSpeedChanged?.Invoke(0);
        }
        //Else resumes the game and updates UI buttons from UIManager
        else
        {
            gameState = GameState.Normal;
            Time.timeScale = 1f;
            onSpeedChanged?.Invoke(1);
        }
    }

    public void ChangeSpeed()
    {
        //If game is paused, then skips this function
        if (gameState == GameState.Paused)
            return;
        
        //Based on the current GameState, switches to next speed and updates UI buttons from UIManager
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
