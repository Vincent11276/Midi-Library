using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;




namespace Midi_Visualizer
{

    public enum EventType : byte
    {
        VoiceNoteOff = 0x80,
        VoiceNoteOn = 0x90,
        VoiceAfterTouch = 0x40,
        VoiceControlChange = 0xB0,
        VoiceProgramChange = 0xC0,
        VoiceChannelPressure = 0xD0,
        VoicePitchBend = 0xE0,
        SystemExclusive = 0xF0
    };

    public enum MetaEventType : byte
    {
        MetaSequence = 0x00,
        MetaText = 0x01,
        MetaCopyright = 0x02,
        MetaTrackName = 0x03,
        MetaInstrumentName = 0x04,
        MetaLyrics = 0x05,
        MetaMarker = 0x06,
        MetaCuePoint = 0x07,
        MetaChannelPrefix = 0x20,
        MetaEndOfTrack = 0x2F,
        MetaSetTempo = 0x51,
        MetaSMPTEOffset = 0x54,
        MetaTimeSignature = 0x58,
        MetaKeySignature = 0x59,
        MetaSequencerSpecific = 0x7F,
    };

    // public sealed record VoiceNoteOff(byte Channel, byte NodeId, byte Velocity, byte Delta);

    public enum MidiMessageType : byte
    {
        VoiceNoteOff,
        VoiceNoteOn,
        VoiceAfterTouch,
        VoiceControlChange,
        VoiceProgramChange,
        VoiceChannelPressure,
        VoicePitchBend,
        MetaSequence,
        MetaText,
        MetaCopyright,
        MetaTrackName,
        MetaInstrumentName,
        MetaLyrics,
        MetaMarker,
        MetaCuePoint,
        MetaChannelPrefix,
        MetaEndOfTrack,
        MetaSetTempo,
        MetaSMPTEOffset,
        MetaTimeSignature,
        MetaKeySignature,
        MetaSequencerSpecific
    }


    /*
     * public class MidiMessage
    {
        public MidiMessageType MessageType;

        public byte Delta { get; }

        public byte Status { get; }

        public IReadOnlyCollection<byte> DataBytes { get; }

        public MidiMessage(byte delta, byte status, List<byte> dataBytes)
        {
            Delta = delta;
            Status = status;
            DataBytes = dataBytes.ToList().AsReadOnly();
        }

        public byte GetDeltaTime()
        {
            return Delta;
        }

        public byte GetType()
        {

        }

        public byte GetChannel()
        {
            return (byte)(Status & 0x0F);
        }

        public byte Get
    }*/

    public struct MidiMessage
    {
        public MidiMessageType type;
        public uint deltaTime;
        public byte channel;
        public byte noteID;
        public byte velocity;
        public byte program;
        public byte control;
        public byte value;
        public byte LS7B;
        public byte MS7B;
        public byte pressure;
        public int tempo;
        public string name;


    };

    public struct MidiTrack
    {
        public uint Length;
        public List<MidiMessage> Messages;

        public MidiTrack(uint length, List<MidiMessage> messages)
        {
            Length = length;
            Messages = messages;
        }
    };


    public class MidiFile
    {
        private readonly List<MidiTrack> tracks = new List<MidiTrack>();

