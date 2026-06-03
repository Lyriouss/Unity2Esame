public class BT_Leaf : BT_Node
{
    //Status Tick delegate functions as the Process of the node (aka Update())
    public delegate Status Tick();
    public Tick ProcessMethod;

    //Constructor of Leaf node that returns the name and ProcessMethod delegate
    public BT_Leaf(string name, Tick processMethod)
    {
        nodeName = name;
        ProcessMethod = processMethod;
    }

    //If Leaf has no Process Method, then automatically returns Failure, else return it's ProcessMethod
    public override Status Process()
    {
        if (ProcessMethod != null)
            return ProcessMethod();
        return Status.Failure;
    }
}
