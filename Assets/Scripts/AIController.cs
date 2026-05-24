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
    private int currentMatCheck = 0;

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

    public static event Action onOrderRequest;

    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        root = new BT_Root("Root");
        BT_Sequence seq = new BT_Sequence("Sequence");
        BT_Selector sel = new BT_Selector("Selector");
        
        BT_Leaf goToStart = new BT_Leaf("GoToStart", GoToStart);
        BT_Leaf receiveRandomOrder = new BT_Leaf("ReceiveRandomOrder", ReceiveRandomOrder);

        BT_Leaf hasMaterials = new BT_Leaf("HasMaterials", HasMaterials);
        BT_Leaf restockMats = new BT_Leaf("RestockMats", RestockMats);
        BT_Leaf collectMats = new BT_Leaf("CollectMats", CollectMats);
        BT_Leaf placeMats = new BT_Leaf("PlaceMats", PlaceMats);
        
        BT_Leaf craftObject = new BT_Leaf("CraftObject", CraftObject);
        BT_Leaf deliverObject = new BT_Leaf("DeliverObject", DeliverObject);
        
        BT_Leaf hasEnergy = new BT_Leaf("HasEnergy", HasEnergy);
        BT_Leaf rest = new BT_Leaf("Rest", Rest);
        
        
        root.AddChild(seq);
            seq.AddChild(goToStart);
            seq.AddChild(receiveRandomOrder);
            seq.AddChild(sel);
                sel.AddChild(hasMaterials);
                sel.AddChild(restockMats);
            seq.AddChild(collectMats);
            seq.AddChild(placeMats);
            seq.AddChild(sel);
                sel.AddChild(hasMaterials);
                sel.AddChild(restockMats);
            seq.AddChild(collectMats);
            seq.AddChild(placeMats);
            seq.AddChild(sel);
                sel.AddChild(hasMaterials);
                sel.AddChild(restockMats);
            seq.AddChild(placeMats);
            seq.AddChild(craftObject);
            seq.AddChild(deliverObject);
                sel.AddChild(hasEnergy);
                sel.AddChild(rest);
    }

    private void Update()
    {
        treeStatus = root.Process();
    }

    private Status HasMaterials()
    {
        if (gm.currentMats[currentMatCheck] < gm.currentOrderMats[currentMatCheck])
        {
            return Status.Failure;
        }
        return Status.Success;
    }
    
    private Status HasEnergy() => throw new System.NotImplementedException();
    
    private Status GoTo(Vector3 destination)
    {
        agent.SetDestination(destination);

        if (Vector3.SqrMagnitude(agent.pathEndPosition - destination) >= stoppingDistance)
        {
            state = ActionState.Idle;
            return Status.Failure;
        }

        state = ActionState.Working;
        return Status.Running;
    }

    private Status GoToStart()
    {
        if (state == ActionState.Idle)
        {
            return GoTo(start.position);
        }
        else if (Vector3.SqrMagnitude(agent.pathEndPosition - start.position) < stoppingDistance)
        {
            state = ActionState.Idle;
            return Status.Success;
        }

        return Status.Running;
    }

    private Status ReceiveRandomOrder()
    {
        onOrderRequest?.Invoke();
            
        return Status.Success;
    }

    private Status RestockMats()
    {
        if (state == ActionState.Idle)
        {
            return GoTo(restockMats[currentMatCheck].position);
        }
        else if (Vector3.SqrMagnitude(agent.pathEndPosition - restockMats[currentMatCheck].position) < stoppingDistance)
        {
            //restock materials action

            state = ActionState.Idle;
            return Status.Success;
        }

        return Status.Running;
    }

    private Status CollectMats()
    {
        throw new System.NotImplementedException();
    }

    private Status PlaceMats()
    {
        throw new System.NotImplementedException();
    }
    
    private Status CraftObject()
    {
        throw new System.NotImplementedException();
    }

    private Status DeliverObject()
    {
        throw new System.NotImplementedException();
    }

    private Status Rest()
    {
        throw new System.NotImplementedException();
    }
}
