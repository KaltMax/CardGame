namespace MonsterTradingCardsGame.API.Server
{
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }

    public static class HttpMethodUtilities
    {
        public static HttpMethod GetMethod(string method)
        {
            var parsedMethod = method.ToLower() switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                "patch" => HttpMethod.Patch,
                _ => throw new InvalidDataException()
            };

            return parsedMethod;
        }
    }
}
