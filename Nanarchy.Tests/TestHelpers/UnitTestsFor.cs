using System;
using System.Linq;
using Moq;
using StructureMap.AutoMocking;

namespace Nanarchy.Tests.TestHelpers
{
    public class MoqServiceLocator : ServiceLocator
    {
        private MockRepository _mockRepository;
        public MoqServiceLocator()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);
        }

        public Mock<T> Create<T>() where T : class
        {
            var mockInstance = _mockRepository.Create<T>();
            return mockInstance;
        }

        public T Service<T>() where T : class
        {
            return _mockRepository.Of<T>().First();
        }

        public object Service(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return _mockRepository.OneOf<T>();
        }
    }
    public abstract class UnitTestsFor<TObjectUnderTest> : IUnitTestsFor
        where TObjectUnderTest : class
    {
        protected TObjectUnderTest ObjectUnderTest;
        MoqServiceLocator _mockRepository;

        protected UnitTestsFor()
        {
            _mockRepository = new MoqServiceLocator();
            MockContainer = new AutoMocker<TObjectUnderTest>(_mockRepository);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether <see cref="MockRepository.VerifyAll" /> will be used or
        ///     <see
        ///         cref="MockRepository.Verify" />
        ///     .
        /// </summary>
        /// <value>
        ///     <c>true</c> to use <see cref="MockRepository.VerifyAll" />; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <c>false</c> be default.
        /// </remarks>
        public bool VerifyAll { get; set; }

        /// <summary>
        ///     Provides access to the auto mocking container
        /// </summary>
        private AutoMocker<TObjectUnderTest> MockContainer { get; set; }

        /// <summary>
        ///     Creates an object of the specified type.
        /// </summary>
        /// <typeparam name="T">A type to create.</typeparam>
        /// <returns>
        ///     Object of the type <typeparamref name="T" />.
        /// </returns>
        /// <remarks>
        ///     Usually used to create objects to test. Any non-existing dependencies
        ///     are mocked.
        ///     <para>Container is used to resolve build dependencies.</para>
        /// </remarks>
        public T Create<T>() where T : class
        {
            var mockInstance = _mockRepository.Create<T>();
            MockContainer.Inject(mockInstance);
            return mockInstance.Object;
        }

        /// <summary>
        ///     Resolves an object from the container.
        /// </summary>
        /// <typeparam name="T">Type to resolve for.</typeparam>
        /// <returns>Resolved object.</returns>
        public T Resolve<T>() where T : class
        {
            return MockContainer.Get<T>();
        }

        /// <summary>
        ///     Captures <paramref name="instance" /> as the object to provide <typeparamref name="TService" /> for mocking.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void Register<TService>(TService instance) where TService : class
        {
            MockContainer.Inject(instance);
        }

        /// <summary>
        ///     Initializes the <see cref="ObjectUnderTest" /> field using an auto-mocking container.
        ///     This method must be called in the test's TestInitialize method.
        /// </summary>
        public virtual void InitObjectUnderTest()
        {
            ObjectUnderTest = MockContainer.ClassUnderTest;
        }

        /// <summary>
        ///     Convenience method for retrieving a Mock that was generated for the <see cref="ObjectUnderTest" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type for which to retrieve the <see cref="Mock{T}" />.
        /// </typeparam>
        /// <returns>A mock object for the given type.</returns>
        protected Mock<T> For<T>() where T : class
        {
            return MockContainer.Get<Mock<T>>();
        }
    }
}