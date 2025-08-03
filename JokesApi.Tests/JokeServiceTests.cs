using JokesApi.Clients;
using JokesApi.Models;
using JokesApi.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace JokesApi.Tests
{

    /// <summary>
    /// I wanted to include some unit tests.  Of course, I'd try to usually test all happy paths, known failures, and edge cases.
    /// But, in the interest of time, I only included a couple of tests here.
    /// I mainly just wanted to highlight that testing is important and unit tests are one of many layers of testing that should normally be used.
    /// </summary>
    public class JokeServiceTests
    {
        private Mock<IDadJokeClient> _mockClient;
        private ILogger<JokeService> _logger;
        private JokeService _service;

        [SetUp]
        public void Setup()
        {
            _mockClient = new Mock<IDadJokeClient>();
            _logger = new LoggerFactory().CreateLogger<JokeService>();
            _service = new JokeService(_mockClient.Object, _logger);
        }

        [Test]
        public async Task GetRandomJokeHappyPath()
        {
            //Arrange
            var expectedJoke = "Why did the chicken cross the road?";
            _mockClient.Setup(x => x.GetRandomJokeAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new JokeResponse { Joke = expectedJoke });

            //Act
            var result = await _service.GetRandomJokeAsync();

            //Assert
            Assert.That(result, Is.EqualTo(expectedJoke));
        }

        [Test]
        public async Task GetRandomJoke_ReturnsNull_WhenClientFails()
        {
            //Arrange
            _mockClient.Setup(x => x.GetRandomJokeAsync(It.IsAny<CancellationToken>())).ReturnsAsync((JokeResponse?)null);

            //Act
            var result = await _service.GetRandomJokeAsync();

            //Assert
            Assert.IsNull(result);
        }
    }
}