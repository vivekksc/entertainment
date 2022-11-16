using Amazon.S3;
using FakeItEasy;
using microservices.entertainment.Models;
using microservices.entertainment.Repositories;
using microservices.entertainment.Repositories.Interfaces;
using microservices.entertainment.Responses;
using microservices.entertainment.Services;
using microservices.entertainment.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static microservices.entertainment.Consumer.Commands.Voucher.GenerateVoucher;

namespace microservices.entertainment.Tests.Commands.Voucher
{
    public class GenerateVoucherTests
    {
        private IVoucherService _voucherService;
        private IHttpContextAccessor _httpContextAccessor;

        public GenerateVoucherTests()
        {
            _voucherService = A.Fake<IVoucherService>();
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();
        }

        [Fact]
        public async void GenerateVoucherSuccess()
        {
            // ASSIGN
            var userId = Guid.NewGuid();
            var voucherTicket = Guid.NewGuid();
            var command = new Command()
            {
                Token = voucherTicket
            };
            var testRedemptionData = new RedemptionModel
            {
                RedemptionId = 123456,
                UserId = userId,
                VoucherTicket = voucherTicket,
                RedemptionValue = 25,
                RedemptionValueCurrency = "$",
                RedemptionDate = DateTime.Now
            };
            var testVoucherResponseData = VoucherResponseModel.Success(string.Empty);

            // ACT
            A.CallTo(() => _voucherService.GetRedemptionAsync(userId, voucherTicket))
                                                .Returns(A<RedemptionModel>.Ignored);
            A.CallTo(() => _voucherService.GenerateVoucherImageAsync(testRedemptionData))
                                               .Returns(Task.FromResult(testVoucherResponseData));
            var handler = new Handler(_voucherService, _httpContextAccessor);
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Result.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async void GenerateVoucherFail()
        {
            // ASSIGN
            var userId = Guid.NewGuid();
            var voucherTicket = Guid.NewGuid();
            var command = new Command()
            {
                Token = voucherTicket
            };
            var testRedemptionData = new RedemptionModel
            {
                RedemptionId = 123456,
                UserId = userId,
                VoucherTicket = voucherTicket,
                RedemptionValue = 25,
                RedemptionValueCurrency = "$",
                RedemptionDate = DateTime.Now
            };
            var testVoucherResponseData = VoucherResponseModel.Failed(String.Empty);

            // ACT
            A.CallTo(() => _voucherService.GetRedemptionAsync(userId, voucherTicket))
                                                .Returns(A<RedemptionModel>.Ignored);
            A.CallTo(() => _voucherService.GenerateVoucherImageAsync(testRedemptionData))
                                               .Returns(Task.FromResult(testVoucherResponseData));
            var handler = new Handler(_voucherService, _httpContextAccessor);
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }
    }
}
