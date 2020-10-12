/// <summary>Only to determine if tapped object is a tree or a building</summary>
public interface IObject
{
    ObjectType ObjectsType { get; }
}

public enum ObjectType
{
    Tree, Building
}