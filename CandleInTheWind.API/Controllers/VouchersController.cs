﻿using CandleInTheWind.API.Models.Vouchers;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VouchersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VouchersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public ActionResult GetAllVouchers()
        {
            return Ok(_context.Vouchers);
        }

        [HttpGet]
        public async Task<ActionResult> GetVouchers()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            var userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users.FindAsync(userId);

            var vouchers = await _context.Vouchers
                                   .Where(voucher => voucher.Points <= user.Points && 
                                                     voucher.Expired > DateTime.Now && 
                                                     voucher.Quantity > 0)
                                   .ToListAsync();

            var voucherResponses = vouchers.Select(voucher => ToVoucherDTO(voucher));

            return Ok(voucherResponses);
        }

        private VoucherDTO ToVoucherDTO(Voucher voucher)
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
    }
}
