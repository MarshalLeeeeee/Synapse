/*
 * Basic node of the game tree
 * Node can be serialized and synchronized from server to client
 * Depending on different client, node can be synchronized differently and partially
 */

public class Node
{
    /* Every node has a unique id at runtime，
     * which serves as the identifier of the node in both server and client. 
     */
    private string id = "";

    /* Init Node with given data or use default data */
    public void Init()
    {

    }
}