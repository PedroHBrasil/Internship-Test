using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTest.Vehicle.OneWheel
{
    /*
     * Contains a car parameters:
     *  - CarID: string which identifies the car;
     *  - SetupID: string which identifies the car's setup;
     *  - Inertia: class which contains the inertia parameters of the car/setup;
     *  - Tire: class which contains the tire parameters of the car/setup;
     *  - Engine: class which contains the engine parameters of the car/setup;
     *  - Transmission: class which contains the transmission parameters of the car/setup;
     *  - Aerodynamic: class which contains the aerodynamic parameters of the car/setup;
     *  - Suspension: class which contains the suspension parameters of the car/setup;
     *  - Brakes: class which contains the brakes parameters of the car/setup;
     */
    [Serializable]
    class Car
    {
        // Properties -----------------------------------------------------------------------------
        public string CarID { get; set; }
        public string SetupID { get; set; }
        public Inertia Inertia { get; set; }
        public Tire Tire { get; set; }
        public Engine Engine { get; set; }
        public Transmission Transmission { get; set; }
        public Aerodynamics Aerodynamics { get; set; }
        public Aerodynamics DRS { get; set; }
        public Suspension Suspension { get; set; }
        public Brakes Brakes { get; set; }

        public double EquivalentHeaveStiffness { get; set; }
        public double LowestSpeed { get; set; }
        public double HighestSpeed { get; set; }

        public List<double> WheelRPMCurve { get; set; }
        public List<double> WheelTorqueCurve { get; set; }
        public List<double> WheelGearCurve { get; set; }
        public List<double> WheelBrakingTorqueCurve { get; set; }
        public List<double> WheelSpecFuelConsCurve { get; set; }

        // Constructors ---------------------------------------------------------------------------
        public Car()
        {
            CarID = "Default";
            SetupID = "Default";
            Inertia = new Inertia();
            Tire = new Tire();
            Engine = new Engine();
            Transmission = new Transmission();
            Aerodynamics = new Aerodynamics();
            DRS = new Aerodynamics();
            Suspension = new Suspension();
            Brakes = new Brakes();
        }

        public Car(string carID, string setupID, Inertia inertia, Tire tire,
            Engine engine, Transmission transmission, Aerodynamics aerodynamics,
            Aerodynamics drs, Suspension suspension, Brakes brakes)
        {
            CarID = carID;
            SetupID = setupID;
            Inertia = inertia;
            Tire = tire;
            Engine = engine;
            Transmission = transmission;
            Aerodynamics = aerodynamics;
            DRS = drs;
            Suspension = suspension;
            Brakes = brakes;

            EquivalentHeaveStiffness = (Suspension.HeaveStiffness * Tire.VerticalStiffness * 4) / (Suspension.HeaveStiffness + Tire.VerticalStiffness * 4);

            WheelRPMCurve = new List<double>();
            WheelTorqueCurve = new List<double>();
            WheelGearCurve = new List<double>();
            WheelBrakingTorqueCurve = new List<double>();
            WheelSpecFuelConsCurve = new List<double>();

            GetLinearAccelerationParameters();
            GetCarOperationSpeedRange();
        }

        // Methods -------------------------------------------------------------------------------
        public override string ToString()
        {
            return "C: " + CarID + " - S: " + SetupID;
        }
        public int GetGearNumberFromCarSpeed(double speed)
        {
            // Gets the aerodynamic coefficients
            AerodynamicMapPoint interpolatedAerodynamicMapPoint = GetAerodynamicCoefficients(speed);
            // Lift force
            double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed / 3.6, 2) / 2;
            // Tire resultant Fz
            double tireFz = (liftForce + Inertia.TotalMass * Inertia.Gravity) / 4;
            // Calculates the wheel radius [mm]
            double wheelRadius = Tire.TireModel.RO * 1000 - tireFz / Tire.VerticalStiffness;
            // Wheel rotational speed
            double wheelCenterAngularSpeed = speed * 60 / (3.6 * 2 * Math.PI * wheelRadius / 1000);
            // Gear interpolation object
            alglib.spline1dbuildlinear(WheelRPMCurve.ToArray(), WheelGearCurve.ToArray(), out alglib.spline1dinterpolant wheelGearCurveInterp);
            // Current gear number
            return (int)Math.Ceiling(alglib.spline1dcalc(wheelGearCurveInterp, wheelCenterAngularSpeed));
        }
        public void GetLinearAccelerationParameters()
        {
            // Splits the engine's curves points in lists
            List<double> engineRPMs = new List<double>();
            List<double> engineTorques = new List<double>();
            List<double> engineBrakingTorques = new List<double>();
            List<double> engineSpecFuelCons = new List<double>();
            foreach (EngineCurvesPoint engineCurvesPoint in Engine.EngineCurves)
            {
                engineRPMs.Add(engineCurvesPoint.RPM);
                engineTorques.Add(engineCurvesPoint.Torque * Engine.MaxThrottle / 100);
                engineBrakingTorques.Add(engineCurvesPoint.BrakingTorque);
                engineSpecFuelCons.Add(engineCurvesPoint.SpecFuelCons);
            }

            // Engine curves adjust to start at zero RPM
            if (engineRPMs[0] != 0)
            {
                // Temporary lists
                List<double> tempEngineRPMs = new List<double>(engineRPMs);
                List<double> tempEngineTorques = new List<double>(engineTorques);
                List<double> tempEngineBrakingTorques = new List<double>(engineBrakingTorques);
                List<double> tempEngineSpecFuelCons = new List<double>(engineSpecFuelCons);
                // Clears the lists
                engineRPMs.Clear();
                engineTorques.Clear();
                engineBrakingTorques.Clear();
                engineSpecFuelCons.Clear();
                // Lists allocation of first element
                engineRPMs.Add(0);
                engineTorques.Add(tempEngineTorques[0]);
                engineBrakingTorques.Add(tempEngineBrakingTorques[0]);
                engineSpecFuelCons.Add(tempEngineSpecFuelCons[0]);
                // List reassembly
                for (int i = 0; i < tempEngineRPMs.Count; i++)
                {
                    engineRPMs.Add(tempEngineRPMs[i]);
                    engineTorques.Add(tempEngineTorques[i]);
                    engineBrakingTorques.Add(tempEngineBrakingTorques[i]);
                    engineSpecFuelCons.Add(tempEngineSpecFuelCons[i]);
                }
            }
            // Engine curves interpolation objects initialization
            alglib.spline1dbuildlinear(engineRPMs.ToArray(), engineTorques.ToArray(), out alglib.spline1dinterpolant engineTorqueInterp);
            alglib.spline1dbuildlinear(engineRPMs.ToArray(), engineBrakingTorques.ToArray(), out alglib.spline1dinterpolant engineBrakingTorqueInterp);
            alglib.spline1dbuildlinear(engineRPMs.ToArray(), engineSpecFuelCons.ToArray(), out alglib.spline1dinterpolant engineSpecFuelConsInterp);
            // Initialization of interpolated engine curves lists
            List<double> engineInterpolatedRPMs = new List<double>();
            List<double> engineInterpolatedTorques = new List<double>();
            List<double> engineInterpolatedBrakingTorques = new List<double>();
            List<double> engineInterpolatedSpecFuelCons = new List<double>();
            // Population of engine curves per RPM
            for (int rpm = 0; rpm <= engineRPMs.Max(); rpm++)
            {
                engineInterpolatedRPMs.Add(rpm);
                engineInterpolatedTorques.Add(alglib.spline1dcalc(engineTorqueInterp, rpm));
                engineInterpolatedBrakingTorques.Add(alglib.spline1dcalc(engineBrakingTorqueInterp, rpm));
                engineInterpolatedSpecFuelCons.Add(alglib.spline1dcalc(engineSpecFuelConsInterp, rpm));
                /*
                engineInterpolatedTorques.Add(engineTorqueInterp.Interpolate(rpm));
                engineInterpolatedBrakingTorques.Add(engineBrakingTorqueInterp.Interpolate(rpm));
                engineInterpolatedSpecFuelCons.Add(engineSpecFuelConsInterp.Interpolate(rpm));
                */
            }

            // Resultant transmission gear ratios at the wheel
            List<double> resultantGearRatiosAtWheel = new List<double>();
            foreach (GearRatio gearRatio in Transmission.GearRatios)
            {
                double resultantGearRatio = gearRatio.Ratio * Transmission.PrimaryRatio * Transmission.FinalRatio;
                resultantGearRatiosAtWheel.Add(resultantGearRatio);
            }

            // Initialization of resultant wheel-engine curves per gear lists
            List<List<double>> resultantWheelEngineRPMsPerGear = new List<List<double>>();
            List<List<double>> resultantWheelEngineTorquesPerGear = new List<List<double>>();
            List<List<double>> resultantWheelEngineBrakingTorquesPerGear = new List<List<double>>();
            List<List<double>> resultantWheelEngineSpecFuelConsPerGear = new List<List<double>>();
            // Resultant engine curves at the wheel for each gear
            for (int iGear = 0; iGear < Transmission.GearRatios.Count; iGear++)
            {
                // Current resultant wheel-engine curves lists initialization
                List<double> resultantWheelEngineRPMs = new List<double>();
                List<double> resultantWheelEngineTorques = new List<double>();
                List<double> resultantWheelEngineBrakingTorques = new List<double>();
                List<double> resultantWheelEngineSpecFuelCons = new List<double>();
                // Resultant wheel-engine curves population
                for (int rpm = 0; rpm <= engineRPMs.Max(); rpm++)
                {
                    resultantWheelEngineRPMs.Add(rpm / resultantGearRatiosAtWheel[iGear]);
                    resultantWheelEngineTorques.Add(engineInterpolatedTorques[rpm] * resultantGearRatiosAtWheel[iGear]);
                    resultantWheelEngineBrakingTorques.Add(engineInterpolatedBrakingTorques[rpm] * resultantGearRatiosAtWheel[iGear]);
                    resultantWheelEngineSpecFuelCons.Add(engineInterpolatedSpecFuelCons[rpm] * resultantGearRatiosAtWheel[iGear]);
                }
                // Resultant wheel-engine curves per gear population
                resultantWheelEngineRPMsPerGear.Add(resultantWheelEngineRPMs);
                resultantWheelEngineTorquesPerGear.Add(resultantWheelEngineTorques);
                resultantWheelEngineBrakingTorquesPerGear.Add(resultantWheelEngineBrakingTorques);
                resultantWheelEngineSpecFuelConsPerGear.Add(resultantWheelEngineSpecFuelCons);
            }

            // Gear Shifting RPMs indexes lists initialization
            List<int> gearEngageRPMsIndexes = new List<int>() { 0 };
            List<int> gearShiftRPMsIndexes = new List<int>();
            // Finds the ideal gear shifting RPMs indexes
            for (int iGear = 0; iGear < Transmission.GearRatios.Count - 1; iGear++)
            {
                // Current RPM curves
                List<double> rpmCurveCurrentGear = new List<double>(resultantWheelEngineRPMsPerGear[iGear]);
                List<double> rpmCurveNextGear = new List<double>(resultantWheelEngineRPMsPerGear[iGear + 1]);
                // Current torque curves
                List<double> torqueCurveCurrentGear = new List<double>(resultantWheelEngineTorquesPerGear[iGear]);
                List<double> torqueCurveNextGear = new List<double>(resultantWheelEngineTorquesPerGear[iGear + 1]);

                // Gears torque comparisson "do while" loop
                int iRPM1 = engineInterpolatedRPMs.Count();
                int iRPM2 = engineInterpolatedRPMs.Count();
                double referenceTorqueVSRPMCurveDistance;
                double currentTorqueVSRPMCurveDistance = Math.Pow(10, 10);
                do
                {
                    // Current gear curve index update
                    iRPM1--;
                    // Minimum ditance between point of the current gear and the torque vs rpm curve from the next gear
                    double minDistance;
                    double currentMinDistance = Math.Pow(10, 10);
                    do
                    {
                        // Next gear curve index update
                        iRPM2--;
                        // Global minimum distance update
                        minDistance = currentMinDistance;
                        // Current minimum distance update
                        currentMinDistance = Math.Sqrt(
                            Math.Pow(rpmCurveCurrentGear[iRPM1] - rpmCurveNextGear[iRPM2], 2) +
                            Math.Pow(torqueCurveCurrentGear[iRPM1] - torqueCurveNextGear[iRPM2], 2));
                    } while (minDistance > currentMinDistance);
                    // Reference distance update
                    referenceTorqueVSRPMCurveDistance = currentTorqueVSRPMCurveDistance;
                    currentTorqueVSRPMCurveDistance = minDistance;
                } while (referenceTorqueVSRPMCurveDistance > currentTorqueVSRPMCurveDistance);
                // Gear shift rpm indexes list population
                gearEngageRPMsIndexes.Add(iRPM2);
                gearShiftRPMsIndexes.Add(iRPM1);
            }
            gearShiftRPMsIndexes.Add((int)engineInterpolatedRPMs.Max());
            // Resultant engine curves at wheel
            for (int iGear = 0; iGear < Transmission.GearRatios.Count; iGear++)
            {
                // Current resultant wheel-engine curves lists initialization
                List<double> resultantWheelEngineRPMs = new List<double>
                    (resultantWheelEngineRPMsPerGear[iGear].GetRange(gearEngageRPMsIndexes[iGear],
                    gearShiftRPMsIndexes[iGear] - gearEngageRPMsIndexes[iGear]));
                List<double> resultantWheelEngineTorques = new List<double>
                    (resultantWheelEngineTorquesPerGear[iGear].GetRange(gearEngageRPMsIndexes[iGear],
                    gearShiftRPMsIndexes[iGear] - gearEngageRPMsIndexes[iGear]));
                List<double> resultantWheelEngineBrakingTorques = new List<double>
                    (resultantWheelEngineBrakingTorquesPerGear[iGear].GetRange(gearEngageRPMsIndexes[iGear],
                    gearShiftRPMsIndexes[iGear] - gearEngageRPMsIndexes[iGear]));
                List<double> resultantWheelEngineSpecFuelCons = new List<double>
                    (resultantWheelEngineSpecFuelConsPerGear[iGear].GetRange(gearEngageRPMsIndexes[iGear],
                    gearShiftRPMsIndexes[iGear] - gearEngageRPMsIndexes[iGear]));
                WheelRPMCurve.AddRange(resultantWheelEngineRPMs);
                WheelTorqueCurve.AddRange(resultantWheelEngineTorques);
                WheelBrakingTorqueCurve.AddRange(resultantWheelEngineBrakingTorques);
                WheelSpecFuelConsCurve.AddRange(resultantWheelEngineSpecFuelCons);
                for (int iRPM = gearEngageRPMsIndexes[iGear]; iRPM < gearShiftRPMsIndexes[iGear]; iRPM++)
                {
                    WheelGearCurve.Add(iGear + 1);
                }
            }
        }
        public void GetCarOperationSpeedRange()
        {
            LowestSpeed = GetCarSpeedFromEngineSpeed(0, 1);
            HighestSpeed = GetCarSpeedFromEngineSpeed(Engine.EngineCurves.Count - 1, Transmission.GearRatios.Count);
        }

        public double GetCarSpeedFromEngineSpeed(int iRPM, int gearNumber)
        {
            // Optimization parameters
            double tol = 1e-6;
            double error = 2 * tol;
            // Optimization variables initialization
            double wheelRadius = Tire.TireModel.RO * 1000;
            double speed;
            int iter = 0;
            // Optimization "do while" loop
            do
            {
                iter++;
                // Speed for current wheel radius
                speed = (Engine.EngineCurves[iRPM].RPM / 
                    (Transmission.GearRatios[gearNumber - 1].Ratio * Transmission.PrimaryRatio * Transmission.FinalRatio))
                    / 60 * (2 * Math.PI * wheelRadius / 1000) * 3.6;
                // Gets the aerodynamic coefficients
                AerodynamicMapPoint interpolatedAerodynamicMapPoint = GetAerodynamicCoefficients(speed);
                // Updates the lift force
                double liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed / 3.6, 2) / 2;
                // Updates the tire vertical load
                double tireFz = (liftForce + Inertia.TotalMass * Inertia.Gravity) / 4;
                // Updates the wheel radius
                double oldWheelRadius = wheelRadius;
                wheelRadius = Tire.TireModel.RO * 1000 - tireFz / Tire.VerticalStiffness;
                // Updates the error (optimization criteria)
                error = Math.Abs(wheelRadius - oldWheelRadius);
            } while (error > tol);
            // Returns the found speed
            return speed;
        }

        public AerodynamicMapPoint GetAerodynamicCoefficients(double speed)
        {
            double tol = 1e-6;
            double error = 2 * tol;
            double carHeight = Suspension.CarHeight;
            double liftForce;
            AerodynamicMapPoint interpolatedAerodynamicMapPoint;
            do
            {
                // Aerodynamic map interpolation
                interpolatedAerodynamicMapPoint = Aerodynamics.InterpolateAerodynamicMap(speed, carHeight);
                // Calculates the lift force
                liftForce = -interpolatedAerodynamicMapPoint.LiftCoefficient * Aerodynamics.FrontalArea * Aerodynamics.AirDensity * Math.Pow(speed / 3.6, 2) / 2;
                // New car height
                double oldCarHeight = carHeight;
                carHeight = Suspension.CarHeight - liftForce / EquivalentHeaveStiffness;
                // Error update
                error = Math.Abs(carHeight - oldCarHeight);
            } while (error > tol);
            return interpolatedAerodynamicMapPoint;
        }
    }
}
