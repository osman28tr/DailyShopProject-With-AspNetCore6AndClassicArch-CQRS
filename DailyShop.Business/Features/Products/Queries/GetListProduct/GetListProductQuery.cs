﻿using AutoMapper;
using DailyShop.Business.Features.Categories.Dtos;
using DailyShop.Business.Features.Products.Dtos;
using DailyShop.Business.Services.Repositories;
using DailyShop.Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;


namespace DailyShop.Business.Features.Products.Queries.GetListProduct
{
    public class GetListProductQuery:IRequest<List<GetListProductDto>>
    {
        public class GetListProductQueryHandler : IRequestHandler<GetListProductQuery, List<GetListProductDto>>
        {
            private readonly IProductRepository _productRepository;
            private readonly IMapper _mapper;

            public GetListProductQueryHandler(IProductRepository productRepository, IMapper mapper)
            {
                _productRepository = productRepository;
                _mapper = mapper;
            }

            public async Task<List<GetListProductDto>> Handle(GetListProductQuery request, CancellationToken cancellationToken)
            {
                List<Product> products = await _productRepository.Query().Include(p => p.Colors).ThenInclude(c=>c.Color).Include(p => p.Sizes).Include(p => p.ProductImages).Include(p => p.User).ToListAsync();
                List<GetListProductDto> mappedGetListProduct = _mapper.Map<List<GetListProductDto>>(products);
              
				foreach (var mappedProduct in mappedGetListProduct)
                {
                    if(mappedProduct.SellerName=="admin admin")
                    {
                        mappedProduct.SellerName = "DailyShop";
                    }
                }
                
                return mappedGetListProduct;
            }
        }
    }
}
