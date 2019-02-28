using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Fphi.CabinPi.Background.Sensors
{
    class SHT31dSensor : I2CSensor
    {
        private const int DefaultAddress = 0x44;

        private const ushort HT31_MEAS_HIGHREP_STRETCH = 0x2C06;
        private const ushort SHT31_MEAS_MEDREP_STRETCH = 0x2C0D;
        private const ushort SHT31_MEAS_LOWREP_STRETCH = 0x2C10;
        //private byte[] SHT31_MEAS_HIGHREP = new byte[] { 0x24, 0x00 };
        private const ushort SHT31_MEAS_HIGHREP = 0x2400;
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


        public SHT31dSensor()
        {
            Address = DefaultAddress;
        }


        protected override async Task<IEnumerable<SensorReading>> GetI2CReadingsAsync()
        {
            
            Write(SHT31_MEAS_HIGHREP);
            await Task.Delay(15);
            var buffer = ReadBytes(6);
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
                    Type= Common.SensorType.InteriorTemperatureC,
                     Sensor= Common.SensorId.Sht31d,
                     Time=DateTime.UtcNow,
                     Value=temperatureC
                },
                new SensorReading
                {
                    Type= Common.SensorType.InteriorTemperatureF,
                     Sensor= Common.SensorId.Sht31d,
                     Time=DateTime.UtcNow,
                     Value=temperatureF
                },
                new SensorReading
                {
                    Type= Common.SensorType.InteriorHumidity,
                     Sensor= Common.SensorId.Sht31d,
                     Time=DateTime.UtcNow,
                     Value=humidity
                }
            };
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
