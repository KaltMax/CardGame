using System.Net.Sockets;
using System.Net;
using System.Text;
using MonsterTradingCardsGame.API.Routing;

namespace MonsterTradingCardsGame.API.Server
{
    public class HttpServer
    {
        private readonly int _port;
        private readonly TcpListener _listener;
        private readonly IRouter _router;
        private bool _isListening;

        public HttpServer(int port, IRouter router)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _router = router;
            _isListening = false;
        }

        public void Start()
        {
            Console.WriteLine($"Starting HTTP server on port {_port}...\n");
            Console.WriteLine("================================================================================\n");
            _isListening = true;
            _listener.Start();

            while (_isListening)
            {
                // ----- 0. Accept the TCP-Client and handle each client in a separate task -----
                var clientSocket = _listener.AcceptTcpClient();
                Task.Run(() => HandleClient(clientSocket));
            }
        }

        public void Stop()
        {
            _isListening = false;
            _listener.Stop();
        }

        public void HandleClient(TcpClient clientSocket)
        {
            using var writer = new StreamWriter(clientSocket.GetStream());
            writer.AutoFlush = true;
            using var reader = new StreamReader(clientSocket.GetStream());

            try
            {
                // 1. Read the HTTP request
                var request = ReadRequest(reader);

                // 2. Process the request
                var response = _router.Route(request);

                // 3. Write the response
                WriteResponse(writer, response);
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // Respond with 400 Bad Request
                var errorResponse = new HttpResponse
                {
                    StatusCode = StatusCode.BadRequest,
                    Payload = "400 Bad Request: Malformed request",
                    ContentType = "text/plain"
                };
                WriteResponse(writer, errorResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");

                // Respond with 500 Internal Server Error
                var errorResponse = new HttpResponse
                {
                    StatusCode = StatusCode.InternalServerError,
                    Payload = "500 Internal Server Error",
                    ContentType = "text/plain"
                };
                WriteResponse(writer, errorResponse);
            }
            finally
            {
                // Close the client socket
                clientSocket.Close();
            }
        }

        public HttpRequest ReadRequest(StreamReader reader)
        {
            var httpRequest = new HttpRequest();

            // 2.1 First line in HTTP contains the method, path, and HTTP version
            var line = reader.ReadLine();
            if (line == null) return httpRequest;

            var firstLineParts = line.Split(' ');
            if (firstLineParts.Length != 3)
            {
                Console.WriteLine("Malformed request line.");
                throw new InvalidDataException("Malformed request line."); // Explicitly throw an exception for invalid data
            }

            httpRequest.Method = HttpMethodUtilities.GetMethod(firstLineParts[0]);
            httpRequest.ResourcePath = firstLineParts[1];
            httpRequest.HttpVersion = firstLineParts[2];

            // Output the request method and path
            Console.WriteLine($"Request: {httpRequest.Method} {httpRequest.ResourcePath}");

            // 2.2 Read the HTTP headers (in HTTP after the first line, until the empty line)
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line); // Output the header line
                if (line == "") break;  // Empty line indicates the end of the HTTP-headers

                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    httpRequest.Header[parts[0].Trim()] = parts[1].Trim();
                }
            }

            // 2.3 Read the body if it exists
            if (httpRequest.Header.TryGetValue("Content-Length", out var contentLengthValue) && int.TryParse(contentLengthValue, out int contentLength))
            {
                var buffer = new char[contentLength];
                reader.Read(buffer, 0, contentLength);
                httpRequest.Payload = new string(buffer);
                Console.WriteLine("Request Body:");
                Console.WriteLine(httpRequest.Payload);
            }

            return httpRequest;
        }

        public void WriteResponse(StreamWriter writer, HttpResponse response)
        {
            // Check if the payload is null or empty and set to "500 Internal Server Error" if needed
            if (string.IsNullOrEmpty(response.Payload))
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = "500 Internal Server Error";
            }

            // Write the HTTP status line
            writer.WriteLine($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}");

            // Use the ContentType directly from the response
            string contentType = response.ContentType;

            // Write headers
            var payloadBytes = Encoding.UTF8.GetBytes(response.Payload);
            writer.WriteLine($"Content-Length: {payloadBytes.Length}");
            writer.WriteLine($"Content-Type: {contentType}");
            writer.WriteLine(); // End of headers

            // Write body
            writer.Write(response.Payload);
            writer.Write("\r\n");

            Console.WriteLine($"\nResponse sent: {response.Payload}\n");
        }
    }
}
