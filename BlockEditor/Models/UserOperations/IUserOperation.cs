namespace BlockEditor.Models
{
    public interface IUserOperation
    {
        bool Execute();
        bool Undo();
    }
}
