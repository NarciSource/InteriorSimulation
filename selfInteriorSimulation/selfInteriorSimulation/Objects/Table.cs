using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Table : InteriorObject
    {
        public Table(Point point) : base(point)
        {
            isType = IsType.Table;
            setImg("desk_dan.PNG");
        }
    }
}
