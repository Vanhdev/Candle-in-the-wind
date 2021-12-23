﻿using CandleInTheWind.API.Models.Carts;
using CandleInTheWind.API.Models.Comments;
using CandleInTheWind.API.Models.Orders;
using CandleInTheWind.API.Models.Posts;
using CandleInTheWind.API.Models.Products;
using CandleInTheWind.API.Models.Users;
using CandleInTheWind.API.Models.Vouchers;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Extensions
{
    public static class Mapper
    {
        public static ProfileDTO ToDTO(this User user)
        {
            return new ProfileDTO()
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                GenderName = user.Gender.GetEnumName(),
                Points = user.Points,
            };
        }

        public static ProductDTO ToDTO(this Product product)
        {
            return new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.Category.Id,
                CategoryName = product.Category.Name,
            };
        }

        public static VoucherDTO ToDTO(this Voucher voucher)
        {
            return new VoucherDTO()
            {
                Id = voucher.Id,
                Name = voucher.Name,
                Expired = voucher.Expired,
                Value = voucher.Value,
                Points = voucher.Points,
            };
        }

        public static CartDTO ToDTO(this Cart cart)
        {
            var product = cart.Product;
            return new CartDTO()
            {
                ProductId = cart.ProductId,
                ProductName = product.Name,
                ProductImageUrl = product.ImageUrl,
                UnitPrice = product.Price,
                Stock = product.Stock,
                Quantity = cart.Quantity,
                Price = product.Price * cart.Quantity,
            };
        }

        public static CommentDTO ToDTO(this Comment comment)
        {
            return new CommentDTO
            {
                Id = comment.Id,
                UserId = comment.User.Id,
                UserName = comment.User.UserName,
                PostId = comment.Post.Id,
                Content = comment.Content,
                Time = comment.Time
            };
        }

        public static OrderDTO ToDTO(this Order order)
        {
            return new OrderDTO
            {
                UserId = order.UserId,
                UserName = order.User.UserName,
                Id = order.Id,
                PurchaseDate = order.PurchasedDate,
                Total = order.Total,
                Status = order.Status,
                StatusName = order.Status.GetEnumName(),
                ProductName = order.OrderProducts.Select(op => op.Product.Name),
                ProductIDs = order.OrderProducts.Select(op => (int)op.ProductId),
                UnitPrices = order.OrderProducts.Select(op => op.Product.Price),
                Quantity = order.OrderProducts.Select(op => op.Quantity),
                ProductImageUrls = order.OrderProducts.Select(op => op.Product.ImageUrl),
                VoucherId = order.Voucher?.Id,
                VoucherName = order.Voucher?.Name,
                VoucherValue = order.Voucher?.Value
            };
        }

        public static SimpleOrderDTO ToSimpleOrderDTO(this Order order)
        {
            return new SimpleOrderDTO
            {
                Id = order.Id,
                PurchaseDate = order.PurchasedDate,
                Total = order.Total,
                Status = order.Status,
                StatusName = order.Status.GetEnumName(),
                ProductName = order.OrderProducts.Select(op => op.Product.Name)
            };
        }

        public static PostDTO ToDTO(this Post post, ApplicationDbContext context)
        {
            var postID = post.Id;
            var Comment_Count = context.Comments.Count(Comment => Comment.Post.Id == postID);

            return new PostDTO
            {
                Id = postID,
                Title = post.Title,
                Content = post.Content,
                ApprovedAt = post.ApprovedAt,
                Commentable = post.Commentable,
                CommentCount = Comment_Count,
                UserId = post.User.Id,
                UserName = post.User.UserName,
            };
        }
    }
}