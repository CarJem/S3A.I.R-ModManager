using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX;
using System.Diagnostics;
using SDL2;
using System.Runtime.InteropServices;

namespace Sonic3AIR_ModLoader
{
    public class JoystickReader 
    {

        private static int GetPOVBitIndex(byte value)
        {
            switch (value)
            {
                case 0x01:
                    return 0;
                case 0x02:
                    return 1;
                case 0x04:
                    return 2;
                case 0x08:
                    return 3;
                case 0x10:
                    return 4;
                case 0x20:
                    return 5;
                case 0x40:
                    return 6;
                case 0x80:
                    return 7;
                default:
                    return 0;
            }
        }

        public static IntPtr GetJoystick()
        {
            SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);


            int joysticks = SDL.SDL_NumJoysticks();
            var joystick = SDL.SDL_JoystickOpen(0); //Better do that only once, cache the pointer

            return joystick;
        }

        private static void WipeEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event sdl_event_clean) == 1)
            {
                //CleanUp
            }
        }


        public static string GetJoystickInput(IntPtr joystick)
        {
            string output = "";

            WipeEvents();

            bool searching = true;

            while (searching)
            {
                SDL.SDL_PollEvent(out SDL.SDL_Event sdl_event);
                if (sdl_event.type == SDL.SDL_EventType.SDL_JOYBUTTONDOWN)
                {
                    byte button = sdl_event.jbutton.button;
                    string id = string.Format("Button{0}", button);
                    output = id;
                    searching = false;
                }

                if (sdl_event.type == SDL.SDL_EventType.SDL_JOYHATMOTION)
                {
                    int hat = (int)sdl_event.jhat.hat * 8 + GetPOVBitIndex(sdl_event.jhat.hatValue);
                    string id = string.Format("POV{0}", hat);
                    output = id;
                    searching = false;


                }

                if (sdl_event.type == SDL.SDL_EventType.SDL_JOYAXISMOTION)
                {
                    float axis_value = sdl_event.jaxis.axisValue / 32767.0f;
                    byte axis_id = sdl_event.jaxis.axis;


                    int axis = (int)sdl_event.jaxis.axis * 2;
                    if (axis_value > 0) axis = axis + 1;

                    if (axis_value < -0.25f)
                    {
                        string id = string.Format("Axis {0}", axis);
                        output = id;
                        searching = false;

                    }
                    else if (axis_value > 0.25f)
                    {
                        string id = string.Format("Axis {0}", axis);
                        output = id;
                        searching = false;

                    }



                }


            }

            WipeEvents();

            if (output != "") return output;
            else return null;


        }
    }


}
