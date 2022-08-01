namespace BlockEditor.Models.UserOperations
{
    public interface IUserOperation
    {
        void Execute();
        void Undo();
    }
}
