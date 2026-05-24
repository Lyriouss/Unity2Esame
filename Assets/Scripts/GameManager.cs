using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private int matVariants = 3;
    private int maxMats = 5;
    public int[] currentMats;

    [Header("Object Crafting")]
    //array used for reference for possible craftable object prefabs
    [SerializeField] private GameObject[] objects;
    //first array is used to identify which object we need materials from
    //second array holds the amount of materials needed for crafting
    [SerializeField] private int[][] objectMats;

    public int randomOrder;
    public List<int> currentOrderMats = new List<int>();

    private void OnEnable()
    {
        AIController.onOrderRequest += GenerateRandomOrder;
    }

    private void OnDisable()
    {
        AIController.onOrderRequest -= GenerateRandomOrder;
    }

    private void Start()
    {
        List<int> matsList = new List<int>();
        for (int i = 0; i < matVariants; i++)
        {
            matsList.Add(i);
            matsList[i] = maxMats;
        }
        currentMats = matsList.ToArray();
    }

    private void GenerateRandomOrder()
    {
        currentOrderMats.Clear();

        randomOrder = Random.Range(0, objects.Length);

        foreach (int mat in objectMats[randomOrder])
        {
            currentOrderMats.Add(mat);
        }
    }
}
