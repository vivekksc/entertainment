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

        public ConsumerController(IWebHostEnvironment environment)
        {
            contentRootPath = environment.ContentRootPath;
        }

        #region ACTIONS

        /// <summary>
        /// Get voucher details.
        /// </summary>
        /// <param name="token">Voucher Access Token from Redemption call.</param>
        /// <param name="type">Voucher type.</param>
        /// <returns></returns>
        [HttpGet(Name = "Voucher")]
        public ActionResult GetVoucher(Guid token, string type)
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
                    var imageUrl = GenerateVoucherImage(validationResult.Redemption);

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
        private string GenerateVoucherImage(Redemption redemption)
        {
            var voucherImage = CreateVoucherImage(redemption);

            return SaveVoucherImageJpeg(voucherImage, $"{redemption.User_id}_{redemption.Voucher_ticket}");
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
        private string SaveVoucherImageJpeg(byte[] voucherImageBytes, string imageFileName)
        {
            // TODO : Upload to S3 bucket.

            // Temporarily saving into application folder.
            var destFilePath = Path.Combine(contentRootPath, $"Assets\\Images\\{imageFileName}.jpg");
            System.IO.File.WriteAllBytes(destFilePath, voucherImageBytes);
            return destFilePath;
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
            // TODO: Fetch the DB record for the given user_id and voucher ticket.
            // Check the redemption table to ensure that the user and the voucher ticket match and are still valid

            // Assuming for the given user_id and voucher ticket there will be single record in the DB.

            return (true, new Redemption
            {
                Redemption_id = 123456,
                User_id = userId,
                Voucher_ticket = voucherTicket,
                Redemption_value = 25,
                Redemption_value_currency = "$",
                Redemption_date = DateTime.Now
            });
        }

        #endregion

    }
}