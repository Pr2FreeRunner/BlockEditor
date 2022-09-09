using System;
using System.Collections.Generic;

namespace BlockEditor.Models
{
    public partial class Hat
    {

        public enum Hats
        {
            Exp = 2,
            Kong = 3,
            Propeller = 4,
            Cowboy = 5,
            Crown = 6,
            Santa = 7,
            Party = 8,
            Top = 9,
            Jump_Start = 10,
            Moon = 11,
            Thief = 12,
            Jigg = 13,
            Artifact = 14,
            Jellyfish = 15,
            Cheese = 16
        }

        public int ID { get; set; }

        public string Name { get; set; }



        public Hat(int id)
        {
            ID = id;
            Name = GetName(id);
        }

        public Hat(string name)
        {
            ID = GetID(name);
            Name = GetName(ID);
        }



        private string GetName(int id)
        {
            if (id < (int)Hats.Exp || id > (int)Hats.Cheese)
                return string.Empty;

            return ((Hats)id).ToString().Replace('_', ' ');
        }

        public static IEnumerable<Hats> GetAllHats()
        {
            foreach (Hats h in Enum.GetValues(typeof(Hats)))
                yield return h;
        }

        private int GetID(string hatName)
        {

            foreach(var hat in GetAllHats()) 
            {
                var id   = (int)hat;
                var name = GetName(id);

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                if (hatName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return id;
            }

            return 0;
        }


    }
}