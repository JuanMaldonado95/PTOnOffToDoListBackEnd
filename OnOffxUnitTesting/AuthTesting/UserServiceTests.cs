using ApplicationCore.Entities.PTOnOff.Auth;
using ApplicationCore.Models.Auth;
using Infrastructure.Data;
using Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OnOffxUnitTesting.AuthTesting
{
    public class UserServiceTests
    {
        private readonly Mock<DbSet<tblAuth>> _mockDbSet;
        private readonly Mock<DbContextPTOnOff> _mockContext;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockDbSet = new Mock<DbSet<tblAuth>>();
            _mockContext = new Mock<DbContextPTOnOff>();
            _mockContext.Setup(c => c.tblAuth).Returns(_mockDbSet.Object);
            _userService = new UserService(_mockContext.Object);
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => _inner.Execute<TResult>(expression);
        }

        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }

        [Theory(DisplayName = "CreateMD5_DebeRetornarHashMD5Correcto")]
        [InlineData("123456", "E10ADC3949BA59ABBE56E057F20F883E")]
        [InlineData("test", "098F6BCD4621D373CADE4E832627B4F6")]
        public async Task CreateMD5_DebeRetornarHashCorrecto(string input, string expectedHash)
        {
            var actualHash = await _userService.CreateMD5(input);
            Assert.Equal(expectedHash, actualHash);
        }

        [Fact(DisplayName = "ValidateUserCredentialsAsync_DebeLanzarExcepcion_EnCasoDeError")]
        public async Task ValidateUserCredentialsAsync_DebeLanzarExcepcion_EnCasoDeError()
        {
            _mockContext.Setup(c => c.tblAuth).Throws(new System.Exception("Error DB"));

            var login = new mLogin { tUserName = "user", tPassword = "hash" };

            await Assert.ThrowsAsync<System.Exception>(() => _userService.ValidateUserCredentialsAsync(login));
        }

        [Fact(DisplayName = "ValidateUserCredentialsAsync_DebeRetornarFalse_SiCredencialesVacias")]
        public async Task ValidateUserCredentialsAsync_DebeRetornarFalse_SiCredencialesVacias()
        {
            var loginData = new mLogin { tUserName = "", tPassword = "" };

            var (isValid, user) = await _userService.ValidateUserCredentialsAsync(loginData);

            Assert.False(isValid);
            Assert.Null(user);

            _mockContext.Verify(c => c.tblAuth, Times.Never);
        }
    }
}