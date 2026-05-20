using UnityEngine;

public class BT_Root : BT_Node
{
    public BT_Root(string name)
    {
        nodeName = name;
    }

    public void PrintTree(BT_Node child, int level)
    {
        Debug.Log(new string('-', level) + child.nodeName);
        foreach (BT_Node node in child.children)
        {
            PrintTree(node, level + 1);
        }
}
}
