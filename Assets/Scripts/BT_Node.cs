using System.Collections.Generic;

public enum Status
{
    Running,
    Success,
    Failure
}

public class BT_Node
{
    public Status status;
    
    //Node list for children nodes to be Processed if node has any
    public List<BT_Node> children = new List<BT_Node>();
    //Index of this node's current child being Processed
    protected int currentChildIndex = 0;

    //String that will then contain this node's name, used to print Behavior Tree in Console
    public string nodeName;

    //Function used on this node to add children nodes to be Processed
    public void AddChild(BT_Node child)
    {
        children.Add(child);
    }
    
    //Virtual Status Process that returns the Process of the current child node
    public virtual Status Process()
    {
        return children[currentChildIndex].Process();
    }
}
