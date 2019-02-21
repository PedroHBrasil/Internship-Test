using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains the parameters of a point of the aerodynamic map of a car:
     *  - WindRelativeSpeed: double which represents the wind speed relative to the car [km/h];
     *  - CarHeight: double which represents the car height from a reference point of the chassis to the ground [mm]
     *  - DragCoefficient: double which represents the drag coefficient for the given speed and height; and
     *  - LiftCoefficient: double which represents the lift coefficient for the given speed and height.
     */
    public class AerodynamicMapPoint
    {
        // Properties ---------------------------------------------------------------------------------------------------
        public double WindRelativeSpeed { get; set; }
        public double CarHeight { get; set; }
        public double DragCoefficient { get; set; }
        public double LiftCoefficient { get; set; }

        // Constructors -------------------------------------------------------------------------------------------------
        public AerodynamicMapPoint()
        {
            WindRelativeSpeed = 0; // km/h
            CarHeight = 0; // mm
            DragCoefficient = 0;
            LiftCoefficient = 0;
        }

        public AerodynamicMapPoint(double windRelativeSpeed, double carHeight, double dragCoefficient, double liftCoefficient)
        {
            WindRelativeSpeed = windRelativeSpeed; // km/h
            CarHeight = carHeight; // mm
            DragCoefficient = dragCoefficient;
            LiftCoefficient = liftCoefficient;
        }
    }
}
