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
                //using (amBX engine = new amBX(1, 0, "ambxTest1", "1.0.0"))
                //{
                //    amBXLight everyLight = engine.CreateLight(CompassDirection.Everywhere, RelativeHeight.AnyHeight);
                //    everyLight.FadeTime = 1000;
                //    everyLight.Color = new amBXColor() { Red = 0f, Green = 1f, Blue = 0f };                    
                //    Thread.Sleep(3000);
                //}
                using (amBX engine = new amBX(1, 0, "ambxTest1", "1.0.0"))
                {
                    amBXLight everyLight = engine.CreateLight(CompassDirection.Everywhere, RelativeHeight.AnyHeight);

                    everyLight.FadeTime = 100;

                    amBXColor green = new amBXColor() { Red = 0f, Green = 1f, Blue = 0f };
                    amBXColor off = new amBXColor() { Red = 0f, Green = 0f, Blue = 0f };

                    for (int i = 0; i < 10; i++)
                    {
                        everyLight.Color = green;
                        Thread.Sleep(200);
                        everyLight.Color = off;
                        Thread.Sleep(200);
                    }
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
                    amBXLight everyLight = engine.CreateLight(CompassDirection.Everywhere, RelativeHeight.AnyHeight);

                    everyLight.FadeTime = 100;

                    amBXColor red = new amBXColor() { Red = 1f, Green = 0f, Blue = 0f };
                    amBXColor off = new amBXColor() { Red = 0f, Green = 0f, Blue = 0f };

                    for (int i = 0; i < 10; i++)
                    {
                        everyLight.Color = red;
                        Thread.Sleep(200);
                        everyLight.Color = off;
                        Thread.Sleep(200);
                    }
                }
            }
            catch { }
        }
    }
}
