﻿using AutoMapper;
using Core.CrossCuttingConcerns.Exceptions;
using DailyShop.Business.Features.Products.Dtos;
using DailyShop.Business.Services.Repositories;
using DailyShop.Business.Services.Repositories.Dapper;
using DailyShop.Entities.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyShop.Business.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public int ProductId { get; set; }
        public UpdatedProductDto UpdatedProductDto { get; set; }
        public List<string>? ProductImagesPath { get; set; }
        public string BodyImagePath { get; set; }
        public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
        {
            private readonly IProductRepository _productRepository;
            private readonly IProductColorRepository _colorRepository;
            private readonly IProductSizeRepository _sizeRepository;
            private readonly IProductImageRepository _productImageRepository;
            private readonly IMapper _mapper;
            public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper, IProductImageRepository productImageRepository, IProductColorRepository colorRepository, IProductSizeRepository sizeRepository)
            {
                _productRepository = productRepository;
                _mapper = mapper;
                _productImageRepository = productImageRepository;
                _colorRepository = colorRepository;
                _sizeRepository = sizeRepository;
            }
            public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
            {
                var product = await _productRepository.Query().Include(c => c.Colors).Include(s => s.Sizes).Include(pi => pi.ProductImages).FirstOrDefaultAsync(x => x.Id == request.ProductId);
                List<Color> colors = await _colorRepository.Query().ToListAsync();
                List<Size> sizes = await _sizeRepository.Query().ToListAsync();

                if (product == null)
                    throw new BusinessException("Güncellemek istediğiniz ürün bulunamadı.");

                product.Name = request.UpdatedProductDto.Name;
                product.Price = request.UpdatedProductDto.Price;
                product.Description = request.UpdatedProductDto.Description;
                product.Stock = request.UpdatedProductDto.Stock;
                product.Status = request.UpdatedProductDto.Status;
                product.CategoryId = request.UpdatedProductDto.CategoryId;
                product.UpdatedAt = DateTime.Now;

                //color-size-image deleted
                if (product.Colors.Count != 0)
                {
                    foreach (var color in product.Colors)
                    {
                        product.Colors.Remove(color);
                        await _colorRepository.DeleteAsync(color.Color, false);
                        //await _dpProductColorRepository.DeleteProductColor(color.ProductId, color.ColorId);
                    }
                }
                if (product.Sizes.Count != 0)
                {
                    foreach (var size in product.Sizes)
                    {
                        product.Sizes.Remove(size);
                        await _sizeRepository.DeleteAsync(size.Size, false);
                    }
                }
                if (product.ProductImages.Any())
                {
                    foreach (var item in product.ProductImages)
                    {
                        product.ProductImages.Remove(item);
                        await _productImageRepository.DeleteAsync(item, false);
                    }
                }
                //color-size-image added
                if (request.ProductImagesPath.Count != 0)
                {
                    foreach (var image in request.ProductImagesPath)
                    {
                        ProductImage productImage = new() { Name = image };
                        product.ProductImages.Add(productImage);
                    }
                }
                foreach (var color in request.UpdatedProductDto.Colors)
                {
                    if (colors.Find(x => x.Name.ToLower() == color.ToLower()) != null)
                    {
                        int colorId = colors.Find(x => x.Name.ToLower() == color.ToLower()).Id;
                        product.Colors?.Add(new ProductColor() { ColorId = colorId });
                    }
                    else
                    {
                        Color newColor = new() { Name = color };
                        product.Colors?.Add(new ProductColor() { Color = newColor });
                    }
                }
                foreach (var size in request.UpdatedProductDto.Sizes)
                {
                    if (sizes.Find(x => x.Name.ToLower() == size.ToLower()) != null)
                    {
                        int sizeId = sizes.Find(x => x.Name.ToLower() == size.ToLower()).Id;
                        product.Sizes?.Add(new ProductSize() { SizeId = sizeId });
                    }
                    else
                    {
                        Size newSize = new() { Name = size };
                        product.Sizes?.Add(new ProductSize() { Size = newSize });
                    }
                }
                if (request.BodyImagePath != null)
                {
                    product.BodyImage = request.BodyImagePath;
                }
                await _productRepository.UpdateAsync(product);
            }
        }
    }
}
