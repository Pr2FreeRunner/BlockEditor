using System;

namespace BlockEditor.Models
{
    public abstract class BaseOperation
    {

        private Guid _id;

        public BaseOperation()
        {
            _id = Guid.NewGuid();
        }
        public Guid GetID()
        {
            return _id;
        }
    }
}
