using System;
using System.Windows;

namespace selfInteriorSimulation.Objects
{
    class Table : InteriorObject
    {
        private Table(Point point) : base(point)
        {
            setImg("table.PNG");
        }
    }
}
