using EXE201.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EXE201.Services.Models.OrderInfoModel;

namespace EXE201.Services.Interfaces
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(TestPaymentRequest model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
