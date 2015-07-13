using Moq;

namespace Nanarchy.Tests.TestHelpers
{
    /// <summary>
    /// Allows for extension methods to be added to UnitTestsFor without requiring a generic type parameter
    /// </summary>
    public interface IUnitTestsFor
    {
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="MockRepository.VerifyAll"/> will be used or <see cref="MockRepository.Verify"/>.
        /// </summary>
        /// <value><c>true</c> to use <see cref="MockRepository.VerifyAll"/>; otherwise, <c>false</c>.</value>
        /// <remarks><c>false</c> be default.</remarks>
        bool VerifyAll { get; set; }

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <typeparam name="T">A type to create.</typeparam>
        /// <returns>
        /// Object of the type <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>Usually used to create objects to test. Any non-existing dependencies
        /// are mocked.
        /// <para>Container is used to resolve build dependencies.</para></remarks>
        T Create<T>() where T : class;

        /// <summary>
        /// Resolves an object from the container.
        /// </summary>
        /// <typeparam name="T">Type to resolve for.</typeparam>
        /// <returns>Resolved object.</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Captures <paramref name="instance"/> as the object to provide <typeparamref name="TService"/> for mocking.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        void Register<TService>(TService instance) where TService : class;
    }
}