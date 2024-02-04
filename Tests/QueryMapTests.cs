using NUnit.Framework;
using UFlow.Addon.ECS.Core.Runtime;

namespace UFlow.Addon.ECS.Tests {
    public sealed class QueryMapTests {
        [Test]
        public void PreInitTest() {
            var world = new World();
            world.CreateEntity().Set(new Test1 {
                someData = 1
            });
            world.CreateEntity().Set(new Test1 {
                someData = 2
            });
            var query = world.BuildQuery().With<Test1>().AsMap<Test1>();
            Assert.That(query.EntityCount, Is.EqualTo(2));
            Assert.That(query.GetEntity(new Test1 { someData = 1 }).Get<Test1>().someData, Is.EqualTo(1));
            Assert.That(query.GetEntity(new Test1 { someData = 2 }).Get<Test1>().someData, Is.EqualTo(2));
            world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }

        [Test]
        public void PostInitTest() {
            var world = new World();
            var query = world.BuildQuery().With<Test1>().AsMap<Test1>();
            world.CreateEntity().Set(new Test1 {
                someData = 1
            });
            Assert.That(query.EntityCount, Is.EqualTo(1));
            Assert.That(query.GetEntity(new Test1 { someData = 1 }).Get<Test1>().someData, Is.EqualTo(1));
            world.CreateEntity().Set(new Test1 {
                someData = 2
            });
            Assert.That(query.EntityCount, Is.EqualTo(2));
            Assert.That(query.GetEntity(new Test1 { someData = 2 }).Get<Test1>().someData, Is.EqualTo(2));
            world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }

        [Test]
        public void PostAddRemoveTest() {
            var world = new World();
            var query = world.BuildQuery().With<Test1>().AsMap<Test1>();
            var firstEntity = world.CreateEntity();
            firstEntity.Set(new Test1 {
                someData = 1
            });
            Assert.That(query.GetEntity(new Test1 { someData = 1 }).Get<Test1>().someData, Is.EqualTo(1));
            var secondEntity = world.CreateEntity();
            secondEntity.Set(new Test1 {
                someData = 2
            });
            Assert.That(query.GetEntity(new Test1 { someData = 2 }).Get<Test1>().someData, Is.EqualTo(2));
            Assert.That(query.EntityCount, Is.EqualTo(2));
            firstEntity.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(1));
            secondEntity.Destroy();
            Assert.That(query.EntityCount, Is.EqualTo(0));
            world.Destroy();
            ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }

        private struct Test1 : IEcsComponentData {
            public int someData;
        }
        
        private struct Test2 : IEcsComponentData {
            public int someData;
        }
    }
}