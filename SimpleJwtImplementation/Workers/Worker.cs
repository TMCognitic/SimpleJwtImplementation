using SimpleJwtImplementation.Models.Dtos;

namespace SimpleJwtImplementation.Workers
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        const string _baseUri = "https://localhost:7079/";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Simple Consumer for Refresh Token
            //Attendre 10 secondes avant de lancer la première connexion à l'api
            logger.LogInformation("Service Starting...");
            await Task.Delay(5000);

            //Login 
            string email = "john.doe@test.be";

            UserDto? dto = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUri);

                using (HttpResponseMessage responseMessage = await client.PostAsync("auth/login", JsonContent.Create(new { Email = email })))
                {
                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        logger.LogError("T'es con ça marche pas!!");
                        return;
                    }

                    dto = await responseMessage.Content.ReadFromJsonAsync<UserDto>();
                }
            }

            if (dto is null)
            {
                logger.LogError("T'es con y a pas de données!!");
                return;
            }

            logger.LogInformation($"Loggué en tant que : {dto.Prenom} {dto.Nom}");
            logger.LogInformation($"Token : {dto.Token}");
            logger.LogInformation($"Refresh : {dto.RefreshToken}");

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Call Api");
                //Appel continu
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {dto.Token}");

                    using (HttpResponseMessage responseMessage = await client.GetAsync("weatherforecast"))
                    {
                        if (!responseMessage.IsSuccessStatusCode)
                        {
                            switch((int)responseMessage.StatusCode)
                            {
                                case StatusCodes.Status401Unauthorized:
                                    try
                                    {
                                        logger.LogWarning("Tentative de refresh tokens");
                                        await RefreshToken(dto);
                                        logger.LogWarning($"Token : {dto.Token}");
                                        logger.LogWarning($"Refresh : {dto.RefreshToken}");                                    
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex.Message);
                                        return;
                                    }
                                    break;
                                default:
                                    logger.LogError($"T'es con ça marche pas!! {(int)responseMessage.StatusCode}");
                                    return;
                            }                            
                        }
                        else
                        {
                            logger.LogInformation(await responseMessage.Content.ReadAsStringAsync());
                            await Task.Delay(10000);
                        }
                    }
                }

            }

            logger.LogInformation("Service Stopped...");
        }

        private async Task RefreshToken(UserDto dto)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUri);

                using (HttpResponseMessage responseMessage = await client.PostAsync("auth/refresh", JsonContent.Create(new { dto.Token, dto.RefreshToken })))
                {
                    responseMessage.EnsureSuccessStatusCode();

                    TokenPairDto? tokenPair = await responseMessage.Content.ReadFromJsonAsync<TokenPairDto>();
                    if (tokenPair is null)
                        throw new InvalidOperationException();

                    dto.Token = tokenPair.Token;
                    dto.RefreshToken = tokenPair.RefreshToken;
                }
            }

        }
    }
}
