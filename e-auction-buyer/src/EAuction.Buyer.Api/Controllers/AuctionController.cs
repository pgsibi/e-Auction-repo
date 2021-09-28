using EAuction.Buyer.Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EAuction.Buyer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IPublishEndpoint _endpoint;
        private readonly IRequestClient<AddAuctionRequestModel> _requestClient;
        private readonly IRequestClient<UpdateAuctionRequestModel> _requestClientDelete;
        private readonly ILogger<AuctionController> _logger;

        /// <summary>
        /// AuctionController
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="requestClient"></param>
        /// <param name="requestClientDelete"></param>
        /// <param name="logger"></param>
        public AuctionController(IPublishEndpoint endpoint, IRequestClient<AddAuctionRequestModel> requestClient, IRequestClient<UpdateAuctionRequestModel> requestClientDelete, ILogger<AuctionController> logger)
        {
            _endpoint = endpoint;
            _requestClient = requestClient;
            _requestClientDelete = requestClientDelete;
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("/place-bid")]
        public async Task<ActionResult<AuctionAddedResponse>> AddAuction([FromBody] AddAuctionRequestModel request, CancellationToken cancellationToken)
        {
            request.CorrelationId = Guid.NewGuid();
            var result = await _requestClient.GetResponse<AuctionAddedResponse>(request);
            return Ok(result);
        }

        [HttpPost("/update-bid/{productId}/{buyerEmailld}/{newBidAmount}")]
        public async Task<ActionResult<AuctionUpdatedResponse>> UpdateAuction([FromRoute] UpdateAuctionRequestModel request, CancellationToken cancellationToken)
        {
            request.CorrelationId = Guid.NewGuid();
            var result = await _requestClient.GetResponse<AuctionUpdatedResponse>(request);
            return Ok(result);
        }


    }
}
