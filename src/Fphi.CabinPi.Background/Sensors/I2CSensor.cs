using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Fphi.CabinPi.Background.Sensors
{
    abstract class I2CSensor : ISensor
    {
        protected I2cDevice _device = null;

        protected int Address { get; set; }
        abstract protected Task<IEnumerable<SensorReading>> GetI2CReadingsAsync();

        public string Name { get; set; }


        public async Task<IEnumerable<SensorReading>> GetReadingsAsync()
        {
            //TODO: Need to verify _device is happy. If not, need to retry.
            if(_device != null)
            {
                return await GetI2CReadingsAsync();
            }

            return new List<SensorReading>();
        }


        public async Task InitializeAsync()
        {
            try
            {

                string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
                IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);


                var connectionSettings = new I2cConnectionSettings(Address); //Default address

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
                await PostInitialize();

            }
            catch (Exception)
            {

                throw;
            }

        }

        protected virtual Task PostInitialize()
        {
            return Task.CompletedTask;
        }

        protected byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            _device.Read(buffer);

            return buffer;
        }

        protected void Write(ushort value)
        {
            byte[] buffer = new byte[] { (byte)(value >> 8), (byte)value };
            _device.Write(buffer);
        }

        protected ushort WriteReadRegister(byte register, ushort value)
        {
            byte[] inputBuffer = new byte[3];
            byte[] outputBuffer = new byte[2];

            inputBuffer[0] = register;
            inputBuffer[1] = (byte)(value >> 8);
            inputBuffer[2] = (byte)value;

            _device.WriteRead(inputBuffer, outputBuffer);

            ushort result = (ushort)((outputBuffer[0] << 8) + outputBuffer[1]);

            Debug.WriteLine($"Wrote {value:x} to {register:x}. Result: {result:x}");
            return result;
        }

        protected void WriteRegister(byte register, byte value)
        {
            byte[] inputBuffer = new byte[2];
            //byte[] outputBuffer = new byte[2];

            inputBuffer[0] = register;
            inputBuffer[1] = value;

            _device.Write(inputBuffer);

            //ushort result = (ushort)((outputBuffer[0] << 8) + outputBuffer[1]);

            Debug.WriteLine($"Wrote {value:x} to register {register:x}.");
        }

        protected ushort ReadUShortRegister(byte register)
        {
            byte[] inputBuffer = new byte[] { register };
            byte[] outputBuffer = new byte[2];

            _device.WriteRead(inputBuffer, outputBuffer);
            ushort result = (ushort)((outputBuffer[0] << 8) + outputBuffer[1]);

            Debug.WriteLine($"Read {result:x} from register {register:x}");
            return result;
        }

        protected byte ReadByteRegister(byte register)
        {
            return ReadRegisterBytes(register, 1)[0];
        }

        protected byte[] ReadRegisterBytes(byte register, int count)
        {
            byte[] inputBuffer = new byte[] { register };
            byte[] outputBuffer = new byte[count];

            _device.WriteRead(inputBuffer, outputBuffer);

            Debug.WriteLine($"Read {BitConverter.ToString(outputBuffer)} from register {register:x}.");

            return outputBuffer;
        }
    }
}
