namespace BlockEditor.Models
{
    public interface IUserOperation
    {
        void Execute();
        void Undo();
    }
}
