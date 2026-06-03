public class BT_Root : BT_Node
{
    //String used to print Behavior Tree in Console
    public string treeLog = null;
    
    //Constructor of Root node that returns the name
    public BT_Root(string name)
    {
        nodeName = name;
    }

    //Gets all Behavior Tree nodes and Sets their names to a single string
    public void GetPrintTree()
    {
        PrintTree(this, 0);
    }
    
    private void PrintTree(BT_Node child, int level)
    {
        //Updates string of treeLog by adding name of the current node with - an amount of times equals to level of node
        treeLog = treeLog + new string('-', level) + child.nodeName + "\n";
        
        //Checks if this node has children
        foreach (BT_Node node in child.children)
        {
            //And for every object that is a child, adds a level to child
            PrintTree(node, level + 1);
        }
    }
}
