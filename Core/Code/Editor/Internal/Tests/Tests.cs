using System.Collections.Generic;
using NUnit.Framework;
using UFlow.Addon.Ecs.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Editor {
    internal static class Tests {
        private static World m_world;
        
        [SetUp]
        public static void SetUp() {
            m_world = World.Create();
        }

        [TearDown]
        public static void TearDown() {
            m_world.Destroy();
            m_world = null;
        }
        
        [Test]
        public static void BasicEntityTest() {
            var entities = new Queue<Entity>();

            for (var i = 0; i < 100; i++)
                entities.Enqueue(m_world.CreateEntity());
            
            while (entities.TryDequeue(out var entity))
                entity.Destroy();
        }
    }
}