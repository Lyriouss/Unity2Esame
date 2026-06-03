public class BT_Selector : BT_Node
{
    //Constructor of Selector node that returns the name
    public BT_Selector(string name)
    {
        nodeName = name;
    }

    public override Status Process()
    {
        //Runs the Process of current child node
        Status childStatus = children[currentChildIndex].Process();

        //Debug.Log(children[currentChildIndex].nodeName + " : " + childStatus);
        
        //When node returns Running, returns this Status to Selector to continue running current node Process
        if (childStatus == Status.Running) return childStatus;

        //If node returns Success, resets currentChildIndex to 0 and ends Selector Process by returning Success
        if (childStatus == Status.Success)
        {
            currentChildIndex = 0;
            return childStatus;
        }
        
        //If child node returns Failure, proceeds to next child node
        currentChildIndex++;

        //If there are no more child nodes and all nodes returned Failure, resets currentChildIndex to 0 and ends Selector Process by returning Failure
        if (currentChildIndex >= children.Count)
        {
            currentChildIndex = 0;
            return Status.Failure;
        }

        //Returns Running at end to continue Selector if either Success or Failure hasn't returned yet
        return Status.Running;
    }
}
