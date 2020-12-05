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
    public class CheckPoint
	{ 
		public enum CheckPointType : int
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
			Cylinder = 45,
			Cylinder2,
			Cylinder3,
		}
	}
}
