using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Table : InteriorObject
    {
        public IsType isType = IsType.Table;
        public Table(Point point) : base(point)
        {
            setImg("table.PNG");
        }
    }
}
