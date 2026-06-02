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

    [SerializeField] private float stoppingDistance = 1.5f;
    private float timer;
    [SerializeField] private float waitTime = 3f;

    [Header("AI Destinations")]
    [SerializeField] private Transform start;
    [SerializeField] private Transform[] collectMats;
    [SerializeField] private Transform[] restockMats;
    [SerializeField] private Transform craftStation;
    [SerializeField] private Transform deliverStation;
    [SerializeField] private Transform rest;

    private NavMeshAgent agent;

    private BT_Root root;

    private Status treeStatus = Status.Running;

    public static event Action onOrderRequest, onObjectCrafting, onCraftingFinished, onDeliverObject, onRestingStart, onRestingEnd;
    public static event Action<int> onMatsChange1, onMatsChange2, onMatsChange3;

    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
                
        root.GetPrintTree();
        Debug.Log(root.treeLog);
    }

    private void Update()
    {
        treeStatus = root.Process();
    }

    #region Behaviour Tree Actions
    private Status GoToAction(Vector3 destination, Action onSuccess)
    {
        return GoTo(destination, onSuccess);
    }
    
    private Status GoToIntAction(Vector3 destination, Action<int> onSuccessInt, int successInt)
    {
        Action onSuccess = null;
        return GoTo(destination, onSuccess, onSuccessInt, successInt);
    }
    
    private Status GoTo(Vector3 destination, Action onSuccess = null, Action<int> onSuccessInt = null, int successInt = 0)
    {
        if (state == ActionState.Idle)
        {
            agent.SetDestination(destination);
            state = ActionState.Working;
        }
        else if (Vector3.SqrMagnitude(agent.pathEndPosition - destination) >= stoppingDistance)
        {
            state = ActionState.Idle;
            return Status.Failure;
        }
        else if (Vector3.SqrMagnitude(destination - agent.transform.position) < stoppingDistance)
        {
            if (onSuccess != null)
                SuccessAction(onSuccess);
            if (onSuccessInt != null)
                SuccessActionInt(onSuccessInt, successInt);
            state = ActionState.Idle;
            return Status.Success;
        }

        return Status.Running;
    }
    
    private void SuccessAction(Action callback) => callback?.Invoke();
    private void SuccessActionInt(Action<int> callback, int i) => callback?.Invoke(i);
    
    private Status GoToStart()
    {
        return GoTo(start.position);
    }

    private Status ReceiveRandomOrder()
    {
        onOrderRequest?.Invoke();
        
        return Status.Success;
    }
    
    private Status HasMats1() => gm.mat1 < gm.orders[gm.randomOrder].material1 ? Status.Failure : Status.Success;

    private Status RestockMats1()
    {
        return GoToIntAction(restockMats[0].position, onMatsChange1, gm.maxMat1 - gm.mat1);
    }

    private Status CollectMats1()
    {
        if (gm.orders[gm.randomOrder].material1 == 0)
            return Status.Success;
        return GoToIntAction(collectMats[0].position, onMatsChange1, -gm.orders[gm.randomOrder].material1);
    }

    private Status PlaceMats1()
    {
        if (gm.orders[gm.randomOrder].material1 == 0)
            return Status.Success;
        return GoTo(craftStation.position);
    }

    private Status HasMats2() => gm.mat2 < gm.orders[gm.randomOrder].material2 ?  Status.Failure : Status.Success;

    private Status RestockMats2()
    {
        return GoToIntAction(restockMats[1].position, onMatsChange2, gm.maxMat2 - gm.mat2);
    }

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
    
    private Status HasMats3() => gm.mat3 < gm.orders[gm.randomOrder].material3 ? Status.Failure : Status.Success;

    private Status RestockMats3()
    {
        return GoToIntAction(restockMats[2].position, onMatsChange3, gm.maxMat3 - gm.mat3);
    }

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
    
    private Status CraftObject()
    {
        onObjectCrafting?.Invoke();
        
        return WaitProcess();
    }

    private Status WaitProcess()
    {
        timer += Time.deltaTime;
        
        if (timer < waitTime)
            return Status.Running;

        onCraftingFinished?.Invoke();
        timer = 0f;
        return Status.Success;
    }

    private Status DeliverObject()
    {
        return GoToAction(deliverStation.position, onDeliverObject);
    }

    private Status HasEnergy() => gm.energyBar <= 0f ? Status.Failure : Status.Success;
    
    private Status GoToRest()
    {
        return GoToAction(rest.position, onRestingStart);
    }

    private Status Rest()
    {
        if (gm.energyBar < gm.energyLimit)
            return Status.Running;

        onRestingEnd?.Invoke();
        return Status.Success;
    }
    #endregion
}
