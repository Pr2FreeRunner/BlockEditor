namespace BlockEditor.Models
{
    public class DeleteBlockOperation : IUserOperation
    {
        private readonly AddBlockOperation _add;


        public DeleteBlockOperation(Map map, SimpleBlock b)
        {
            _add = new AddBlockOperation(map, b);
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
