using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Fphi.CabinPi.Background.Sensors
{
    class BMP388Sensor : ISensor
    {
        private struct TrimmingCoefficients
        {
            public double T1;
            public double T2;
            public double T3;
            public double P1;
            public double P2;
            public double P3;
            public double P4;
            public double P5;
            public double P6;
            public double P7;
            public double P8;
            public double P9;
            public double P10;
            public double P11;
        }

        public string Name { get; set; }

        private const int DefaultAddress = 0x77;
        private TrimmingCoefficients _coefficients;

        private enum Registers
        {
            ChipId = 0x00,
            Status = 0x03,
            PressureData = 0x04,
            TempData = 0x07,
            Control = 0x1b,
            OSR = 0x1c,
            ODR = 0x1d,
            Config = 0x1f,
            CalibrationData = 0x31,
            Command = 0x7e
        }

        public async Task<IEnumerable<SensorReading>> GetReadingsAsync()
        {
            I2cDevice bmp388 = null;
            try
            {

                string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
                IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);


                var connectionSettings = new I2cConnectionSettings(DefaultAddress);

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
                bmp388 = await I2cDevice.FromIdAsync(devices[0].Id, connectionSettings);

                ReadCoefficients(bmp388);



                WriteRegister(bmp388, Registers.Control, 0x13);
                while ((ReadByte(bmp388, Registers.Status) & 0x60) != 0x60)
                {
                    await Task.Delay(2);
                }

                var data = ReadRegister(bmp388, Registers.PressureData, 6);

                var uncomp_press = data[2] << 16 | data[1] << 8 | data[0];
                var uncomp_temp = data[5] << 16 | data[4] << 8 | data[3];

                var partial_data1 = uncomp_temp - _coefficients.T1;
                var partial_data2 = partial_data1 * _coefficients.T2;

                var t_lin = partial_data2 + (partial_data1 * partial_data1) * _coefficients.T3;

                //partial_data1 = calib_data->par_p6 * calb_data->t_lin
                partial_data1 = _coefficients.P6 * t_lin;
                //partial_data2 = calib_data->par7 * (calib_data->t_lin * calib_data->t_lin);
                partial_data2 = _coefficients.P7 * (t_lin * t_lin);
                //partial_data3 = calib_data->par_p8 * (calib_data->t_lin * calib_data->t_lin * calib_data->t_lin);
                var partial_data3 = _coefficients.P8 * (t_lin * t_lin * t_lin);
                //partial_out1 = calib_data->par_p5 + partial_data1 + partial_data2 + partial_data3;
                var partial_out1 = _coefficients.P5 + partial_data1 + partial_data2 + partial_data3;

                //partial_data1 = calib_data->par_p2 * calib_data->t_lin;
                partial_data1 = _coefficients.P2 * t_lin;
                //partial_data2 = calib_data->par_p3 * (calib_data->t_lin * calib_data->t_lin);
                partial_data2 = _coefficients.P3 * (t_lin * t_lin);
                //partial_data3 = calib_data->par_p4 * (calib_data->t_lin * calib_data->t_lin * calib_data->t_lin);
                partial_data3 = _coefficients.P4 * (t_lin * t_lin * t_lin);
                //partial_out2 = (float)uncomp_press * (calib_data->par_p1 + partial_data1 + partial_data2 + partial_data3);
                var partial_out2 = uncomp_press * (_coefficients.P1 + partial_data1 + partial_data2 + partial_data3);

                //partial_data1 = (float)uncomp_press * (float)uncomp_press
                partial_data1 = uncomp_press * (double)uncomp_press;
                //partial_data2 = calib_data->par_p9 + calib_data->par_p10 * calib_data->t_lin
                partial_data2 = _coefficients.P9 + _coefficients.P10 * t_lin;
                //partial_data3 = partial_data1 * partial_data2;
                partial_data3 = partial_data1 * partial_data2;
                //partial_data4 = partial_data3 + ((float)uncomp_press * (float)uncomp_press * (float)uncomp_press) * calib_data->par_p11;
                var partial_data4 = partial_data3 + (uncomp_press * (double)uncomp_press * uncomp_press) * _coefficients.P11;
                //comp_press = partial_out1 + partial_out2 + partial_data4;
                var comp_press = partial_out1 + partial_out2 + partial_data4;

                return new SensorReading[] {
                    new SensorReading
                    {
                        Type= Common.SensorType.PressurePascals,
                         Sensor= Common.SensorId.BMP388,
                         Time=DateTime.UtcNow,
                         Value=comp_press
                    },
                    new SensorReading
                    {
                        Type= Common.SensorType.PressureInHg,
                         Sensor= Common.SensorId.BMP388,
                         Time=DateTime.UtcNow,
                         Value=comp_press *  0.00029530D
                    },
                    new SensorReading
                    {
                        Type= Common.SensorType.InteriorTemperatureC,
                         Sensor= Common.SensorId.BMP388,
                         Time=DateTime.UtcNow,
                         Value=t_lin
                    }

                };

                // Start the polling timer.
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                bmp388.Dispose();
                bmp388 = null;
            }
        }

        private void ReadCoefficients(I2cDevice bmp388)
        {
            var calibrationData = ReadRegister(bmp388, Registers.CalibrationData, 21);
            _coefficients.T1 = ((ushort)(calibrationData[0] + (calibrationData[1] << 8))) / Math.Pow(2, -8);
            _coefficients.T2 = ((ushort)(calibrationData[2] + (calibrationData[3] << 8))) / Math.Pow(2, 30);
            _coefficients.T3 = ((sbyte)calibrationData[4]) / Math.Pow(2, 48);
            _coefficients.P1 = (((short)(calibrationData[5] + (calibrationData[6] << 8))) - Math.Pow(2, 14)) / Math.Pow(2, 20);
            _coefficients.P2 = (((short)(calibrationData[7] + (calibrationData[8] << 8))) - Math.Pow(2, 14)) / Math.Pow(2, 29);
            _coefficients.P3 = ((sbyte)calibrationData[9]) / Math.Pow(2, 32);
            _coefficients.P4 = ((sbyte)calibrationData[10]) / Math.Pow(2, 37);
            _coefficients.P5 = ((ushort)(calibrationData[11] + (calibrationData[12] << 8))) / Math.Pow(2, -3);
            _coefficients.P6 = ((ushort)(calibrationData[13] + (calibrationData[14] << 8))) / Math.Pow(2, 6);
            _coefficients.P7 = ((sbyte)calibrationData[15]) / Math.Pow(2, 8);
            _coefficients.P8 = ((sbyte)calibrationData[16]) / Math.Pow(2, 15);
            _coefficients.P9 = ((short)(calibrationData[17] + (calibrationData[18] << 8))) / Math.Pow(2, 48);
            _coefficients.P10 = ((sbyte)calibrationData[19]) / Math.Pow(2, 48);
            _coefficients.P11 = ((sbyte)calibrationData[20]) / Math.Pow(2, 65);
        }

        private void WriteRegister(I2cDevice device, Registers register, byte data)
        {
            byte[] inputBuffer = new byte[2];
            //byte[] outputBuffer = new byte[2];

            inputBuffer[0] = (byte)register;
            inputBuffer[1] = data;

            device.Write(inputBuffer);

            //ushort result = (ushort)((outputBuffer[0] << 8) + outputBuffer[1]);

            Debug.WriteLine($"Wrote {data:x} to {register}.");

        }

        private byte ReadByte(I2cDevice device, Registers register)
        {
            return ReadRegister(device, register, 1)[0];
        }

        private byte[] ReadRegister(I2cDevice device, Registers register, int count)
        {
            byte[] inputBuffer = new byte[] { (byte)register };
            byte[] outputBuffer = new byte[count];

            device.WriteRead(inputBuffer, outputBuffer);

            Debug.WriteLine($"Read {outputBuffer:x} from {register}.");

            return outputBuffer;
        }

        public async Task InitializeAsync()
        {

        }
    }
}
