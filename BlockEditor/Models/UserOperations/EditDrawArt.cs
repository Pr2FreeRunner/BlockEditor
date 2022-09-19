using LevelModel.Models.Components.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using static LevelModel.Models.Level;

namespace BlockEditor.Models
{
    public class EditDrawArt : BaseOperation, IUserOperation
    {

        private List<DrawArt> _original;
        private bool _invalid;

        private readonly Map _map;
        private readonly ArtType _artType;
        private readonly Action<List<DrawArt>> _edit;

        public EditDrawArt(Map map, ArtType type, Action<List<DrawArt>> edit)
        {
            _map = map;
            _edit = edit;
            _artType = type;
            _invalid = false;

            switch (type)
            {
                case ArtType.DrawArt00: _original = map.Level.DrawArt00; break;
                case ArtType.DrawArt0:  _original = map.Level.DrawArt0;  break;
                case ArtType.DrawArt1:  _original = map.Level.DrawArt1;  break;
                case ArtType.DrawArt2:  _original = map.Level.DrawArt2;  break;
                case ArtType.DrawArt3:  _original = map.Level.DrawArt3;  break;

                default: _invalid = true; break;
            }
        }

        public bool Execute(bool redo = false)
        {
            if (_map == null || !_original.Any() || _edit == null || _invalid)
                return false;

            var temp = new List<DrawArt>(_original);
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
                case ArtType.DrawArt00: _map.Level.DrawArt00 = _original; break;
                case ArtType.DrawArt0:  _map.Level.DrawArt0  = _original; break;
                case ArtType.DrawArt1:  _map.Level.DrawArt1  = _original; break;
                case ArtType.DrawArt2:  _map.Level.DrawArt2  = _original; break;
                case ArtType.DrawArt3:  _map.Level.DrawArt3  = _original; break;

                default: return false;
            }

            _map.LoadArt();
            return true;
        }
    }
}
