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
    
    public List<BT_Node> children = new List<BT_Node>();
    public BT_Node() {}

    public int currentChildIndex = 0;

    public string nodeName;

    public void AddChild(BT_Node child)
    {
        children.Add(child);
    }
    
    public virtual Status Process()
    {
        return children[currentChildIndex].Process();
    }
}
