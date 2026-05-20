using UnityEngine;
using UnityEngine.AI;

public enum ActionState
{
    Idle,
    Working
}

public class AIController : MonoBehaviour
{
    private ActionState state = ActionState.Idle;
    
    //action variables

    private NavMeshAgent agent;

    private BT_Root root;

    private Status treeStatus = Status.Running;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        root = new BT_Root("Root");
        BT_Sequence seq = new BT_Sequence("Sequence");
        BT_Selector sel = new BT_Selector("Selector");
        
        BT_Leaf goToStart = new BT_Leaf("GoToStart", GoToStart);
        BT_Leaf receiveRandomOrder = new BT_Leaf("ReceiveRandomOrder", ReceiveRandomOrder);

        BT_Leaf hasMaterials = new BT_Leaf("HasMaterials", HasMaterials);
        BT_Leaf restockMat1 = new BT_Leaf("RestockMat1", RestockMat1);
        BT_Leaf collectMat1 = new BT_Leaf("CollectMat1", CollectMat1);
        BT_Leaf placeMat1 = new BT_Leaf("PlaceMat1", PlaceMat1);
        
        BT_Leaf craftObject = new BT_Leaf("CraftObject", CraftObject);
        BT_Leaf deliverObject = new BT_Leaf("DeliverObject", DeliverObject);
        
        BT_Leaf hasEnergy = new BT_Leaf("HasEnergy", HasEnergy);
        BT_Leaf rest = new BT_Leaf("Rest", Rest);
        
        
        root.AddChild(seq);
            seq.AddChild(goToStart);
            seq.AddChild(receiveRandomOrder);
            seq.AddChild(sel);
                sel.AddChild(hasMaterials);
                sel.AddChild(restockMat1);
            seq.AddChild(collectMat1);
            seq.AddChild(placeMat1);
            seq.AddChild(craftObject);
            seq.AddChild(deliverObject);
                sel.AddChild(hasEnergy);
                sel.AddChild(rest);
    }

    private void Update()
    {
        treeStatus = root.Process();
    }

    private Status HasMaterials() => throw new System.NotImplementedException();
    
    private Status HasEnergy() => throw new System.NotImplementedException();
    

    private Status GoToStart()
    {
        throw new System.NotImplementedException();
    }

    private Status ReceiveRandomOrder()
    {
        throw new System.NotImplementedException();
    }

    private Status RestockMat1()
    {
        throw new System.NotImplementedException();
    }

    private Status CollectMat1()
    {
        throw new System.NotImplementedException();
    }

    private Status PlaceMat1()
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
