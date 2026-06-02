using UnityEngine;

public class BT_Sequence : BT_Node
{
    public BT_Sequence(string name)
    {
        nodeName = name;
    }

    public override Status Process()
    {
        Status childStatus = children[currentChildIndex].Process();
        
        //Debug.Log(children[currentChildIndex].nodeName + " : " + childStatus);
        
        if (childStatus == Status.Running) return Status.Running;
        
        if (childStatus == Status.Failure) return Status.Failure;
        
        currentChildIndex++;
        
        if (currentChildIndex >= children.Count)
        {
            currentChildIndex = 0;
            return Status.Success;
        }
        
        return Status.Running;
    }
}
