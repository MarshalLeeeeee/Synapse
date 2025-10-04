
public class PlayerEntityCommon : Entity
{
    /// <summary>
    /// dynamic components: component name -> component instance
    /// </summary>
    protected StringKeyDictionaryNode components = new StringKeyDictionaryNode();

    protected PlayerEntityCommon(string eid = "") : base(eid) { }

    public override string ToString()
    {
        return $"PlayerEntity({components})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(entityId);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    #endregion
}
