using System;
using UnityEngine;
using UnityEngine.AI;

public enum ActionState
{
    Idle,
    Working
}

public class AIController : MonoBehaviour
{
    private GameManager gm;

    private ActionState state = ActionState.Idle;

    [SerializeField] private float craftingWaitTime = 3f;
    private float timer;
    
    [Header("Energy Controller")]
    [SerializeField] private float energyLimit = 50f;
    [SerializeField] private float energyBar;
    [SerializeField] private float recoveryRate = 5f;
    private bool needsEnergy;

    //Travel points for AI Nav Mesh
    [Header("AI Destinations")]
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private Transform start;
    [SerializeField] private Transform[] collectMats;
    [SerializeField] private Transform[] restockMats;
    [SerializeField] private Transform craftStation;
    [SerializeField] private Transform deliverStation;
    [SerializeField] private Transform rest;

    private NavMeshAgent agent;

    private BT_Root root;

    private Status treeStatus = Status.Running;

    public static event Action<float, float> onEnergyChanged;
    
    //Actions to be called when reaching Success on specific nodes
    public static event Action onOrderRequest, onObjectCrafting, onCraftingFinished, onDeliverObject;
    public static event Action<int> onMatsChange1, onMatsChange2, onMatsChange3;

    private void Awake()
    {
        //Assigns variable components
        gm = FindAnyObjectByType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        
        //Initialization of Behavior Tree Nodes by getting name of the node and Status action of Leafs
        root = new BT_Root("Root");
        BT_Sequence seq = new BT_Sequence("Sequence");
        BT_Selector matSel1 = new BT_Selector("Material1 Selector");
        BT_Selector matSel2  = new BT_Selector("Material2 Selector");
        BT_Selector matSel3  = new BT_Selector("Material3 Selector");
        BT_Selector energySel = new BT_Selector("Energy Selector");
        BT_Sequence energySeq = new BT_Sequence("Energy Sequence");
        
        BT_Leaf goToStart = new BT_Leaf("GoToStart", GoToStart);
        BT_Leaf receiveRandomOrder = new BT_Leaf("ReceiveRandomOrder", ReceiveRandomOrder);

        BT_Leaf hasMats1 = new BT_Leaf("HasMaterials1", HasMats1);
        BT_Leaf restockMats1 = new BT_Leaf("RestockMats1", RestockMats1);
        BT_Leaf collectMats1 = new BT_Leaf("CollectMats1", CollectMats1);
        BT_Leaf placeMats1 = new BT_Leaf("PlaceMats1", PlaceMats1);
        BT_Leaf hasMats2 = new BT_Leaf("HasMaterials2", HasMats2);
        BT_Leaf restockMats2 = new BT_Leaf("RestockMats2", RestockMats2);
        BT_Leaf collectMats2 = new BT_Leaf("CollectMats2", CollectMats2);
        BT_Leaf placeMats2 = new BT_Leaf("PlaceMats1", PlaceMats2);
        BT_Leaf hasMats3 = new BT_Leaf("HasMaterials3", HasMats3);
        BT_Leaf restockMats3 = new BT_Leaf("RestockMats3", RestockMats3);
        BT_Leaf collectMats3 = new BT_Leaf("CollectMats3", CollectMats3);
        BT_Leaf placeMats3 = new BT_Leaf("PlaceMats1", PlaceMats3);
        
        BT_Leaf craftObject = new BT_Leaf("CraftObject", CraftObject);
        BT_Leaf deliverObject = new BT_Leaf("DeliverObject", DeliverObject);
        
        BT_Leaf hasEnergy = new BT_Leaf("HasEnergy", HasEnergy);
        BT_Leaf goToRest = new BT_Leaf("GoToRest", GoToRest);
        BT_Leaf rest = new BT_Leaf("Rest", Rest);
        
        //Construction of Behavior Tree by going through the process from top to bottom
        root.AddChild(seq);
            seq.AddChild(goToStart);
            seq.AddChild(receiveRandomOrder);
            seq.AddChild(matSel1);
                matSel1.AddChild(hasMats1);
                matSel1.AddChild(restockMats1);
            seq.AddChild(collectMats1);
            seq.AddChild(placeMats1);
            seq.AddChild(matSel2);
                matSel2.AddChild(hasMats2);
                matSel2.AddChild(restockMats2);
            seq.AddChild(collectMats2);
            seq.AddChild(placeMats2);
            seq.AddChild(matSel3);
                matSel3.AddChild(hasMats3);
                matSel3.AddChild(restockMats3);
            seq.AddChild(collectMats3);
            seq.AddChild(placeMats3);
            seq.AddChild(craftObject);
            seq.AddChild(deliverObject);
            seq.AddChild(energySel);
                energySel.AddChild(hasEnergy);
                energySel.AddChild(energySeq);
                    energySeq.AddChild(goToRest);
                    energySeq.AddChild(rest);
        
        //Gets the string of entire Behavior Tree structure to be printed on the console
        root.GetPrintTree();
        Debug.Log(root.treeLog);
    }

