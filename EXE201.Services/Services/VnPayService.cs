using EXE201.Services.Interfaces;
using EXE201.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Serenity_Solution.Unity.VnPayLibrary;

namespace EXE201.Services.Services
{
    public class VnPayService : IVnPayServicecs
    {
        private readonly IConfiguration _config;
        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        //doctor detail
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            string paymentUrl = "";

            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VNPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", model.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán chi phí tư vấn cho: " + model.Psychologist.Id + "." + model.Psychologist.Name);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VNPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_OrderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }
            string orderDescription = string.Empty;
            if (vnp_OrderInfo.StartsWith("Thanh toán chi phí tư vấn cho: "))
            {
                orderDescription = vnp_OrderInfo.Split(new string[] { "Thanh toán chi phí tư vấn cho: " }, StringSplitOptions.None)[1].Split('.')[0];
            }
            else if (vnp_OrderInfo.StartsWith("Xử lí yêu cầu của bác sĩ: "))
            {
                orderDescription = vnp_OrderInfo.Split(new string[] { "Xử lí yêu cầu của bác sĩ: " }, StringSplitOptions.None)[1];
            }
            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = orderDescription,
                OrderId = vnp_OrderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
        //test payment 
        public string CreateTestPaymentUrl(HttpContext context, TestPaymentRequest model)
        {
            string paymentUrl = "";

            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VNPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", model.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán chi phí tư vấn cho bài test");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnTestUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public TestPaymentRespone TestPaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VNPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_OrderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new TestPaymentRespone
                {
                    Success = false
                };
            }
            string orderDescription = string.Empty;
            if (vnp_OrderInfo.StartsWith("Thanh toán chi phí tư vấn cho: "))
            {
                orderDescription = vnp_OrderInfo.Split(new string[] { "Thanh toán chi phí tư vấn cho: " }, StringSplitOptions.None)[1].Split('.')[0];
            }
            else if (vnp_OrderInfo.StartsWith("Xử lí yêu cầu của bác sĩ: "))
            {
                orderDescription = vnp_OrderInfo.Split(new string[] { "Xử lí yêu cầu của bác sĩ: " }, StringSplitOptions.None)[1];
            }
            return new TestPaymentRespone
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = orderDescription,
                OrderId = vnp_OrderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }

    }
}
