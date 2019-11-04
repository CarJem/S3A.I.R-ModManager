using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX;
using System.Diagnostics;

namespace Sonic3AIR_ModLoader
{
    public class DirectInputReader 
    {
        private static List<JoystickOffset> AxisFlags = new List<JoystickOffset>() { JoystickOffset.X, JoystickOffset.Y, JoystickOffset.RotationX, JoystickOffset.RotationY };

        public static Joystick GetDevice()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            List<Guid> JoystickGuids = new List<Guid>();
            var joystickGuid = Guid.Empty;


            // Find Gamepads
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                JoystickGuids.Add(deviceInstance.InstanceGuid);
            }


            // Find Joysticks
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
            {
                JoystickGuids.Add(deviceInstance.InstanceGuid);
            }

            // TODO: Allow for Better Controller Selection
            if (JoystickGuids.Count >= 1) joystickGuid = JoystickGuids.FirstOrDefault();

            // If Joystick not found, return null
            if (joystickGuid == Guid.Empty)
            {
                Debug.Print("No joystick/Gamepad found.");
                return null;
            }

            // If Joystick found, return it
            else
            {
                var joystick = new Joystick(directInput, joystickGuid);

                Debug.Print("Found Joystick/Gamepad: {0}", joystick.Information.ProductName);

                return joystick;


            }
        }

        public static string GetDeviceName(Joystick joystick)
        {
            return joystick.Information.ProductName;
        }

        public static JoystickUpdate? GetDeviceInput(Joystick joystick)
        {
            JoystickUpdate? result = null;
            bool inputNotFound = true;

            joystick.Properties.DeadZone = 5000;

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Console.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (inputNotFound)
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                {
                    result = state;
                    Debug.Print(state.ToString());
                    inputNotFound = false;
                }
            }

            joystick.Unacquire();

            return result;

        }
    }


}
