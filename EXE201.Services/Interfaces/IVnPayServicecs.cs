using EXE201.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.Services.Interfaces
{
    public interface IVnPayServicecs
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);

        string CreateTestPaymentUrl(HttpContext context, TestPaymentRequest model);
        TestPaymentRespone TestPaymentExecute(IQueryCollection collections);

    }
}
