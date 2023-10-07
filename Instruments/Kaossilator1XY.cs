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
    class KaossilatorXYLeftHand : Instrument
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

        public KaossilatorXYLeftHand(string name) : base(name)
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


                //X
                double leftMaxX = body.Joints[JointType.ShoulderRight].Position.X + MainWindow.armLength - MainWindow.armLength * rightThreshold;
                double rightMaxX = body.Joints[JointType.ShoulderRight].Position.X + MainWindow.armLength * leftThreshold;
                double posX = body.Joints[JointType.WristRight].Position.X;
                float percentageX  =  (float)(1 - Utils.Clamp(0, 1, (float)((posX - leftMaxX) / (rightMaxX - leftMaxX))));

                //Y
                double leftMaxY = body.Joints[JointType.ShoulderRight].Position.Y - MainWindow.armLength + MainWindow.armLength * 0.1;
                double rightMaxY = body.Joints[JointType.ShoulderRight].Position.Y + MainWindow.armLength * 0.7;
                double posY = body.Joints[JointType.WristRight].Position.Y;
                float percentageY = (float)Utils.Clamp(0, 1, (float)((posY - leftMaxY) / (rightMaxY - leftMaxY)));

                

                //X
                double leftMaxX2 = body.Joints[JointType.ShoulderLeft].Position.X - MainWindow.armLength + MainWindow.armLength * rightThreshold;
                double rightMaxX2 = body.Joints[JointType.ShoulderLeft].Position.X - MainWindow.armLength * leftThreshold;
                double posX2 = body.Joints[JointType.WristLeft].Position.X;
                float percentageX2 = (float)Utils.Clamp(0, 1, (float)((posX2 - leftMaxX2) / (rightMaxX2 - leftMaxX2)));

                //Y
                double leftMaxY2 = body.Joints[JointType.ShoulderLeft].Position.Y - MainWindow.armLength + MainWindow.armLength * 0.1;
                double rightMaxY2 = body.Joints[JointType.ShoulderLeft].Position.Y + MainWindow.armLength * 0.7;
                double posY2 = body.Joints[JointType.WristLeft].Position.Y;
                float percentageY2 = (float)Utils.Clamp(0, 1, (float)((posY2 - leftMaxY2) / (rightMaxY2 - leftMaxY2)));

                PlaySound(lWristVelocity, Math.Abs(percentageX) > Math.Abs(percentageX2) ? 1 - Math.Abs(percentageX) : 1 - Math.Abs(percentageX2), Math.Abs(percentageY) > Math.Abs(percentageY2) ? Math.Abs(percentageY) : Math.Abs(percentageY2));

            return false;
        }

        double rightThreshold = 0.3;
        double leftThreshold = 0.2;

        internal void setArpegioSpeed(Body body)
        {
            double leftMaxY = body.Joints[JointType.ShoulderRight].Position.Y - MainWindow.armLength + MainWindow.armLength * 0.1;
            double rightMaxY = body.Joints[JointType.ShoulderRight].Position.Y + MainWindow.armLength * 0.7;
            double posY = body.Joints[JointType.WristRight].Position.Y;
            float percentageY = (float)Utils.Clamp(0, 1, (float)((posY - leftMaxY) / (rightMaxY - leftMaxY)));

            int speed = (int)(percentageY * 127);
            if (body.HandRightState == HandState.Closed)
            {
                var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_ARP_speed, speed, 0);
                msg4.SendNow();
            }
        }

        internal void setArpegioSpeed(int speed)
        {
            var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_ARP_speed, speed, 0);
            msg4.SendNow();
        }


        private void PlaySound(double rWristVelocity, float percentageX, float percentageY)
        {
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
            int y = Convert.ToInt32(Math.Abs(percentageY) * 127 );

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
