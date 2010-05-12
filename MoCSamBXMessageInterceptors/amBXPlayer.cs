using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using amBXLib;

namespace MoCSamBXMessageInterceptors
{
    public class amBXPlayer
    {

        public void PlaySuccess()
        {
            try
            {
                using (amBX engine = new amBX(1, 0, "ambxTest1", "1.0.0"))
                {
                    amBXLight everyLight =

                        engine.CreateLight(

                            CompassDirection.Everywhere, RelativeHeight.AnyHeight);

                    everyLight.FadeTime = 1000;

                    everyLight.Color = new amBXColor() { Red = 0f, Green = 1f, Blue = 0f };

                    Thread.Sleep(3000);
                }
            }
            catch { }
        }

        public void PlayFailure()
        {
            try
            {
                using (amBX engine = new amBX(1, 0, "ambxTest1", "1.0.0"))
                {
                    amBXLight everyLight =

                        engine.CreateLight(

                            CompassDirection.Everywhere, RelativeHeight.AnyHeight);

                    everyLight.FadeTime = 300;

                    amBXColor red = new amBXColor() { Red = 1f, Green = 0f, Blue = 0f };
                    amBXColor off = new amBXColor() { Red = 0f, Green = 0f, Blue = 0f };

                    for (int i = 0; i < 5; i++)
                    {
                        everyLight.Color = red;
                        Thread.Sleep(300);
                        everyLight.Color = off;
                        Thread.Sleep(300);
                    }
                }
            }
            catch { }
        }
    }
}
