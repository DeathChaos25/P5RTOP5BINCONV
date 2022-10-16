using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amicitia.IO.Binary;

namespace P5RTOP5BINCONV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("P5RTOP5BINCONV [path to (supported file)]; currently supported Formats:\n- .CLT (crowd NPC files)\n- FNT/ENT .bin file (FNPC)\n- .bin file (icon parts, texpack, roadmap, weather binary or CorpTBL)\n-" +
                    " shdPersona/Enemy pdd file\n- fldBGMCnd ftd\n- .HTB files (field pac hit trigger files)\n- .spd files (recalculates tex location)\n- speaker .dat file\n- .ENV files\n");
            }
            else
            {
                FileInfo arg0 = new FileInfo(args[0]);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (arg0.Extension == ".CLT")
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");
                    List<UInt32> StringPointers = new List<UInt32>();

                    using (BinaryObjectReader P5RCLTFile = new BinaryObjectReader(args[0], Endianness.Big, Encoding.GetEncoding(932)))
                    {
                        uint CLTVersion = P5RCLTFile.ReadUInt32();
                        var P5ROnlyField = 0;

                        if (CLTVersion < 386008576) // P5R Version 0x17020600 
                        {
                            Console.WriteLine("Invalid CLT Version, must be 0x17020600 or higher");
                            return;
                        }

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                        System.IO.Directory.CreateDirectory(savePath);
                        savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".CLT");

                        //Console.WriteLine($"Saving file to{savePath}");

                        using (BinaryObjectWriter NewCLTFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                        {
                            NewCLTFile.Write(353440256); // P5 Version 0x15111200

                            int Entry_Num = P5RCLTFile.ReadInt32();
                            NewCLTFile.Write(Entry_Num);

                            // Write First Entry Type
                            NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // Block_Type
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field04
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field08
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field0C
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field10
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field14
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field16
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field18
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field1A
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field1C
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field20
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field24
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field26
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field28
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field2A
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field2C
                            NewCLTFile.Write(P5RCLTFile.ReadSingle()); // Field30
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field32
                            NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // Field34
                            // End Write

                            if (Entry_Num > 0)
                            {
                                //float unk18 = P5RCLTFile.ReadSingle();
                                NewCLTFile.Write(P5RCLTFile.ReadSingle());

                                for (int i = 0; i < Entry_Num - 1; i++)
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field00;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field02;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field04;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field06;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 field08;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field0A;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field0C;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field0E;
                                    if (CLTVersion >= 0x15111200)
                                    {
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field10;
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field12;
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field14;
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field16;
                                    }
                                    int CrowdUnitAmount = P5RCLTFile.ReadInt32();
                                    NewCLTFile.Write(CrowdUnitAmount); // u32 CrowdUnitAmount<name= "Crowd Unit Num" >;

                                    if (CrowdUnitAmount < 6)
                                    {
                                        CrowdUnitAmount = 5;
                                    }

                                    for (int j = 0; j < CrowdUnitAmount; j++)
                                    {
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16());
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16());
                                    }
                                    // Additional Data, "Type 1"
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Quat_Rotation_X;
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Quat_Rotation_Y;
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Quat_Rotation_Z;
                                    ushort cond = P5RCLTFile.ReadUInt16();
                                    NewCLTFile.Write(cond); // u16 Field0C<comment= "Unknown Conditional" >;
                                    Console.WriteLine(cond);
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field0E;
                                    if (CLTVersion >= 0x17020600) // P5R only, skip
                                    {
                                        P5ROnlyField = P5RCLTFile.ReadUInt16();
                                        P5ROnlyField = P5RCLTFile.ReadUInt16();
                                    }
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field10;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Spawn_Num;
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Field14;

                                    uint Conditional = P5RCLTFile.ReadUInt32();
                                    if (Conditional == 5)
                                    {
                                        Conditional = 2;
                                    }
                                    NewCLTFile.Write(Conditional); // u32 Unk_Cnd2<comment= "Unknown Conditional" >;

                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field1A;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field1C;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field1E;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field20;
                                    ushort NumOfPathingNodes2 = P5RCLTFile.ReadUInt16();
                                    NewCLTFile.Write(NumOfPathingNodes2); // u16 Num_of_Pathing_Nodes<comment= "First node is spawn point." >;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field24;
                                    for (int j = 0; j < NumOfPathingNodes2; j++)
                                    {
                                        NewCLTFile.Write(P5RCLTFile.ReadSingle());
                                        NewCLTFile.Write(P5RCLTFile.ReadSingle());
                                        NewCLTFile.Write(P5RCLTFile.ReadSingle());
                                    }
                                    // additional data end
                                }
                                // Last Type Start
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field00;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field02;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field04;
                                NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field06;
                                if (CLTVersion >= 0x15111200)
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field08;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field0A;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field0C;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field0E;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field10;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field12;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field14;
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field16;
                                }
                                // crowd unit data
                                int CrowdUnitAmount2 = P5RCLTFile.ReadInt32();
                                NewCLTFile.Write(CrowdUnitAmount2); // u32 CrowdUnitAmount<name= "Crowd Unit Num" >;

                                if (CrowdUnitAmount2 < 6)
                                {
                                    CrowdUnitAmount2 = 5;
                                }

                                for (int j = 0; j < CrowdUnitAmount2; j++)
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16());
                                    NewCLTFile.Write(P5RCLTFile.ReadInt16());
                                }
                                // crowd unit data end

                                // Additional Data, "Type 1"
                                NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Quat_Rotation_X;
                                NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Quat_Rotation_Y;
                                NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Quat_Rotation_Z;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field0C<comment= "Unknown Conditional" >;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field0E;
                                if (CLTVersion >= 0x17020600) // P5R only, skip
                                {
                                    P5ROnlyField = P5RCLTFile.ReadUInt16();
                                    P5ROnlyField = P5RCLTFile.ReadUInt16();
                                }
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field10;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Spawn_Num;
                                NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Field14;

                                uint Conditional2 = P5RCLTFile.ReadUInt32();
                                if (Conditional2 == 5)
                                {
                                    Conditional2 = 2;
                                }
                                NewCLTFile.Write(Conditional2); // u32 Unk_Cnd2<comment= "Unknown Conditional" >;

                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field1A;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field1C;
                                NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field1E;
                                NewCLTFile.Write(P5RCLTFile.ReadInt16()); // s16 Field20;
                                ushort NumOfPathingNodes = P5RCLTFile.ReadUInt16();
                                NewCLTFile.Write(NumOfPathingNodes); // u16 Num_of_Pathing_Nodes<comment= "First node is spawn point." >;
                                NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Field24;
                                for (int j = 0; j < NumOfPathingNodes; j++)
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle());
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle());
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle());
                                }
                                // additional data end

                                if (P5RCLTFile.Position >= P5RCLTFile.Length)
                                {
                                    Console.WriteLine("File Converted Successfully!");
                                    return;
                                }

                                uint EntryNum = 0;

                                if (CLTVersion == 0x15052600)
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // u32 Unk1;

                                    ushort EntryNum1 = P5RCLTFile.ReadUInt16();
                                    NewCLTFile.Write(EntryNum1); // u16 Entry_Num2;
                                    EntryNum = EntryNum1;

                                    NewCLTFile.Write(P5RCLTFile.ReadUInt16()); // u16 Unk2;
                                }
                                else
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Unk1;
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Unk2;

                                    uint Entrynum2 = P5RCLTFile.ReadUInt32();
                                    NewCLTFile.Write(Entrynum2); // u32 Entry_Num2;
                                    EntryNum = Entrynum2;

                                    NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // u32 Unk4;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // u32 Unk5;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // u32 Unk6;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // u32 Unk7;
                                    NewCLTFile.Write(P5RCLTFile.ReadUInt32()); // u32 Unk8;
                                }
                                for (int d = 0; d < EntryNum; d++)
                                {
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 X_Coordinate;
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Y_Coordinate;
                                    NewCLTFile.Write(P5RCLTFile.ReadSingle()); // f32 Z_Coordinate;
                                }

                                if (P5RCLTFile.Position >= P5RCLTFile.Length)
                                {
                                    Console.WriteLine("File Converted Successfully!");
                                    return;
                                }
                            }
                        }

                    }
                }
                else if (arg0.Extension == ".bin")
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    if (arg0.Name.Contains("corptbl"))
                    {
                        List<UInt32> StringPointers = new List<UInt32>();

                        using (BinaryObjectReader P5RcorptblFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                        {

                            var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                            System.IO.Directory.CreateDirectory(savePath);
                            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".bin");

                            using (BinaryObjectWriter NewcorptblFile = new BinaryObjectWriter(savePath, Endianness.Little, Encoding.GetEncoding(932)))
                            {
                                for (int i = 0; i < P5RcorptblFile.Length / 188; i++)
                                {
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // s16 FBN_NPC_ID;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // s16 Field02;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // s16 Field04;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // s16 Field06;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field08;

                                    short BitFlag = 0;
                                    short FlagSection = 0;

                                    for (int j = 1; j <= 22; j++)
                                    {
                                        BitFlag = P5RcorptblFile.ReadInt16();
                                        FlagSection = P5RcorptblFile.ReadInt16();

                                        //Console.WriteLine($"FlagSection is {FlagSection}");

                                        if (FlagSection != -1 && BitFlag != -1)
                                        {
                                            BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                            FlagSection = 0;
                                        }

                                        NewcorptblFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                        NewcorptblFile.Write(FlagSection); // s16 Bitflag_related;
                                    }

                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // u16 Field_MajorId;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadByte()); // u8 Field_MinorId
                                    NewcorptblFile.Write(P5RcorptblFile.ReadByte()); // u8 Field_SubId;

                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // u16 NPC_ResourceId;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field6A;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // u16 Field6E;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // u32 Field70;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field74;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // s16 Field78;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt16()); // s16 Field7A;
                                    var skip1 = P5RcorptblFile.ReadInt32(); // u32 P5RNew<hidden= true >;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadSingle()); // f32 Field7C;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadSingle()); // f32 Field80;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadSingle()); // f32 Field84;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadSingle()); // f32 Field88;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadSingle()); // f32 Field8C;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field90;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field94;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field98;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 Field9C;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // f32 FieldA0;
                                    NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 FieldA4;
                                    //NewcorptblFile.Write(P5RcorptblFile.ReadInt32()); // s32 ExtraBitFlag1;

                                    BitFlag = P5RcorptblFile.ReadInt16();
                                    FlagSection = P5RcorptblFile.ReadInt16();

                                    //Console.WriteLine($"FlagSection is {FlagSection}");

                                    if (FlagSection != -1 && BitFlag != -1)
                                    {
                                        BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                        FlagSection = 0;
                                    }

                                    NewcorptblFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                    NewcorptblFile.Write(FlagSection); // s16 Bitflag_related;

                                    var skip2 = P5RcorptblFile.ReadInt32(); // 

                                    var ExtraFlag2 = P5RcorptblFile.ReadInt32(); // 
                                    var FieldB0 = P5RcorptblFile.ReadInt32(); // 

                                    NewcorptblFile.Write(FieldB0);
                                    NewcorptblFile.Write(ExtraFlag2); // fix order
                                }
                                while (P5RcorptblFile.Position < P5RcorptblFile.Length) //write remainder of file without going past EOF
                                {
                                    NewcorptblFile.Write(P5RcorptblFile.ReadByte());
                                }
                            }
                        }
                    }
                    else if (arg0.Name.Contains("icon") || arg0.Name.Contains("parts")) //Thank you DC for the copy-paste Material
                    {
                        List<UInt32> StringPointers = new List<UInt32>();

                        using (BinaryObjectReader P5RroadmapFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                        {

                            var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                            System.IO.Directory.CreateDirectory(savePath);
                            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".bin");

                            using (BinaryObjectWriter NewroadmapFile = new BinaryObjectWriter(savePath, Endianness.Little, Encoding.GetEncoding(932)))
                            {
                                for (int i = 0; i < P5RroadmapFile.Length / 72; i++)
                                {
                                    NewroadmapFile.Write(P5RroadmapFile.ReadInt32()); // s32 Map/Icon Id;
                                    for (int j = 0; j < 6; j++)
                                    {
                                        float resolutionThing = P5RroadmapFile.ReadSingle();
                                        resolutionThing = resolutionThing * 0.6666666666666667f;
                                        NewroadmapFile.Write(resolutionThing);
                                    }

                                    short BitFlag = 0;
                                    short FlagSection = 0;

                                    BitFlag = P5RroadmapFile.ReadInt16();
                                    FlagSection = P5RroadmapFile.ReadInt16();

                                    if (FlagSection != -1 && BitFlag != -1)
                                    {
                                        BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                        FlagSection = 0;
                                    }

                                    NewroadmapFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                    NewroadmapFile.Write(FlagSection); // s16 Bitflag_related;
                                    NewroadmapFile.Write(P5RroadmapFile.ReadInt32()); //Field20
                                    for (int k = 0; k < 8; k++)
                                    {
                                        float resolutionThing = P5RroadmapFile.ReadSingle();
                                        float convertedResolutionThing = resolutionThing * 0.6666666666666667f;
                                        if (arg0.Name.Contains("parts"))
                                        {
                                            NewroadmapFile.Write(convertedResolutionThing);
                                        }
                                        else
                                        {
                                            NewroadmapFile.Write(resolutionThing);
                                        }

                                    }
                                    NewroadmapFile.Write(P5RroadmapFile.ReadInt32()); //Field44

                                }
                                while (P5RroadmapFile.Position < P5RroadmapFile.Length) //write remainder of file without going past EOF
                                {
                                    NewroadmapFile.Write(P5RroadmapFile.ReadByte());
                                }
                            }
                        }
                    }
                    else if (arg0.Name.Contains("texpack"))
                    {
                        List<UInt32> StringPointers = new List<UInt32>();

                        using (BinaryObjectReader P5RroadmapFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                        {

                            var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                            System.IO.Directory.CreateDirectory(savePath);
                            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".bin");

                            using (BinaryObjectWriter NewroadmapFile = new BinaryObjectWriter(savePath, Endianness.Little, Encoding.GetEncoding(932)))
                            {
                                for (int i = 0; i < P5RroadmapFile.Length / 72; i++)
                                {
                                    for (int j = 0; j < 17; j++)
                                    {
                                        if (j == 3 || j == 12)
                                        {
                                            short BitFlag = 0;
                                            short FlagSection = 0;

                                            BitFlag = P5RroadmapFile.ReadInt16();
                                            FlagSection = P5RroadmapFile.ReadInt16();

                                            if (FlagSection != -1 && BitFlag != -1)
                                            {
                                                BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                                FlagSection = 0;
                                            }

                                            NewroadmapFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                            NewroadmapFile.Write(FlagSection); // s16 Bitflag_related;
                                        }
                                        else if (j == 17)
                                        {
                                            P5RroadmapFile.ReadUInt32();
                                        }
                                        else
                                        {
                                            NewroadmapFile.Write(P5RroadmapFile.ReadUInt32());
                                        }
                                    }
                                    P5RroadmapFile.ReadUInt32();
                                }

                                while (P5RroadmapFile.Position < P5RroadmapFile.Length) //write remainder of file without going past EOF
                                {
                                    NewroadmapFile.Write(P5RroadmapFile.ReadByte());
                                }
                            }
                        }
                    }
                    else if (arg0.Name.Contains("roadmap"))
                    {
                        List<UInt32> StringPointers = new List<UInt32>();

                        using (BinaryObjectReader P5RroadmapFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                        {

                            var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                            System.IO.Directory.CreateDirectory(savePath);
                            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".bin");

                            using (BinaryObjectWriter NewroadmapFile = new BinaryObjectWriter(savePath, Endianness.Little, Encoding.GetEncoding(932)))
                            {
                                for (int i = 0; i < P5RroadmapFile.Length / 16; i++)
                                {
                                    NewroadmapFile.Write(P5RroadmapFile.ReadUInt16()); // Field Major Id
                                    NewroadmapFile.Write(P5RroadmapFile.ReadUInt16()); // Field Minor Id

                                    for (int j = 0; j < 2; j++)
                                    {
                                        short mapcoord = P5RroadmapFile.ReadInt16();
                                        float fmapcoord = Convert.ToSingle(mapcoord);
                                        fmapcoord = (MathF.Round((float)(fmapcoord * 0.6666666666666667f)));
                                        Console.WriteLine(fmapcoord);
                                        Console.ReadKey();
                                        NewroadmapFile.Write((short)mapcoord);
                                    }

                                    NewroadmapFile.Write(P5RroadmapFile.ReadUInt16()); // Texpack entry minus texpack index
                                    NewroadmapFile.Write(P5RroadmapFile.ReadUInt16()); // Field0a
                                    NewroadmapFile.Write(P5RroadmapFile.ReadUInt32()); // Minimap mode
                                }

                                while (P5RroadmapFile.Position < P5RroadmapFile.Length) //write remainder of file without going past EOF
                                {
                                    NewroadmapFile.Write(P5RroadmapFile.ReadByte());
                                }
                            }
                        }
                    }
                    else if (arg0.Name.Contains("baiu") || arg0.Name.Contains("gouu") || arg0.Name.Contains("kanpa")
                        || arg0.Name.Contains("pollen") || arg0.Name.Contains("rain") || arg0.Name.Contains("snow"))
                    {
                        List<UInt32> StringPointers = new List<UInt32>();

                        using (BinaryObjectReader P5REnvFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                        {

                            var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                            System.IO.Directory.CreateDirectory(savePath);
                            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".bin");

                            using (BinaryObjectWriter NewEnvFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                            {
                                int header = P5REnvFile.ReadInt32();
                                uint version = P5REnvFile.ReadUInt32();
                                if (version == 34148883)
                                {
                                    Console.WriteLine("Invalid Version " + version);
                                    return;
                                }

                                NewEnvFile.Write(header);
                                NewEnvFile.Write(319949058);
                                NewEnvFile.Write(P5REnvFile.ReadUInt32());
                                NewEnvFile.Write(P5REnvFile.ReadUInt32());
                                for (int i = 0; i < ((P5REnvFile.Length - 16) / 212); i++)
                                {
                                    for (int j = 0; j < 132; j++)
                                    {
                                        NewEnvFile.Write(P5REnvFile.ReadByte());
                                    }
                                    NewEnvFile.Write(P5REnvFile.ReadSingle());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadSingle());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt32());
                                    NewEnvFile.Write(P5REnvFile.ReadSingle());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());
                                    NewEnvFile.Write(P5REnvFile.ReadUInt16());

                                    for (int j = 0; j < 10; j++)
                                    {
                                        NewEnvFile.Write(P5REnvFile.ReadSingle());
                                    }

                                    P5REnvFile.ReadUInt32();
                                }
                            }
                        }
                    }
                    else
                    {

                        List<UInt32> StringPointers = new List<UInt32>();

                        using (BinaryObjectReader P5RFNTFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                        {

                            var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                            System.IO.Directory.CreateDirectory(savePath);
                            savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".bin");

                            using (BinaryObjectWriter NewFNTFile = new BinaryObjectWriter(savePath, Endianness.Little, Encoding.GetEncoding(932)))
                            {
                                for (int i = 0; i < P5RFNTFile.Length / 0x74; i++)
                                {
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 FBN_NPC_ID;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Resource_NPC_ID;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x04;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x06;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x08;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x0A;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 BF_Procedure_Index;

                                    short BitFlag = P5RFNTFile.ReadInt16();
                                    short FlagSection = P5RFNTFile.ReadInt16();

                                    //Console.WriteLine($"FlagSection is {FlagSection}");

                                    if (FlagSection != -1 && BitFlag != -1)
                                    {
                                        BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                        FlagSection = 0;
                                    }

                                    NewFNTFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                    NewFNTFile.Write(FlagSection); // s16 Bitflag_related;

                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x14;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x18;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x1C;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x20;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x24;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x26;
                                    int P5RIgnore = P5RFNTFile.ReadInt32(); // s32 Field_New<hidden= true >;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 NPC_Interaction_Type;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x2C;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x30;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x34;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x38;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x3C;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x40;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x44;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x48;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x4C;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 PossiblyProcedureIndex2;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x54;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x56;


                                    BitFlag = P5RFNTFile.ReadInt16(); // s16 Bitflag_enabled;
                                    FlagSection = P5RFNTFile.ReadInt16(); // s16 flagsection;

                                    if (FlagSection != -1 && BitFlag != -1)
                                    {
                                        BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                        FlagSection = 0;
                                    }

                                    NewFNTFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                    NewFNTFile.Write(FlagSection); // s16 Bitflag_related;

                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Field_0x5C;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Prompt_Name_Type;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 NPC_Prompt_Name;
                                    NewFNTFile.Write(P5RFNTFile.ReadSingle()); // f32 Field_0x64;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x68;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt32()); // s32 Field_0x6C;
                                }
                            }
                        }
                    }// fnpc fnt file
                }
                else if (arg0.Extension == ".pdd")
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    using (BinaryObjectReader P5RPDDFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                    {

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(arg0.FullName) + "_converted" + Path.GetExtension(arg0.FullName));

                        using (BinaryObjectWriter NewPDDFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                        {
                            NewPDDFile.Write(P5RPDDFile.ReadUInt32()); // File header
                            NewPDDFile.Write(P5RPDDFile.ReadUInt32());
                            NewPDDFile.Write(P5RPDDFile.ReadUInt32());
                            NewPDDFile.Write(P5RPDDFile.ReadUInt32());

                            for (int i = 0; i < (P5RPDDFile.Length - 0x10) / 0x4c; i++)
                            {
                                NewPDDFile.Write(P5RPDDFile.ReadUInt16()); // ushort Field00; // 0001 - enabled
                                NewPDDFile.Write(P5RPDDFile.ReadUInt16()); // ushort Field02;

                                NewPDDFile.Write(P5RPDDFile.ReadUInt16()); // ushort Field04;
                                NewPDDFile.Write(P5RPDDFile.ReadUInt16()); // ushort Field06;
                                NewPDDFile.Write(P5RPDDFile.ReadUInt16()); // ushort Field08;
                                NewPDDFile.Write(P5RPDDFile.ReadUInt16()); // ushort Field0A;

                                NewPDDFile.Write(P5RPDDFile.ReadSingle() * 2.4f); // Vec3f View; offset view to account for resolution difference
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec3f View;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec3f View;

                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec3f Rot;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec3f Rot;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec3f Rot;

                                NewPDDFile.Write(P5RPDDFile.ReadUInt32()); // uint Field24; // 0
                                NewPDDFile.Write(P5RPDDFile.ReadUInt32()); // uint Field28; // 0

                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // float Field2C;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // float Field30;

                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // float Scale;

                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec4f Light;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec4f Light;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec4f Light;
                                NewPDDFile.Write(P5RPDDFile.ReadSingle()); // Vec4f Light;

                                NewPDDFile.Write(P5RPDDFile.ReadUInt32()); // uint LightRGBA;
                            }
                        }

                    }
                }
                else if (arg0.Extension == ".ftd" && arg0.Name.Contains("fldBGMCnd"))
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    using (BinaryObjectReader P5RFTDFile = new BinaryObjectReader(args[0], Endianness.Big, Encoding.GetEncoding(932)))
                    {

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                        System.IO.Directory.CreateDirectory(savePath);
                        savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".ftd");

                        using (BinaryObjectWriter NewFTDFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                        {
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32()); // File header
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());

                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());


                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());
                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());

                            var numOfEntries = P5RFTDFile.ReadUInt32();
                            NewFTDFile.Write(numOfEntries);

                            NewFTDFile.Write(P5RFTDFile.ReadUInt32());

                            for (int i = 0; i < numOfEntries; i++)
                            {
                                NewFTDFile.Write(P5RFTDFile.ReadInt16()); // int16 fldMajorID;
                                NewFTDFile.Write(P5RFTDFile.ReadInt16()); // int16 fldMinorID;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // MonthType monthStart;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // byte dayStart;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // MonthType monthEnd;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // byte dayEnd;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // byte Unk1;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // WeatherType Weather;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // byte unk_Flag;
                                NewFTDFile.Write(P5RFTDFile.ReadByte()); // byte FF;
                                NewFTDFile.Write(P5RFTDFile.ReadInt16()); // int16 unk;
                                NewFTDFile.Write(P5RFTDFile.ReadInt16()); // MusicID musicID;
                                //flag tomfoolery
                                short FlagSection = P5RFTDFile.ReadInt16();
                                short BitFlag = P5RFTDFile.ReadInt16();

                                //Console.WriteLine($"FlagSection is {FlagSection}");

                                if (FlagSection != -1 && BitFlag != -1)
                                {
                                    BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                    FlagSection = 0;
                                }

                                NewFTDFile.Write(FlagSection); // s16 Bitflag_related;
                                NewFTDFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                            }
                        }

                    }
                }
                else if (arg0.Extension == ".HTB")
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    using (BinaryObjectReader P5RHBNFile = new BinaryObjectReader(args[0], Endianness.Big, Encoding.GetEncoding(932)))
                    {

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                        System.IO.Directory.CreateDirectory(savePath);
                        savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".HTB");

                        using (BinaryObjectWriter NewHBNFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                        {
                            NewHBNFile.Write(P5RHBNFile.ReadUInt32()); // File header
                            NewHBNFile.Write(P5RHBNFile.ReadUInt32());
                            int filesize = P5RHBNFile.ReadInt32();
                            NewHBNFile.Write(filesize);
                            NewHBNFile.Write(P5RHBNFile.ReadUInt32());

                            int numOfEntries = P5RHBNFile.ReadInt32();
                            NewHBNFile.Write(numOfEntries);
                            NewHBNFile.Write(P5RHBNFile.ReadUInt32());
                            NewHBNFile.Write(P5RHBNFile.ReadUInt32());
                            NewHBNFile.Write(P5RHBNFile.ReadUInt32());

                            NewHBNFile.Endianness = Endianness.Little;
                            P5RHBNFile.Endianness = Endianness.Little;

                            for (int i = 0; i < numOfEntries; i++)
                            {
                                //flag tomfoolery
                                short BitFlag = 0;
                                short FlagSection = 0;

                                for (int j = 0; j < 6; j++)
                                {
                                    BitFlag = P5RHBNFile.ReadInt16();
                                    FlagSection = P5RHBNFile.ReadInt16();

                                    //Console.WriteLine($"FlagSection is {FlagSection}");

                                    if (FlagSection != -1 && BitFlag != -1)
                                    {
                                        BitFlag = ReturnConvertedFlag(FlagSection, BitFlag);
                                        FlagSection = 0;
                                    }

                                    NewHBNFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                    NewHBNFile.Write(FlagSection); // s16 Bitflag_related;
                                }
                                for (int j = 0; j < 9; j++)
                                {
                                    NewHBNFile.Write(P5RHBNFile.ReadUInt32()); // we dont care to modify the rest of the block
                                }
                            }
                            long targetPadding = (int)((filesize - NewHBNFile.Position % filesize) % filesize);
                            if (targetPadding > 0)
                            {
                                for (int j = 0; j < targetPadding; j++)
                                {
                                    NewHBNFile.WriteByte((byte)0);
                                }
                            }
                        }

                    }
                }
                else if (arg0.Extension == ".spd")
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    using (BinaryObjectReader P5RSPDFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                    {

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                        System.IO.Directory.CreateDirectory(savePath);
                        savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".spd");

                        using (BinaryObjectWriter NewSPDFile = new BinaryObjectWriter(savePath, Endianness.Little, Encoding.GetEncoding(932)))
                        {
                            NewSPDFile.Write(P5RSPDFile.ReadUInt32()); // File header
                            NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                            int filesize = P5RSPDFile.ReadInt32();
                            NewSPDFile.Write(filesize);
                            NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                            NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                            short textureCount = P5RSPDFile.ReadInt16();
                            NewSPDFile.Write(textureCount);

                            ushort entryCount = P5RSPDFile.ReadUInt16();
                            NewSPDFile.Write(entryCount);
                            NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                            NewSPDFile.Write(P5RSPDFile.ReadUInt32());

                            for (int j = 0; j < textureCount; j++)
                            {
                                for (int k = 0; k < 12; k++)
                                {
                                    NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                                }
                            }

                            for (int i = 0; i < entryCount; i++)
                            {
                                for (int l = 0; l < 8; l++)
                                {
                                    NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                                }
                                float scale;
                                if (arg0.Name.Contains("4K"))
                                {
                                    scale = 2f;
                                }
                                else
                                {
                                    scale = 0.6666666666666667f;
                                }
                                int spdX1 = P5RSPDFile.ReadInt32();
                                NewSPDFile.Write(CheckAndDivideSPD(spdX1, scale));
                                int spdY1 = P5RSPDFile.ReadInt32();
                                NewSPDFile.Write(CheckAndDivideSPD(spdY1, scale));
                                int spdX2 = P5RSPDFile.ReadInt32();
                                NewSPDFile.Write(CheckAndDivideSPD(spdX2, scale));
                                int spdY2 = P5RSPDFile.ReadInt32();
                                NewSPDFile.Write(CheckAndDivideSPD(spdY2, scale));

                                NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                                NewSPDFile.Write(P5RSPDFile.ReadUInt32());

                                int spdXScale = P5RSPDFile.ReadInt32();
                                NewSPDFile.Write(CheckAndDivideSPD(spdXScale, scale));
                                int spdYScale = P5RSPDFile.ReadInt32();
                                NewSPDFile.Write(CheckAndDivideSPD(spdYScale, scale));

                                for (int m = 0; m < 24; m++)
                                {
                                    NewSPDFile.Write(P5RSPDFile.ReadUInt32());
                                }

                            }
                            long remainingFileLength = P5RSPDFile.Length - NewSPDFile.Length;
                            for (int i = 0; i < remainingFileLength; i++)
                            {
                                NewSPDFile.Write(P5RSPDFile.ReadByte());
                            }
                        }

                    }
                }
                else if (arg0.Extension == ".PCD")
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    using (BinaryObjectReader P5RPCDFile = new BinaryObjectReader(args[0], Endianness.Little, Encoding.GetEncoding(932)))
                    {

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                        System.IO.Directory.CreateDirectory(savePath);
                        savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + ".PCD");

                        P5RPCDFile.At(4, SeekOrigin.Begin);
                        int version = P5RPCDFile.ReadInt32();
                        if (version == 67108865)
                        {
                            Console.WriteLine("Invalid Version");
                            return;
                        }
                        P5RPCDFile.At(0, SeekOrigin.Begin);
                        using (BinaryObjectWriter NewPCDFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                        {
                            NewPCDFile.Write(1179665220); // File header (FPCD)
                            P5RPCDFile.ReadInt32();
                            byte pcdVersion = P5RPCDFile.ReadByte(); //5 removes the first float, 6 removes the first 3
                            P5RPCDFile.ReadByte();
                            P5RPCDFile.ReadInt16();
                            NewPCDFile.Write(16777220); // 01 00 00 04 (Version number)?

                            for (int i = 0; i < 12; i++)
                            {
                                int value = P5RPCDFile.ReadInt32();
                                if (i == 1)
                                {
                                    NewPCDFile.Write((int)P5RPCDFile.Length + 4);
                                }
                                else
                                {
                                    NewPCDFile.Write(value);
                                }
                            }
                            for (int j = 0; j < (P5RPCDFile.Length - 56) / 4; j++)
                            {
                                Single valuef = P5RPCDFile.ReadSingle();
                                if ((j != 8 || pcdVersion == 5) && j != 6 && (j != 7 || pcdVersion == 5))
                                {
                                    NewPCDFile.Write(valuef);
                                }
                            }

                        }

                    }
                }
                else if ((arg0.Extension == ".ENV"))
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    using (BinaryObjectReader P5REnvFile = new BinaryObjectReader(args[0], Endianness.Big, Encoding.GetEncoding(932)))
                    {

                        var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "ConvertedOutput");
                        System.IO.Directory.CreateDirectory(savePath);
                        savePath = Path.Combine(savePath, Path.GetFileNameWithoutExtension(arg0.FullName) + Path.GetExtension(arg0.Extension));

                        using (BinaryObjectWriter NewEnvFile = new BinaryObjectWriter(savePath, Endianness.Big, Encoding.GetEncoding(932)))
                        {
                            uint Magic = P5REnvFile.ReadUInt32(); //Magic
                            uint version = P5REnvFile.ReadUInt32();
                            if (version != 17846416 && version != 17846528)
                            {
                                Console.WriteLine("Invalid Env Version: " + version.ToString("X"));
                                return;
                            }
                            NewEnvFile.Write(Magic);
                            NewEnvFile.Write(17846384); //Version
                            P5REnvFile.ReadUInt32();
                            NewEnvFile.Write((int)2); //File Type
                            NewEnvFile.Write(P5REnvFile.ReadUInt32()); //Field0C
                            NewEnvFile.Write(P5REnvFile.ReadInt16()); //Field10
                            //Field Model Section
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Diffuse Red
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Diffuse Green
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Diffuse Blue
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Diffuse Alpha

                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Ambient Red
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Ambient Green
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Ambient Blue
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Ambient Alpha

                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Specular Red
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Specular Green
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Specular Blue
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Specular Alpha

                            for (int i = 0; i < 232 / 4; i++)
                            {
                                NewEnvFile.Write(P5REnvFile.ReadSingle());
                            }

                            //Character Model Section

                            NewEnvFile.Write(P5REnvFile.ReadInt16()); //Field10
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Diffuse Red
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Diffuse Green
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Diffuse Blue
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Diffuse Alpha

                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Ambient Red
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Ambient Green
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Ambient Blue
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Ambient Alpha

                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Specular Red
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Specular Green
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Specular Blue
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Specular Alpha

                            for (int i = 0; i < 109; i++)
                            {
                                NewEnvFile.Write(P5REnvFile.ReadByte()); //Rest of the Character Model Section and Fog Section
                            }

                            //Lighting Section

                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Enable Graphical Output
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Enable Bloom
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Enable Glare
                            NewEnvFile.Write((byte)0);
                            P5REnvFile.ReadUInt32();
                            if (version == 17846528)
                            {
                                P5REnvFile.ReadUInt32();
                            }
                            else
                            {
                                P5REnvFile.ReadByte();
                                P5REnvFile.ReadUInt16();
                            }
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Bloom Amount? * 0.75
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Bloom Detail?
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Bloom White Level?
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Bloom Dark Level?
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Glare Sensivity?
                            int loopCount;
                            if (version == 17846528)
                            {
                                loopCount = 88;
                            }
                            else
                            {
                                loopCount = 80;
                            }
                            for (int i = 0; i < loopCount / 4; i++)
                            {
                                P5REnvFile.ReadSingle(); //Royal Only Section
                            }
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Glare Length
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Glare Chromatic Abberation
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Glare Direction
                            NewEnvFile.Write(P5REnvFile.ReadInt32()); //Glare Mode

                            //Unknown Section

                            NewEnvFile.Write(P5REnvFile.ReadByte());
                            P5REnvFile.ReadUInt32();
                            NewEnvFile.Write(P5REnvFile.ReadSingle() / 4); //Field1F2
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field1F6
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field1FA
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Field1FD
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field1FE
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field202
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field206
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field20A
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field20E
                            NewEnvFile.Write(P5REnvFile.ReadUInt32()); //Field212
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Field216
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field217
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field21B
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field21F
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field223
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field227
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Field22B

                            //Field Shadow Section

                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Shadow Far Clip
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field294
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field298
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field29C
                            NewEnvFile.Write(P5REnvFile.ReadUInt32()); //Field2A0
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Shadow Near Clip
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Field Shadow Darkness
                            NewEnvFile.Write(P5REnvFile.ReadUInt32()); 

                            //Unknown Royal Section

                            for (int i = 0; i < 4; i++)
                            {
                                P5REnvFile.ReadSingle();
                            }

                            //Color Grading Section

                            NewEnvFile.Write(P5REnvFile.ReadByte());
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Cyan
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Magenta
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Yellow
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Dodge
                            NewEnvFile.Write(P5REnvFile.ReadSingle()); //Burn

                            //Second Unknown Section

                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadByte());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());

                            //Physics Section

                            NewEnvFile.Write(P5REnvFile.ReadByte());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadByte());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());
                            NewEnvFile.Write(P5REnvFile.ReadSingle());

                            //Sky Coloring

                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Red
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Green
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Blue
                            NewEnvFile.Write(P5REnvFile.ReadByte()); //Alpha

                            NewEnvFile.Write(P5REnvFile.ReadUInt32());
                        }

                    }
                }
                else Console.WriteLine("https://youtu.be/Uuw6PdJvW88");
            }
        }

        static int CheckAndDivideSPD(int spdParam, float scale)
        {
            if (spdParam > 1)
            {
                float spdParamf = Convert.ToSingle(spdParam);
                spdParamf *= scale;
                return Convert.ToInt32(spdParamf);
            }
            return spdParam;
        }

        static short ReturnConvertedFlag( int FlagSection, int BitFlag)
        {
            if ( FlagSection == 0 )
            {
                if ( BitFlag > 2048 )

                {
                    Console.WriteLine($"Warning: Flag Overflow from Section 0! Max: 2048 - Current:{BitFlag} - Overflow by { BitFlag - 2048 }");
                }
            }
            else if (FlagSection == 0x1000)
            {
                if (BitFlag > 2048)
                {
                    Console.WriteLine($"Warning: Flag Overflow from Section 1! Max: 2048 - Current:{BitFlag} - Overflow by { BitFlag - 2048 }");
                    BitFlag -= 2048; // section size
                    BitFlag += 8959; // p5 max
                }
                else BitFlag += 2048;
            }
            else if (FlagSection == 0x2000)
            {
                if (BitFlag > 4096)
                {
                    Console.WriteLine($"Warning: Flag Overflow from Section 2! Max: 4096 - Current:{BitFlag} - Overflow by { BitFlag - 4096 }"); BitFlag -= 4096;  // section size
                    BitFlag += 394; // section 1 overflow
                    BitFlag += 8959; // p5 max
                }
                else BitFlag += 4096;
            }
            else if (FlagSection == 0x3000)
            {
                if (BitFlag > 256)
                {
                    Console.WriteLine($"Warning: Flag Overflow from Section 3! Max: 256 - Current:{BitFlag} - Overflow by { BitFlag - 256 }");
                    BitFlag -= 256; // section size
                    BitFlag += 394; // section 1 overflow
                    BitFlag += 1024; // section 2 overflow
                    BitFlag += 8959; // p5 max
                }
                else BitFlag += 8192;
            }
            else if (FlagSection == 0x4000)
            {
                if (BitFlag > 256)
                {
                    Console.WriteLine($"Warning: Flag Overflow from Section 4! Max: 256 - Current:{BitFlag} - Overflow by { BitFlag - 256 }");
                    BitFlag -= 256; // section size
                    BitFlag += 394; // section 1 overflow
                    BitFlag += 1024; // section 2 overflow
                    BitFlag += 256; // section 3 overflow
                    BitFlag += 8959; // p5 max
                }
                else BitFlag += 8448;
            }
            else if (FlagSection == 0x5000)
            {
                if (BitFlag > 256)
                {
                    Console.WriteLine($"Warning: Flag Overflow from Section 5! Max: 256 - Current:{BitFlag} - Overflow by { BitFlag - 256 }");
                    BitFlag -= 256; // section size
                    BitFlag += 394; // section 1 overflow
                    BitFlag += 1024; // section 2 overflow
                    BitFlag += 256; // section 3 overflow
                    BitFlag += 256; // section 4 overflow
                    BitFlag += 8959; // p5 max
                }
                else BitFlag += 8704;
            }
          
            if ( BitFlag > 8959 )

            {
                Console.WriteLine($"Warning: Flag Overflow! Flag higer than possible P5 max flag! Max: 8959 - Current:{BitFlag} - Overflow by { BitFlag - 8959 }");
            }

            return (short)BitFlag;
        }
    }
}