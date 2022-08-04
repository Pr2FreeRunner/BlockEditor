namespace BlockEditor.Models
{
    public class DeleteBlockOperation : IUserOperation
    {
        private readonly AddBlockOperation _add;


        public DeleteBlockOperation(Map map, int blockID, MyPoint p)
        {
            _add = new AddBlockOperation(map, blockID, p);
        }

        public bool Execute()
        {
            return _add.Undo();
        }

        public bool Undo()
        {
            return _add.Execute();
        }
    }
}
