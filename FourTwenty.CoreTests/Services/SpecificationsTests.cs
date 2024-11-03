using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System;
using System.Linq;
using FourTwenty.Core.Specifications;
using System.Collections.Generic;

namespace FourTwenty.CoreTests.Services
{
    [TestClass]
    public class SpecificationsTests
    {

        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TestSpecification : BaseSpecification<TestEntity>
        {
            private readonly Expression<Func<TestEntity, bool>> _expression;



            public TestSpecification(Expression<Func<TestEntity, bool>> expression)
            {
                _expression = expression;
            }

            public override Expression<Func<TestEntity, bool>> ToExpression()
            {
                return _expression;
            }


            public virtual void AddTestInclude(Expression<Func<TestEntity, object>> includeExpression) => AddInclude(includeExpression);


            public virtual void AddTestInclude(string includeString) => AddInclude(includeString);


            public virtual void ApplyTestOrderBy(Expression<Func<TestEntity, object>> orderByExpression) =>
                ApplyOrderBy(orderByExpression);

            public virtual void ApplyTestOrderByDescending(
                Expression<Func<TestEntity, object>> orderByDescendingExpression) =>
                ApplyOrderByDescending(orderByDescendingExpression);

            public virtual void ApplyTestOrderBy(string ordering, params object[] args) =>
                ApplyOrderBy(ordering, args);

            public virtual void ApplyTestPaging(int pageNumber, int pageSize) => ApplyPaging(pageNumber, pageSize);

        }

        private IQueryable<TestEntity> GetTestEntities()
        {
            return new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" },
                new TestEntity { Id = 3, Name = "Test3" }
            }.AsQueryable();
        }


        [TestMethod]
        public void GetQuery_AppliesCriteria()
        {
            var spec = new TestSpecification(x => x.Id > 1);
            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec);

            Assert.AreEqual(2, query.Count());
            Assert.IsTrue(query.All(x => x.Id > 1));
        }

        [TestMethod]
        public void GetQuery_AppliesIncludes()
        {
            var spec = new TestSpecification(x => x.Id > 1);
            spec.AddTestInclude(x => x.Name);

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec);

            // Since we are not actually querying a real database, we can't verify the includes directly.
            // Instead, we ensure that the query is still valid and returns the expected results.
            Assert.AreEqual(2, query.Count());
        }

        [TestMethod]
        public void GetQuery_AppliesOrderBy()
        {
            var spec = new TestSpecification(x => x.Id > 0);
            spec.ApplyTestOrderBy(x => x.Name);

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec).ToList();

            Assert.AreEqual("Test1", query[0].Name);
            Assert.AreEqual("Test2", query[1].Name);
            Assert.AreEqual("Test3", query[2].Name);
        }

        [TestMethod]
        public void GetQuery_AppliesOrderByDescending()
        {
            var spec = new TestSpecification(x => x.Id > 0);
            spec.ApplyTestOrderByDescending(x => x.Name);

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec).ToList();

            Assert.AreEqual("Test3", query[0].Name);
            Assert.AreEqual("Test2", query[1].Name);
            Assert.AreEqual("Test1", query[2].Name);
        }

        [TestMethod]
        public void GetQuery_AppliesPaging()
        {
            var spec = new TestSpecification(x => x.Id > 0);
            spec.ApplyTestPaging(1, 2);

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec).ToList();

            Assert.AreEqual(2, query.Count);
            Assert.AreEqual(1, query[0].Id);
            Assert.AreEqual(2, query[1].Id);
        }


        [TestMethod]
        public void GetQuery_AppliesDynamicOrderBy()
        {
            var spec = new TestSpecification(x => x.Id > 0);
            spec.ApplyTestOrderBy("Name");

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec).ToList();

            Assert.AreEqual("Test1", query[0].Name);
            Assert.AreEqual("Test2", query[1].Name);
            Assert.AreEqual("Test3", query[2].Name);
        }

        [TestMethod]
        public void GetQuery_AppliesDynamicOrderByWithArgs()
        {
            var spec = new TestSpecification(x => x.Id > 0);
            spec.ApplyTestOrderBy("Name", new object[] { });

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec).ToList();

            Assert.AreEqual("Test1", query[0].Name);
            Assert.AreEqual("Test2", query[1].Name);
            Assert.AreEqual("Test3", query[2].Name);
        }

        [TestMethod]
        public void GetQuery_AppliesStringIncludes()
        {
            var spec = new TestSpecification(x => x.Id > 1);
            spec.AddTestInclude("Name");

            var query = SpecificationEvaluator<TestEntity>.GetQuery(GetTestEntities(), spec);

            // Since we are not actually querying a real database, we can't verify the includes directly.
            // Instead, we ensure that the query is still valid and returns the expected results.
            Assert.AreEqual(2, query.Count());
        }
    }
}
