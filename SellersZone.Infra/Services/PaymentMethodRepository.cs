using Microsoft.EntityFrameworkCore;
using SellersZone.Core.DTOs;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Wrapper;
using SellersZone.Infra.Helpers;

namespace SellersZone.Infra.Services
{
    internal class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly StoreContext _db;
        public PaymentMethodRepository(StoreContext db)
        {
            _db = db;
        }

        public Response<PaymentMethodDto> GetPaymentMethod(RequestParam param)
        {
            try
            {
                IQueryable<PaymentMethod> query = _db.PaymentMethods.Include(c => c.CustomFields);

                if (param.Id != null)
                {
                    query = query.Where(p => p.Id == param.Id);
                }

                if (!string.IsNullOrWhiteSpace(param.Search))
                {
                    query = query.Where(p => p.Name!.Contains(param.Search) || p.NameAr!.Contains(param.Search));
                }

                var paymentMethodDto = query
                     .OrderByDescending(payment => payment.Ordering)
                     .Select(payment => new PaymentMethodDto
                     {
                         Id = payment.Id,
                         Name = string.IsNullOrEmpty(payment.Name) ? payment.NameAr : payment.Name,
                         NameAr = payment.NameAr,                     
                         Ordering = payment.Ordering,
                         CreationDate = payment.CreatedAt,
                         ModificationDate = payment.UpdatedAt,
                         CustomFields = payment.CustomFields != null ? payment.CustomFields.Select(cf => new CustomFieldDto()
                         {
                             Id = cf.Id,
                             FieldName = cf.FieldName,
                             FieldNameAr = cf.FieldNameAr,
                             FieldType = cf.FieldType,
                             IsRequired = cf.IsRequired,
                             FieldPlaceHolder = cf.FieldPlaceHolder,
                             FieldPlaceHolderAr = cf.FieldPlaceHolderAr,
                             FieldDefualtValue = cf.FieldDefualtValue,
                             FieldDefualtValueAr = cf.FieldDefualtValueAr,
                             Ordering = cf.Ordering,
                             CreationDate = cf.CreatedAt
                         }).ToList() : new List<CustomFieldDto>(),
                     }).ToList();

                return new Response<PaymentMethodDto>
                {
                    Data = paymentMethodDto,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<PaymentMethodDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

        public Response<PaymentMethodDto> AddPaymentMethod(PaymentMethodDto dto)
        {
            try
            {
                if (DtoValidator.AreAnyNullOrEmpty(dto.Name, dto.NameAr))
                {
                    throw new Exception("Please fill all required fields");
                }

                var paymentMethod = new PaymentMethod
                {
                    Name = dto.Name,
                    NameAr = dto.NameAr,
                    Ordering = dto.Ordering,
                    CustomFields = dto.CustomFields?.Select(cf => new CustomField
                    {
                        FieldName = cf.FieldName,
                        FieldNameAr = cf.FieldNameAr,
                        FieldType = cf.FieldType,
                        IsRequired = cf.IsRequired,
                        FieldPlaceHolder = cf.FieldPlaceHolder,
                        FieldPlaceHolderAr = cf.FieldPlaceHolderAr,
                        FieldDefualtValue = cf.FieldDefualtValue,
                        FieldDefualtValueAr = cf.FieldDefualtValueAr,
                        Ordering = cf.Ordering
                    }).ToList()
                };

                // Add the new payment method to the database
                _db.PaymentMethods.Add(paymentMethod);
                _db.SaveChanges();

                return new Response<PaymentMethodDto>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                // Handle exception
                return new Response<PaymentMethodDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }


        public Response<PaymentMethodDto> UpdatePaymentMethod(PaymentMethodDto paymentMethodDto)
        {
            try
            {
                if (DtoValidator.AreAnyNullOrEmpty(paymentMethodDto.Name, paymentMethodDto.NameAr))
                {
                    throw new Exception("Please fill all required fields");
                }

                var existingPaymentMethod = _db.PaymentMethods
                    .Include(pm => pm.CustomFields)
                    .FirstOrDefault(pm => pm.Id == paymentMethodDto.Id);

                if (existingPaymentMethod == null)
                {
                    throw new Exception("Payment method not found");
                }

                // Update payment method 
                existingPaymentMethod.Name = paymentMethodDto.Name;
                existingPaymentMethod.NameAr = paymentMethodDto.NameAr;
                existingPaymentMethod.Ordering = paymentMethodDto.Ordering;

                // Handle custom fields
                var updatedCustomFields = paymentMethodDto.CustomFields.Select(cf => new CustomField
                {
                    Id = cf.Id,
                    FieldName = cf.FieldName,
                    FieldNameAr = cf.FieldNameAr,
                    FieldType = cf.FieldType,
                    IsRequired = cf.IsRequired,
                    FieldPlaceHolder = cf.FieldPlaceHolder,
                    FieldPlaceHolderAr = cf.FieldPlaceHolderAr,
                    FieldDefualtValue = cf.FieldDefualtValue,
                    FieldDefualtValueAr = cf.FieldDefualtValueAr,
                    Ordering = cf.Ordering,
                    PaymentMethodId = existingPaymentMethod.Id
                }).ToList();

                // Remove custom fields that are not in the updated list
                _db.CustomFields.RemoveRange(existingPaymentMethod.CustomFields
                    .Where(cf => !updatedCustomFields.Any(u => u.Id == cf.Id)));

                // Update or add custom fields
                foreach (var updatedField in updatedCustomFields)
                {
                    var existingField = existingPaymentMethod.CustomFields
                        .FirstOrDefault(cf => cf.Id == updatedField.Id);

                    if (existingField != null)
                    {
                        // Update existing field
                        _db.Entry(existingField).CurrentValues.SetValues(updatedField);
                    }
                    else
                    {
                        // Add new field
                        existingPaymentMethod.CustomFields.Add(updatedField);
                    }
                }

                _db.SaveChanges();

                return new Response<PaymentMethodDto>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<PaymentMethodDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true
                };
            }
        }

    }
}
