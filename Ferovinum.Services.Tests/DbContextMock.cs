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
                //Mock<EntityEntry<TData>> entityEntry = new Mock<EntityEntry<TData>>(args: It.IsAny<InternalEntityEntry>());

                dbSetMock.As<IQueryable<TData>>().Setup(s => s.Provider).Returns(lstDataQueryable.Provider);
                dbSetMock.As<IQueryable<TData>>().Setup(s => s.Expression).Returns(lstDataQueryable.Expression);
                dbSetMock.As<IQueryable<TData>>().Setup(s => s.ElementType).Returns(lstDataQueryable.ElementType);
                dbSetMock.As<IQueryable<TData>>().Setup(s => s.GetEnumerator()).Returns(() => lstDataQueryable.GetEnumerator());
                //dbSetMock.Setup(x => x.Add(It.IsAny<TData>())).Callback<TData>(lstData.Add);
                dbSetMock.Setup(x => x.Attach(It.IsAny<TData>())).Callback<TData>(lstData.Add);
                //dbSetMock.Setup(x => x.Update(It.IsAny<TData>())).Callback<TData>(t => { var index = lstData.FindIndex(x => x.Id == t.Id); lstData[index] = t; });
                //dbSetMock.Setup(x => x.AddRange(It.IsAny<IEnumerable<TData>>())).Callback<IEnumerable<TData>>(lstData.AddRange);
                //dbSetMock.Setup(x => x.Remove(It.IsAny<TData>())).Callback<TData>(t => lstData.Remove(t)).Returns<TData>(t => entityEntry.Object);
                //dbSetMock.Setup(x => x.RemoveRange(It.IsAny<IEnumerable<TData>>())).Callback<IEnumerable<TData>>(ts =>
                //{
                //    foreach (var t in ts) { lstData.Remove(t); }
                //});

                //dbContext.Setup(m => m.Add<TData>(It.IsAny<TData>())).Callback<TData>(lstData.Add);
                dbContext.Setup(m => m.Attach<TData>(It.IsAny<TData>())).Callback<TData>(lstData.Add);
                //dbContext.Setup(m => m.Update<TData>(It.IsAny<TData>())).Callback<TData>(t => { var index = lstData.FindIndex(x => x.Id == t.Id); lstData[index] = t; });
                dbContext.Setup(m => m.Set<TData>()).Returns(dbSetMock.Object);
                dbContext.Setup(dbSetSelectionExpression).Returns(dbSetMock.Object);


                //Mock<EntityEntry<Book>> entityEntryBook = new Mock<EntityEntry<Book>>(args: It.IsAny<InternalEntityEntry>());
                //Mock<EntityEntry<Author>> entityEntryAuthor = new Mock<EntityEntry<Author>>(args: It.IsAny<InternalEntityEntry>());
                //Mock<EntityEntry<Publisher>> entityEntryPublisher = new Mock<EntityEntry<Publisher>>(args: It.IsAny<InternalEntityEntry>());
                //dbContext.Setup(m => m.Entry<Book>(It.IsAny<Book>())).Returns(entityEntryBook.Object);
                //dbContext.Setup(m => m.Entry<Author>(It.IsAny<Author>())).Returns(entityEntryAuthor.Object);
                //dbContext.Setup(m => m.Entry<Publisher>(It.IsAny<Publisher>())).Returns(entityEntryPublisher.Object);

                return dbContext;
            }
        }
    

}
