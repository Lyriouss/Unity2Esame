public class BT_Sequence : BT_Node
{
    //Constructor of Sequence node that returns the name
    public BT_Sequence(string name)
    {
        nodeName = name;
    }

    public override Status Process()
    {
        //Runs the Process of current child node
        Status childStatus = children[currentChildIndex].Process();
        
        //Debug.Log(children[currentChildIndex].nodeName + " : " + childStatus);
        
        //When node returns Running, returns this Status to Sequence to continue running current node Process
        if (childStatus == Status.Running) return childStatus;
        
        //If node returns Failure, resets currentChildIndex to 0 and ends Sequence Process by returning Failure
        if (childStatus == Status.Failure) 
        {
            currentChildIndex = 0;
            return childStatus;
        }
        
        //If child node returns Success, proceeds to next child node
        currentChildIndex++;
        
        //If there are no more child nodes and all nodes returned Success, resets currentChildIndex to 0 and ends Sequence Process by returning Success
        if (currentChildIndex >= children.Count)
        {
            currentChildIndex = 0;
            return Status.Success;
        }
        
        //Returns Running at end to continue Sequence if either Success or Failure hasn't returned yet
        return Status.Running;
    }
}
