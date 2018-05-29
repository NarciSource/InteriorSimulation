using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Table : InteriorObject
    {
        public Table() : base()
        {
            isType = IsType.Table;
            setImg("desk_dan.PNG");
            
        }
    }
}
