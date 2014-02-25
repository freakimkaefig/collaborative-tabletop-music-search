using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Waf.UnitTesting;
using Waf.BookLibrary.Library.Applications.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace Waf.BookLibrary.Library.Applications.Test.Services
{
    [TestClass]
    public class EntityObservableCollectionTest
    {
        [TestMethod]
        public void CollectionTest()
        {
            AssertHelper.ExpectedException<ArgumentNullException>(() => new EntityObservableCollection<MockEntity>(null));

            MockObjectSet<MockEntity> objectSet = new MockObjectSet<MockEntity>();
            EntityObservableCollection<MockEntity> collection = new EntityObservableCollection<MockEntity>(objectSet);

            MockEntity mockEntity = new MockEntity();
            bool handlerCalled = false;
            NotifyCollectionChangedEventHandler handler = (sender, e) =>
                {
                    Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
                    Assert.AreEqual(mockEntity, e.NewItems.Cast<MockEntity>().Single());
                    Assert.AreEqual(mockEntity, collection.Single());
                    Assert.AreEqual(mockEntity, objectSet.ObjectsToAdd.Single());
                    handlerCalled = true;
                };
            collection.CollectionChanged += handler;
            collection.Add(mockEntity);
            collection.CollectionChanged -= handler;
            Assert.IsTrue(handlerCalled);

            collection.Insert(0, new MockEntity());
            collection.Add(new MockEntity());

            objectSet.ClearMock();
            collection.Remove(mockEntity);
            Assert.IsFalse(collection.Contains(mockEntity));
            Assert.AreEqual(mockEntity, objectSet.ObjectsToRemove.Single());
           
            MockEntity oldMockEntity = new MockEntity();
            MockEntity newMockEntity = new MockEntity();
            collection.Insert(0, oldMockEntity);
            objectSet.ClearMock();
            collection[0] = newMockEntity;
            Assert.AreEqual(newMockEntity, collection[0]);
            Assert.AreEqual(oldMockEntity, objectSet.ObjectsToRemove.Single());
            Assert.AreEqual(newMockEntity, objectSet.ObjectsToAdd.Single());

            MockEntity[] entities = collection.ToArray();
            objectSet.ClearMock();
            collection.Clear();
            Assert.IsFalse(collection.Any());
            Assert.IsTrue(entities.SequenceEqual(objectSet.ObjectsToRemove));
        }



        private class MockEntity
        {
        }

        private class MockObjectSet<T> : Collection<T>, IObjectSet<T> where T : class
        {
            private readonly List<T> objectsToAdd = new List<T>();
            private readonly List<T> objectsToRemove = new List<T>();


            public void ClearMock()
            {
                objectsToAdd.Clear();
                objectsToRemove.Clear();
            }
            
            public IList<T> ObjectsToAdd { get { return objectsToAdd; } }

            public IList<T> ObjectsToRemove { get { return objectsToRemove; } }


            public void AddObject(T entity) { objectsToAdd.Add(entity); }

            public void Attach(T entity) { throw new NotImplementedException(); }

            public void DeleteObject(T entity) { objectsToRemove.Add(entity); }

            public void Detach(T entity) { throw new NotImplementedException(); }

            public Type ElementType { get { throw new NotImplementedException(); } }

            public Expression Expression { get { throw new NotImplementedException(); } }

            public IQueryProvider Provider { get { throw new NotImplementedException(); } }
        }
    }
}
