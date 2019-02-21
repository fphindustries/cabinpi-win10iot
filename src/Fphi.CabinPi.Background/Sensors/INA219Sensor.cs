using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Fphi.CabinPi.Background.Sensors
{
    class CurrentOverflowException : Exception { }

    class INA219Sensor : ISensor
    {

        private const ushort ResetConfiguration = 32768; //bit 15
        private enum Registers
        {
            Configuration = 0,
            ShuntVoltage = 1,
            BusVoltage = 2,
            Power = 3,
            Current = 4,
            Calibration = 5
        }

        public enum VoltageRange
        {
            Range16V = 0,
            Range32V = 8192 //bit 13
        }

        /// <summary>
        /// Sets Programmable Gain Amplifier (PGA) gain and range. Note that the PGA defaults to ÷8 (320mV range).
        /// Configured via bits 12 & 11 in the Configuration Register.
        /// </summary>
        public enum Gain
        {
            /// <summary>
            /// Gain 1, Range +/- 40mV
            /// </summary>
            Gain40mV = 0,     // 0 0
            /// <summary>
            /// Gain /2, Range +/- 80mV
            /// </summary>
            Gain80mV = 2048,  // 0 1
            /// <summary>
            /// Gain /4, Range +/- 160mV
            /// </summary>
            Gain160mV = 4096, // 1 0
            /// <summary>
            /// Gain /8, Range +/- 320 mV (Default)
            /// </summary>
            Gain320mV = 6144, // 1 1
            AutoGain = -1
        }

        /// <summary>
        /// These bits adjust the Bus ADC resolution (9-, 10-, 11-, or 12-bit) or set the number of samples used when
        /// averaging results for the Bus Voltage Register(02h)
        /// </summary>
        public enum BusADCResolution
        {
            /// <summary>
            /// 9-bit conversion time  84us
            /// </summary>
            ADC9Bit = 0,    //  0   x   0   0
            /// <summary>
            /// 10-bit conversion time 148us
            /// </summary>
            ADC10Bit = 128,   //  0   x   0   1
            /// <summary>
            /// 11-bit conversion time 2766us
            /// </summary>
            ADC11Bit = 256,   //  0   x   1   0
            /// <summary>
            /// 12-bit conversion time 532us
            /// </summary>
            ADC12Bit = 384,   //  0   x   1   1
            /// <summary>
            /// 2 samples at 12-bit, conversion time 1.06ms
            /// </summary>
            ADC2Samp = 1152,   //  1   0   0   1
            /// <summary>
            /// 4 samples at 12-bit, conversion time 2.13ms
            /// </summary>
            ADC4Samp = 1280,  //  1   0   1   0
            /// <summary>
            /// 8 samples at 12-bit, conversion time 4.26ms
            /// </summary>
            ADC8Samp = 1408,  //  1   0   1   1
            /// <summary>
            /// 16 samples at 12-bit,conversion time 8.51ms
            /// </summary>
            ADC16Samp = 1536, //  1   1   0   0
            /// <summary>
            /// 32 samples at 12-bit, conversion time 17.02ms
            /// </summary>
            ADC32Samp = 1664, //  1   1   0   1
            /// <summary>
            /// 64 samples at 12-bit, conversion time 34.05ms
            /// </summary>
            ADC64Samp = 1792, //  1   1   1   0
            /// <summary>
            /// 128 samples at 12-bit, conversion time 68.10ms
            /// </summary>
            ADC128Samp = 1920 //  1   1   1   1
        }

        /// <summary>
        /// These bits adjust the Shunt ADC resolution (9-, 10-, 11-, or 12-bit) or set the number of samples used when
        /// averaging results for the Shunt Voltage Register(01h).
        /// </summary>
        public enum ShuntADCResolution
        {
            /// <summary>
            /// 9-bit conversion time  84us
            /// </summary>
            ADC9Bit = 0,    //  0   x   0   0
            /// <summary>
            /// 10-bit conversion time 148us
            /// </summary>
            ADC10Bit = 8,   //  0   x   0   1
            /// <summary>
            /// 11-bit conversion time 2766us
            /// </summary>
            ADC11Bit = 16,   //  0   x   1   0
            /// <summary>
            /// 12-bit conversion time 532us
            /// </summary>
            ADC12Bit = 24,   //  0   x   1   1
            /// <summary>
            /// 2 samples at 12-bit, conversion time 1.06ms
            /// </summary>
            ADC2Samp = 72,   //  1   0   0   1
            /// <summary>
            /// 4 samples at 12-bit, conversion time 2.13ms
            /// </summary>
            ADC4Samp = 80,  //  1   0   1   0
            /// <summary>
            /// 8 samples at 12-bit, conversion time 4.26ms
            /// </summary>
            ADC8Samp = 88,  //  1   0   1   1
            /// <summary>
            /// 16 samples at 12-bit,conversion time 8.51ms
            /// </summary>
            ADC16Samp = 96, //  1   1   0   0
            /// <summary>
            /// 32 samples at 12-bit, conversion time 17.02ms
            /// </summary>
            ADC32Samp = 104, //  1   1   0   1
            /// <summary>
            /// 64 samples at 12-bit, conversion time 34.05ms
            /// </summary>
            ADC64Samp = 112, //  1   1   1   0
            /// <summary>
            /// 128 samples at 12-bit, conversion time 68.10ms
            /// </summary>
            ADC128Samp = 120 //  1   1   1   1
        }

        /// <summary>
        /// Selects continuous, triggered, or power-down mode of operation. 
        /// These bits default to continuous shunt and bus measurement mode.
        /// </summary>
        private enum OperatingMode
        {
            PowerDown = 0,
            ShuntVoltageTriggered = 1,
            BusVoltageTriggered = 2,
            ShuntAndBusTriggered = 3,
            ADCOff = 4,
            ShuntVoltageContinuous = 5,
            BusVoltageContinuous = 6,
            ShuntAndBusContinuous = 7
        }

        private enum ConfigurationRegisterBits
        {
            Reset = 15,
            BusVoltageRange = 13,
            PGAShuntVoltage1 = 12,
            PGAShuntVoltage0 = 11,
            BusADCResolution4 = 10,
            BusADCResolution3 = 9,
            BusADCResolution2 = 8,
            BusADCResolution1 = 7,
            ShuntADCResolution4 = 6,
            ShuntADCResolution3 = 5,
            ShuntADCResolution2 = 4,
            ShuntADCResolution1 = 3,
        }

        //private static double[] _gainVolts = new double[] { 0.04, 0.08, 0.16, 0.32 };
        private static Dictionary<Gain, double> _gainVolts = new Dictionary<Gain, double> {
            { Gain.Gain40mV, 0.04D },
            { Gain.Gain80mV, 0.08D },
            { Gain.Gain160mV, 0.16D },
            { Gain.Gain320mV, 0.32D }
        };

        //private static int[] _busRange = new int[] { 16, 32 };
        private static Dictionary<VoltageRange, double> _busRange = new Dictionary<VoltageRange, double>
        {
            {VoltageRange.Range16V, 16D },
            {VoltageRange.Range32V, 32D }
        };

        private const double BusMillivoltsLsb = 4.0D; //4mV
        private const double ShuntMillivoltsLsb = 0.01D;
        private const double CalibrationFactor = 0.04096D;
        private const double MaxCalibrationValue = 0xFFFE; //Max value supported (65534 decimal)

        /// <summary>
        /// In the spec (p17) the current LSB factor for the minimum LSB is
        /// # documented as 32767, but a larger value (100.1% of 32767) is used
        /// # to guarantee that current overflow can always be detected.
        /// </summary>
        private const double CurrentLsbFactor = 32800D;

        private const int DefaultAddress = 0x40;
        private readonly double _shuntOhms;
        private readonly double _maxExpectedAmps;
        private readonly int _address;
        private double _minDeviceCurrentLsb;
        private Gain _gain;
        private bool _autoGainEnabled;
        private double _currentLsb;

        private VoltageRange _voltageRange;

        private I2cDevice _device;
        private double _powerLsb;


        public string Name { get; set; }

        public async Task<IEnumerable<SensorReading>> GetReadingsAsync()
        {
            try
            {

                string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
                IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);


                var connectionSettings = new I2cConnectionSettings(_address); //Default address

                // If this next line crashes with an ArgumentOutOfRangeException,
                // then the problem is that no I2C devices were found.
                //
                // If the next line crashes with Access Denied, then the problem is
                // that access to the I2C device (HTU21D) is denied.
                //
                // The call to FromIdAsync will also crash if the settings are invalid.
                //
                // FromIdAsync produces null if there is a sharing violation on the device.
                // This will result in a NullReferenceException in Timer_Tick below.
                _device = await I2cDevice.FromIdAsync(devices[0].Id, connectionSettings);

                Reset();

                var configuration = ReadRegister(Registers.Configuration);
                var calibration = ReadRegister(Registers.Calibration);
                Configure();


                double shuntVoltage = ReadRegister(Registers.ShuntVoltage) * ShuntMillivoltsLsb;
                double busVoltage = (ReadRegister(Registers.BusVoltage) >> 3) * BusMillivoltsLsb / 1000D;
                double power = ReadRegister(Registers.Power) * _powerLsb * 1000D;
                double current = ReadRegister(Registers.Current) * _currentLsb * 1000D;
                double supplyVoltage = busVoltage + (shuntVoltage / 1000D);

                return new SensorReading[] {
                new SensorReading
                {
                    Type= Common.SensorType.BusVoltage,
                     Sensor= Common.SensorId.INA219,
                     Time=DateTime.UtcNow,
                     Value=busVoltage
                },
                new SensorReading
                {
                    Type= Common.SensorType.Current,
                     Sensor= Common.SensorId.INA219,
                     Time=DateTime.UtcNow,
                     Value=current
                },
                new SensorReading
                {
                    Type= Common.SensorType.Power,
                     Sensor= Common.SensorId.INA219,
                     Time=DateTime.UtcNow,
                     Value=power
                },
                new SensorReading
                {
                    Type= Common.SensorType.ShuntVoltage,
                     Sensor= Common.SensorId.INA219,
                     Time=DateTime.UtcNow,
                     Value=shuntVoltage
                },
                new SensorReading
                {
                    Type= Common.SensorType.SupplyVoltage,
                     Sensor= Common.SensorId.INA219,
                     Time=DateTime.UtcNow,
                     Value=supplyVoltage
                }
            };
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                _device.Dispose();
                _device = null;
            }

        }



        public INA219Sensor(double shuntOhms, double maxExpectedAmps = 0D, int address = DefaultAddress)
        {
            _shuntOhms = shuntOhms;
            _maxExpectedAmps = maxExpectedAmps;
            _address = address;

            _minDeviceCurrentLsb = CalculateMinCurrentLsb();
            _autoGainEnabled = false;



        }

        public async Task InitializeAsync()
        {

        }

        /// <summary>
        /// Configures and calibrates how the INA219 will take measurements.
        /// </summary>
        /// <param name="voltageRange">The full scale voltage range
        /// <param name="gain">The gain which controls the maximum range of the shunt</param>
        /// <param name="busResolution">The bus ADC resolution (9, 10, 11, or 12-bit) or
        /// set the number of samples used when averaging results</param>
        /// <param name="shuntResolution">The shunt ADC resolution (9, 10, 11, or 12-bit) or
        /// set the number of samples used when averaging results</param>
        public void Configure(VoltageRange voltageRange = VoltageRange.Range32V, Gain gain = Gain.AutoGain, BusADCResolution busResolution = BusADCResolution.ADC12Bit, ShuntADCResolution shuntResolution = ShuntADCResolution.ADC12Bit)
        {
            _voltageRange = voltageRange;

            if (_maxExpectedAmps != 0D)
            {
                if (gain == Gain.AutoGain)
                {
                    _autoGainEnabled = true;
                    _gain = DetermineGain(_maxExpectedAmps);
                }
                else
                {
                    _gain = gain;
                }
            }
            else
            {
                if (gain != Gain.AutoGain)
                {
                    _gain = gain;
                }
                else
                {
                    _autoGainEnabled = true;
                    _gain = Gain.Gain40mV;
                }
            }

            Debug.WriteLine($"Gain set to {_gainVolts[_gain]}V");

            Debug.WriteLine($"shunt ohms: {_shuntOhms}, bus max volts: {_busRange[_voltageRange]}, shunt volts max: {_gainVolts[_gain]} {_maxExpectedAmps}, bus ADC: {busResolution}, shunt ADC: {shuntResolution}");

            Calibrate(_voltageRange, _gain, _maxExpectedAmps);

            SetConfigurationRegister(voltageRange, _gain, busResolution, shuntResolution);
        }


        /// <summary>
        /// Put the INA219 into power down mode.
        /// </summary>
        public void Sleep()
        {

            ushort configuration = ReadRegister(Registers.Configuration);
            configuration = (ushort)(configuration & 0xFFF8);
            WriteRegister(Registers.Configuration, configuration);
        }

        /// <summary>
        /// Wake the INA219 from power down mode
        /// </summary>
        /// <returns></returns>
        public void WakeAsync()
        {
            ushort configuration = ReadRegister(Registers.Configuration);
            configuration = (ushort)(configuration | 0x0007);
            WriteRegister(Registers.Configuration, configuration);

            //40us delay to recover from powerdown (p14 of spec). We'll delay 1ms 
            //since that's all that's practical with the stock timer.
            Thread.Sleep(1);
        }

        /// <summary>
        /// Returns true if the sensor has detect current overflow. In 
        /// this case the current and power values are invalid.
        /// </summary>
        public bool CurrentOverflowed { get; private set; }

        /// <summary>
        /// Reset the INA219 to its default configuration.
        /// </summary>
        public void Reset()
        {
            WriteRegister(Registers.Configuration, ResetConfiguration);
        }

        private void HandleCurrentOverflow()
        {
            if (_autoGainEnabled)
            {
                while (CurrentOverflowed)
                {
                    IncreaseGain();
                }
            }
            else
            {
                throw new CurrentOverflowException();
            }
        }

        private Gain DetermineGain(double maxExpectedAmps)
        {
            var shuntV = maxExpectedAmps * _shuntOhms;

            if (shuntV > _gainVolts[Gain.Gain320mV])
            {
                throw new ArgumentOutOfRangeException($"Expected amps {maxExpectedAmps}, out of range, use a lower value shunt resistor");
            }

            var gain = _gainVolts.Values.Where(v => v > shuntV).Min();
            return _gainVolts.Single(v => v.Value == gain).Key;
        }

        private void IncreaseGain()
        {
            Debug.WriteLine("Current overflow detected - attempting to increase gain");

            Gain gain = ReadGain();

            if (gain < Gain.Gain320mV)
            {
                gain = gain + 2048; //Increments the gain setting by one
                Calibrate(_voltageRange, gain);
                SetGain(gain);
                //1ms delay required for configuration to take effect
                Thread.Sleep(1);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Device limit reached, cannot increase gain");
            }
        }



        private void SetConfigurationRegister(VoltageRange voltageRange, Gain gain, BusADCResolution busResolution, ShuntADCResolution shuntResolution, OperatingMode mode = OperatingMode.ShuntAndBusContinuous)
        {
            var configuration = (ushort)((ushort)voltageRange + (ushort)gain + (ushort)busResolution + (ushort)shuntResolution + (ushort)mode);
            WriteRegister(Registers.Configuration, configuration);
        }

        private void Calibrate(VoltageRange voltageRange, Gain gain, double maxExpectedAmps = 0D)
        {
            double busVoltsMax = _busRange[voltageRange];
            double shuntVoltsMax = _gainVolts[gain];

            Debug.WriteLine($"calibrate called with: bus max volts: {busVoltsMax}, max shunt volts: {shuntVoltsMax}V{maxExpectedAmps}");

            var maxPossibleAmps = shuntVoltsMax / _shuntOhms;

            Debug.WriteLine($"Max possible current: {maxPossibleAmps}A");

            _currentLsb = DetermineCurrentLsb(maxExpectedAmps, maxPossibleAmps);
            Debug.WriteLine($"Current LSB: {_currentLsb} A/bit");

            _powerLsb = _currentLsb * 20;
            Debug.WriteLine($"Power LSB: {_powerLsb} W/bit");

            var maxCurrent = _currentLsb * 32767; //Magic Number!
            Debug.WriteLine($"Max current before overflow: {maxCurrent}A");

            var maxShuntVoltage = maxCurrent * _shuntOhms;
            Debug.WriteLine($"Max shunt voltage before overflow: {maxShuntVoltage * 1000}mV");

            var calibration = (ushort)Math.Truncate(CalibrationFactor / (_currentLsb * _shuntOhms));
            Debug.WriteLine($"Calibration: {calibration}");


            WriteRegister(Registers.Calibration, calibration);

        }

        private double DetermineCurrentLsb(double maxExpectedAmps, double maxPossibleAmps)
        {
            double currentLsb = 0D;

            if (maxExpectedAmps > 0)
            {
                if (maxExpectedAmps > Math.Round(maxPossibleAmps, 3))
                {
                    throw new ArgumentException($"Expected current of {maxPossibleAmps}A is greater than max possible current of {maxPossibleAmps}A");
                }

                if (maxExpectedAmps < maxPossibleAmps)
                {
                    currentLsb = maxExpectedAmps / CurrentLsbFactor;
                }
                else
                {
                    currentLsb = maxPossibleAmps / CurrentLsbFactor;
                }
            }
            else
            {
                currentLsb = maxPossibleAmps / CurrentLsbFactor;
            }

            if (currentLsb < _minDeviceCurrentLsb)
            {
                currentLsb = _minDeviceCurrentLsb;
            }

            return currentLsb;
        }


        private double CalculateMinCurrentLsb()
        {
            return CalibrationFactor / (_shuntOhms * MaxCalibrationValue);
        }

        private Gain ReadGain()
        {
            var configuration = ReadRegister(Registers.Configuration);
            var gain = (Gain)(configuration & 0x1800); //Mask out other configuration
            Debug.WriteLine($"Gain is currently {gain}");

            return gain;
        }

        private void SetGain(Gain gain)
        {
            var configuration = ReadRegister(Registers.Configuration);
            configuration = (ushort)(configuration & 0xE7FF); //wipe current gain setting
            configuration = (ushort)(configuration + gain);
            _gain = gain;
            WriteRegister(Registers.Configuration, configuration);
        }


        private ushort WriteRegister(Registers register, ushort data)
        {
            byte[] inputBuffer = new byte[3];
            byte[] outputBuffer = new byte[2];

            inputBuffer[0] = (byte)register;
            inputBuffer[1] = (byte)(data >> 8);
            inputBuffer[2] = (byte)data;

            _device.WriteRead(inputBuffer, outputBuffer);

            ushort result = (ushort)((outputBuffer[0] << 8) + outputBuffer[1]);

            Debug.WriteLine($"Wrote {data:x} to {register}. Result: {result:x}");
            return result;

        }

        private ushort ReadRegister(Registers register)
        {
            byte[] inputBuffer = new byte[] { (byte)register };
            byte[] outputBuffer = new byte[2];

            _device.WriteRead(inputBuffer, outputBuffer);
            ushort result = (ushort)((outputBuffer[0] << 8) + outputBuffer[1]);

            Debug.WriteLine($"Read {result:x} from register {register}");
            return result;

        }
    }
}