        public bool LoadFile(String filePath)
        {
            bool isDebug = true;

            if (!File.Exists(filePath)) return false;

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {

                // Read MIDI Header (Fixed Size)
                uint fileID = SwapUInt32(reader.ReadUInt32());
                uint headerLength = SwapUInt32(reader.ReadUInt32());
                ushort format = (ushort)SwapUInt16(reader.ReadUInt16());
                ushort trackCount = (ushort)SwapUInt16(reader.ReadUInt16());
                ushort resolution = (ushort)SwapUInt16(reader.ReadUInt16());

                Console.WriteLine("AHDAGAGAGA");

                if (isDebug)
                {
                    Console.WriteLine("============= HEADER =============");
                    Console.WriteLine("File ID = " + UIntToString(fileID));
                    Console.WriteLine("Header Length = " + UIntToString(headerLength));
                    Console.WriteLine("Format = " + UShortToString(format));
                    Console.WriteLine("Track Count = " + UShortToString(trackCount));
                    Console.WriteLine("Resolution = " + UShortToString(resolution));
                    Console.WriteLine('\n');
                }

                // Read Tracks          
                for (ushort trackIndex = 0; trackIndex < trackCount; ++trackIndex)
                {
                    tracks.Add(new MidiTrack());    
                    uint trackID = SwapUInt32(reader.ReadUInt32());
                    uint trackLength = SwapUInt32(reader.ReadUInt32());

                    if (isDebug)
                    {
                        Console.WriteLine("============= NEW TRACK #{0} =============", trackIndex);
                        Console.WriteLine("Track ID = " + UIntToString(trackID));
                        Console.WriteLine("Track Length = " + UIntToString(trackLength));
                    }
                    int counter = 0;
                    byte prevStatus = 0x00;

                    while (true)
                    {
                        uint deltaTime = ReadValue(reader);
                        byte status = reader.ReadByte();
                        
                        if ((status & 0x80) == 0x00) // Kung ang MSB ng status sa message(ngayon) ay 1 
                        {
                            status = prevStatus; // Status sa message(ngayon) ay set to status ng message(previous)

                            reader.BaseStream.Seek(-2, SeekOrigin.Current); // move read pointer 1 byte back para sync ulit kasi running status
                        }
                        else prevStatus = status; // save ang status(ngayon) para sa next message in case na running status

                        if ((byte)(status & 0xF0) == (byte)EventType.VoiceNoteOff)
                        {
                            byte noteID = reader.ReadByte();
                            byte noteVelocity = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceNoteOff;
                            message.channel = (byte)(status & 0x0F);
                            message.noteID = noteID;
                            message.velocity = noteVelocity;

                            //tracks[trackIndex].Events.Add(message);

                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"note_off\", NoteID={2}, Velocity={3}]", 
                                counter, deltaTime, noteID, noteVelocity);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.VoiceNoteOn)
                        {
                            byte noteID = reader.ReadByte();
                            byte noteVelocity = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceNoteOn;
                            message.channel = (byte)(status & 0x0F);
                            message.noteID = noteID;
                            message.velocity = noteVelocity;

                            // basta velocity is 0, automatic note off
                            if (noteVelocity == 0)
                            {
                                if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"note_off\", NoteID={2}, Velocity={3}]",
                                counter, deltaTime, noteID, noteVelocity);
                                message.type = MidiMessageType.VoiceNoteOff;
                            }

                            //tracks[trackIndex].Events.Add(message);


                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"note_on\", NoteID={2}, Velocity={3}]", 
                                counter, deltaTime, noteID, noteVelocity);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.VoiceAfterTouch)
                        {
                            byte noteID = reader.ReadByte();
                            byte noteVelocity = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceAfterTouch;
                            message.channel = (byte)(status & 0x0F);
                            message.noteID = noteID;
                            message.velocity = noteVelocity;

                           // tracks[trackIndex].Events.Add(message);

                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"after_touch\", NoteID={2}, Velocity={3}]", 
                                counter, deltaTime, noteID, noteVelocity);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.VoiceControlChange)
                        {
                            byte controlID = reader.ReadByte();
                            byte controlValue = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceControlChange;
                            message.channel = (byte)(status & 0x0F);
                            message.control = controlID;
                            message.value = controlValue;

                           // tracks[trackIndex].Events.Add(message);

                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"control_change\", control={2}, value={3}]", 
                                counter, deltaTime, controlID, controlValue);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.VoiceProgramChange)
                        {
                            byte programID = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceAfterTouch;
                            message.channel = (byte)(status & 0x0F);
                            message.program = programID;

                           // tracks[trackIndex].Events.Add(message);

                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"program_change\", program={2}]", 
                                counter, deltaTime, programID);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.VoiceChannelPressure)   
                        {
                            byte channelPressure = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceAfterTouch;
                            message.channel = (byte)(status & 0x0F);
                            message.pressure = channelPressure;

                          //  tracks[trackIndex].Events.Add(message);

                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"channel_pressure\", program={2}]",
                                counter, deltaTime, channelPressure);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.VoicePitchBend)
                        {
                            byte LS7B = reader.ReadByte();
                            byte MS7B = reader.ReadByte();

                            MidiMessage message = new MidiMessage();
                            message.deltaTime = deltaTime;
                            message.type = MidiMessageType.VoiceAfterTouch;
                            message.channel = (byte)(status & 0x0F);
                            message.LS7B = LS7B;
                            message.MS7B = MS7B;

                            // tracks[trackIndex].Events.Add(message);


                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={0}, Type=\"pitch_bend\", control={1}, value={2}]", 
                                counter, deltaTime, LS7B, MS7B);
                        }
                        else if ((byte)(status & 0xF0) == (byte)EventType.SystemExclusive)
                        {
                            byte type = reader.ReadByte();
                            uint length = ReadValue(reader);

                            string bytesStr = "";
                            for (uint j = 0; j < length; j++)
                                bytesStr += ByteToString(reader.ReadByte());
                            bytesStr = Regex.Replace(bytesStr, ".{2}", "$0 ").Trim();

                            if (isDebug) Console.WriteLine("Message #{0}: Delta Time={1}, Type=\"system_exclusive\", length={2}, data=IDK!",
                                counter, deltaTime, length);

                            if (type ==  (byte)MetaEventType.MetaEndOfTrack)
                            {
                                if (isDebug) Console.WriteLine("============= END OF TRACK #{0}=============\n", trackIndex);
                                break;
                            }
                        }
                        else if (isDebug) Console.WriteLine("Unrecognized Status Byte Command!" + status.ToString("X2"));

                        counter++;
                    }
                }
            }

            if (isDebug) Console.WriteLine("Parsing Done!");

            return true;
        }

        private uint ReadValue(BinaryReader reader)
        {
            uint deltaTime = reader.ReadByte();
            uint prevByte = 0;

            // If MSB of delta time is set.. keep reading!
            if ((deltaTime & 0x80) != 0x00)
            {
                // Extract bottom 7 bits
                deltaTime &= 0x7F;

                // If MSB of delta time is set.. keep reading!
                while ((prevByte & 0x80) != 0x00)
                {
                    // Read the next byte
                    prevByte = reader.ReadByte();

                    // Shift the bits of delta time 7 times to the left and set the bottom 7 bits by the previous byte
                    deltaTime = (deltaTime << 7) | (prevByte & 0x7F);
                }
            } 
            return deltaTime;
        }

        private string ReadString(BinaryReader reader, uint length)
        {
            string s = "";
            for (uint i = 0; i < length; i++)
                s += reader.ReadChar();
            return s;
        }

        private uint SwapUInt32(uint n)
        { 
            return (((n >> 24) & 0xFF) | ((n << 8) & 0xFF0000) | ((n >> 8) & 0xFF00) | ((n << 24) & 0xFF000000));
        }
        private uint SwapUInt16(ushort n)
        {
            return (uint)((n >> 8) | (n << 8));
        }

        private string UIntToString(uint uint_) => Regex.Replace(Convert.ToString(uint_, 16).PadLeft(8, '0'), ".{2}", "$0 ").Trim();

        private string UShortToString(ushort ushort_) => Regex.Replace(Convert.ToString(ushort_, 16).PadLeft(4, '0'), ".{2}", "$0 ").Trim();
        private string ByteToString(byte byte_) => Convert.ToString(byte_, 16).PadLeft(2, '0');


    }

    public partial class Form1 : Form
    {
        public string _defaultFilePath = @"C:\Users\Vincent\source\repos\Midi Visualizer\Midi Visualizer\MidiFiles\C Dur Scale.mid";

        public Form1()
        {
            AllocConsole();

            InitializeComponent();

            filePath_tb.Text = _defaultFilePath;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void FileSelectBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                RestoreDirectory = true,
                Title = "Browse MIDI files",
                DefaultExt = "mid",
                CheckFileExists = true,
                CheckPathExists = true
            };
            openFileDialog1.ShowDialog();

            string filePath = openFileDialog1.FileName;

            filePath_tb.Text = filePath;
            ParseMidiFile(filePath);
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            ParseMidiFile(filePath_tb.Text);
        }

        private bool ParseMidiFile(String filePath)
        {
            MidiFile midiFile = new MidiFile();
            if (!midiFile.LoadFile(filePath))
                return false;
            return true;
        }
    }
}

