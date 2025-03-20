using MonsterTradingCardsGame.API.Routing;
using MonsterTradingCardsGame.API.Server;
using HttpMethod = MonsterTradingCardsGame.API.Server.HttpMethod;
using NSubstitute;
using System.Text;

namespace MonsterTradingCardsGame.Tests
{
    internal class HttpServerTests
    {
        [Test]
        public void ReadRequest_ShouldParseHttpRequestCorrectly()
        {
            // Arrange
            const string httpRequestString = "GET /test HTTP/1.1\r\nHost: localhost\r\n\r\n";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(httpRequestString));
            using var reader = new StreamReader(stream);
            var server = new HttpServer(8081, Substitute.For<IRouter>());

            // Act
            var httpRequest = server.ReadRequest(reader);

            // Assert
            Assert.That(httpRequest.Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(httpRequest.ResourcePath, Is.EqualTo("/test"));
            Assert.That(httpRequest.HttpVersion, Is.EqualTo("HTTP/1.1"));
            Assert.That(httpRequest.Header["Host"], Is.EqualTo("localhost"));
        }

        [Test]
        public void ReadRequest_ShouldFailOnMalformedRequestLine()
        {
            // Arrange
            const string httpRequestString = "INVALIDREQUESTLINE\r\nHost: localhost\r\n\r\n"; // Malformed first line
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(httpRequestString));
            using var reader = new StreamReader(stream);

            var server = new HttpServer(8081, Substitute.For<IRouter>());

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => server.ReadRequest(reader), "Expected exception for malformed request line.");
        }

        [Test]
        public void WriteResponse_ShouldFormatHttpResponseCorrectly()
        {
            // Arrange
            var response = new HttpResponse
            {
                StatusCode = StatusCode.Ok,
                Payload = "Hello, World!",
                ContentType = "text/plain"
            };

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            var server = new HttpServer(8081, Substitute.For<IRouter>());

            // Act
            server.WriteResponse(writer, response);

            stream.Position = 0; // Reset stream position to read written data
            using var reader = new StreamReader(stream);
            var responseText = reader.ReadToEnd();

            // Assert
            Assert.That(responseText, Does.Contain("HTTP/1.1 200 Ok"));
            Assert.That(responseText, Does.Contain("Content-Type: text/plain"));
            Assert.That(responseText, Does.Contain("Hello, World!"));
        }

        [Test]
        public void WriteResponse_ShouldFailOnEmptyResponsePayload()
        {
            // Arrange
            var response = new HttpResponse
            {
                StatusCode = StatusCode.Ok,
                Payload = string.Empty, // Empty payload
                ContentType = "text/plain"
            };

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            var server = new HttpServer(8081, Substitute.For<IRouter>());

            // Act
            server.WriteResponse(writer, response);

            stream.Position = 0; // Reset stream position to read written data
            using var reader = new StreamReader(stream);
            var responseText = reader.ReadToEnd();

            // Assert
            Assert.That(responseText, Does.Contain("500 InternalServerError"), "Expected error response for empty payload.");
            Assert.That(responseText, Does.Contain("500 Internal Server Error"), "Expected 'Internal Server Error' message in the body.");
        }

        [Test]
        public void ReadRequest_ShouldPopulatePayload_WhenContentLengthIsValid()
        {

            // Arrange
            var httpRequest = "POST / HTTP/1.1\r\n" +
                              "Content-Length: 11\r\n" +
                              "\r\n" +
                              "Hello World";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(httpRequest));
            var reader = new StreamReader(stream);
            var server = new HttpServer(8081, Substitute.For<IRouter>());

            // Act
            var request = server.ReadRequest(reader);

            // Assert
            Assert.That(request.Payload, Is.EqualTo("Hello World"));
        }
    }
}
