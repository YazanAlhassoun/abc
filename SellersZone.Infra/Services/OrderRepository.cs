using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SellersZone.Core.DTOs;
using SellersZone.Core.Enums;
using SellersZone.Core.Interfaces;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;
using SellersZone.Core.Wrapper;
using SellersZone.Infra.Helpers;
using System.Linq;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SellersZone.Infra.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public OrderRepository(StoreContext db, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


        public Response<OrderDto> GetOrder(RequestParam param, ClaimsPrincipal userClaims)
         => param.Id == null ? GetOrders(param, userClaims) : GetOrderById(param, userClaims);


        // pass user id to get (User Order) for admin
        // pass token to get (User Order) for client
        // paging on Oreders
        public Response<OrderDto> GetOrders(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                bool isAdmin = userClaims.IsAdmin();

                // Extract user UserId
                var userId = string.Empty;
                if (!isAdmin)
                {
                    var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                    userId = user.Id;
                }

                // filter on status / sku / from,to date / bayer, seller, phone and store names
                IQueryable<Order> query = _db.Orders.Where(o =>
                    (!param.Status.HasValue || o.Status == Convert.ToInt32(param.Status)) &&
                    (!param.FromDate.HasValue || o.CreatedAt >= param.FromDate.Value) &&
                    (!param.ToDate.HasValue || o.CreatedAt <= param.ToDate.Value) &&
                    ((string.IsNullOrEmpty(userId) && isAdmin) || o.AppUserId == userId)).Include(d => d.State);

                if (!string.IsNullOrWhiteSpace(param.Search))
                {
                    var searchUpper = param.Search.ToUpper();
                    query = query.Where(o =>
                        (o.SKU.ToUpper().Contains(searchUpper)) ||
                        (o.BuyerName != null && o.BuyerName.ToUpper().Contains(searchUpper)) ||
                        (o.BuyerPhone != null && o.BuyerPhone.Contains(searchUpper)) ||
                        (o.Note != null && o.Note.ToUpper().Contains(searchUpper))
                    );
                }

                int totalOrders = query.Count();
                var orderDtoList = query.Skip((param.Page - 1) * param.ItemPerPage)
                                        .Take(param.ItemPerPage)
                                        .OrderBy(o => o.Id)
                                        .Select(o => new OrderDto
                                        {
                                            Id = o.Id,
                                            BuyerName = o.BuyerName,
                                            BuyerPhone = o.BuyerPhone,
                                            DeliveryTo = o.State.NameAr,
                                            CreationDate = o.CreatedAt,
                                            Status = o.Status,
                                            StoreName = o.StoreName,
                                            SubTotal = o.SubTotal,
                                            DeliveryAmount = o.State.Price,
                                            Total = o.SubTotal + o.State.Price,
                                        }).ToList();

                var paginationMetaData = new PaginationMetaData();
                if (totalOrders > param.ItemPerPage)
                {
                    var requestUrl = _httpContextAccessor?.HttpContext?.Request;
                    paginationMetaData = new PaginationMetaData(param.Page, totalOrders, param.ItemPerPage, requestUrl);
                }

                return new Response<OrderDto>
                {
                    Data = orderDtoList,
                    Pagination = paginationMetaData,
                    IsSuccess = true
                };
            }
            catch (ArgumentNullException argEx)
            {
                return new Response<OrderDto>
                {
                    IsError = true,
                    ErrorMessage = argEx.Message,
                    StatusCode = 400
                };
            }
            catch (Exception ex)
            {
                return new Response<OrderDto>
                {
                    IsError = true,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public Response<OrderDto> GetOrderById(RequestParam param, ClaimsPrincipal userClaims)
        {
            try
            {
                bool isAdmin = userClaims.IsAdmin();
                if (isAdmin && param?.Id == null) throw new ArgumentNullException(nameof(param.Id), "Order id can't be null");

                var orderQuery = _db.Orders
                    .Include(o => o.ProductsOrder)
                    .ThenInclude(po => po.Product)
                    .Include(o => o.State)
                    .AsQueryable();

                if (isAdmin)
                {
                    orderQuery = orderQuery.Where(o => o.Id == param.Id);
                }
                else
                {
                    var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                    if (user == null)
                    {
                        throw new Exception("User not found");
                    }

                    orderQuery = orderQuery.Where(o => o.AppUserId == user.Id && o.Id == param.Id);
                }

                var order = orderQuery.FirstOrDefault();

                if (order == null) throw new Exception("Order not found");

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    BuyerName = order.BuyerName,
                    BuyerPhone = order.BuyerPhone,
                    DeliveryTo = order.State.NameAr,
                    CreationDate = order.CreatedAt,
                    Status = order.Status,
                    StoreName = order.StoreName,
                    SubTotal = order.SubTotal,
                    DeliveryAmount = order.State.Price,
                    Total = order.SubTotal + order.State.Price,
                    ItemCount = order.ItemCount,
                    Products = order.ProductsOrder.Select(p => new ProductDto
                    {
                        Id = p.Product.Id,
                        Name = p.Product.Name,
                        NameAr = p.Product.NameAr,
                        IsActive = p.Product.IsActive,
                        InStock = p.Product.InStock,
                        MainImageUrl = p.Product.MainImageUrl,
                        ProfitPrice = p.Product.ProfitPrice,
                        SalePrice = p.Product.SalePrice,
                        SKU = p.Product.SKU,
                    }).ToList()
                };

                return new Response<OrderDto>
                {
                    Data = new List<OrderDto> { orderDto },
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response<OrderDto>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    StatusCode = ex is ArgumentNullException ? 400 : 500
                };
            }
        }



        public Response<OrderDto> AddOreder(OrderDto orederDto, ClaimsPrincipal userClaims)
        {
            try
            {
                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (user == null) throw new Exception("User can't be null");

                var productsFromCart = _db.Carts.Where(x => x.AppUserId == user.Id).Include(p => p.Product).ToList();
                if (productsFromCart.Count == 0) throw new Exception($"Cart for {user.UserName} is null");

                var productsOrder = new List<ProductsOrder>();
                var orderSku = GenerateSKU();
                int? productCount = 0;
                decimal? subTotal = 0;

                foreach (var cartItem in productsFromCart)
                {
                    productCount += cartItem.Quantity;
                    subTotal += cartItem?.Product?.SalePrice * cartItem?.Quantity;
                    var product = new ProductsOrder
                    {
                        ProductId = cartItem?.Product != null ? cartItem.Product.Id : 0,
                        Quantity = (int)(cartItem?.Quantity != null ? cartItem.Quantity : 0),
                        OrderSku = orderSku,
                    };
                    productsOrder.Add(product);
                }

                var order = new Order()
                {
                    AppUserId = user.Id,
                    BuyerName = orederDto.BuyerName,
                    BuyerPhone = orederDto.BuyerPhone,
                    ItemCount = (int)productCount,
                    StateId = orederDto.DeliveryId,
                    DeliveryToTxt = orederDto.DeliveryToDetails,
                    Status = (int)OrderStatus.OnRequest,
                    StoreName = orederDto.StoreName,
                    ProductsOrder = productsOrder,
                    SubTotal = (decimal)subTotal,
                    CollectionAmount = orederDto.CollectionAmount,
                    Note = orederDto.Note,
                    SKU = orderSku,
                    CreatedAt = DateTime.Now
                };

                // remove product from cart
                _db.Carts.RemoveRange(productsFromCart);

                _db.Orders.Add(order);
                _db.SaveChanges();

                var response = new Response<OrderDto>()
                {
                    IsSuccess = true
                };

                return response;
            }

            catch (Exception ex)
            {
                return new Response<OrderDto>
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }

        }

        // Status
        // OrginDeliveryAmount --> required if want to change status to Delivered
        // ShippingUrl
        // ShippingBarCode
        // CostShippingPrice 
        // DeliveryReturnPayer --> if oreder status was (Returned)
        // CostReturnedPrice --> if oreder status was (Returned) + DeliveryReturnPayer.Us
        public Response<OrderDto> UpdateOrder(OrderDto dto, ClaimsPrincipal userClaims)
        {
            try
            {
                var user = _userManager.FindByEmailFromClaims(userClaims).GetAwaiter().GetResult();
                if (!userClaims.IsAdmin() || !userClaims.IsSubAdmin()) throw new Exception("Unauthorized");

                var oldOrder = _db.Orders
                    .Where(x => x.Id == dto.Id)
                    .Include(x => x.ProductsOrder)
                    .ThenInclude(p => p.Product)
                    .FirstOrDefault();

                if (oldOrder?.IsOrderLifecycleEnded == true && oldOrder?.Status != dto.Status)
                {
                    throw new Exception("لايمكن تغير حالة الاوردر بسبب نقل الارباح المتوقعة الى الارباح الفعلية");
                }

                if (oldOrder != null)
                {
                    if (oldOrder.IsOrderLifecycleEnded == false)
                    {
                        if (dto.Status == (int)OrderStatus.Delivered || dto.Status == (int)OrderStatus.Canceled || dto.Status == (int)OrderStatus.Returned)
                        {
                            if (!dto.CostShippingPrice.HasValue)
                            {
                                throw new Exception("يجب ادخال تكلفة التوصيل الفعلية");
                            }

                            var sellerWallet = _db.Wallets.FirstOrDefault(w => w.AppUsers.Any(u => u.Id == oldOrder.AppUserId));
                            var adminWallet = _db.Wallets.FirstOrDefault(a => a.AppUsers.Any(u => u.Id == user.Id));

                            if (sellerWallet != null && adminWallet != null)
                            {
                                if (dto.Status == (int)OrderStatus.Delivered && oldOrder.Status != (int)OrderStatus.Delivered)
                                {
                                    decimal salePrice = 0;
                                    decimal shippingPrice = 0;

                                    foreach (var productsOrder in oldOrder.ProductsOrder)
                                    {
                                        salePrice += productsOrder.Product.SalePrice;
                                    }
                                    shippingPrice = oldOrder.State.Price;

                                    #region profits
                                    // seller Profit
                                    var sellerProfit = oldOrder.CollectionAmount - (salePrice + shippingPrice);
                                    sellerWallet.ExpectedProfit += sellerProfit;
                                    oldOrder.SellerProfit = sellerProfit;

                                    // admin Profit
                                    var shippingProfit = oldOrder.State.Price - (dto.CostShippingPrice ?? 0);
                                    var adminProfit = salePrice + shippingProfit;
                                    adminWallet.ExpectedProfit += adminProfit;
                                    oldOrder.shippingProfit = shippingProfit;
                                    oldOrder.AdminProfit = adminProfit;
                                    #endregion

                                    #region Scheduled expected profit to profit
                                    // Cancel any existing scheduled job for this order
                                    if (!string.IsNullOrEmpty(oldOrder.ScheduledJobId) && oldOrder.JobCompleted == false)
                                    {
                                        BackgroundJob.Delete(oldOrder.ScheduledJobId);
                                    }

                                    // Schedule background job to move seller (expected profit) to (seller profit) after 3 days
                                    int profitTransferDays = _configuration.GetValue<int>("OrderSettings:ProfitTransferDays");

                                    var jobId = BackgroundJob.Schedule<IWalletRepository>(
                                        service => service.MoveExpectedProfitToProfit(oldOrder.Id, sellerWallet.Id, user.Id, sellerProfit, adminProfit),
                                        TimeSpan.FromDays(profitTransferDays));

                                    // Save the job ID to the order
                                    oldOrder.ScheduledJobId = jobId;
                                    oldOrder.ProfitTransferDate = DateTime.UtcNow;
                                    oldOrder.JobCompleted = false;
                                    #endregion

                                    #region Log action
                                    var log = new Log
                                    {
                                        LogMessage = $"قام {user.FirstName} بتغيير حالة الطلب الى تم التسليم",
                                        Action = "تم التسليم",
                                        UserId = user.Id,
                                        OrderId = oldOrder.Id
                                    };
                                    oldOrder.Logs ??= new List<Log>();
                                    oldOrder.Logs.Add(log);
                                    #endregion

                                    oldOrder.isDelivered = true;
                                }

                                else if (dto.Status == (int)OrderStatus.Returned && oldOrder.Status != (int)OrderStatus.Returned)
                                {
                                    decimal returnedPrice = 0;
                                    decimal shippingPrice = 0;
                                    decimal ShippingProfit = 0;

                                    //Check if try to change status to returned before deliverd
                                    if (oldOrder.isDelivered == false)
                                    {
                                        throw new Exception("لايمكن تغير حالة الطلب الى مرتجع قبل الوصول");
                                    }

                                    if (!string.IsNullOrEmpty(oldOrder.ScheduledJobId) && oldOrder.JobCompleted == false)
                                    {
                                        BackgroundJob.Delete(oldOrder.ScheduledJobId);
                                    }

                                    if (dto.DeliveryReturnPayer == (int)DeliveryReturnPayer.Customer)
                                    {
                                        #region deducted
                                        // seller deducted                                      
                                        sellerWallet.ExpectedProfit -= oldOrder.SellerProfit.GetValueOrDefault(0);

                                        // admin deducted
                                        decimal shippingProfit = oldOrder.shippingProfit.GetValueOrDefault(0);
                                        decimal adminDeducte = oldOrder.AdminProfit.GetValueOrDefault(0) - shippingProfit;
                                        oldOrder.shippingProfit -= shippingProfit;
                                        adminWallet.ExpectedProfit -= adminDeducte;
                                        #endregion
                                    }

                                    else if (dto.DeliveryReturnPayer == (int)DeliveryReturnPayer.Seller)
                                    {
                                        #region deducted
                                        // seller deducted
                                        decimal sellerDeducte = oldOrder.SellerProfit.GetValueOrDefault(0) + oldOrder.State.Price + oldOrder.State.ReturnedPrice;
                                        sellerWallet.ExpectedProfit -= sellerDeducte;

                                        // admin deducted
                                        decimal shippingProfit = oldOrder.shippingProfit.GetValueOrDefault(0);
                                        decimal adminDeducte = oldOrder.AdminProfit.GetValueOrDefault(0) - shippingProfit;
                                        oldOrder.shippingProfit -= shippingProfit;
                                        adminWallet.ExpectedProfit -= adminDeducte;
                                        #endregion
                                    }

                                    else if (dto.DeliveryReturnPayer == (int)DeliveryReturnPayer.Us)
                                    {
                                        if (!dto.CostReturnedPrice.HasValue)
                                        {
                                            throw new Exception("يجب ادخال تكلفة توصيل المرتجع الفعلية");
                                        }

                                        #region deducted
                                        // seller deducted                                      
                                        sellerWallet.ExpectedProfit -= oldOrder.SellerProfit.GetValueOrDefault(0);

                                        // admin deducted
                                        decimal shippingProfit = oldOrder.shippingProfit.GetValueOrDefault(0); // 0.5
                                        decimal shippingProfitDeducte = (oldOrder.AdminProfit.GetValueOrDefault(0) - shippingProfit); // 10.5 - 0.5 = 10.0
                                        decimal adminDeducted = shippingProfitDeducte - oldOrder.CostShippingPrice.GetValueOrDefault(0); // 10.0 - 2 = 8
                                        adminWallet.ExpectedProfit -= adminDeducted - oldOrder.CostReturnedPrice.GetValueOrDefault(0); // 8 - 1  = 7  --> losing (-3) from 10

                                        oldOrder.shippingProfit -= shippingProfit;
                                        #endregion
                                    }

                                    #region Log action
                                    var log = new Log
                                    {
                                        LogMessage = $"قام {user.FirstName} بتغيير حالة الطلب الى مرتجع",
                                        Action = "مرتجع",
                                        UserId = user.Id,
                                        OrderId = oldOrder.Id
                                    };
                                    oldOrder.Logs ??= new List<Log>();
                                    oldOrder.Logs.Add(log);
                                    #endregion

                                    oldOrder.isDelivered = false;
                                    oldOrder.isReturned = true;
                                    oldOrder.IsOrderLifecycleEnded = true;
                                }

                                else if (dto.Status == (int)OrderStatus.Canceled && oldOrder.Status != (int)OrderStatus.Canceled)
                                {
                                    // business inside if that mean if status was (Delivered) first then change it to (Canceled)
                                    // this case will happen by mistake from admin
                                    if (!string.IsNullOrEmpty(oldOrder.ScheduledJobId) && oldOrder.JobCompleted == false)
                                    {
                                        BackgroundJob.Delete(oldOrder.ScheduledJobId);

                                        #region deducted
                                        // seller deducted                                      
                                        sellerWallet.ExpectedProfit -= oldOrder.SellerProfit.GetValueOrDefault(0);

                                        // admin deducted                                     
                                        adminWallet.ExpectedProfit -= oldOrder.AdminProfit.GetValueOrDefault(0);
                                        #endregion
                                    }

                                    oldOrder.isDelivered = false;
                                    oldOrder.isCanceled = true;
                                    oldOrder.IsOrderLifecycleEnded = true;

                                    // Canceled reson
                                }
                            }
                            else
                            {
                                throw new Exception("يوجد مشكلة في عملية جلب المحفظة, الرجاء ادخال الارباح او الخصومات يدويا لكل محفظة");
                            }
                        }
                    }

                    oldOrder.Status = dto.Status;
                    oldOrder.CostShippingPrice = dto.CostShippingPrice;
                    oldOrder.CostReturnedPrice = dto.CostReturnedPrice;
                    oldOrder.ShippingUrl = dto.ShippingUrl;
                    oldOrder.ShippingBarCode = dto.ShippingBarCode;
                    oldOrder.DashboardNote = dto.DashboardNote;
                    oldOrder.Note = dto.Note;
                    _db.SaveChanges();

                }
                else
                {
                    throw new Exception("لايمكن العثور على الاوردر");
                }

                return new Response<OrderDto>
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new Response<OrderDto>
                {
                    ErrorMessage = ex.Message,
                    IsError = true,
                };
            }
        }

        private string GenerateSKU()
        {
            return "#" + Guid.NewGuid().ToString().Substring(0, 8);
        }

    }
}
