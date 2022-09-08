using System;

namespace BlockEditor.Models
{

    public class BlockLimitException : Exception
    {
        private const string BaseMsg = "Maximum block limit of 50k was hit!";

        public BlockLimitException() : base (BaseMsg) { }
        public BlockLimitException(string msg) : base((msg ?? string.Empty) + BaseMsg) { }

    }

    public class OverwriteException : Exception
    {
        private const string BaseMsg = "Enable 'Overwrite' option to overwrite blocks.";

        public OverwriteException() : base(BaseMsg) { }

    }

}
