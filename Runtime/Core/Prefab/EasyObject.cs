namespace Easy;

public class EasyObject : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public int InstanceID;
    /// <summary>
    /// 路径
    /// </summary>
    public string URL
    {
        get => url;
    }
    
    protected void Init()
    {
        InstanceID = EasyObject.instanceID++;
    }
}