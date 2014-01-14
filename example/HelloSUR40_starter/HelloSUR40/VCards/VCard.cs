using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurLaRoute.VCards
{
    class VCard
    {
        private int id;
        private String name;
        private String room;

        public VCard(int id, String name, String room)
        {
            this.id = id;
            this.name = name;
            this.room = room;
        }

        public String Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        public String Room
        {
            set { this.room = value; }
            get { return this.room; }
        }
    }
}
