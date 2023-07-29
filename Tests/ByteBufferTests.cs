using NUnit.Framework;
using UFlow.Addon.ECS.Core.Runtime;

namespace UFlow.Addon.ECS.Tests {
    public sealed class ByteBufferTests {
        [Test]
        public void IntWriteReadTest() {
            var buffer = new ByteBuffer();
            const int value = 5;
            buffer.Write(value);
            buffer.ResetCursor();
            Assert.That(buffer.ReadInt(), Is.EqualTo(5));
        }

        [Test]
        public void IntArrayWriteReadTest() {
            var buffer = new ByteBuffer();
            var values = new[] { 1, 2, 3, 4, 5 };
            buffer.Write(values);
            buffer.ResetCursor();
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
            buffer.ResetCursor();
            Assert.That(buffer.ReadInt(), Is.EqualTo(5));
            Assert.That(buffer.ReadInt(), Is.EqualTo(1));
            Assert.That(buffer.ReadInt(), Is.EqualTo(2));
            Assert.That(buffer.ReadInt(), Is.EqualTo(3));
            Assert.That(buffer.ReadInt(), Is.EqualTo(4));
            Assert.That(buffer.ReadInt(), Is.EqualTo(5));
        }

        [Test]
        public void StringWriteReadTest() {
            var buffer = new ByteBuffer();
            var str = "Testing string";
            buffer.Write(str);
            buffer.ResetCursor();
            Assert.That(buffer.ReadString(), Is.EqualTo(str));
        }
    }
}