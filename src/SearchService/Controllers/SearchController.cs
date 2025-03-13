using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;
[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    public SearchController()
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery]SearchParams parameters)
    {
        var query = DB.PagedSearch<Item,Item>();

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query.Match(Search.Full, parameters.SearchTerm).SortByTextScore();
        }

        query = parameters.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Ascending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };

        query = parameters.FilterBy switch
        {
            "finished" => query.Match(x=>x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x=>x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x=>x.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrEmpty(parameters.Seller))
        {
            query.Match(x=>x.Seller == parameters.Seller);
        }

        if (!string.IsNullOrEmpty(parameters.Winner))
        {
            query.Match(x=>x.Winner == parameters.Winner);
        }

        query.PageNumber(parameters.Page);
        query.PageSize(parameters.PageSize);
        
        var result = await query.ExecuteAsync();
        
        return Ok(new
        {
            results= result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}