using FakeItEasy;
using microservices.entertainment.Repositories.Interfaces;
using microservices.entertainment.Services.Interfaces;
using Shouldly;
using System.Runtime.InteropServices;
using System;
using microservices.entertainment.Models;
using microservices.entertainment.Data.DbModels;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using microservices.entertainment.Services;
using Amazon.S3.Model;
using Amazon.S3;
using ThirdParty.BouncyCastle.Utilities.IO.Pem;
using microservices.entertainment.Utils;

namespace microservices.entertainment.Tests.Services
{
    public class VoucherServiceTests
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAmazonS3 _awsS3Client;

        public VoucherServiceTests()
        {
            _voucherRepository = A.Fake<IVoucherRepository>();
            _configuration = A.Fake<IConfiguration>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();
            _awsS3Client = A.Fake<IAmazonS3>();
        }

        [Fact]
        public async void GetRedemptionShouldExist()
        {
            // ASSIGN
            var userId = Guid.NewGuid();
            var voucherTicket = Guid.NewGuid();
            var testData = new RedemptionModel
            {
                RedemptionId = 123456,
                UserId = userId,
                VoucherTicket = voucherTicket,
                RedemptionValue = 25,
                RedemptionValueCurrency = "$",
                RedemptionDate = DateTime.Now
            };

            // ACT
            A.CallTo(() => _voucherRepository.GetRedemptionAsync(userId, voucherTicket))
                                                .Returns(Task.FromResult(testData));
            var voucherService = new VoucherService(_webHostEnvironment, _configuration, _voucherRepository);
            var response = await voucherService.GetRedemptionAsync(userId, voucherTicket).ConfigureAwait(false);

            // ASSERT
            response?.Redemption.ShouldNotBeNull();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Found);
            response.Redemption.UserId.ShouldBe(userId);
            response.Redemption.VoucherTicket.ShouldBe(voucherTicket);
        }

        [Fact]
        public async void GetRedemptionShouldNotExist()
        {
            // ASSIGN
            var userId = Guid.NewGuid();
            var voucherTicket = Guid.NewGuid();
            RedemptionModel testData = null;

            // ACT
            A.CallTo(() => _voucherRepository.GetRedemptionAsync(userId, voucherTicket))
                                                .Returns(Task.FromResult<RedemptionModel>(testData));
            var voucherService = new VoucherService(_webHostEnvironment, _configuration, _voucherRepository);
            var response = await voucherService.GetRedemptionAsync(userId, voucherTicket).ConfigureAwait(false);

            // ASSERT
            response?.Redemption.ShouldBeNull();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GenerateVoucherImageSuccess()
        {
            // ASSIGN
            var testData = new RedemptionModel
            {
                RedemptionId = 123456,
                UserId = Guid.NewGuid(),
                VoucherTicket = Guid.NewGuid(),
                RedemptionValue = 25,
                RedemptionValueCurrency = "$",
                RedemptionDate = DateTime.Now
            };

            // ACT
            A.CallTo(() => _awsS3Client.PutObjectAsync(A<PutObjectRequest>.Ignored))
                                                .Returns(Task.FromResult(new PutObjectResponse()));
            var voucherService = new VoucherService(_webHostEnvironment, _configuration, _voucherRepository);
            var response = await voucherService.GenerateVoucherImageAsync(testData).ConfigureAwait(false);

            // ASSERT
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async void GenerateVoucherImageFailed()
        {
            // ASSIGN
            var testData = new RedemptionModel
            {
                RedemptionId = 123456,
                UserId = Guid.NewGuid(),
                VoucherTicket = Guid.NewGuid(),
                RedemptionValue = 25,
                RedemptionValueCurrency = "$",
                RedemptionDate = DateTime.Now
            };

            // ACT
            A.CallTo(() => _awsS3Client.PutObjectAsync(A<PutObjectRequest>.Ignored))
                                                .Throws<InvalidOperationException>();
            var voucherService = new VoucherService(_webHostEnvironment, _configuration, _voucherRepository);
            var response = await voucherService.GenerateVoucherImageAsync(testData).ConfigureAwait(false);

            // ASSERT
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.InternalServerError);
            response.Messages.Count.ShouldBe(1);
            response.Messages[0].ShouldBeNullOrEmpty();
        }
    }
}