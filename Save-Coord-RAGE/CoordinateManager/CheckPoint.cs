using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace Save_Coord_RAGE.CoordinateManager
{
    internal class CheckPoint : IDeletable, ISpatial
    {
        public Vector3 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector3 NextPosition { get; set; }
        public float Radius { get; private set; }
        public float Height { get; private set; }
        public Color Color { get; set; }


        public void Delete()
        {
            throw new NotImplementedException();
        }

        public float DistanceTo(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo2D(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo2D(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public float TravelDistanceTo(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float TravelDistanceTo(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }
    }
}
