using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InternshipTest.UIClasses.ResultsAnalysis
{
    public class LapTimeSimulationTrackMapParameters
    {
        #region Properties
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        public LapTimeSimulationResultsAuxiliaryTypes.ResultTypes ResultType { get; set; }
        public string ResultTypeString { get; set; }
        #endregion
        #region Constructors
        public LapTimeSimulationTrackMapParameters() { }
        public LapTimeSimulationTrackMapParameters(string resultTypeString)
        {
            ResultTypeString = resultTypeString;
            ResultType = LapTimeSimulationResultsAuxiliaryTypes.GetResultTypeFromString(ResultTypeString);
        }
        #endregion
        #region Methods
        public SfSurfaceChart AddDataToTrackMap(Results.LapTimeSimulationResultsViewModel lapTimeSimulationResultsViewModel, double size)
        {
            RowSize = 2;
            ColumnSize = lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count();
            SfSurfaceChart trackMap = _InitializeTrackMap(size);
            trackMap = _AddDataToTrackMap(trackMap, lapTimeSimulationResultsViewModel);
            return trackMap;
        }
        private SfSurfaceChart _InitializeTrackMap(double size)
        {
            return new SfSurfaceChart()
            {
                Header = "Track Map",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                RowSize = this.RowSize,
                ColumnSize = this.ColumnSize,
                Height = size,
                Width = size,
                ColorBar = new ChartColorBar() { DockPosition = ChartDock.Right },
                Type = SurfaceType.Surface,
                EnableRotation = true,
                EnableZooming = true
            };
        }
        private SfSurfaceChart _AddDataToTrackMap(SfSurfaceChart trackMap, Results.LapTimeSimulationResultsViewModel lapTimeSimulationResultsViewModel)
        {
            for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
            {
                trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, 0, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
            }
            switch (ResultType)
            {
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragCoefficient:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].DragCoefficient, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Drag Coefficient",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftCoefficient:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].LiftCoefficient, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Lift Coefficient",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDownforceDistribution:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].DownforceDistribution, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Downforce Distribution [%]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicDragForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].DragForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Drag Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicLiftForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].LiftForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Lift Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicFrontLiftForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontLiftForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Front Lift Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.AerodynamicRearLiftForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearLiftForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Aero Rear Lift Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Distance:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].ElapsedDistance, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Covered Distance [m]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EngineAvailablePower:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].EngineAvailablePower, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Engine Available Power [hp]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePower:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].EnginePower, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Engine Power [hp]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.EnginePowerUsage:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].EnginePowerUsage, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Engine Power Usage [%]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FuelConsumption:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FuelConsumption, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Fuel Consumption [mL]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Gear:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].GearNumber, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Gear Number",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.InertiaEfficiency:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].InertiaEfficiency, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Inertia Efficiency [%]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralAcceleration:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].LateralAcceleration, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Lateral Acceleration [m/s²]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LateralForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].LateralForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Lateral Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalAcceleration:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].LongitudinalAcceleration, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Longitudinal Acceleration [m/s²]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].LongitudinalForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Longitudinal Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferSprung:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].SprungLongitudinalLoadTransfer, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Sprung Longitudinal Load Transfer [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferTotal:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].TotalLongitudinalLoadTransfer, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Total Longitudinal Load Transfer [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.LongitudinalLoadTransferUnsprung:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].UnsprungLongitudinalLoadTransfer, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Unsprung Longitudinal Load Transfer [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesAvailablePower:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontBrakesAvailablePower, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Brakes Available Power [hp]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPower:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontBrakesPower, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Brakes Power [hp]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontBrakesPowerUsage:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontBrakesPowerUsage, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Brakes Power Usage [%]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontRideHeight:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontRideHeight, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Ride Height [mm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontSuspensionDeflection:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontSuspensionDeflection, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Suspension Deflection [mm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelAngularSpeed:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontWheelAngularSpeed, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Wheel Angular Speed [rpm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLoad:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontWheelLoad, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Wheel Load [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelLongitudinalForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontWheelLongitudinalForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Wheel Longitudinal Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelRadius:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontWheelRadius, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Wheel Radius [mm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.FrontWheelTorque:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].FrontWheelTorque, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Front Wheel Torque [Nm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesAvailablePower:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearBrakesAvailablePower, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Brakes Available Power [hp]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPower:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearBrakesPower, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Brakes Power [hp]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearBrakesPowerUsage:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearBrakesPowerUsage, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Brakes Power Usage [%]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearRideHeight:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearRideHeight, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Ride Height [mm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearSuspensionDeflection:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearSuspensionDeflection, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Suspension Deflection [mm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelAngularSpeed:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearWheelAngularSpeed, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Wheel Angular Speed [rpm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLoad:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearWheelLoad, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Wheel Load [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelLongitudinalForce:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearWheelLongitudinalForce, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Wheel Longitudinal Force [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelRadius:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearWheelRadius, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Wheel Radius [mm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.RearWheelTorque:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].RearWheelTorque, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Rear Wheel Torque [Nm]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Speed:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].Speed, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Speed [km/h]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.Time:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].ElapsedTime, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Elapsed Time [s]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                case LapTimeSimulationResultsAuxiliaryTypes.ResultTypes.VerticalLoadTotal:
                    for (int i = 0; i < lapTimeSimulationResultsViewModel.ResultsDisplayCollection.Count(); i++)
                    {
                        trackMap.Data.AddPoints(lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesX, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].TotalVerticalLoad, lapTimeSimulationResultsViewModel.ResultsDisplayCollection[i].CoordinatesY);
                    }
                    trackMap.YAxis = new SurfaceAxis()
                    {
                        Header = "Total Vertical Load [N]",
                        Minimum = trackMap.Data.YValues.Min(),
                        Maximum = trackMap.Data.YValues.Max(),
                        LabelFormat = "0.00",
                        FontSize = 15
                    };
                    break;
                default:
                    break;
            }
            trackMap.XAxis = new SurfaceAxis()
            {
                Header = "X Coordinates [m]",
                Minimum = trackMap.Data.XValues.Min(),
                Maximum = trackMap.Data.XValues.Max(),
                LabelFormat = "0",
                FontSize = 15
            };
            trackMap.ZAxis = new SurfaceAxis()
            {
                Header = "Y Coordinates [m]",
                Minimum = trackMap.Data.ZValues.Min(),
                Maximum = trackMap.Data.ZValues.Max(),
                LabelFormat = "0",
                FontSize = 15
            };
            return trackMap;
        }
        #endregion
    }
}
