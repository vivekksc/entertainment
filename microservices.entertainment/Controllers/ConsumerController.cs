using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using microservices.entertainment.Data.Contracts;
using microservices.entertainment.Data;
using microservices.entertainment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;

namespace microservices.entertainment.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ConsumerController : ControllerBase
    {
        private readonly string contentRootPath;

        private readonly IConfiguration _configuration;

        private readonly IVoucherDataManager _data;

        public ConsumerController(IWebHostEnvironment environment, IConfiguration configuration, IVoucherDataManager data)
        {
            contentRootPath = environment.ContentRootPath;
            _configuration = configuration;
            _data = data;
        }

        #region ACTIONS

        /// <summary>
        /// Get voucher details.
        /// </summary>
        /// <param name="token">Voucher Access Token from Redemption call.</param>
        /// <param name="type">Voucher type.</param>
        /// <returns></returns>
        [HttpGet(Name = "Voucher")]
        public async Task<ActionResult> GetVoucher(Guid token, string type)
        {
            try
            {
                var response = new ApiResponse();

                // Assuming the user info will be acquired from the Request context
                Guid userId = Guid.NewGuid(); // TODO: Assign the user info from context.

                // 1. Validate that the incoming request is entitled to a voucher
                var validationResult = ValidateVoucherRedemption(userId, token);

                if (validationResult.IsValid && validationResult.Redemption != null)
                {
                    // 2. Create a jpg image for this user with the correct voucher details
                    // 3. Upload the image to the S3 bucket that stores the voucher images
                    var imageUrl = await GenerateVoucherImageAsync(validationResult.Redemption).ConfigureAwait(false);

                    response.Messages = new List<string> { "Success" };
                    response.Result = new Voucher { Uri = imageUrl };
                }
                else
                {
                    response.Messages = new List<string> { "Invalid request! Provided voucher token is not entitled for voucher." };
                    return BadRequest(response);
                }

                return Created(string.Empty, response);
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Generates voucher image.
        /// </summary>
        /// <param name="redemption"></param>
        /// <returns></returns>
        private async Task<string> GenerateVoucherImageAsync(Redemption redemption)
        {
            var voucherImage = CreateVoucherImage(redemption);

            return await SaveVoucherImageJpegAsync(voucherImage, $"{redemption.User_id}_{redemption.Voucher_ticket}").ConfigureAwait(false);
        }

        /// <summary>
        /// Creates voucher image with the redemption details.
        /// </summary>
        /// <param name="redemption"></param>
        /// <returns></returns>
        private byte[] CreateVoucherImage(Redemption redemption)
        {
            if (redemption == null)
                return new byte[0];

            var redemptionId = $"REDEMPTION ID: {redemption.Redemption_id}";
            var redemptionDate = $"Expires: {redemption.Redemption_date:MMM dd, yyyy}";
            var redemptionValue = redemption.Redemption_value.ToString();
            var redemptionValueCurrency = redemption.Redemption_value_currency;

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

        #region DATA

        /// <summary>
        /// Validate that the request is entitled to a voucher.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="voucherTicket"></param>
        private (bool IsValid, Redemption Redemption) ValidateVoucherRedemption(Guid userId, Guid voucherTicket)
        {
            var validationResult = (false, new Redemption());

            // Check the redemption table to ensure that the user and the voucher ticket match and are still valid
            // Assuming that for the given user_id and voucher ticket there will be single record in the DB.
            var redemption = _data.GetRedemption(userId, voucherTicket);
            
            if (redemption?.User_id == userId)
            {
                validationResult = (true, redemption);
            }

            return validationResult;
        }

        #endregion

    }
}