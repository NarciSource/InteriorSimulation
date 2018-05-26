using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Table : InteriorObject
    {
        public Table(Point point) : base(point)
        {
            setImg("table.PNG");
        }
    }
}
