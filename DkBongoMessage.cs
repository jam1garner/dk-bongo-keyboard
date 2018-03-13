using System;
/*
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00006F08 <--- clap
Button depressed.
000000FFFF00006008
Button depressed.
000000FFFF00002008
Button depressed.
000000FFFF00001708
Button depressed.
000000FFFF00001408
Button depressed.
000000FFFF00000F08
Button depressed.
000000FFFF00000C08
Button depressed.
000000FFFF00000C08
Button depressed.
000000FFFF00000B08
Button depressed.
000000FFFF00000B08
Button depressed.
000000FFFF00000B08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000B08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08
Button depressed.
000000FFFF00000A08

 Byte 1:
 1 bit - top right
 2 bit - bottom right
 4 bit - bottom left
 8 bit - top left

 Byte 2:
 2 bit - start button
     */
namespace DkBongoKeyboard
{
    public class DkBongoMessage
    {
        public bool rightBongoTopPressed;
        public bool rightBongoBottomPressed;
        public bool leftBongoBottomPressed;
        public bool leftBongoTopPressed;

        public bool startPressed;

        public byte micLevel;

        public DkBongoMessage()
        {

        }

        public DkBongoMessage(byte[] message)
        {
            rightBongoTopPressed = (message[0] & 0x1) != 0;
            rightBongoBottomPressed = (message[0] & 0x2) != 0;
            leftBongoBottomPressed = (message[0] & 0x4) != 0;
            leftBongoTopPressed = (message[0] & 0x8) != 0;

            startPressed = (message[1] & 0x2) != 0;

            micLevel = message[7];

            if(message[3] != 0xFF)
            {
                Console.WriteLine("Wrong port");
            }
            else
            {
                //Console.WriteLine($"Buttons pressed tr {rightBongoTopPressed} br {rightBongoBottomPressed} bl {leftBongoBottomPressed} tl {leftBongoTopPressed} start {startPressed} mic {micLevel}");

            }
            //Console.WriteLine(BitConverter.ToString(message).Replace("-", ""));
        }
    }
}
