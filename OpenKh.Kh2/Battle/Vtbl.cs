﻿using System.Collections.Generic;
using System.IO;
using Xe.BinaryMapper;

namespace OpenKh.Kh2.Battle
{
    public class Vtbl
    {
        [Data] public byte CharacterId { get; set; }
        [Data] public byte ActionId { get; set; }
        [Data] public byte Priority { get; set; }
        [Data] public byte Unknown03 { get; set; } //Padding?
        [Data(offset: 4, count: 5)] public List<Voice> Voices { get; set; }

        public class Voice
        {
            [Data] public sbyte VsbIndex { get; set; }
            [Data] public sbyte Weight { get; set; } //(0 = normal random; 100 = guaranteed run)
        }

        public static BaseBattle<Vtbl> Read(Stream stream) => BaseBattle<Vtbl>.Read(stream);
    }
}
