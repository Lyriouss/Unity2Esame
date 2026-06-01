using UnityEngine;

public class BT_Root : BT_Node
{
    public string treeLog = null;
    
    public BT_Root(string name)
    {
        nodeName = name;
    }

    public void GetPrintTree()
    {
        PrintTree(this, 0);
    }
    
    private void PrintTree(BT_Node child, int level)
    {
        treeLog = treeLog + new string('-', level) + child.nodeName + "\n";
        
        foreach (BT_Node node in child.children)
        {
            PrintTree(node, level + 1);
        }
    }
}
