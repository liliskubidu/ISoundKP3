using Microsoft.Kinect;
using Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.OSC;

namespace GesturalMusic
{
    abstract class Instrument
    {
        public string name;
        private Filter filter;

        // For rate limiting
        private DateTime lastNotePlayed;
        private static TimeSpan rateLimit = new TimeSpan(0, 0, 0, 0, 30);

        // For hand closed state (playing state)
        HandState handStateLast;
        int lastSemitone;

        public Instrument(string name)
        {
            this.name = name;
            filter = new Filter(this.name);

            lastNotePlayed = DateTime.Now;
            handStateLast = HandState.Unknown;
            lastSemitone = -1;
        }

        /// <summary>
        /// switch sound off on kaossilator
        /// </summary>
        internal void soundOf()
        {
            this.ison = false;
            var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_TOUCH_PAD_ON_OFF, 0, 0);
            msg4.SendNow();
        }

        internal void soundOn()
        {
            var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_TOUCH_PAD_ON_OFF, 127, 0);
            msg4.SendNow();
        }

        /// <summary>
        /// set program 0-127
        /// </summary>
        /// <param name="program"></param>
        internal void setProgram(int program)
        {
            var msg5 = new ProgramChangeMessage(StaticMidi.device, Channel.Channel1, (Midi.Instrument)program, 1);// (Midi.Instrument)speed, 0);
            msg5.SendNow();
        }

        internal void arpegioOn()
        {
            var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_ARP_ONOFF, 127, 0);
            msg4.SendNow();
        }

        internal void arpegioOff()
        {
            var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_ARP_ONOFF, 0, 0);
            msg4.SendNow();
        }

        internal void setVolume(int value)
        {
            var msg4 = new ControlChangeMessage(StaticMidi.device, Channel.Channel1, Midi.Control.KORG_VOLUME, value, 0);
            msg4.SendNow();
        }

        internal bool ison = false;

        public abstract void OnEnd();


        public void PlayNote(int pitch, int velocity = 127, int duration = 500, int midiChannel = 1)
        {
            if (lastNotePlayed + rateLimit <= DateTime.Now)
            {
                Console.WriteLine("Playing: " + this.name + " " + pitch + " " + velocity + " " + duration + " " + midiChannel);
                OscElement elem = new OscElement("/" + this.name, pitch, velocity, duration, midiChannel);
                MainWindow.osc.Send(elem);

                lastNotePlayed = DateTime.Now;
            }
        }
        public abstract bool CheckAndPlayNote(Body body);

        private int getSemitone(double percentage, HandState handState)
        {
            List<Tuple<double, int>> semitoneRanges;

            if (handState == HandState.Closed)
            {
                // black semitones (minimums)
                semitoneRanges = new List<Tuple<double, int>>
                {
                    Tuple.Create(0.0  ,  1),
                    Tuple.Create(1.5/7,  3),
                    Tuple.Create(2.5/7, -1),
                    Tuple.Create(3.5/7,  6),
                    Tuple.Create(4.5/7,  8),
                    Tuple.Create(5.5/7, 10)
                };
            }
            else if (handState == HandState.Open)
            {
                // white semitones (minimums)
                semitoneRanges = new List<Tuple<double, int>>
                {
                    Tuple.Create(0.0  ,  0),
                    Tuple.Create(1.0/7,  2),
                    Tuple.Create(2.0/7,  4),
                    Tuple.Create(3.0/7,  5),
                    Tuple.Create(4.0/7,  7),
                    Tuple.Create(5.0/7,  9),
                    Tuple.Create(6.0/7, 11)
                };
            }
            else
            {
                // Console.WriteLine("Left hand not open or closed.");
                throw new Exception();
            }

            for (int i = semitoneRanges.Count - 1; i >= 0; i--)
            {
                if (percentage > semitoneRanges[i].Item1)
                {
                    return semitoneRanges[i].Item2;
                }
            }
            return semitoneRanges[0].Item2;
        }
    }
}