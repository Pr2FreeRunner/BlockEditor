using BlockEditor.Helpers;
using LevelModel.Models.Components;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.Design.AxImporter;

namespace BlockEditor.Models
{
    public class ConnectTeleports
    {

        private List<SimpleBlock> _blocks { get; }
        public string Options { get; set; }
        public int Count => _blocks?.Count ?? 0;



        public ConnectTeleports()
        {
            Options = string.Empty;
            _blocks = new List<SimpleBlock>();
        }

        public void Start()
        {
            _blocks.Clear();
            Options = string.Empty;

            if (MySettings.FirstConnectTeleports)
            {
                MessageUtil.ShowInfo("Click on all the teleport blocks you wish to connect.");
                MySettings.FirstConnectTeleports = false;
            }
        }

        public void Add(SimpleBlock b)
        {
            if(b.IsEmpty())
                return;

            if(b.ID != Block.TELEPORT)
                return;


            if(_blocks.Where(x => x.Position.Value == b.Position.Value).Count() > 0)
                return;

            _blocks.Add(b);
        }

        public bool IsSelected(MyPoint? p)
        {
            if(p == null)
                return false;

            return _blocks.RemoveEmpty().Any(b => b.Position == p.Value);
        }

        public List<SimpleBlock> GetAddedBlocks()
        {
            var result = new List<SimpleBlock>();

            if(_blocks.Count == 0)
                return result;

            foreach (var b in _blocks)
            {
                if(b.IsEmpty())
                    continue;

                result.Add(new SimpleBlock(b.ID, b.Position.Value, Options ?? string.Empty));
            }

            return result;
        }

        public void ClearSelectedBlocks()
        {
            _blocks?.Clear();
        }
    }
}
