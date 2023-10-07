using Microsoft.Kinect;
using Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ventuz.OSC;

namespace GesturalMusic
{
    class KaossilatorXYRandomProgram : Instrument
    {
        public string name;

        private DateTime lastTimeLPadPlayed;
        private DateTime lastTimeRPadPlayed;
        private int lastLPadPlayed;
        private int lastRPadPlayed;

        private static TimeSpan rateLimit = new TimeSpan(0, 0, 0, 0, 10);
        private static TimeSpan reHitLimit = new TimeSpan(0, 0, 0, 0, 200);

        // Velocity tracker
        private double lWristVelocity;
        private double rWristVelocity;
        private Joint lWristLocationLast;
        private Joint rWristLocationLast;
        private DateTime lastFrame;

        int _count = 0;

        public KaossilatorXYRandomProgram(string name) : base(name)
        {
            this.name = name;

            lastTimeLPadPlayed = DateTime.Now;
            lastTimeRPadPlayed = DateTime.Now;

            lWristVelocity = 0f;
            rWristVelocity = 0f;
        }

        new public void PlayNote(int x, int y)
        {
            var xmsg = new ControlChangeMessage(MainWindow.midiOutput, Channel.Channel12, Midi.Control.Volume, x, 0);
            xmsg.SendNow();
            var ymsg = new ControlChangeMessage(MainWindow.midiOutput, Channel.Channel13, Midi.Control.Volume, y, 0);
            ymsg.SendNow();
        }

        public override void OnEnd()
        {
            this.soundOf();
        }

        new public void StopNote()
        {
            //noteOn.SwitchOff();
        }

        public override bool CheckAndPlayNote(Body body)
        {
            //********************************************** X osa = rychlost pohybu zapesti

            double threshold = 0.85 * MainWindow.armLength;         // Don't even look for a hit unless more than threshold out
            double xThreshold = 0.4 * MainWindow.armLength;         // Don't even look for a hit unless more than threshold out in X

            // Set locations if first time
            if (lWristLocationLast == null || rWristLocationLast == null)
            {
                lWristLocationLast = body.Joints[JointType.WristLeft];
                rWristLocationLast = body.Joints[JointType.WristRight];
                lastFrame = DateTime.Now;
                lastLPadPlayed = -1;
                lastRPadPlayed = -1;

                return false;
            }

            // Set velocities
            TimeSpan dt = DateTime.Now - lastFrame;
            lastFrame = DateTime.Now;

            // left hand
            Joint lWrist = body.Joints[JointType.WristLeft];

            double dLWristPos = (lWrist.Position.X < lWristLocationLast.Position.X ? -1 : 1) * Utils.Length(lWrist, lWristLocationLast);
            double newLWristVelocity = dLWristPos / dt.TotalMilliseconds;

            // right hand
            Joint rWrist = body.Joints[JointType.WristRight];
            double dRWristPos = (double)Utils.LengthFloat(rWrist, rWristLocationLast);
            double newRWristVelocity = dRWristPos / dt.TotalMilliseconds;

            // Update velocities
            lWristVelocity = newLWristVelocity;
            rWristVelocity = newRWristVelocity;
            lWristLocationLast = lWrist;
            rWristLocationLast = rWrist;

            //********************************************** Y osa = vyska zapesti


            //double len = Utils.Length(body.Joints[JointType.SpineShoulder], body.Joints[JointType.Head]);
            //if (len > headLenMax) headLenMax = len;
            //if (headLenMin > len) headLenMin = len;

            //float percentageY = (float)Utils.Clamp(0, 1, (float)((len - headLenMin) / (headLenMax - headLenMin)));

            //PlaySound(lWristVelocity, percentageY, percentageY);
            //setArpegioSpeed(body);

            string a = "";
            for(int i = 0; i <= 127; i++)
            {
                this.setProgram(i);
                Thread.Sleep(400);
                //a += $"L{i.ToString("000")}={i},\r\n";
            }

            return false;
        }

        double rightThreshold = 0.3;
        double leftThreshold = 0.2;
        double headLenMax = 0;
        double headLenMin = 10;

        internal void setArpegioSpeed(Body body)
        {
            double leftMaxY = body.Joints[JointType.ShoulderRight].Position.Y - MainWindow.armLength + MainWindow.armLength * 0.1;
            double rightMaxY = body.Joints[JointType.ShoulderRight].Position.Y + MainWindow.armLength * 0.7;
            double posY = body.Joints[JointType.WristRight].Position.Y;
            float percentageY = (float)Utils.Clamp(0, 1, (float)((posY - leftMaxY) / (rightMaxY - leftMaxY)));

            int speed = (int)(percentageY * 127);
            if (body.HandRightState == HandState.Closed)
            {
                var msg5 = new ProgramChangeMessage(StaticMidi.device, Channel.Channel1, (Midi.Instrument)speed, 0);// (Midi.Instrument)speed, 0);
                msg5.SendNow();
            }
        }

        private void PlaySound(double rWristVelocity, float percentageX, float percentageY)
        {
            if (percentageX == float.NaN || percentageY == float.NaN) return;

            if (_count < 0)
            {
                _count++;
                return;
            }
            _count = 0;
            int x = Convert.ToInt32(Math.Abs(percentageX) * 127);
            //double per = Math.Abs(rWristVelocity) * 100;
            //if (per > 127) per = 127;
            //int y = Convert.ToInt32(127 - per);
            int y = Convert.ToInt32(Math.Abs(percentageY) * 127);

            var msg = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_PAD_X, x, 0);
            msg.SendNow();
            var msg2 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_PAD_Y, y, 0);
            msg2.SendNow();
            if (!ison)
            {
                soundOn();
                ison = true;
            }

        }
    }
}
