using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SDL2;
using System.Runtime.InteropServices;

namespace Sonic3AIR_ModManager
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

        public static IntPtr GetJoystick(int index = 0)
        {
            SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);

            var joystick = SDL.SDL_JoystickOpen(index); //Better do that only once, cache the pointer

            return joystick;
        }

        public static List<string> GetJoysticks()
        {
            SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);

            SDL.SDL_JoystickUpdate();

            int joystickCount = SDL.SDL_NumJoysticks();

            List<string> inputNames = new List<string>();

            for (int i = 0; i < joystickCount; i++)
            {
                //if (SDL.SDL_JoystickGetAttached(GetJoystick(i)) == SDL.SDL_bool.SDL_TRUE)
                inputNames.Add(SDL.SDL_JoystickNameForIndex(i));
            }

            return inputNames;
        }

        public static int GetJoystickCount()
        {
            SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);

            SDL.SDL_JoystickUpdate();

            int joystickCount = SDL.SDL_NumJoysticks();

            return joystickCount;
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
