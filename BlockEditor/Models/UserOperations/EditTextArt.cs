using LevelModel.Models.Components.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using static LevelModel.Models.Level;

namespace BlockEditor.Models
{
    public class EditTextArt : BaseOperation, IUserOperation
    {

        private List<TextArt> _original;

        private readonly Map _map;
        private readonly ArtType _artType;
        private readonly Action<List<TextArt>> _edit;
        private bool _invalid;

        public EditTextArt(Map map, ArtType type, Action<List<TextArt>> edit)
        {
            _map = map;
            _edit = edit;
            _artType = type;
            _invalid = false;

            switch (type)
            {
                case ArtType.TextArt00: _original = map.Level.TextArt00; break;
                case ArtType.TextArt0: _original  = map.Level.TextArt0; break;
                case ArtType.TextArt1: _original  = map.Level.TextArt1; break;
                case ArtType.TextArt2: _original  = map.Level.TextArt2; break;
                case ArtType.TextArt3: _original  = map.Level.TextArt3; break;

                default: _invalid = true; break;
            }
        }

        public bool Execute(bool redo = false)
        {
            if (_map == null || !_original.Any() || _edit == null || _invalid)
                return false;

            var temp = new List<TextArt>(_original);
            _edit(_original);
            _original = temp;

            _map.LoadArt();
            return true;
        }

        public bool Undo()
        {
            if (_map == null || _invalid)
                return false;

            switch (_artType)
            {
                case ArtType.TextArt00: _map.Level.TextArt00 = _original; break;
                case ArtType.TextArt0:  _map.Level.TextArt0 = _original; break;
                case ArtType.TextArt1:  _map.Level.TextArt1 = _original; break;
                case ArtType.TextArt2:  _map.Level.TextArt2 = _original; break;
                case ArtType.TextArt3:  _map.Level.TextArt3 = _original; break;

                default: return false;
            }

            _map.LoadArt();
            return true;
        }
    }
}
