using UnityEngine;

public class BT_Selector : BT_Node
{
    public BT_Selector(string name)
    {
        nodeName = name;
    }

    public override Status Process()
    {
        Status childStatus = children[currentChildIndex].Process();

        //Debug.Log(children[currentChildIndex].nodeName + " : " + childStatus);

        if (childStatus == Status.Running) return childStatus;

        if (childStatus == Status.Success)
        {
            currentChildIndex = 0;
            return childStatus;
        }
        
        currentChildIndex++;

        if (currentChildIndex >= children.Count)
        {
            currentChildIndex = 0;
            return Status.Failure;
        }

        return Status.Running;
    }
}
