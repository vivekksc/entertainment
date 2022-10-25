using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Entertainment.Microservices.Responses;
using microservices.entertainment.Models;
using microservices.entertainment.Repositories.Interfaces;
using microservices.entertainment.Responses;
using microservices.entertainment.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace microservices.entertainment.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly string contentRootPath;

        private readonly IConfiguration _configuration;
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IWebHostEnvironment environment, IConfiguration configuration, IVoucherRepository voucherRepository)
        {
            contentRootPath = environment.ContentRootPath;
            _configuration = configuration;
            _voucherRepository = voucherRepository;
        }

        /// <inheritdoc/>
        public async Task<RedemptionResponseModel> GetRedemptionAsync(Guid userId, Guid voucherTicket)
        {
            try
            {
                var response = await _voucherRepository.GetRedemptionAsync(userId, voucherTicket).ConfigureAwait(false);
                return response != null
                    ? RedemptionResponseModel.Success(response)
                    : RedemptionResponseModel.NotFound();
            }
            catch (Exception ex)
            {
                return RedemptionResponseModel.Failed(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<string> GenerateVoucherImageAsync(RedemptionModel redemption)
        {
            var voucherImage = CreateVoucherImage(redemption);

            return await SaveVoucherImageJpegAsync(voucherImage, $"{redemption.UserId}_{redemption.VoucherTicket}").ConfigureAwait(false);
        }

        #region PRIVATE METHODS

        /// <summary>
        /// Creates voucher image with the redemption details.
        /// </summary>
        /// <param name="redemption"></param>
        /// <returns></returns>
        private byte[] CreateVoucherImage(RedemptionModel redemption)
        {
            if (redemption == null)
                return new byte[0];

            var redemptionId = $"REDEMPTION ID: {redemption.RedemptionId}";
            var redemptionDate = $"Expires: {redemption.RedemptionDate:MMM dd, yyyy}";
            var redemptionValue = redemption.RedemptionValue.ToString();
            var redemptionValueCurrency = redemption.RedemptionValueCurrency;

            PointF redemptionValueCurrency_location = new(285f, 92f);
            PointF redemptionValue_location = new(295f, 92f);
            PointF redemptionId_location = new(110f, 145f);
            PointF redemptionDate_location = new(130f, 170f);

            var sampleImageFilePath = Path.Combine(contentRootPath, "Assets/Images/sample.jpg");
            Image imgBackground = Image.FromFile(sampleImageFilePath);
            using (Graphics graphics = Graphics.FromImage(imgBackground))
            {
                using (Font arialFont = new Font("Arial", 14, FontStyle.Bold))
                {
                    graphics.DrawString(redemptionValueCurrency, arialFont, Brushes.DarkBlue, redemptionValueCurrency_location);
                    graphics.DrawString(redemptionValue, arialFont, Brushes.DarkBlue, redemptionValue_location);
                }

                using (Font arialFont = new Font("Arial", 12, FontStyle.Bold))
                {
                    graphics.DrawString(redemptionId, arialFont, Brushes.Black, redemptionId_location);
                    graphics.DrawString(redemptionDate, arialFont, Brushes.Black, redemptionDate_location);
                }
            }

            var ms = new MemoryStream();
            imgBackground.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }

        /// <summary>
        /// Saves the voucher image.
        /// </summary>
        /// <param name="voucherImageBytes">byte array.</param>
        private async Task<string> SaveVoucherImageJpegAsync(byte[] voucherImageBytes, string imageFileName)
        {
            #region Save into local directory

            // Temporarily saving into application folder.
            //var destFilePath = Path.Combine(contentRootPath, $"Assets\\Images\\{imageFileName}.jpg");
            //System.IO.File.WriteAllBytes(destFilePath, voucherImageBytes);

            #endregion

            #region Upload to S3 bucket

            var awsS3BucketName = _configuration.GetValue<string>("AwsS3Bucket:Name");
            var awsS3BucketSubDirectoryPath = _configuration.GetValue<string>("AwsS3Bucket:SubDirectoryPath");
            var awsS3BucketRegion = _configuration.GetValue<RegionEndpoint>("AwsS3Bucket:Region");
            IAmazonS3 _awsS3Client = new AmazonS3Client(awsS3BucketRegion);

            if (!string.IsNullOrWhiteSpace(awsS3BucketSubDirectoryPath))
                awsS3BucketName = $"{awsS3BucketName}@/{awsS3BucketSubDirectoryPath}";

            #region Option1: using PUT API
            var awsS3UploadRequest = new PutObjectRequest
            {
                BucketName = awsS3BucketName,
                Key = imageFileName,
                ContentType = "text/plain",
                InputStream = new MemoryStream(voucherImageBytes)
            };
            awsS3UploadRequest.Metadata.Add("x-amz-meta-title", imageFileName);

            var aswS3UploadResponse = await _awsS3Client.PutObjectAsync(awsS3UploadRequest).ConfigureAwait(false);
            #endregion

            #region Option2: using TransferUtility
            //TransferUtility utility = new TransferUtility(_awsS3Client);
            //TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
            //{
            //    BucketName = awsS3BucketName,
            //    Key = imageFileName,
            //    InputStream = new MemoryStream(voucherImageBytes)
            //};

            //await utility.UploadAsync(uploadRequest).ConfigureAwait(false);
            #endregion

            #endregion

            return String.Empty; // ToDo: return the S3 bucket URL of the image.
        }

        #endregion
    }
}
