
public class Component : ComponentCommon
{
    public Component() : base() { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (Component)Activator.CreateInstance(typeof(Component), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy Component.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static Component Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (Component)Activator.CreateInstance(typeof(Component), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize Component.", ex);
        }
    }

    #endregion
}

[SyncNode(NodeTypeConst.TypeComponents)]
public class Components : ComponentsCommon
{
    public override int nodeType => NodeTypeConst.TypeComponents;

    public Components(
        params KeyValuePair<string, Component>[] kvps
    ) : base(kvps) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (Components)Activator.CreateInstance(typeof(Components), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy Components.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static Components Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (Components)Activator.CreateInstance(typeof(Components), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryNode.", ex);
        }
    }

    #endregion
}