    private void Start()
    {
        //Sets energy bar at start to it's max amount
        energyBar = energyLimit;
        needsEnergy = false;
        timer = 0f;
    }

    private void Update()
    {
        //Runs through the Behavior Tree's current Process (starts from beginning upon reaching the end)
        treeStatus = root.Process();
        //Consumes or Recharges energy based on whether it's resting or not
        ChangeEnergy();
    }

    private void ChangeEnergy()
    {
        //If AI doesn't need energy
        if (!needsEnergy)
        {
            //Consumes 1 energy every second
            energyBar -= Time.deltaTime;
        }
        //If instead it needsEnergy
        else
        {
            //Recovers energy based on recovery rate amount every second
            energyBar += recoveryRate * Time.deltaTime;
        }
        
        //Always keeps the value of energy bar within 0 and it's max value
        energyBar = Mathf.Clamp(energyBar, 0f, energyLimit);
        //Changes the energy bar screen fill amount from UIManager
        onEnergyChanged?.Invoke(energyBar, energyLimit);
    }

    #region Behaviour Tree Actions
    #region Go To Processes
    //GoTo action when needing to pass an event callback for Success
    private Status GoToAction(Vector3 destination, Action onSuccess)
    {
        return GoTo(destination, onSuccess);
    }
    
    //GoTo action when needing to pass an event callback that contains an int reference for Success
    private Status GoToIntAction(Vector3 destination, Action<int> onSuccessInt, int successInt)
    {
        Action onSuccess = null;
        return GoTo(destination, onSuccess, onSuccessInt, successInt);
    }
    
    //GoTo travels to the assigned destination of nav mesh and checks different factors to return a specific Status result
    //Action references are called when Status returns Success and have an overload so that they are not obligatory to add when calling GoTo()
    private Status GoTo(Vector3 destination, Action onSuccess = null, Action<int> onSuccessInt = null, int successInt = 0)
    {
        //If ActionState is in Idle state, assigns travel point to Nav Mesh and changes ActionState to Working
        if (state == ActionState.Idle)
        {
            agent.SetDestination(destination);
            state = ActionState.Working;
        }
        //If transform point to travel to resides outside Nav Mesh bounds, returns Failure and changes state to Idle
        else if (Vector3.SqrMagnitude(agent.pathEndPosition - destination) >= stoppingDistance)
        {
            state = ActionState.Idle;
            return Status.Failure;
        }
        //Upon reaching the vicinity of travel point (within stoppingDistance) returns Success and changes state to Idle
        else if (Vector3.SqrMagnitude(destination - agent.transform.position) < stoppingDistance)
        {
            //If there are any Action references, calls those Action upon Success
            if (onSuccess != null)
                onSuccess?.Invoke();
            if (onSuccessInt != null)
                onSuccessInt?.Invoke(successInt);
            
            state = ActionState.Idle;
            return Status.Success;
        }

        //Returns Running if no other Status is returned and continues current node
        return Status.Running;
    }
    #endregion
    
    #region Starting Process
    //Goes to start position
    private Status GoToStart() => GoTo(start.position);

    //Calls a random order from GameManager and immediately returns Success
    private Status ReceiveRandomOrder()
    {
        onOrderRequest?.Invoke();
        
        return Status.Success;
    }
    #endregion
    
    #region Collection Processes
    //From this point, repeats Processes from HasMats1() to PlaceMats1() multiple times but with different references
    //Bool checks the first materials to see if there are enough to collect for order
    private Status HasMats1() => gm.mat1 < gm.orders[gm.randomOrder].material1 ? Status.Failure : Status.Success;

