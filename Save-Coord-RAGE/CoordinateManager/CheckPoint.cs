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
		private Vector3 _position;
        public Vector3 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				ReCreate();
			}
		}
		private Vector3 _nextpos;
        public Vector3 NextPosition
        {
            get { return _nextpos; }
			set { _nextpos = value; ReCreate(); }
        }
		private float NearHeight = float.NaN;
		private float _radius;
        public float Radius
        {
			get { return _radius; }
			set 
			{
				_radius = value;
				if (NearHeight != float.NaN) SetHeight(NearHeight, Height, value);
				else SetHeight(Height, Height, value);
			}
        }
		private float _height;
        public float Height
        {
			get { return _height; }
			set
            {
				_height = value;
				if (NearHeight != float.NaN) SetHeight(NearHeight, value, Radius);
				else SetHeight(value, value, Radius);
            }
        }
		private Color _color;
        public Color Color
        {
            get { return _color; }
			set { _color = value; SetColor(value); }
        }
		public CheckPointType Type { get; private set; }
		public int Handle { get; private set; }
		public bool IsValid { get; private set; } = false;

		public CheckPoint(Vector3 pos, Color color, float radius, float height, CheckPointType type)
        {
			Position = pos;
			NextPosition = pos;
			Color = color;
			Radius = radius;
			Height = height;
			Type = type;
			CreateCheckpoint();
        }
		public CheckPoint(Vector3 position, Vector3 nextPosition, Color color, float radius, float height, CheckPointType type)
        {
			Position = position;
			NextPosition = nextPosition;
			Color = color;
			Radius = radius;
			Height = height;
			Type = type;
			CreateCheckpoint();
        }

		private void ReCreate()
        {
			Delete();
			CreateCheckpoint();
        }
		private void CreateCheckpoint()
        {
			Vector3 pos = Position;
			float? ze = World.GetGroundZ(pos, true, false);
			pos.Z = ze.Value;
			try
            {
				int _handle = NativeFunction.Natives.CREATE_CHECKPOINT<int>(Type, pos.X, pos.Y, pos.Z, NextPosition.X, NextPosition.Y, NextPosition.Z, Radius, Color.R, Color.G, Color.B, Color.A, 0);
				Handle = _handle;
				IsValid = true;
				SetHeight(Height, Height, Radius);
			} catch (Exception e)
            {
				$"There's an error while trying to create checkpoint {e.Message}".ToLog();
				e.ToString().ToLog();
				IsValid = false;
            }
        }
		public void SetColor(Color color)
        {
			Color = color;
			if (IsValid)
				NativeFunction.Natives.SET_CHECKPOINT_RGBA(color.R, color.G, color.B, color.A);
		}
		public void SetHeight(float nearHeight, float farHeight, float radius)
        {
			NearHeight = nearHeight;
			Height = farHeight;
			Radius = radius;
			if (IsValid)
				NativeFunction.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(Handle, nearHeight, farHeight, radius);
		}
        public void Delete()
        {
			if (IsValid)
				NativeFunction.Natives.DELETE_CHECKPOINT(Handle);

			IsValid = false;
		}

        public float DistanceTo(Vector3 position)
        {
			return Position.DistanceTo(position);
        }

        public float DistanceTo(ISpatial spatialObject)
        {
			return Position.DistanceTo(spatialObject);
        }

        public float DistanceTo2D(Vector3 position)
        {
			return Position.DistanceTo2D(position);
        }

        public float DistanceTo2D(ISpatial spatialObject)
        {
			return Position.DistanceTo2D(spatialObject);
        }

        public float TravelDistanceTo(Vector3 position)
        {
			return Position.TravelDistanceTo(position);
        }

        public float TravelDistanceTo(ISpatial spatialObject)
        {
			return Position.TravelDistanceTo(spatialObject);
        }
		public override bool Equals(object obj)
		{
			if (obj is CheckPoint checkpoint)
			{
				return Handle == checkpoint.Handle;
			}

			return false;
		}
		public static bool operator ==(CheckPoint left, CheckPoint right)
        {
			return left is null ? right is null : left.Equals(right);
        }
		public static bool operator !=(CheckPoint left, CheckPoint right)
		{
			return !(left == right);
		}
		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}
        public override string ToString()
        {
			return $"Checkpoint ==> Handle: {Handle}, Type: {Type} IsValid: {IsValid}";
        }
        public enum CheckPointType
		{
			CylinderSingleArrow,
			CylinderDoubleArrow,
			CylinderTripleArrow,
			CylinderCycleArrow,
			CylinderCheckerboard,
			CylinderSingleArrow2,
			CylinderDoubleArrow2,
			CylinderTripleArrow2,
			CylinderCycleArrow2,
			CylinderCheckerboard2,
			RingSingleArrow,
			RingDoubleArrow,
			RingTripleArrow,
			RingCycleArrow,
			RingCheckerboard,
			SingleArrow,
			DoubleArrow,
			TripleArrow,
			CycleArrow,
			Checkerboard,
			CylinderSingleArrow3,
			CylinderDoubleArrow3,
			CylinderTripleArrow3,
			CylinderCycleArrow3,
			CylinderCheckerboard3,
			CylinderSingleArrow4,
			CylinderDoubleArrow4,
			CylinderTripleArrow4,
			CylinderCycleArrow4,
			CylinderCheckerboard4,
			CylinderSingleArrow5,
			CylinderDoubleArrow5,
			CylinderTripleArrow5,
			CylinderCycleArrow5,
			CylinderCheckerboard5,
			RingPlaneUp,
			RingPlaneLeft,
			RingPlaneRight,
			RingPlaneDown,
			Empty,
			Ring,
			Empty2,
			Cyclinder = 45,
			Cyclinder2,
			Cyclinder3,
		}
	}
}
