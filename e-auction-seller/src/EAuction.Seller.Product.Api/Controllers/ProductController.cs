using EAuction.Seller.Product.Contracts.Events;
using EAuction.Seller.Product.Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EAuction.Auction.Contracts.Queries;

namespace EAuction.Seller.Product.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublishEndpoint _endpoint;
        private readonly IRequestClient<SellerProductModel> _requestClient;
        private readonly IRequestClient<SellerProductDeleteRequestModel> _requestClientDelete;
        private readonly IRequestClient<ProductListRequestModel> _requestClientProductList;
        private readonly IRequestClient<GetProductBidDetails> _requestClientAuction;        
        private readonly ILogger<ProductController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="requestClient"></param>
        /// <param name="logger"></param>
        public ProductController(IPublishEndpoint endpoint, IRequestClient<SellerProductModel> requestClient, IRequestClient<SellerProductDeleteRequestModel> requestClientDelete, ILogger<ProductController> logger)
        {
            _endpoint = endpoint;
            _requestClient = requestClient;
            _requestClientDelete = requestClientDelete;
            _logger = logger;
        }

        // Get /e-auction/api/v1/seller/email/products
        [HttpGet("/e-auction/api/v1/seller/{email}/products")]
        public async Task<ActionResult<IList<ProductInfoModel>>> Get(string email, CancellationToken cancellationToken = default)
        {
            var request = new ProductListRequestModel()
            {
                CorrelationId = Guid.NewGuid(),
                EmailId = email
            };

            var result = await _requestClientProductList.GetResponse<ProductListResponseModel>(request);
            return Ok(result);

        }

        // GET /e-auction/api/v1/seller/show-bids/{productId}
        [HttpGet("/e-auction/api/v1/seller/show-bids/{productId}")]
        public async Task<ActionResult<GetProductBidDetails.Response>> GetProductBids(string productId, CancellationToken cancellationToken)
        {
            var request = new GetProductBidDetails()
            {
                CorrelationId = Guid.NewGuid(),
                ProductId = productId
            };
            var result = await _requestClientAuction.GetResponse<GetProductBidDetails.Response>(request);
            return Ok(result);
        }


        // POST /e-auction/api/v1/seller/add-product
        [HttpPost("/e-auction/api/v1/seller/add-product")]
        public async Task<ActionResult<SellerProductAddedResponse>> Post([FromBody] SellerProductModel request)
        {
            request.CorrelationId = NewId.NextGuid();
            var result = await _requestClient.GetResponse<SellerProductAddedResponse>(request);
            return Ok(result);
        }


        [HttpDelete("/e-auction/api/v1/seller/delete/{productId}")]
        public async Task<ActionResult> Delete(string productId, CancellationToken cancellationToken)
        {
            var request = new SellerProductDeleteRequestModel() { CorrelationId = Guid.NewGuid(), ProductId = productId };
            var result = await _requestClientDelete.GetResponse<SellerProductDeletedResponse>(request);
            return Ok(result);
        }
    }
}