    //If HasMats1 bool check returns Failure
    //Goes to restock area and upon reaching travel point, refills materials to max without going over material limit
    private Status RestockMats1() => GoToIntAction(restockMats[0].position, onMatsChange1, gm.maxMat1 - gm.mat1);

    private Status CollectMats1()
    {
        //If none of these materials are needed, immediately returns Success to skip GoTo()
        if (gm.orders[gm.randomOrder].material1 == 0)
            return Status.Success;
        //Goes to collect materials from collect area, and consumes an amount equivalent to the quantity the order requires
        return GoToIntAction(collectMats[0].position, onMatsChange1, -gm.orders[gm.randomOrder].material1);
    }

    private Status PlaceMats1()
    {
        //If none of these materials are needed, immediately returns Success to skip GoTo()
        if (gm.orders[gm.randomOrder].material1 == 0)
            return Status.Success;
        //Goes to place the materials at the crafting station (although it only travels to this point after collecting materials)
        return GoTo(craftStation.position);
    }

    //Same process as shown above but with different material references
    private Status HasMats2() => gm.mat2 < gm.orders[gm.randomOrder].material2 ?  Status.Failure : Status.Success;

    private Status RestockMats2() => GoToIntAction(restockMats[1].position, onMatsChange2, gm.maxMat2 - gm.mat2);

    private Status CollectMats2()
    {
        if (gm.orders[gm.randomOrder].material2 == 0)
            return Status.Success;
        return GoToIntAction(collectMats[1].position, onMatsChange2, -gm.orders[gm.randomOrder].material2);
    }
    
    private Status PlaceMats2()
    {
        if (gm.orders[gm.randomOrder].material2 == 0)
            return Status.Success;
        return GoTo(craftStation.position);
    }
    
    //Same process as shown above but with different material references
    private Status HasMats3() => gm.mat3 < gm.orders[gm.randomOrder].material3 ? Status.Failure : Status.Success;

    private Status RestockMats3() => GoToIntAction(restockMats[2].position, onMatsChange3, gm.maxMat3 - gm.mat3);

    private Status CollectMats3()
    {
        if (gm.orders[gm.randomOrder].material3 == 0)
            return Status.Success;
        return GoToIntAction(collectMats[2].position, onMatsChange3, -gm.orders[gm.randomOrder].material3);
    }
    
    private Status PlaceMats3()
    {
        if (gm.orders[gm.randomOrder].material3 == 0)
            return Status.Success;
        return GoTo(craftStation.position);
    }
    #endregion
    
    #region Crafting Processes
    //After last PlaceMats() Process, Nav Mesh will already be at crafting station
    private Status CraftObject()
    {
        //So it will immediately call crafting event
        onObjectCrafting?.Invoke();
        
        //And wait at that position for a set amount of time
        return WaitProcess();
    }

    private Status WaitProcess()
    {
        timer += Time.deltaTime;
        
        //Returns Running if timer has not finished yet
        if (timer < craftingWaitTime)
            return Status.Running;

        //Upon reaching end of timer, deactivates crafting object through GameManager and resets timer for next time while returning Success
        onCraftingFinished?.Invoke();
        timer = 0f;
        return Status.Success;
    }

    //Goes to the delivery station and upon reaching the point, deactivates order screen through UIManager
    private Status DeliverObject() => GoToAction(deliverStation.position, onDeliverObject);
    #endregion

    #region Energy Processes
    //Bool check to see if the AI has enough energy to continue orders
    private Status HasEnergy() => energyBar <= 0f ? Status.Failure : Status.Success;
    
    //If bool check returns Failure, goes to rest area
    private Status GoToRest() => GoTo(rest.position);

    //Then starts Rest Process
    private Status Rest()
    {
        //Sets needsEnergy to true so that energy can start to recuperate
        needsEnergy = true;
        
        //As long as current energy is below its max value, returns Running
        if (energyBar < energyLimit)
            return Status.Running;

        //As soon as energy reaches it's limit, sets needsEnergy to false so that it starts consuming energy again and returns Success
        needsEnergy = false;
        return Status.Success;
    }
    #endregion
    #endregion
}
