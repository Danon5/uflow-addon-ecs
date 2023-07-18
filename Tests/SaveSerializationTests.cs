using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace UFlow.Addon.ECS.Tests {
    public sealed class SaveSerializationTests {
        [Test]
        public void ComponentSerializeDeserializeTest() {
            var buffer = new ByteBuffer();
            var test1 = new Test1 {
                someData1 = 1,
                someData2 = 2,
                someData3 = 3
            };
            SaveSerializer.SerializeComponent(buffer, ref test1);
            buffer.ResetCursor();
            var result = SaveSerializer.DeserializeComponent<Test1>(buffer);
            Assert.That(result.someData1, Is.EqualTo(1));
            Assert.That(result.someData2, Is.EqualTo(0));
            Assert.That(result.someData3, Is.EqualTo(3));
        }

        [Test]
        public void EntitySerializeDeserializeTest() {
            var buffer = new ByteBuffer();
            var world = new World();
            var entity = world.CreateEntity();
            entity.Set(new Test1 {
                someData1 = 1,
                someData2 = 2,
                someData3 = 3
            });
            SaveSerializer.SerializeEntity(buffer, entity);
            buffer.ResetCursor();
            entity.Destroy();
            var deserializedEntity = SaveSerializer.DeserializeEntity(buffer, world);
            ref var test1 = ref deserializedEntity.Get<Test1>();
            Assert.That(test1.someData1, Is.EqualTo(1));
            Assert.That(test1.someData2, Is.EqualTo(0));
            Assert.That(test1.someData3, Is.EqualTo(3));
            world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }

        [EcsSerializable("SerializationTestsComp1")]
        private struct Test1 : IEcsComponent {
            [Save] public int someData1;
            public int someData2;
            [Save] public int someData3;
        }
    }
}