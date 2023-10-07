// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Midi
{
    /// <summary>
    /// General MIDI instrument, used in Program Change messages.
    /// </summary>
    /// <remarks>
    /// <para>The MIDI protocol defines a Program Change message, which can be used to switch a
    /// device among "presets".  The General MIDI specification further standardizes those presets
    /// into the specific instruments in this enum.  General-MIDI-compliant devices will
    /// have these particular instruments; non-GM devices may have other instruments.</para>
    /// <para>MIDI instruments are one-indexed in the spec, but they're zero-indexed in code, so
    /// we have them zero-indexed here.</para>
    /// <para>This enum has extension methods, such as <see cref="InstrumentExtensionMethods.Name"/>
    /// and <see cref="InstrumentExtensionMethods.IsValid"/>, defined in
    /// <see cref="InstrumentExtensionMethods"/>.</para>
    /// </remarks>
    public enum Instrument
    {
        L000 = 0, L001 = 1, L002 = 2, L003 = 3, L004 = 4, L005 = 5, L006 = 6, L007 = 7, L008 = 8, L009 = 9, L010 = 10, L011 = 11, L012 = 12,
        L013 = 13, L014 = 14, L015 = 15, L016 = 16, L017 = 17, L018 = 18, L019 = 19, L020 = 20, L021 = 21, L022 = 22, L023 = 23, L024 = 24,
        L025 = 25, L026 = 26, L027 = 27, L028 = 28, L029 = 29, L030 = 30, L031 = 31, L032 = 32, L033 = 33, L034 = 34, L035 = 35, L036 = 36,
        L037 = 37, L038 = 38, L039 = 39,
        A040 = 40, A041 = 41, A042 = 42, A043 = 43, A044 = 44, A045 = 45, A046 = 46, A047 = 47, A048 = 48, A049 = 49, A050 = 50, A051 = 51,
        A052 = 52, A053 = 53, A054 = 54,
        B055 = 55, B056 = 56, B057 = 57, B058 = 58, B059 = 59, B060 = 60, B061 = 61, B062 = 62, B063 = 63, B064 = 64, B065 = 65, B066 = 66,
        B067 = 67, B068 = 68, B069 = 69, B070 = 70, B071 = 71, B072 = 72, B073 = 73, B074 = 74, B075 = 75, B076 = 76, B077 = 77, B078 = 78,
        B079 = 79, B080 = 80, B081 = 81, B082 = 82, B083 = 83, B084 = 84, B085 = 85, B086 = 86, B087 = 87, B088 = 88, B089 = 89, B090 = 90,
        B091 = 91, B092 = 92, B093 = 93, B094 = 94,
        C095 = 95, C096 = 96, C097 = 97, C098 = 98, C099 = 99, C100 = 100, C101 = 101, C102 = 102, C103 = 103, C104 = 104, C105 = 105, C106 = 106,
        C107 = 107, C108 = 108, C109 = 109, C110 = 110, C111 = 111, C112 = 112, C113 = 113, C114 = 114, C115 = 115, C116 = 116, C117 = 117,
        C118 = 118, C119 = 119, C120 = 120, C121 = 121, C122 = 122, C123 = 123, C124 = 124, C125 = 125, C126 = 126, C127 = 127, x129=129
    };

    /// <summary>
    /// Extension methods for the Instrument enum.
    /// </summary>
    public static class InstrumentExtensionMethods
    {
        /// <summary>
        /// Returns true if the specified instrument is valid.
        /// </summary>
        /// <param name="instrument">The instrument to test.</param>
        public static bool IsValid(this Instrument instrument)
        {
            return (int)instrument >= 0 && (int)instrument < 128;
        }

        /// <summary>
        /// Throws an exception if instrument is not valid.
        /// </summary>
        /// <param name="instrument">The instrument to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The instrument is out-of-range.
        /// </exception>
        public static void Validate(this Instrument instrument)
        {
            if (!instrument.IsValid())
            {
                throw new ArgumentOutOfRangeException("Instrument out of range");
            }
        }

        /// <summary>
        /// General Midi instrument names, used by GetInstrumentName().
        /// </summary>
        private static string[] InstrumentNames = new string[]
        {
            // Piano Family:
            "Acoustic Grand Piano",
            "Bright Acoustic Piano",
            "Electric Grand Piano",
            "Honky-tonk Piano",
            "Electric Piano 1",
            "Electric Piano 2",
            "Harpsichord",
            "Clavinet",

            // Chromatic Percussion Family:
            "Celesta",
            "Glockenspiel",
            "Music Box",
            "Vibraphone",
            "Marimba",
            "Xylophone",
            "Tubular Bells",
            "Dulcimer",

            // Organ Family:
            "Drawbar Organ",
            "Percussive Organ",
            "Rock Organ",
            "Church Organ",
            "Reed Organ",
            "Accordion",
            "Harmonica",
            "Tango Accordion",

            // Guitar Family:
            "Acoustic Guitar (nylon)",
            "Acoustic Guitar (steel)",
            "Electric Guitar (jazz)",
            "Electric Guitar (clean)",
            "Electric Guitar (muted)",
            "Overdriven Guitar",
            "Distortion Guitar",
            "Guitar harmonics",

            // Bass Family:
            "Acoustic Bass",
            "Electric Bass (finger)",
            "Electric Bass (pick)",
            "Fretless Bass",
            "Slap Bass 1",
            "Slap Bass 2",
            "Synth Bass 1",
            "Synth Bass 2",

            // Strings Family:
            "Violin",
            "Viola",
            "Cello",
            "Contrabass",
            "Tremolo Strings",
            "Pizzicato Strings",
            "Orchestral Harp",
            "Timpani",

            // Ensemble Family:
            "String Ensemble 1",
            "String Ensemble 2",
            "Synth Strings 1",
            "Synth Strings 2",
            "Choir Aahs",
            "Voice Oohs",
            "Synth Voice",
            "Orchestra Hit",

            // Brass Family:
            "Trumpet",
            "Trombone",
            "Tuba",
            "Muted Trumpet",
            "French Horn",
            "Brass Section",
            "Synth Brass 1",
            "Synth Brass 2",
            	
            // Reed Family:
            "Soprano Sax",
            "Alto Sax",
            "Tenor Sax",
            "Baritone Sax",
            "Oboe",
            "English Horn",
            "Bassoon",
            "Clarinet",

            // Pipe Family:
            "Piccolo",
            "Flute",
            "Recorder",
            "Pan Flute",
            "Blown Bottle",
            "Shakuhachi",
            "Whistle",
            "Ocarina",

            // Synth Lead Family:
            "Lead 1 (square)",
            "Lead 2 (sawtooth)",
            "Lead 3 (calliope)",
            "Lead 4 (chiff)",
            "Lead 5 (charang)",
            "Lead 6 (voice)",
            "Lead 7 (fifths)",
            "Lead 8 (bass + lead)",

            // Synth Pad Family:
            "Pad 1 (new age)",
            "Pad 2 (warm)",
            "Pad 3 (polysynth)",
            "Pad 4 (choir)",
            "Pad 5 (bowed)",
            "Pad 6 (metallic)",
            "Pad 7 (halo)",
            "Pad 8 (sweep)",

            // Synth Effects Family:
            "FX 1 (rain)",
            "FX 2 (soundtrack)",
            "FX 3 (crystal)",
            "FX 4 (atmosphere)",
            "FX 5 (brightness)",
            "FX 6 (goblins)",
            "FX 7 (echoes)",
            "FX 8 (sci-fi)",

            // Ethnic Family:
            "Sitar",
            "Banjo",
            "Shamisen",
            "Koto",
            "Kalimba",
            "Bag pipe",
            "Fiddle",
            "Shanai",

            // Percussive Family:
            "Tinkle Bell",
            "Agogo",
            "Steel Drums",
            "Woodblock",
            "Taiko Drum",
            "Melodic Tom",
            "Synth Drum",
            "Reverse Cymbal",

            // Sound Effects Family:
            "Guitar Fret Noise",
            "Breath Noise",
            "Seashore",
            "Bird Tweet",
            "Telephone Ring",
            "Helicopter",
            "Applause",
            "Gunshot"
        };

        /// <summary>
        /// Returns the human-readable name of a MIDI instrument.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <exception cref="ArgumentOutOfRangeException">The instrument is out-of-range.
        /// </exception>
        public static string Name(this Instrument instrument)
        {
            instrument.Validate();
            return InstrumentNames[(int)instrument];
        }
    }
}
