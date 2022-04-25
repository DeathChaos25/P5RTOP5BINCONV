using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amicitia.IO.Binary;

namespace P5CLTCONV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Usage:\nP5CLTCONV [path to CLT, FNT/ENT bin file, or shdPersona/Enemy pdd file]");
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

                        if ( CLTVersion < 386008576 ) // P5R Version 0x17020600 
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

                                for ( int i = 0; i < Entry_Num -1; i++ )
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

                                    for ( int j = 0; j < CrowdUnitAmount; j++ )
                                    {
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16());
                                        NewCLTFile.Write(P5RCLTFile.ReadInt16());
                                    }
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

                                    uint Conditional = P5RCLTFile.ReadUInt32();
                                    if ( Conditional == 5 )
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
                                    for ( int j = 0; j < NumOfPathingNodes2; j++ )
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

                                if ( P5RCLTFile.Position >= P5RCLTFile.Length )
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
                                for ( int d = 0; d < EntryNum; d++ )
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
                else if ( arg0.Extension == ".bin" )
                {
                    Console.WriteLine($"Attempting to convert { arg0.Name }");

                    if ( arg0.Name.Contains("corptbl") )
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

                                    for ( int j = 1; j <= 22; j++ )
                                    {
                                        BitFlag = P5RcorptblFile.ReadInt16();
                                        FlagSection = P5RcorptblFile.ReadInt16();

                                        //Console.WriteLine($"FlagSection is {FlagSection}");

                                        if (FlagSection == 0x1000)
                                        {
                                            BitFlag += 2048;
                                            FlagSection = 0;
                                        }
                                        else if (FlagSection == 0x2000)
                                        {
                                            BitFlag += 4096;
                                            FlagSection = 0;
                                        }
                                        else if (FlagSection == 0x3000)
                                        {
                                            BitFlag += 8192;
                                            FlagSection = 0;
                                        }
                                        else if (FlagSection == 0x4000)
                                        {
                                            BitFlag += 8448;
                                            FlagSection = 0;
                                        }
                                        else if (FlagSection == 0x5000)
                                        {
                                            BitFlag += 8704;
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

                                    if (FlagSection == 0x1000)
                                    {
                                        BitFlag += 2048;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x2000)
                                    {
                                        BitFlag += 4096;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x3000)
                                    {
                                        BitFlag += 8192;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x4000)
                                    {
                                        BitFlag += 8448;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x5000)
                                    {
                                        BitFlag += 8704;
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
                                    for (int iAgain = 0; iAgain < 6; iAgain++)
                                    {
                                        float resolutionThing = P5RroadmapFile.ReadSingle();
                                        resolutionThing = resolutionThing * 0.6666666666666667f;
                                        NewroadmapFile.Write(resolutionThing);
                                    }

                                    short BitFlag = 0;
                                    short FlagSection = 0;

                                    BitFlag = P5RroadmapFile.ReadInt16();
                                    FlagSection = P5RroadmapFile.ReadInt16();

                                    if (FlagSection == 0x1000)
                                    {
                                        BitFlag += 2048;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x2000)
                                    {
                                        BitFlag += 4096;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x3000)
                                    {
                                        BitFlag += 8192;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x4000)
                                    {
                                        BitFlag += 8448;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x5000)
                                    {
                                        BitFlag += 8704;
                                        FlagSection = 0;
                                    }

                                    NewroadmapFile.Write(BitFlag); // s16 if_Bitflag_disabled;
                                    NewroadmapFile.Write(FlagSection); // s16 Bitflag_related;
                                    NewroadmapFile.Write(P5RroadmapFile.ReadInt32()); //Field20
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field24
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field28
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field2C
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field30
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field34
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field38
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field3C
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field40
                                    NewroadmapFile.Write(P5RroadmapFile.ReadSingle()); //Field44

                                }
                                while (P5RroadmapFile.Position < P5RroadmapFile.Length) //write remainder of file without going past EOF
                                {
                                    NewroadmapFile.Write(P5RroadmapFile.ReadByte());
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

                                    if (FlagSection == 0x1000)
                                    {
                                        BitFlag += 2048;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x2000)
                                    {
                                        BitFlag += 4096;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x3000)
                                    {
                                        BitFlag += 8192;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x4000)
                                    {
                                        BitFlag += 8448;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x5000)
                                    {
                                        BitFlag += 8704;
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
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 if_Bitflag_enabled;
                                    NewFNTFile.Write(P5RFNTFile.ReadInt16()); // s16 Bitflag_related;
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

                            for (int i = 0; i < ( P5RPDDFile.Length - 0x10) / 0x4c; i++)
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

                                if (FlagSection == 0x1000)
                                {
                                    BitFlag += 2048;
                                    FlagSection = 0;
                                }
                                else if (FlagSection == 0x2000)
                                {
                                    BitFlag += 4096;
                                    FlagSection = 0;
                                }
                                else if (FlagSection == 0x3000)
                                {
                                    BitFlag += 8192;
                                    FlagSection = 0;
                                }
                                else if (FlagSection == 0x4000)
                                {
                                    BitFlag += 8448;
                                    FlagSection = 0;
                                }
                                else if (FlagSection == 0x5000)
                                {
                                    BitFlag += 8704;
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

                                for ( int j = 0; j < 6; j++ )
                                {
                                    BitFlag = P5RHBNFile.ReadInt16();
                                    FlagSection = P5RHBNFile.ReadInt16();

                                    //Console.WriteLine($"FlagSection is {FlagSection}");

                                    if (FlagSection == 0x1000)
                                    {
                                        BitFlag += 2048;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x2000)
                                    {
                                        BitFlag += 4096;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x3000)
                                    {
                                        BitFlag += 8192;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x4000)
                                    {
                                        BitFlag += 8448;
                                        FlagSection = 0;
                                    }
                                    else if (FlagSection == 0x5000)
                                    {
                                        BitFlag += 8704;
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
                else Console.WriteLine("https://youtu.be/Uuw6PdJvW88");
            }
        }
    }
}
