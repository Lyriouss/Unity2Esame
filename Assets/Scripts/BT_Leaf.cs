using UnityEngine;

public class BT_Leaf : BT_Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public BT_Leaf(string name, Tick processMethod)
    {
        nodeName = name;
        ProcessMethod = processMethod;
    }

    public override Status Process()
    {
        if (ProcessMethod != null)
            return ProcessMethod();
        return Status.Failure;
    }
}
