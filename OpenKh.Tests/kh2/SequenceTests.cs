﻿using OpenKh.Common;
using OpenKh.Kh2;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace OpenKh.Tests.kh2
{
    public class SequenceTests
    {
        private const string FilePath = "kh2/res/eh_l.layd";
        private const int Offset = 0x5B4;
        private const int Length = 0x7D4;

        [Fact]
        public void IsValidTest() => new MemoryStream()
            .Using(stream =>
            {
                stream.WriteByte(0x53);
                stream.WriteByte(0x45);
                stream.WriteByte(0x51);
                stream.WriteByte(0x44);
                stream.Write(new byte[44]);
                stream.Position = 0;
                Assert.True(Sequence.IsValid(stream));
                Assert.Equal(0, stream.Position);
            });

        [Fact]
        public void IsInvalidHeaderTest() => new MemoryStream()
            .Using(stream =>
            {
                stream.WriteByte(0xCC);
                stream.WriteByte(0xCC);
                stream.WriteByte(0xCC);
                stream.WriteByte(0xCC);
                stream.Write(new byte[44]);
                stream.Position = 0;
                Assert.False(Sequence.IsValid(stream));
                Assert.Equal(0, stream.Position);
            });

        [Fact]
        public void IsInvalidStreamLengthTest() => new MemoryStream()
            .Using(stream =>
            {
                stream.WriteByte(0x53);
                stream.WriteByte(0x45);
                stream.WriteByte(0x51);
                stream.WriteByte(0x44);
                stream.Position = 0;
                Assert.False(Sequence.IsValid(stream));
                Assert.Equal(0, stream.Position);
            });

        [Fact]
        public void ValidateSequenceHeader() => StartTest(stream =>
        {
            var sequence = Sequence.Read(stream);
            Assert.Equal(3, sequence.Sprites.Count);
            Assert.Equal(15, sequence.SpriteGroups.Sum(x => x.Count));
            Assert.Equal(3, sequence.SpriteGroups.Count);
            Assert.Equal(2, sequence.AnimationGroups.Count);
            Assert.Equal(10, sequence.AnimationGroups.Sum(x => x.Animations.Count));
        });

        [Fact]
        public void WriteSequenceTest() => StartTest(stream =>
        {
            var expected = new BinaryReader(stream).ReadBytes((int)stream.Length);
            var sequence = new MemoryStream(expected).Using(x => Sequence.Read(x));
            new MemoryStream().Using(x =>
            {
                sequence.Write(x);
                x.Position = 0;
                var actual = new BinaryReader(x).ReadBytes((int)x.Length);

                Assert.Equal(expected.Length, actual.Length);
                for (var i = 0; i < expected.Length; i++)
                {
                    var ch1 = expected[i];
                    var ch2 = actual[i];
                    Assert.True(ch1 == ch2, $"Expected {ch1:X02} but found {ch2:X02} at {i:X}");
                }
            });
        });

        private void StartTest(Action<Stream> action) =>
            Common.FileOpenRead(FilePath, x =>
                action(new Xe.IO.SubStream(x, Offset, Length)));
    }
}
