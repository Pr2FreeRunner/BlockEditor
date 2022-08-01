namespace BlockEditor.Models.UserOperations
{
    public class DeleteBlockOperation : IUserOperation
    {
        private readonly AddBlockOperation _add;


        public DeleteBlockOperation(Map map, int blockID, MyPoint p)
        {
            _add = new AddBlockOperation(map, blockID, p);
        }

        public void Execute()
        {
            _add?.Undo();
        }

        public void Undo()
        {
            _add?.Execute();
        }
    }
}
