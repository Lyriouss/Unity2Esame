using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("World Space UI")]
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text[] orderMatText;
    [SerializeField] private Image objectScreenSprite;
    [SerializeField] private Sprite[] objectCraftSprites;
    [SerializeField] private Image energyBar;
    [SerializeField] private TMP_Text mat1Text;
    [SerializeField] private TMP_Text mat2Text;
    [SerializeField] private TMP_Text mat3Text;

    [Header("Behaviour Tree Drawings")] 
    [SerializeField] private GameObject usedBehaviourTree;
    [SerializeField] private GameObject scrappedBT;

    private void OnEnable()
    {
        GameManager.onOrderGenerated += UpdateOrderInfo;
        GameManager.onSpeedChanged += UpdateSpeedText;
        GameManager.onMaterial1Changed += UpdateMat1Text;
        GameManager.onMaterial2Changed += UpdateMat2Text;
        GameManager.onMaterial3Changed += UpdateMat3Text;
        
        AIController.onEnergyChanged += UpdateEnergyBar;
        AIController.onDeliverObject += HideOrderInfo;
    }

    private void OnDisable()
    {
        GameManager.onOrderGenerated -= UpdateOrderInfo;
        GameManager.onSpeedChanged -= UpdateSpeedText;
        GameManager.onMaterial1Changed -= UpdateMat1Text;
        GameManager.onMaterial2Changed -= UpdateMat2Text;
        GameManager.onMaterial3Changed -= UpdateMat3Text;
        
        AIController.onEnergyChanged -= UpdateEnergyBar;
        AIController.onDeliverObject -= HideOrderInfo;
    }

    private void Start()
    {
        //Sets all UI elements to default values at start of the game
        UpdateSpeedText(1);
        HideOrderInfo();
        energyBar.fillAmount = 1f;
        usedBehaviourTree.SetActive(false);
        scrappedBT.SetActive(false);
    }
    
    //Updates energy bar screens fill amount 
    private void UpdateEnergyBar(float currentEnergy, float maxEnergy) => energyBar.fillAmount = currentEnergy / maxEnergy;

    private void UpdateOrderInfo(ObjectCraftingScriptable order, int orderIndex)
    {
        //Updates first screen with material amount needed for each material
        orderMatText[0].text = "Mat 1: " + order.material1.ToString();
        orderMatText[1].text = "Mat 2: " + order.material2.ToString();
        orderMatText[2].text = "Mat 3: " + order.material3.ToString();
        //And second screen with the image of object to be crafted
        objectScreenSprite.sprite = objectCraftSprites[orderIndex];
        
        //Then activates the order information game objects from screens
        foreach (TMP_Text text in orderMatText)
        {
            text.gameObject.SetActive(true);
        }
        objectScreenSprite.gameObject.SetActive(true);
    }

    private void HideOrderInfo()
    {
        //Deactivates game objects of order information from screens
        foreach (TMP_Text text in orderMatText)
        {
            text.gameObject.SetActive(false);
        }
        objectScreenSprite.gameObject.SetActive(false);
    }
    
    private void UpdateSpeedText(int speed)
    {
        //Based on speed reference of game passed, changes relative button texts that indicate game speed
        switch (speed)
        {
            case 0:
                speedText.text = "-";
                pauseText.text = "I>";
                break;
            
            case 1:
                speedText.text = ">";
                pauseText.text = "II";
                break;
            
            case 2:
                speedText.text = ">>";
                pauseText.text = "II";
                break;
            
            case 3:
                speedText.text = ">>>";
                pauseText.text = "II";
                break;
        }
    }
    
    //Updates material count for box with first materials
    private void UpdateMat1Text(int mat1, int maxMat1)
    {
        mat1Text.text = mat1.ToString() + "/" + maxMat1.ToString();
    }

    //Updates material count for box with second materials
    private void UpdateMat2Text(int mat2, int maxMat2)
    {
        mat2Text.text = mat2.ToString() + "/" + maxMat2.ToString();
    }

    //Updates material count for box with third materials
    private void UpdateMat3Text(int mat3, int maxMat3)
    {
        mat3Text.text = mat3.ToString() + "/" + maxMat3.ToString();
    }
    
    #region Behaviour Tree Drawings
    //Shows image of the Behavior Tree that is being used for exam
    public void OpenBehaviourTree() => usedBehaviourTree.SetActive(true);

    //Hides Behaviour Tree Drawing
    public void CloseBehaviourTree() => usedBehaviourTree.SetActive(false);

    //Shows other Behavior Tree Drawing made while planning
    public void NextBTTab() => scrappedBT.SetActive(true);
    
    //Hides other Behavior Tree Drawing made while planning while keeping other Behavior Tree shown
    public void PreviousBTTab() => scrappedBT.SetActive(false);
    #endregion
}