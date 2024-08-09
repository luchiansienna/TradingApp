using Ferovinum.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Moq;
using System.Linq.Expressions;

namespace Ferovinum.Services.Tests
{
        public class DbContextMock
        {
            public static Mock<TContext> GetMock<TData, TContext>(Mock<TContext> dbContext, List<TData> lstData, Expression<Func<TContext, DbSet<TData>>> dbSetSelectionExpression) where TData : class where TContext : DbContext
            {
                IQueryable<TData> lstDataQueryable = lstData.AsQueryable();
                Mock<DbSet<TData>> dbSetMock = new Mock<DbSet<TData>>();

                dbSetMock.As<IQueryable<TData>>().Setup(s => s.Provider).Returns(lstDataQueryable.Provider);
                dbSetMock.As<IQueryable<TData>>().Setup(s => s.Expression).Returns(lstDataQueryable.Expression);
                dbSetMock.As<IQueryable<TData>>().Setup(s => s.ElementType).Returns(lstDataQueryable.ElementType);
                dbSetMock.As<IQueryable<TData>>().Setup(s => s.GetEnumerator()).Returns(() => lstDataQueryable.GetEnumerator());
                dbSetMock.Setup(x => x.Attach(It.IsAny<TData>())).Callback<TData>(lstData.Add);

                dbContext.Setup(m => m.Attach<TData>(It.IsAny<TData>())).Callback<TData>(lstData.Add);
                dbContext.Setup(m => m.Set<TData>()).Returns(dbSetMock.Object);
                dbContext.Setup(dbSetSelectionExpression).Returns(dbSetMock.Object);

                return dbContext;
            }
        }
    

}
