using System;

namespace BlockEditor.Models
{
    public interface IUserOperation
    {

        bool Execute(bool redo = false);
        bool Undo();
        Guid GetID();

    }
}
