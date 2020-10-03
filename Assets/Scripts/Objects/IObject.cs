public interface IObject
{
    ObjectType ObjectsType { get; }
}

public enum ObjectType
{
    Tree, Building
}