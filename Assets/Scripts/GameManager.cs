using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Min(3)] public int maxMat1 = 5;
    public int mat1;
    [Min(3)] public int maxMat2 = 5;
    public int mat2;
    [Min(3)] public int maxMat3 = 5;
    public int mat3;

    public float energyBar;
    public float energyLimit;
    [SerializeField] private float recoveryRate;
    private bool needsEnergy = false;

    [Header("Object Crafting")]
    //array used for reference for possible craftable object prefabs
    [SerializeField] private GameObject[] objects;
    public ObjectCraftingScriptable[] orders;
    public int randomOrder;

    private void OnEnable()
    {
        AIController.onOrderRequest += GenerateRandomOrder;
        AIController.onMatsChange1 += UpdateMaterials1;
        AIController.onMatsChange2 += UpdateMaterials2;
        AIController.onMatsChange3 += UpdateMaterials3;
        AIController.onRestingStart += StartResting;
        AIController.onRestingEnd += StopResting;
    }

    private void OnDisable()
    {
        AIController.onOrderRequest -= GenerateRandomOrder;
    }

    private void Start()
    {
        mat1 = maxMat1;
        mat2 = maxMat2;
        mat3 = maxMat3;
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
    }

    private void GenerateRandomOrder()
    {
        randomOrder = Random.Range(0, orders.Length);
    }

    private void UpdateMaterials1(int changeAmount) => mat1 += changeAmount;
    
    private void UpdateMaterials2(int changeAmount) => mat2 += changeAmount;
    
    private void UpdateMaterials3(int changeAmount) => mat3 += changeAmount;

    private void StartResting() => needsEnergy = true;
    
    private void StopResting() => needsEnergy = false;
}
