using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Wrapper.Errors
{
    public class ApiStatusError
    {
        public ApiStatusError(int statusCode = 0, string errorMessage = null)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage ?? GetDefaultMessageForStatusCode(statusCode);

        }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "تأكد من تعبئة جميع الحقول",
                401 => "ليس لديك صلاحية",
                404 => "لا يمكن العثور على المورد المطلوب.",
                500 => "حدث خطأ في الخادم",
                _ => ""
            };
        }
    }
}
