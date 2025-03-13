using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public AuctionServiceHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        this._httpClient = httpClient;
        this._configuration = configuration;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item,string>()
            .Sort(x=>x.Descending(a=>a.UpdatedAt))
            .Project(x=>x.UpdatedAt.ToString())
            .ExecuteFirstAsync();
        string test = _configuration["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated;
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<Item>>(_configuration["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}