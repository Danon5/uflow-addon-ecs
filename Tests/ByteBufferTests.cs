using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace UFlow.Addon.ECS.Tests {
    public sealed class ByteBufferTests {
        [Test]
        public void IntWriteReadTest() {
            var buffer = new ByteBuffer();
            const int value = 5;
            buffer.Write(value);
            buffer.Complete();
            Assert.That(buffer.ReadInt(), Is.EqualTo(5));
        }

        [Test]
        public void IntArrayWriteReadTest() {
            var buffer = new ByteBuffer();
            var values = new[] { 1, 2, 3, 4, 5 };
            buffer.Write(values);
            buffer.Complete();
            buffer.ReadIntArrayInto(values);
            Assert.That(values[0], Is.EqualTo(1));
            Assert.That(values[1], Is.EqualTo(2));
            Assert.That(values[2], Is.EqualTo(3));
            Assert.That(values[3], Is.EqualTo(4));
            Assert.That(values[4], Is.EqualTo(5));
        }

        [Test]
        public void IntArrayWriteReadRawTest() {
            var buffer = new ByteBuffer();
            var values = new[] { 1, 2, 3, 4, 5 };
            buffer.Write(values);
            buffer.Complete();
            Assert.That(buffer.ReadUShort(), Is.EqualTo(5));
            Assert.That(buffer.ReadInt(), Is.EqualTo(1));
            Assert.That(buffer.ReadInt(), Is.EqualTo(2));
            Assert.That(buffer.ReadInt(), Is.EqualTo(3));
            Assert.That(buffer.ReadInt(), Is.EqualTo(4));
            Assert.That(buffer.ReadInt(), Is.EqualTo(5));
        }
    }
}