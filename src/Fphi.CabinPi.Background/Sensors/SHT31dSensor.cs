using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Fphi.CabinPi.Background.Sensors
{
    class SHT31dSensor : ISensor
    {
        private const ushort HT31_MEAS_HIGHREP_STRETCH = 0x2C06;
        private const ushort SHT31_MEAS_MEDREP_STRETCH = 0x2C0D;
        private const ushort SHT31_MEAS_LOWREP_STRETCH = 0x2C10;
        private byte[] SHT31_MEAS_HIGHREP = new byte[] { 0x24, 0x00 };
        private const ushort SHT31_MEAS_MEDREP = 0x240B;
        private const ushort SHT31_MEAS_LOWREP = 0x2416;
        private const ushort SHT31_READSTATUS = 0xF32D;
        private const ushort SHT31_CLEARSTATUS = 0x3041;
        private const ushort SHT31_SOFTRESET = 0x30A2;
        private const ushort SHT31_HEATER_ON = 0x306D;
        private const ushort SHT31_HEATER_OFF = 0x3066;

        private const ushort SHT31_STATUS_DATA_CRC_ERROR = 0x0001;
        private const ushort SHT31_STATUS_COMMAND_ERROR = 0x0002;
        private const ushort SHT31_STATUS_RESET_DETECTED = 0x0010;
        private const ushort SHT31_STATUS_TEMPERATURE_ALERT = 0x0400;
        private const ushort SHT31_STATUS_HUMIDITY_ALERT = 0x0800;
        private const ushort SHT31_STATUS_HEATER_ACTIVE = 0x2000;
        private const ushort SHT31_STATUS_ALERT_PENDING = 0x8000;

        public async Task<IEnumerable<SensorReading>> GetReadings()
        {
            I2cDevice sht31d = null;
            try
            {

                string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
                IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);


                var connectionSettings = new I2cConnectionSettings(0x44); //Default address

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
                sht31d = await I2cDevice.FromIdAsync(devices[0].Id, connectionSettings);
                byte[] buffer = new byte[6];
                sht31d.Write(SHT31_MEAS_HIGHREP);
                await Task.Delay(15);
                sht31d.Read(buffer);
                //sht31d.WriteRead(SHT31_MEAS_HIGHREP, buffer);

                if (CRC8(new byte[] { buffer[0], buffer[1] }) != buffer[2])
                {
                    throw new InvalidOperationException("CRC Mismatch");
                }
                double rawTemperature = buffer[0] << 8 | buffer[1];
                double temperatureC = 175.0 * rawTemperature / 0xFFFF - 45.0;
                double temperatureF = 315.0 * rawTemperature / 0xFFFF - 49.0;

                if (CRC8(new byte[] { buffer[3], buffer[4] }) != buffer[5])
                {
                    throw new InvalidOperationException("CRC Mismatch");
                }
                double rawHumidity = buffer[3] << 8 | buffer[4];
                double humidity = 100.0 * rawHumidity / 0xFFFF;


                return new SensorReading[] {
                new SensorReading
                {
                    Name="TemperatureC",
                     Sensor="SHT31d",
                     Time=DateTime.UtcNow,
                     Value=temperatureC
                },
                new SensorReading
                {
                    Name="TemperatureF",
                     Sensor="SHT31d",
                     Time=DateTime.UtcNow,
                     Value=temperatureF
                },
                new SensorReading
                {
                    Name="Humidity",
                     Sensor="SHT31d",
                     Time=DateTime.UtcNow,
                     Value=humidity
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
                sht31d.Dispose();
                sht31d = null;
            }
        }

        private byte CRC8(byte[] buffer)
        {
            byte polynomial = 0x31;
            byte crc = 0xFF;

            for (int i = 0; i < buffer.Length; i++)
            {
                crc ^= buffer[i];
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 0x80) > 0)
                    {
                        crc = (byte)((crc << 1) ^ polynomial);
                    }
                    else
                    {
                        crc = (byte)(crc << 1);
                    }
                }
            }

            return (byte)(crc & 0xFF);
        }
    }
}
